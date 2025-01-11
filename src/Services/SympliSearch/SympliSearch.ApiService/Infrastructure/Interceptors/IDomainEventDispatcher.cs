using SympliSearch.ApiService.Domain.Common;
using SympliSearch.ApiService.Domain.Events;

namespace SympliSearch.ApiService.Infrastructure.Interceptors;

public interface IDomainEventDispatcher
{
    public Task DispatchAsync<T>(IEnumerable<T> domainEvents) where T : IDomainEvent;
    public Task DispatchAsync<T>(IDomainEvent domainEvent) where T : IDomainEvent;
}
