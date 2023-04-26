namespace NotificationService.Models;

public record struct DelayIssue(string productId, string productionLineId, int delay, DateTime timestamp);
