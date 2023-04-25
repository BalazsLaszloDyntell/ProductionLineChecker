namespace SignalProcessService.Models;

public record struct ProductionIssue(string ProductId, string ProductionLineId, int DelayInMm, DateTime Timestamp);