using MassTransit;
using Microsoft.Extensions.Logging;
using RedLockNet;
using SearchService.Application.Common.Interfaces;
using SearchService.Domain.Entities;
using SearchService.Domain.Events;

namespace SearchService.Application.Features.Seo.DomainEvents;

public record GetSeoSuccessfullyDomainEvent : DomainEventBase
{
    public string Positions { get; set; }
    public string Keyword { get; set; }
    public string Url { get; set; }
    public Guid SearchByUserId { get; set; }
}

public class GetSeoDomainEventConsumer : IConsumer<GetSeoSuccessfullyDomainEvent>
{
    private readonly ILogger<GetSeoDomainEventConsumer> _logger;
    private readonly IApplicationDbContext _context;

    public GetSeoDomainEventConsumer(ILogger<GetSeoDomainEventConsumer> logger, IApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task Consume(ConsumeContext<GetSeoSuccessfullyDomainEvent> context)
    {
        var @event = context.Message;

        _logger.LogInformation("Processed");
    }
}
