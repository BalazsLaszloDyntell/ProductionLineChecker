using Newtonsoft.Json;
using Simulation.Proxies;
using Microsoft.AspNetCore.SignalR;
using SignalRChat.Hubs;
using System.IO;

namespace Simulation.ScannerSimulation;

public class Scanner {
  private readonly ISignalProcessService _signalProcessService;
  private ProductionSimulation _prodSim;
  private string filePath = @"./Data/Products.json";
  private Random _rnd;
  private int _prodLine;
  private int _minEntryDelayInMS = 50;
  private int _maxEntryDelayInMS = 5000;
  private int _minExitDelayInS = 4;
  private int _maxExitDelayInS = 10;
  public Scanner(int prodLine, ISignalProcessService signalProcessService, ProductionSimulation prodSim) {
      _rnd = new Random();
      _prodLine = prodLine;
      _signalProcessService = signalProcessService;
      _prodSim = prodSim;
  }
  public Task StartProd()
  {
    while(true) {

      try {
        TimeSpan entryDelay = TimeSpan.FromMilliseconds(_rnd.Next(_minEntryDelayInMS, _maxEntryDelayInMS) + _rnd.NextDouble());
        Task.Delay(entryDelay).Wait();

        Task.Run(async () =>
        {
          DateTime entryTimestamp = DateTime.Now;
          var production = await GetProduction();
          production.Timestamp = entryTimestamp;
          production.Checkpoint = "Checkpoint 1";
          //await _signalprocessService.SendProductionEntryAsync(production);

          //send to UI
          await _prodSim.Clients.All.SendAsync("StartProd", JsonConvert.SerializeObject(production));

          TimeSpan exitDelay = TimeSpan.FromSeconds(_rnd.Next(_minExitDelayInS, _maxExitDelayInS) + _rnd.NextDouble());
          Task.Delay(exitDelay).Wait();

          production.Timestamp = DateTime.Now;
          production.Checkpoint = "Checkpoint 2";

          //await _signalprocessService.SendProductionExitAsync(production);

          //send to UI
          await _prodSim.Clients.All.SendAsync("StartProd", JsonConvert.SerializeObject(production));
        }).Wait();
      } catch(Exception e) {
        Console.WriteLine(e);
      }
    }
  }

  private async Task<Production> GetProduction() {

    try {
      var productionsJson = await File.ReadAllTextAsync(filePath);
      List<Production> productions = JsonConvert.DeserializeObject<List<Production>>(productionsJson);

      Random random = new Random();
      int index = random.Next(10);
      Production production = productions[index];

      production.ProductionLineId = "Line " + _prodLine;

      return production;

    } catch(Exception e) {
      Console.WriteLine(e.Message);
      return new Production();
    }
  }
}