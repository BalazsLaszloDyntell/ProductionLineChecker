using Microsoft.AspNetCore.Mvc;
using SignalProcessService.DomainServices;
using SignalProcessService.Repositories;
using SignalProcessService.Events;
using SignalProcessService.Models;
using Dapr.Client;

namespace SignalProcessService.Controllers;

[ApiController]
[Route("")]
public class ProcessController : ControllerBase
{
    private readonly ILogger<ProcessController> _logger;
    private readonly IProductStateRepository _productStateRepository;
    private readonly IExpectedSpeedCalculator _expectedSpeedCalculator;
    private readonly string _productionLineId;

    public ProcessController(
        ILogger<ProcessController> logger,
        IProductStateRepository productStateRepository,
        IExpectedSpeedCalculator expectedSpeedCalculator)
    {
        _logger = logger;
        _productStateRepository = productStateRepository;
        _expectedSpeedCalculator = expectedSpeedCalculator;
        _productionLineId = expectedSpeedCalculator.GetProductionLineId();
    }

#if !USE_ACTORMODEL

    [HttpPost("entrycheckpoint")]
    public async Task<ActionResult> ProductEntryAsync(ProductRegistered msg)
    {
        try
        {
            // log entry
            _logger.LogInformation($"ENTRY detected in lane {msg.Lane} at {msg.Timestamp.ToString("hh:mm:ss")} " +
                $"of product with barcode {msg.Barcode}.");
            Console.WriteLine("asdasdasd");
            // store product state
            var productState = new ProductState(msg.Barcode, msg.Timestamp, null);
            await _productStateRepository.SaveProductStateAsync(productState);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while processing ENTRY");
            return StatusCode(500);
        }
    }

    [HttpPost("exitcheckpoint")]
    public async Task<ActionResult> ProductExitAsync(ProductRegistered msg, [FromServices] DaprClient daprClient)
    {
        try
        {
            // get product state
            var state = await _productStateRepository.GetProductStateAsync(msg.Barcode);
            if (state == default(ProductState))
            {
                return NotFound();
            }

            // log exit
            _logger.LogInformation($"EXIT detected in lane {msg.Lane} at {msg.Timestamp.ToString("hh:mm:ss")} " +
                $"of product with barcode {msg.Barcode}.");

            // update state
            var exitState = state.Value with { ExitTimestamp = msg.Timestamp };
            await _productStateRepository.SaveProductStateAsync(exitState);

            // handle possible speeding violation
            int delay = _expectedSpeedCalculator.DetermineDelay(exitState.EntryTimestamp, exitState.ExitTimestamp.Value);
            if (true)
            {
                var productionIssue = new ProductionIssue
                {
                    ProductId = msg.Barcode,
                    ProductionLineId = _productionLineId,
                    DelayInMm = delay,
                    Timestamp = msg.Timestamp
                };

                // publish speedingviolation (Dapr publish / subscribe)
                await daprClient.PublishEventAsync("pubsub", "speedingviolations", productionIssue);
            }

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while processing EXIT");
            return StatusCode(500);
        }
    }

#else

        [HttpPost("entrycheckpoint")]
        public async Task<ActionResult> ProductEntryAsync(ProductRegistered msg)
        {
            try
            {
                var actorId = new ActorId(msg.LicenseNumber);
                var proxy = ActorProxy.Create<IProductActor>(actorId, nameof(ProductActor));
                await proxy.RegisterEntryAsync(msg);
                return Ok();
            }
            catch
            {
                return StatusCode(500);
            }
        }

        [HttpPost("exitcheckpoint")]
        public async Task<ActionResult> ProductExitAsync(ProductRegistered msg)
        {
            try
            {
                var actorId = new ActorId(msg.LicenseNumber);
                var proxy = ActorProxy.Create<IProductActor>(actorId, nameof(ProductActor));
                await proxy.RegisterExitAsync(msg);
                return Ok();
            }
            catch
            {
                return StatusCode(500);
            }
        }

#endif

}
