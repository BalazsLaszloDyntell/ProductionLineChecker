// create web-app
using SignalProcessService.Actors;
using SignalProcessService.DomainServices;
using SignalProcessService.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IExpectedSpeedCalculator>(
    new DefaultExpectedSpeedCalculator("A12", 10, 100, 5));

builder.Services.AddSingleton<IProductStateRepository, DaprProductStateRepository>();

var daprHttpPort = Environment.GetEnvironmentVariable("DAPR_HTTP_PORT") ?? "3600";
var daprGrpcPort = Environment.GetEnvironmentVariable("DAPR_GRPC_PORT") ?? "60000";
builder.Services.AddDaprClient(builder => builder
    .UseHttpEndpoint($"http://localhost:{daprHttpPort}")
    .UseGrpcEndpoint($"http://localhost:{daprGrpcPort}"));

builder.Services.AddControllers();

builder.Services.AddActors(options =>
{
    options.Actors.RegisterActor<ProductActor>();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseCloudEvents();

app.MapControllers();
app.MapActorsHandlers();

app.Run("http://localhost:6000");
