namespace SignalProcessService.Models;

public record struct ProductState
{
    public string Barcode { get; init; }
    public DateTime EntryTimestamp { get; init; }
    public DateTime? ExitTimestamp { get; init; }

    public ProductState(string barcode, DateTime entryTimestamp, DateTime? exitTimestamp = null)
    {
        this.Barcode = barcode;
        this.EntryTimestamp = entryTimestamp;
        this.ExitTimestamp = exitTimestamp;
    }
}
