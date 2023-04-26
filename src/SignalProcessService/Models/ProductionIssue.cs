namespace SignalProcessService.Models;

public record struct ProductionIssue(string ProductId, string ProductionLineId, int Delay, DateTime Timestamp);