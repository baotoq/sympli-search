using MassTransit;
using Microsoft.Extensions.Logging;
using RedLockNet;
using SympliSearch.Domain.Events;

namespace SympliSearch.Application.Features.Seo.DomainEvents;

public record GetSeoDomainEvent : DomainEventBase
{
}

public class GetSeoDomainEventConsumer : IConsumer<GetSeoDomainEvent>
{
    private readonly IDistributedLockFactory _distributedLockFactory;
    private readonly ILogger<GetSeoDomainEventConsumer> _logger;

    public GetSeoDomainEventConsumer(ILogger<GetSeoDomainEventConsumer> logger, IDistributedLockFactory distributedLockFactory)
    {
        _logger = logger;
        _distributedLockFactory = distributedLockFactory;
    }

    public async Task Consume(ConsumeContext<GetSeoDomainEvent> context)
    {
        var @event = context.Message;

        _logger.LogInformation("Processed");
    }
}
