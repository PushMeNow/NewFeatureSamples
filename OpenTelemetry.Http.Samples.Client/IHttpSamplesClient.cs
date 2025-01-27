using OpenTelemetry.Http.Samples.Domain;

namespace OpenTelemetry.Http.Samples.Client;

public interface IHttpSamplesClient
{
	Task<IReadOnlyCollection<CountryHistoryRecordResponse>> GetCountyHistory(string ip, string? countryCode);
	Task WriteHistory(CountryHistoryRecordRequest request);
}
