namespace Countries.Repositories.Postgres;

public interface IMigrator
{
	Task TryMigrate(CancellationToken cancellationToken);
}
