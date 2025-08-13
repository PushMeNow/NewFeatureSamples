using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Kafka.Infrastructure.Repository.Postgres.Entities;

namespace OpenTelemetry.Kafka.Infrastructure.Repository.Postgres;

internal sealed class Repository<TEntity>(PostContext context) : IRepository<TEntity> where TEntity : class, IUniqueId
{
	private readonly DbSet<TEntity> _set = context.Set<TEntity>();

	public IAsyncEnumerable<TEntity> GetAll(bool track = false)
	{
		return GetQuery(track).AsAsyncEnumerable();
	}

	public Task<TEntity?> GetById(Guid id, bool track = false)
	{
		return GetQuery(track).FirstOrDefaultAsync(q => q.Id == id);
	}

	public async Task<TEntity> Create(TEntity entity)
	{
		await _set.AddAsync(entity);
		await context.SaveChangesAsync();
		return entity;
	}

	public Task Update(TEntity entity)
	{
		context.Entry(entity).State = EntityState.Modified;
		return context.SaveChangesAsync();
	}

	public async Task Delete(Guid id)
	{
		if (typeof(TEntity).GetInterface(nameof(ISoftDelete)) is not null)
		{
			var entity = (await GetQuery(true).FirstOrDefaultAsync(q => q.Id == id)) as ISoftDelete;
			entity.IsDeleted = true;
			await context.SaveChangesAsync();
		}else
		{
			await GetQuery(true).Where(q => q.Id == id).ExecuteDeleteAsync();
		}
	}

	private IQueryable<TEntity> GetQuery(bool track)
	{
		var query = _set.AsQueryable();
		if (!track) query = query.AsNoTracking();

		return query;
	}
}
