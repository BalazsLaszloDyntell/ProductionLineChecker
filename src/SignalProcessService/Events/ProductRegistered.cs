namespace SignalProcessService.Events;

public record struct ProductRegistered(int Lane, string Barcode, DateTime Timestamp);