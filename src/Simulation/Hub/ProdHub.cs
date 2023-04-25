using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.IO;

namespace SignalRChat.Hubs
{
  public class ProdHub : Hub
  {
    private string filePath = @"./Data/Products.json";
    public async Task StartProd()
    {
      await Clients.All.SendAsync("StartProd", await AddToProduction());
    }

    private async Task<string> AddToProduction() {

      try {
        var productionsJson = await File.ReadAllTextAsync(filePath);
        List<Production> productions = JsonConvert.DeserializeObject<List<Production>>(productionsJson);
 
        Random random = new Random();
        int index = random.Next(10);
        Production production = productions[index];

        var productionLineId = "Line " + (new Random().Next(1, 3)).ToString();
        production.ProductionLineId = productionLineId;

        return JsonConvert.SerializeObject(production);

      } catch(Exception e) {
        Console.WriteLine(e.Message);
        return "";
      }
    }
  }
}