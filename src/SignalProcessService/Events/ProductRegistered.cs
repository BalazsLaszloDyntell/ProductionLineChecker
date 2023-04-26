namespace SignalProcessService.Events;

public record struct ProductRegistered(string ProdLine, string Barcode, string ProductName, DateTime Timestamp);