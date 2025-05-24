namespace Countries.Repositories.Postgres.Entities;

public sealed class CountryHistoryRecord(
	string ip,
	string? countryCode,
	string serviceName)
{
	public Guid Id { get; set; }

	public string Ip { get; set; } = ip;

	public string? CountryCode { get; set; } = countryCode;

	public string ServiceName { get; set; } = serviceName;

	public DateTime CreatedDate { get; set; }
}
