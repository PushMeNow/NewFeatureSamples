namespace Countries.Repositories.Postgres.Entities;

public sealed class CountryHistoryRecord
{
	public CountryHistoryRecord(string ip, string? countryCode, string serviceName)
	{
		Ip = ip;
		CountryCode = countryCode;
		ServiceName = serviceName;
	}

	public Guid Id { get; set; }

	public string Ip { get; set; }

	public string? CountryCode { get; set; }

	public string ServiceName { get; set; }

	public DateTime CreatedDate { get; set; }
}
