﻿using Abot2.Core;
using Abot2.Crawler;
using Abot2.Poco;
using Abot2.Tintin247.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Diagnostics;

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

            Log.Information("------------------------- Starting up! ----------------------------");
            Stopwatch sw = Stopwatch.StartNew();
          
            var listUri = new List<string> { "https://m.baomoi.com/tin-moi.epi", "https://m.baomoi.com" };
         
            var existed = CheckExistedCategory();
            if(!existed)
            {
                await CrawlCategory("https://m.baomoi.com");
            }
            await Crawler(listUri);
            sw.Stop();
            TimeSpan ts = sw.Elapsed;

            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}", ts.Minutes, ts.Seconds,ts.Milliseconds);
            Log.Information(string.Format("--------------- Done, RunTime -> {0} --------------", elapsedTime));
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
                var listDataCrawl = new List<Article>();
                var dataCategory = GetDataCategory();
                //var cache = RedisConnectorHelper.Connection.GetDatabase();
                //var dataRedisCategory = await cache.StringGetAsync("CategoryMemory");
                //if (string.IsNullOrEmpty(dataRedisCategory))
                //{
                //    dataCategory = GetDataCategory();
                //    await cache.StringSetAsync("CategoryMemory",JsonSerializer.Serialize(dataCategory));
                //}
                //else
                //{
                //    dataCategory = JsonSerializer.Deserialize<List<ArticleCategory>>(dataRedisCategory);
                //}
            


                var dataArticleCache = GetDataCrawl();
                //var cacheName = "ArticleMemory";
                //var dataRedis = await cache.StringGetAsync(cacheName);
                //if (string.IsNullOrEmpty(dataRedis))
                //{
                //    dataArticleCache = GetDataCrawl();
                //    await cache.StringSetAsync(cacheName, JsonSerializer.Serialize(dataArticleCache));
                //}
                //else
                //{
                //    dataArticleCache = JsonSerializer.Deserialize<List<Article>>(dataRedis);
                //}

                foreach (var Uri in listUri)
                {
                    var isHot = !Uri.Contains("tin-moi");
                    var crawledPage = await PageRequester(Uri);
                    Log.Information("{crawledPage}", new { url = crawledPage.Uri, status = Convert.ToInt32(crawledPage.HttpResponseMessage?.StatusCode) });
                    var aricleList = crawledPage.AngleSharpHtmlDocument.QuerySelector(".timeline").Children;
                    if (aricleList != null)
                    {
                        foreach (var article in aricleList.Where(x=>x.ClassName.Contains("rank1-stories") || x.ClassName.Contains("story")))
                        {
                            AngleSharp.Dom.IElement elementData = article;
                            var isRank1 = article.ClassName.Contains("rank1-stories");
                            if (isRank1)
                            {
                                elementData = article.QuerySelector(".story");
                            }

                            var aid = elementData.GetAttribute("data-aid");

                            var storyElement = elementData.QuerySelector(".story__link");
                            var existed = dataArticleCache.Any(x => x.Aid == aid);
                            if (storyElement != null && !existed)
                            {
                                var storyMeta = storyElement.QuerySelector(".story__meta");

                                var title = storyElement.QuerySelector(".story__heading").TextContent.Trim();
                                var link = storyElement.GetAttribute("href");
                                var storyThumbElement = storyElement.QuerySelector(".story__thumb>img") ?? storyElement.QuerySelector(".story__thumb>.is-first>img");
                                string thumbImage = storyThumbElement == null ? null : storyThumbElement.GetAttribute("data-src") ?? storyThumbElement.GetAttribute("src");
                                var sourceImage = storyMeta.QuerySelector(".source-image")?.GetAttribute("src");
                                var sourceName = storyMeta.QuerySelector(".source-image")?.GetAttribute("alt");
                                var datetime = storyMeta.QuerySelector(".friendly").GetAttribute("datetime");
                                var description = storyElement.QuerySelector(".story__summary").TextContent.Trim();
                      
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

                                    var articleCrawled = new Article
                                    {
                                        Aid = aid,
                                        Title = title,
                                        Slug = StringHelper.ConvertShortName(title),
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
                                        CategoryId = category,
                                        ArticleCategoryId = category,
                                        IsHot = isHot,
                                        IsRank1 = isRank1,
                                        ViewCount = 0,
                                        CommentCount = 0,
                                        CreatedBy = "0bdd8200-ff66-46f3-bfb0-78a43e1124cb",
                                        CreatedOn = DateTime.Now,
                                        IsPublished = true
                                    };
                                    listDataCrawl.Add(articleCrawled);
                                    dataArticleCache.Add(articleCrawled);
                                }
                            }
                        }
                    }
                }

                if (listDataCrawl.Count > 0)
                {
                    //await cache.StringSetAsync(cacheName, JsonSerializer.Serialize(listDataCrawl));

                    using (var dbContext = new tintin247comContext())
                    {
                        await dbContext.Articles.AddRangeAsync(listDataCrawl);
                        await dbContext.SaveChangesAsync();
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

        private static List<Article> GetDataCrawl()
        {
            using (var dbcontext = new tintin247comContext())
            {
                return  dbcontext.Articles.OrderByDescending(x=>x.CreatedOn).Take(300).ToList<Article>();
            }
        }

        private static async Task CrawlCategory(string Uri)
        {
            var crawledPage = await PageRequester(Uri);
            Log.Information("{crawledPage}", new { url = crawledPage.Uri, status = Convert.ToInt32(crawledPage.HttpResponseMessage?.StatusCode) });

            var listDefault = new List<ArticleCategory>() { new ArticleCategory
            {
                Title ="Tin mới",
                Slug = StringHelper.ConvertShortName("Tin mới"),
                CreatedBy =  "0bdd8200-ff66-46f3-bfb0-78a43e1124cb",
                CreatedOn = DateTime.Now,
                Order = 1
            },
             new ArticleCategory
            {
                Title ="Tin nóng",
                Slug = StringHelper.ConvertShortName("Tin nóng"),
                CreatedBy =  "0bdd8200-ff66-46f3-bfb0-78a43e1124cb",
                CreatedOn = DateTime.Now,
                Order = 2
            }};
            using (var dbContext = new tintin247comContext())
            {
                await dbContext.ArticleCategories.AddRangeAsync(listDefault);
                await dbContext.SaveChangesAsync();
                var chude = new ArticleCategory
                {
                    Title = "Chủ đề",
                    Slug = StringHelper.ConvertShortName("Chủ đề"),
                    CreatedBy = "0bdd8200-ff66-46f3-bfb0-78a43e1124cb",
                    CreatedOn = DateTime.Now,
                    Order = 3
                };
                await dbContext.ArticleCategories.AddAsync(chude);
                await dbContext.SaveChangesAsync();
                var idChude = chude.Id;

                var order = 4;
                var newAricleCategory = crawledPage.AngleSharpHtmlDocument.QuerySelectorAll(".nav__parent").Where(x => x.ClassName.Contains("child"));
                foreach (var item in newAricleCategory)
                {
                    var titleParent = item.Children[0].QuerySelector("span").TextContent.Trim();
                    var childrens = item.Children[1].Children;

                    var category = new ArticleCategory
                    {
                        Title = titleParent,
                        Slug = StringHelper.ConvertShortName(titleParent),
                        CreatedBy = "0bdd8200-ff66-46f3-bfb0-78a43e1124cb",
                        CreatedOn = DateTime.Now,
                        Order = order,
                        ParentId = idChude
                    };
                    await dbContext.ArticleCategories.AddAsync(category);
                    await dbContext.SaveChangesAsync();
                    order += 1;
                    var id = category.Id;
                    foreach (var item2 in childrens)
                    {
                        var title2 = item2.QuerySelector("span").TextContent.Trim();
                        await dbContext.ArticleCategories.AddAsync(new ArticleCategory
                        {
                            Title = title2,
                            Slug = StringHelper.ConvertShortName(title2),
                            ParentId = id,
                            CreatedBy = "0bdd8200-ff66-46f3-bfb0-78a43e1124cb",
                            CreatedOn = DateTime.Now,
                            Order =order
                        });
                        await dbContext.SaveChangesAsync();
                        order += 1;
                    }

                }
            }
        }

        private static List<ArticleCategory> GetDataCategory()
        {
            using (var dbcontext = new tintin247comContext())
            {
                return dbcontext.ArticleCategories.ToList<ArticleCategory>();
            }
        }

        private static bool CheckExistedCategory()
        {
            using (var dbcontext = new tintin247comContext())
            {
                return  dbcontext.ArticleCategories.Any();
            }
        }
    }
}
