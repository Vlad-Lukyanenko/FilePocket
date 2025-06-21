namespace FilePocket.Domain.Entities.Abstractions;

public interface IAmSoftDeletedEntity
{
    bool IsDeleted { get; }
    DateTime? DeletedAt { get; }

    void MarkAsDeleted(DateTime? deletedAt);

    void RestoreFromDeleted();
}