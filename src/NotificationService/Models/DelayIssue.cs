namespace NotificationService.Models;

public record struct DelayIssue(string productId, string productName, string productionLineId, int delay, DateTime startTimestamp, DateTime endTimestamp);
