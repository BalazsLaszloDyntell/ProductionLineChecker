using Microsoft.AspNetCore.SignalR;
using Simulation.Proxies;
using Simulation.ScannerSimulation;

namespace SignalRChat.Hubs
{
  public class ProductionSimulation : Hub
  {
    public async Task StartProd()
    {
      int prodLines = 3;
      Scanner[] scanners = new Scanner[prodLines];
      for (var i = 0; i < prodLines; i++)
      {
          int scanNumber = i + 1;
          var trafficControlService = await MqttSignalProcessService.CreateAsync(scanNumber);
          scanners[i] = new Scanner(scanNumber, trafficControlService, this);
      }
      Parallel.ForEach(scanners, scan => scan.StartProd());

      Task.Run(() => Thread.Sleep(Timeout.Infinite)).Wait();
    }
  }
}