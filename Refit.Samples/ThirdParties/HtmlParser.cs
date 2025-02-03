using HtmlAgilityPack;

namespace Refit.Samples.ThirdParties;

internal sealed class HtmlParser
{
	public static string[] GetArticleTitles(string html)
	{
		var document = new HtmlDocument
		{
			OptionEmptyCollection = true
		};
		document.LoadHtml(html);
		var htmlNodes = document.DocumentNode.SelectNodes("//div[@class='tm-articles-list']/article//h2/a/span").Nodes().ToArray();
		return htmlNodes.Select(q => q.InnerText).ToArray();
	}
}
