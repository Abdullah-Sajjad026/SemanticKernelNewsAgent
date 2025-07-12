using System.ComponentModel;
using Microsoft.SemanticKernel;
using SimpleFeedReader;

namespace SemanticKernalNewsAPI.Plugins;

public class NewsPlugin
{
    [KernelFunction("get_news")]
    [Description("Get trending real world news for a category")]
    [return: Description("List of news with information like title, summary, publish date, uri, tags etc")]
    public async Task<List<FeedItem>> GetNews(
        [Description("News category like Technology, Business, Sports")]
        string category = "Technology")
    {
        Console.WriteLine($"NewsPlugin.GetNews called with category: {category}");

        var reader = new FeedReader();
        var newsFeed = await reader.RetrieveFeedAsync($"https://rss.nytimes.com/services/xml/rss/nyt/{category}.xml");

        var newsItems = newsFeed.Take(5).ToList();

        return newsItems;
    }
}