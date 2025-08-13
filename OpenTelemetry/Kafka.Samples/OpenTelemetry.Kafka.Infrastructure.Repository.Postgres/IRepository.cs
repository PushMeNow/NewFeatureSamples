using OpenTelemetry.Kafka.Infrastructure.Repository.Postgres.Entities;

namespace OpenTelemetry.Kafka.Infrastructure.Repository.Postgres;

public interface IRepository<TEntity> where TEntity : class, IUniqueId
{
	IAsyncEnumerable<TEntity> GetAll(bool track = false);
	Task<TEntity?> GetById(Guid id, bool track = false);
	Task<TEntity> Create(TEntity entity);
	Task Update(TEntity entity);
	Task Delete(Guid id);
}
