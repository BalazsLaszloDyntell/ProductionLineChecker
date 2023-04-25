using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Simulation.Proxies;
using System.IO;

namespace SignalRChat.Hubs
{
  public class ProdHub : Hub
  {
    private readonly IProductionControlService _productionControlService;
    private string filePath = @"./Data/Products.json";
    private Random _rnd = new Random();
    private int _minEntryDelayInMS = 50;
    private int _maxEntryDelayInMS = 5000;
    private int _minExitDelayInS = 4;
    private int _maxExitDelayInS = 10;
    public async Task StartProd()
    {
      while(true) {
        TimeSpan entryDelay = TimeSpan.FromMilliseconds(_rnd.Next(_minEntryDelayInMS, _maxEntryDelayInMS) + _rnd.NextDouble());
        Task.Delay(entryDelay).Wait();

        DateTime entryTimestamp = DateTime.Now;
        var production = await GetProduction();
        production.Timestamp = entryTimestamp;
        production.Checkpoint = "Checkpoint 1";
        //await _productionControlService.SendProductionEntryAsync(production);

        //send to UI
        await Clients.All.SendAsync("StartProd", JsonConvert.SerializeObject(production));

        TimeSpan exitDelay = TimeSpan.FromSeconds(_rnd.Next(_minExitDelayInS, _maxExitDelayInS) + _rnd.NextDouble());
        Task.Delay(exitDelay).Wait();

        production.Timestamp = DateTime.Now;
        production.Checkpoint = "Checkpoint 2";

        //await _productionControlService.SendProductionExitAsync(production);

        //send to UI
        await Clients.All.SendAsync("StartProd", JsonConvert.SerializeObject(production));
      }
    }

    private async Task<Production> GetProduction() {

      try {
        var productionsJson = await File.ReadAllTextAsync(filePath);
        List<Production> productions = JsonConvert.DeserializeObject<List<Production>>(productionsJson);
 
        Random random = new Random();
        int index = random.Next(10);
        Production production = productions[index];

        var productionLineId = "Line " + (new Random().Next(1, 3)).ToString();
        production.ProductionLineId = productionLineId;

        return production;

      } catch(Exception e) {
        Console.WriteLine(e.Message);
        return new Production();
      }
    }
  }
}