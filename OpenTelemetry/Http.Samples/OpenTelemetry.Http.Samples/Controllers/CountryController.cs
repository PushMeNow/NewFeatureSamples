using Counties.Client;
using Microsoft.AspNetCore.Mvc;

namespace OpenTelemetry.Http.Samples.Controllers;

[ApiController]
[Route("api/countries")]
public sealed class CountryController(ICountiesClient countiesClient) : ControllerBase
{
	[HttpGet]
	public async Task<CountryResponse?> GetCountry()
	{
		var countryResponse = await countiesClient.GetCountry();

		return countryResponse;
	}
}
