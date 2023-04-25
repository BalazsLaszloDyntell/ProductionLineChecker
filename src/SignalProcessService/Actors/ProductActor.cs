using Dapr.Actors.Runtime;
using Dapr.Client;
using SignalProcessService.DomainServices;
using SignalProcessService.Events;
using SignalProcessService.Models;

namespace SignalProcessService.Actors;

public class ProductActor : Actor, IProductActor, IRemindable
{
    public readonly IExpectedSpeedCalculator _expectedSpeedCalculator;
    private readonly string _productionLineId;
    private readonly DaprClient _daprClient;

    public ProductActor(ActorHost host, DaprClient daprClient, IExpectedSpeedCalculator expectedSpeedCalculator) : base(host)
    {
        _daprClient = daprClient;
        _expectedSpeedCalculator = expectedSpeedCalculator;
        _productionLineId = _expectedSpeedCalculator.GetProductionLineId();
    }

    public async Task RegisterEntryAsync(ProductRegistered msg)
    {
        try
        {
            Logger.LogInformation($"ENTRY detected in lane {msg.Lane} at " +
                $"{msg.Timestamp.ToString("hh:mm:ss")} " +
                $"of product with barcode {msg.Barcode}.");

            // store vehicle state
            var productState = new ProductState(msg.Barcode, msg.Timestamp);
            await this.StateManager.SetStateAsync("ProductState", productState);

            // register a reminder for cars that enter but don't exit within 20 seconds
            // (they might have broken down and need road assistence)
            await RegisterReminderAsync("ProductLost", null,
                TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(20));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error in RegisterEntry");
        }
    }

    public async Task RegisterExitAsync(ProductRegistered msg)
    {
        try
        {
            Logger.LogInformation($"EXIT detected in lane {msg.Lane} at " +
                $"{msg.Timestamp.ToString("hh:mm:ss")} " +
                $"of product with barcode {msg.Barcode}.");

            // remove lost vehicle timer
            await UnregisterReminderAsync("ProductLost");

            // get vehicle state
            var productState = await this.StateManager.GetStateAsync<ProductState>("ProductState");
            productState = productState with { ExitTimestamp = msg.Timestamp };
            await this.StateManager.SetStateAsync("ProductState", productState);

            // handle expected speed
            int delay = _expectedSpeedCalculator.DetermineExpectedSpeedInMm(
                productState.EntryTimestamp, productState.ExitTimestamp.Value);
            if (delay > 0)
            {
                var productionIssue = new ProductionIssue
                {
                    ProductId = msg.Barcode,
                    ProductionLineId = _productionLineId,
                    DelayInMm = delay,
                    Timestamp = msg.Timestamp
                };

                // publish speedingviolation (Dapr publish / subscribe)
                await _daprClient.PublishEventAsync("pubsub", "speedingviolations", productionIssue);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error in RegisterExit");
        }
    }

    public async Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
    {
        if (reminderName == "ProductLost")
        {
            // remove lost vehicle timer
            await UnregisterReminderAsync("ProductLost");

            var productState = await this.StateManager.GetStateAsync<ProductState>("ProductState");

            Logger.LogInformation($"Lost track of product with barcode {productState.Barcode}. " +
                "Sending road-assistence.");

            // send road assistence ...
        }
    }
}
