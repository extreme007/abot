using Abot2.Core;
using Abot2.Crawler;
using Abot2.Poco;
using Abot2.Tintin247.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abot2.Tintin247
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .Enrich.WithThreadId()
               .WriteTo.Console(outputTemplate: Constants.LogFormatTemplate)
               .CreateLogger();

            Log.Information("Demo starting up!");

            await Crawler("https://m.baomoi.com/tin-moi.epi");
            //await DemoSimpleCrawler();

            Log.Information("Demo done!");
            Console.ReadKey();
        }

        private static async Task DemoSimpleCrawler()
        {
            var config = new CrawlConfiguration
            {
                MaxPagesToCrawl = 25,
                MinCrawlDelayPerDomainMilliSeconds = 3000
            };
            var crawler = new PoliteWebCrawler(config);

            crawler.PageCrawlCompleted += Crawler_PageCrawlCompleted;

            var crawlResult = await crawler.CrawlAsync(new Uri("http://wvtesting2.com"));
            
        }

        private static void Crawler_PageCrawlCompleted(object sender, PageCrawlCompletedArgs e)
        {

        }

        private static async Task Crawler(string Uri)
        {
            var crawledPage = await PageRequester(Uri);
            Log.Information("{crawledPage}", new { url = crawledPage.Uri, status = Convert.ToInt32(crawledPage.HttpResponseMessage.StatusCode) });

            var listDataCrawl = new List<DataCrawlTemp>();
            var newAricleList = crawledPage.AngleSharpHtmlDocument.QuerySelectorAll(".timeline>.story");
            foreach(var article in newAricleList)
            {
                var storyElement = article.QuerySelector(".story__link");
                if(storyElement!= null)
                {
                    var storyMeta = storyElement.QuerySelector(".story__meta");

                    var title = storyElement.QuerySelector(".story__heading").TextContent.Trim();
                    var link = storyElement.GetAttribute("href");
                    var storyThumbElement = storyElement.QuerySelector(".story__thumb>img") ?? storyElement.QuerySelector(".story__thumb>.is-first>img");
                    string thumbImage = storyThumbElement == null ? null : storyThumbElement.GetAttribute("data-src") ?? storyThumbElement.GetAttribute("src");
                    var sourceImage = storyMeta.QuerySelector(".source-image").GetAttribute("src");
                    var sourceName = storyMeta.QuerySelector(".source-image")?.GetAttribute("alt");
                    var datetime = storyMeta.QuerySelector(".friendly").GetAttribute("datetime");
                    var description = storyElement.QuerySelector(".story__summary").TextContent.Trim();
                    var aid = article.GetAttribute("data-aid");
                    string fullLink = string.Format("{0}{1}", "https://baomoi.com", link);

                    //Get Detail by link
                    var crawledPageDetail = await PageRequester(fullLink);
                    var newAricleDetail = crawledPageDetail.AngleSharpHtmlDocument.QuerySelector(".article");
                    var type = string.Empty; 
                    var typePhoto = newAricleDetail.ClassName.Contains("article--photo");
                    var typeVideo = newAricleDetail.ClassName.Contains("article--video");
                    if(typePhoto)
                    {
                        type = "Photo";
                    }else if (typeVideo)
                    {
                        type = "Video";
                    }
                    else
                    {
                        type = "Text";
                    }
                    var fullDescription = newAricleDetail.QuerySelector(".article__sapo").TextContent.Trim();
                    var content = newAricleDetail.QuerySelector(".article__body").OuterHtml;
                    var tagsElements = newAricleDetail.QuerySelector(".article__tag").QuerySelectorAll(".keyword");
                    var sourceLink = newAricleDetail.QuerySelector(".bm-source>a>.source").TextContent.Trim();
                    var authorNameElement = newAricleDetail.QuerySelectorAll(".body-author");
                    var authorName = authorNameElement != null && authorNameElement.Length > 0 ? authorNameElement[0].QuerySelector("strong").TextContent.Trim(): null;
                    var listTags = new List<string>();
                    foreach(var tag in tagsElements)
                    {
                        listTags.Add(tag.TextContent.Trim());
                    }

                    listDataCrawl.Add(new DataCrawlTemp
                    {
                        Id = Guid.NewGuid(),
                        Titile = title,
                        Description = description,
                        ThumbImage = thumbImage,
                        Link = link,
                        FullLink = fullLink,
                        SourceImage = sourceImage,
                        SourceName = sourceName,
                        PostedDatetime = Convert.ToDateTime(datetime),
                        Aid = Convert.ToInt64(aid),
                        CreatedDateTime = DateTime.Now,
                        FullDescription = fullDescription,
                        Content = content,
                        SourceLink = sourceLink,
                        AuthorName = authorName,
                        Tags = listTags.Count > 0 ? string.Join(',', listTags.ToArray()) : null,
                        Type = type
                    });
                }    
            }
            if(listDataCrawl.Count > 0)
            {
                using (var context = new tintin247Context())
                {
                   await context.AddRangeAsync(listDataCrawl);
                   await context.SaveChangesAsync();
                }
            }    
        }

        private static async Task<CrawledPage> PageRequester(string uri)
        {
            var config = new CrawlConfiguration
            {
                MaxPagesToCrawl = 1,
                MinCrawlDelayPerDomainMilliSeconds = 3000
            };
            var pageRequester = new PageRequester(config, new WebContentExtractor());

           return await pageRequester.MakeRequestAsync(new Uri(uri));
        }
    }
}
