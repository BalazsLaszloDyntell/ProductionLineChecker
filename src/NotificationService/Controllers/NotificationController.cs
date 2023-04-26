using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Dapr;
using Dapr.Client;
using NotificationService.Models;
using NotificationService.Helpers;

namespace NotificationService.Controllers;

[ApiController]
[Route("")]
public class NotificationController : ControllerBase
{
    private static string? _productBarcode = null;
    private readonly ILogger<NotificationController> _logger;

    public NotificationController(ILogger<NotificationController> logger,
        DaprClient daprClient)
    {
        _logger = logger;

        if (_productBarcode == null)
        {
            string secretName = Environment.GetEnvironmentVariable("FINE_CALCULATOR_LICENSE_SECRET_NAME") ?? "finecalculator.licensekey";
            var metadata = new Dictionary<string, string> { { "namespace", "dapr-signalprocess" } };
            var secrets = daprClient.GetSecretAsync(
                "signalprocess-secrets", secretName, metadata).Result;
            _productBarcode = secrets[secretName];
        
        }
    }

    [Topic("pubsub", "delayissue", "deadletters", false)]
    [Route("collectdelay")]
    [HttpPost()]
    public async Task<ActionResult> CollectDelay(DelayIssue delayIssue, [FromServices] DaprClient daprClient)
    {
        var body = EmailUtils.CreateEmailBody(delayIssue, null); //TODO: product info
        var metadata = new Dictionary<string, string>
        {
            ["emailFrom"] = "noreply@cfca.gov",
            ["emailTo"] = "boss@gmail.com",
            ["subject"] = $"Delay issue on the {delayIssue.productionLineId}"
        };
        await daprClient.InvokeBindingAsync("sendmail", "create", body, metadata);

        _logger.LogInformation("Email sended to boss@gmail.com");

        return Ok();
    }

    [Topic("pubsub", "deadletters")]
    [Route("deadletters")]
    [HttpPost()]
    public ActionResult HandleDeadLetter(object message)
    {
        try
        {
            var messageJson = JsonSerializer.Serialize<object>(message);
            _logger.LogInformation($"Unhandled message content: {messageJson}");
        }
        catch 
        {
            _logger.LogError("Unhandled message's payload could not be deserialized.");
        }

        return Ok();
    }
}
