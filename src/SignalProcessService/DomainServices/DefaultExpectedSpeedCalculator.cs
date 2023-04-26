namespace SignalProcessService.DomainServices;

public class DefaultExpectedSpeedCalculator : IExpectedSpeedCalculator
{
    private readonly string _productionLineId;
    private readonly int _sectionLengthInMeter;
    private readonly int _maxExpectedSpeedInMm;
    private readonly int _legalCorrectionInMm;

    public DefaultExpectedSpeedCalculator(string productionLineId, int sectionLengthInMeter, int maxExpectedSpeedInMm, int legalCorrectionInMm)
    {
        _productionLineId = productionLineId;
        _sectionLengthInMeter = sectionLengthInMeter;
        _maxExpectedSpeedInMm = maxExpectedSpeedInMm;
        _legalCorrectionInMm = legalCorrectionInMm;
    }

    public int DetermineDelay(DateTime entryTimestamp, DateTime exitTimestamp)
    {
        double elapsedMinutes = exitTimestamp.Subtract(entryTimestamp).TotalSeconds; // 1 sec. == 1 min. in simulation
        double avgSpeedInM = Math.Round((_sectionLengthInMeter / elapsedMinutes) * 60);
        int delay = Convert.ToInt32(avgSpeedInM - _maxExpectedSpeedInMm - _legalCorrectionInMm);
        return delay;
    }

    public string GetProductionLineId()
    {
        return _productionLineId;
    }
}
