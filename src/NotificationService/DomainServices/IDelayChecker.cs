namespace NotificationService.DomainServices;

public interface IDelayChecker
{
    public bool HasDelay(string barcode, int delay);
}
