namespace OpenTelemetry.Http.Samples.Domain;

public sealed record CountryHistoryRecordResponse(
	string Ip,
	string? CountryCode,
	DateTime CreatedDate);
