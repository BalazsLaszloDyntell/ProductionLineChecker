using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace SignalRChat.Hubs
{
    public class ProdHub : Hub
    {
      public async Task StartProd()
      {
        await Clients.All.SendAsync("StartProd", AddToProduction());
      }

      private string AddToProduction() {
        var barcode = "test" + (new Random().Next(1, 5)).ToString() ;
        var name = "Product " + (new Random().Next(1, 10)).ToString();
        var productionLineId = "Line " + (new Random().Next(1, 3)).ToString();
        
        Production production = new Production()
        {
            Barcode = barcode,
            Name = name,
            ProductionLineId = productionLineId
        };

        return JsonConvert.SerializeObject(production);
      }
    }
}