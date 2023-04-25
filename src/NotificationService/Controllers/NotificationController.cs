using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Dapr;
using NotificationService.DomainServices;
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
    private readonly IDelayChecker _delayChecker;

    public NotificationController(ILogger<NotificationController> logger,
        IDelayChecker delayChecker,
        DaprClient daprClient)
    {
        _logger = logger;
        _delayChecker = delayChecker;

        if (_productBarcode == null)
        {
            bool useKubernetesSecrets = Convert.ToBoolean(Environment.GetEnvironmentVariable("USE_KUBERNETES_SECRETS") ?? "false");
            string secretName = Environment.GetEnvironmentVariable("FINE_CALCULATOR_LICENSE_SECRET_NAME") ?? "finecalculator.licensekey";
            var metadata = new Dictionary<string, string> { { "namespace", "dapr-trafficcontrol" } };
            if (useKubernetesSecrets)
            {
                var k8sSecrets = daprClient.GetSecretAsync(
                    "kubernetes", "trafficcontrol-secrets", metadata).Result;
                _productBarcode = k8sSecrets[secretName];
            }
            else
            {
                var secrets = daprClient.GetSecretAsync(
                    "trafficcontrol-secrets", secretName, metadata).Result;
                _productBarcode = secrets[secretName];
            }
        }
    }

    [Topic("pubsub", "speedingviolations", "deadletters", false)]
    [Route("collectfine")]
    [HttpPost()]
    public async Task<ActionResult> CollectFine(DelayIssue delayIssue, [FromServices] DaprClient daprClient)
    {
        bool isOk = _delayChecker.HasDelay(_productBarcode!, delayIssue.Delay);

        var body = EmailUtils.CreateEmailBody(delayIssue, null); //TODO: product info
        var metadata = new Dictionary<string, string>
        {
            ["emailFrom"] = "noreply@cfca.gov",
            ["emailTo"] = "dyntelldevelop@gmail.com",
            ["subject"] = $"Speeding violation on the {delayIssue.ProductionLineID}"
        };
        await daprClient.InvokeBindingAsync("sendmail", "create", body, metadata);

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
