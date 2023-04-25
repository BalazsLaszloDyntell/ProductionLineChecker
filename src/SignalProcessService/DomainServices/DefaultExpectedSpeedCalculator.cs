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

    public int DetermineExpectedSpeedInMm(DateTime entryTimestamp, DateTime exitTimestamp)
    {
        double elapsedMinutes = exitTimestamp.Subtract(entryTimestamp).TotalSeconds; // 1 sec. == 1 min. in simulation
        double avgSpeedInKmh = Math.Round((_sectionLengthInMeter / elapsedMinutes) * 60);
        int violation = Convert.ToInt32(avgSpeedInKmh - _maxExpectedSpeedInMm - _legalCorrectionInMm);
        return violation;
    }

    public string GetProductionLineId()
    {
        return _productionLineId;
    }
}
