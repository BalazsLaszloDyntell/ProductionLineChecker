namespace SignalProcessService.DomainServices;

public interface IExpectedSpeedCalculator
{
    int DetermineExpectedSpeedInMm(DateTime entryTimestamp, DateTime exitTimestamp);
    string GetProductionLineId();
}
