namespace SignalProcessService.Events;

public record struct ProductRegistered(string ProdLine, string Barcode, string ProcudtName, DateTime Timestamp);