using Abot2.Core;
using Abot2.Crawler;
using Abot2.Poco;
using Abot2.Tintin247.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

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

            Log.Information("Starting up!");
            //await CrawlCategory("https://m.baomoi.com");

            var listUri = new List<string> { "https://m.baomoi.com/tin-moi.epi", "https://m.baomoi.com" };
            await Crawler(listUri);


            Log.Information("Done!");
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

            var crawlResult = await crawler.CrawlAsync(new Uri("https://m.baomoi.com"));
            
        }

        private static void Crawler_PageCrawlCompleted(object sender, PageCrawlCompletedArgs e)
        {

        }

        private static async Task Crawler(List<string> listUri)
        {
            try
            {
                var dataCategory = new List<ArticleCategory>();
                var cache = RedisConnectorHelper.Connection.GetDatabase();
                var dataRedisCategory = await cache.StringGetAsync("CategoryMemory");
                if (string.IsNullOrEmpty(dataRedisCategory))
                {
                    dataCategory = GetDataCategory();
                    await cache.StringSetAsync("CategoryMemory",JsonSerializer.Serialize(dataCategory));
                }
                else
                {
                    dataCategory = JsonSerializer.Deserialize<List<ArticleCategory>>(dataRedisCategory);
                }
               

                foreach (var Uri in listUri)
                {
                    var isHot = !Uri.Contains("tin-moi");
                    var cacheName = isHot ? "ArticleMemoryHot" : "ArticleMemoryNew";
                    var crawledPage = await PageRequester(Uri);
                    Log.Information("{crawledPage}", new { url = crawledPage.Uri, status = Convert.ToInt32(crawledPage.HttpResponseMessage?.StatusCode) });
                    IEnumerable<AngleSharp.Dom.IElement> newAricleListNotExisted = null;
                    var listDataCrawl = new List<Article>();
                    var dataRedis = await cache.StringGetAsync(cacheName);
                    var dataCrawled = string.IsNullOrEmpty(dataRedis) ? GetDataCraw() : JsonSerializer.Deserialize<List<Article>>(dataRedis);
                    var aricleList = crawledPage.AngleSharpHtmlDocument.QuerySelectorAll(".timeline>.story");
                    if (aricleList != null)
                    {
                        var listAid = dataCrawled.Select(x => x.Aid);
                        newAricleListNotExisted = aricleList.Where(x => !listAid.Contains(x.GetAttribute("data-aid")));

                        foreach (var article in newAricleListNotExisted)
                        {
                            var storyElement = article.QuerySelector(".story__link");
                            if (storyElement != null)
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
                                Log.Information("{crawledPageDetail}", new { url = crawledPageDetail.Uri, status = Convert.ToInt32(crawledPageDetail.HttpResponseMessage?.StatusCode) });
                                //Category

                                var angleSharpHtmlDocumentDetail = crawledPageDetail.AngleSharpHtmlDocument;
                                var breadcrumb = angleSharpHtmlDocumentDetail.QuerySelector(".breadcrumb").QuerySelectorAll(".item>a");
                                var lastBreadcrumb = breadcrumb[breadcrumb.Count() - 1].TextContent.Trim();
                                var category = dataCategory.FirstOrDefault(x => x.Title == lastBreadcrumb).Id;

                                var newAricleDetail = angleSharpHtmlDocumentDetail.QuerySelector(".article");
                                if (newAricleDetail != null)
                                {
                                    var type = string.Empty;
                                    var typePhoto = newAricleDetail.ClassName.Contains("article--photo");
                                    var typeVideo = newAricleDetail.ClassName.Contains("article--video");
                                    if (typePhoto)
                                    {
                                        type = "Photo";
                                    }
                                    else if (typeVideo)
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
                                    var author = authorNameElement != null && authorNameElement.Length > 0 ? authorNameElement[0].QuerySelector("strong").TextContent.Trim() : null;
                                    var listTags = new List<string>();
                                    foreach (var tag in tagsElements)
                                    {
                                        listTags.Add(tag.TextContent.Trim());
                                    }

                                    listDataCrawl.Add(new Article
                                    {
                                        Aid = aid,
                                        Title = title,
                                        MetaTitle = StringHelper.ConvertShortName(title),
                                        RewriteUrl = null,
                                        Description = description,
                                        ThumbImage = thumbImage,
                                        Link = link,
                                        FullLink = fullLink,
                                        SourceImage = sourceImage,
                                        SourceName = sourceName,
                                        PostedDatetime = Convert.ToDateTime(datetime),
                                        FullDescription = fullDescription,
                                        Content = content,
                                        SourceLink = sourceLink,
                                        Author = author,
                                        Tags = listTags.Count > 0 ? string.Join(',', listTags.ToArray()) : null,
                                        Type = type,
                                        Category = category,
                                        IsHot = isHot,
                                        IsRank1 = false,
                                        ViewCount = 0,
                                        CommentCount = 0,
                                        CreatedBy = null,
                                        CreatedOn = DateTime.Now,
                                        LastModifiedBy = null,
                                        LastModifiedOn = null,
                                        IsDeleted = false
                                    });
                                }
                            }
                        }
                        if (listDataCrawl.Count > 0)
                        {
                            //Lưu cache
                            await cache.StringSetAsync(cacheName, JsonSerializer.Serialize(listDataCrawl));

                            using (var dbContext = new aspnetcoreheroContext())
                            {
                                await dbContext.Articles.AddRangeAsync(listDataCrawl);
                                await dbContext.SaveChangesAsync();
                            }

                        }
                    }
                }
              
            }
            catch (Exception ex)
            {
                Log.Error("{crawledPage}",ex.Message);
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

        private static List<Article> GetDataCraw()
        {
            using (var dbcontext = new aspnetcoreheroContext())
            {
                return  dbcontext.Articles.OrderBy(x=>x.PostedDatetime).Take(200).ToList<Article>();
            }
        }

        private static async Task CrawlCategory(string Uri)
        {
            var crawledPage = await PageRequester(Uri);
            Log.Information("{crawledPage}", new { url = crawledPage.Uri, status = Convert.ToInt32(crawledPage.HttpResponseMessage?.StatusCode) });

            var listDataCrawl = new List<ProductCategory>();
            var newAricleCategory = crawledPage.AngleSharpHtmlDocument.QuerySelectorAll(".nav__parent").Where(x=>x.ClassName.Contains("child"));
            foreach(var item in newAricleCategory)
            {
                var titleParent = item.Children[0].QuerySelector("span").TextContent.Trim();
                var childrens = item.Children[1].Children;
                using (var dbContext = new aspnetcoreheroContext())
                {
                    var category = new ArticleCategory
                    {
                        Title = titleParent,
                        MetaTitle = StringHelper.ConvertShortName(titleParent),
                        IsDeleted = false,
                        CreatedOn = DateTime.Now
                    };
                    await dbContext.ArticleCategories.AddAsync(category);
                    await dbContext.SaveChangesAsync();
                    var id = category.Id;
                    foreach(var item2 in childrens)
                    {
                        var title2 = item2.QuerySelector("span").TextContent.Trim();
                        await dbContext.ArticleCategories.AddAsync(new ArticleCategory
                        {
                            Title = title2,
                            MetaTitle = StringHelper.ConvertShortName(title2),
                            ParentId = id,
                            IsDeleted = false,
                            CreatedOn = DateTime.Now
                        });
                        await dbContext.SaveChangesAsync();
                    }
                }
            }
        }

        private static List<ArticleCategory> GetDataCategory()
        {
            using (var dbcontext = new aspnetcoreheroContext())
            {
                return dbcontext.ArticleCategories.ToList<ArticleCategory>();
            }
        }
    }
}
