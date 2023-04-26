namespace SignalProcessService.DomainServices;

public class DefaultExpectedSpeedCalculator : IExpectedSpeedCalculator
{
    private readonly string _productionLineId;

    public DefaultExpectedSpeedCalculator(string productionLineId, int sectionLengthInMeter, int maxExpectedSpeedInMm, int legalCorrectionInMm)
    {
        _productionLineId = productionLineId;
    }

    public int DetermineDelay(DateTime entryTimestamp, DateTime exitTimestamp)
    {
        return Convert.ToInt32(exitTimestamp.Subtract(entryTimestamp).TotalSeconds / 60);
    }

    public string GetProductionLineId()
    {
        return _productionLineId;
    }
}
