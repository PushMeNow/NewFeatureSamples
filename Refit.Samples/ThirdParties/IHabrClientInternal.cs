namespace Refit.Samples.ThirdParties;

public interface IHabrClientInternal
{
	[Get("/ru/feed")]
	Task<ApiResponse<string>> GetMain(CancellationToken cancellationToken = default);
}


internal sealed class HabrClient(IHabrClientInternal client)
{
	public async Task<string[]> GetMain(CancellationToken cancellationToken = default)
	{
		var response = await client.GetMain(cancellationToken);
		return HtmlParser.GetArticleTitles(response.Content);
	}
}
