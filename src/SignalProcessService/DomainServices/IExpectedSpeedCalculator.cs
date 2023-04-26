namespace SignalProcessService.DomainServices;

public interface IExpectedSpeedCalculator
{
    int DetermineDelay(DateTime entryTimestamp, DateTime exitTimestamp);
    string GetProductionLineId();
}
