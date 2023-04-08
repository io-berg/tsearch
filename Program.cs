using Microsoft.Playwright;
using TSearch;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var query = string.Join(" ", args);

        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync();

        var page = await browser.NewPageAsync();
        await page.GotoAsync("https://html.duckduckgo.com/html/");

        List<DuckDuckGoResult> Results = new List<DuckDuckGoResult>();

        await page.TypeAsync("input[name=q]", query);
        await page.ClickAsync("input[type=submit]");
        await page.WaitForSelectorAsync("div.result__body");
        var results = await page.QuerySelectorAllAsync("div.result__body");
        foreach (var result in results)
        {
            var title = await result.QuerySelectorAsync("a.result__a");
            var link = await result.QuerySelectorAsync("a.result__a");
            var description = await result.QuerySelectorAsync("a.result__snippet");

            Results.Add(new DuckDuckGoResult
            {
                Title = await title.TextContentAsync(),
                Link = await link.GetAttributeAsync("href"),
                Description = await description.TextContentAsync()
            });
        }

        foreach (var result in Results)
        {
            Console.WriteLine(result.Title);
            Console.WriteLine(result.Link);
            Console.WriteLine(result.Description);
            Console.WriteLine();
        }
    }
}