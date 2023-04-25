namespace NotificationService.Models;

public record struct DelayIssue(string ProductId, string ProductionLineID, int Delay, DateTime Timestamp);
