namespace NotificationService.DomainServices;

public class DelayChecker : IDelayChecker
{
    public bool HasDelay(string barcode, int delay)
    {
        if (barcode != "asd123")
        {
            throw new InvalidOperationException("Invalid barcode specified.");
        }

        if (delay < 5)
        {
            return false;
        }

        return true;
    }
}
