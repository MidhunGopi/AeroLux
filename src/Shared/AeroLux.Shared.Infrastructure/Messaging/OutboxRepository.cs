using AeroLux.Shared.Kernel.Messaging;
using AeroLux.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AeroLux.Shared.Infrastructure.Messaging;

/// <summary>
/// EF Core implementation of the outbox repository for reliable message delivery.
/// Note: SaveChanges should be called by the Unit of Work, not by the repository.
/// For operations that need immediate persistence (like the outbox processor),
/// use the SaveChanges parameter or inject IUnitOfWork.
/// </summary>
public class OutboxRepository : IOutboxRepository
{
    private readonly DbContext _context;
    private readonly IUnitOfWork? _unitOfWork;

    public OutboxRepository(DbContext context, IUnitOfWork? unitOfWork = null)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        await _context.Set<OutboxMessage>().AddAsync(message, cancellationToken);
        // Let the Unit of Work handle SaveChanges as part of the transaction
    }

    public async Task<IReadOnlyList<OutboxMessage>> GetUnprocessedAsync(int batchSize, CancellationToken cancellationToken = default)
    {
        return await _context.Set<OutboxMessage>()
            .Where(m => m.ProcessedAt == null && m.RetryCount < 5)
            .OrderBy(m => m.CreatedAt)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }

    public async Task MarkAsProcessedAsync(Guid messageId, CancellationToken cancellationToken = default)
    {
        var message = await _context.Set<OutboxMessage>()
            .FirstOrDefaultAsync(m => m.Id == messageId, cancellationToken);

        if (message is not null)
        {
            message.MarkAsProcessed();
            // For outbox processing, we need to save immediately
            if (_unitOfWork is not null)
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            else
                await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task MarkAsFailedAsync(Guid messageId, string error, CancellationToken cancellationToken = default)
    {
        var message = await _context.Set<OutboxMessage>()
            .FirstOrDefaultAsync(m => m.Id == messageId, cancellationToken);

        if (message is not null)
        {
            message.MarkAsFailed(error);
            // For outbox processing, we need to save immediately
            if (_unitOfWork is not null)
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            else
                await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
