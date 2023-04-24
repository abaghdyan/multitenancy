namespace Multitenancy.Services.Abstractions;

public interface IDataTransferService
{
    Task TransferDataAsync(int tenantId, int newStorageId);
}

