using Countries.Repositories.Postgres.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Countries.Repositories.Postgres.Configurations;

internal sealed class CountryHistoryRecordConfiguration : IEntityTypeConfiguration<CountryHistoryRecord>
{
	public void Configure(EntityTypeBuilder<CountryHistoryRecord> builder)
	{
		builder.HasKey(q => q.Id);
		builder.Property(q => q.CountryCode).HasMaxLength(2).IsRequired(false);
		builder.Property(q => q.Ip).IsRequired();
		builder.Property(q => q.ServiceName).HasMaxLength(50).IsRequired();
	}
}
