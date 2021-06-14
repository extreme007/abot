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
using System.Diagnostics;
using AngleSharp;
using AngleSharp.Html.Parser;
using AngleSharp.Html.Dom;
using AngleSharp.Dom;
using Hangfire;
using Hangfire.SqlServer;
using System.Threading;
using Hangfire.Storage;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MailKit.Security;
using MailKit.Net.Smtp;

namespace Abot2.Tintin247
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();

            var config = host.Services.GetRequiredService<Microsoft.Extensions.Configuration.IConfiguration>();

            GlobalConfiguration.Configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseColouredConsoleLogProvider()
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(config.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    UsePageLocksOnDequeue = true,
                    DisableGlobalLocks = true
                });

            using (var connection = JobStorage.Current.GetConnection())
            {
                foreach (var recurringJob in connection.GetRecurringJobs())
                {
                    Log.Information(string.Format("Delete RecurringJob: {0} ", recurringJob.Id));
                    RecurringJob.RemoveIfExists(recurringJob.Id);
                }
       
                RecurringJob.AddOrUpdate(() => BackgroundJobCrawl(), "*/30 * * * *");
                Log.Information("Create RecurringJob Success");

                //var recurring = connection.GetRecurringJobs().FirstOrDefault(x => x.Id == "Program.BackgroundJobCrawl");
                //if (recurring == null)
                //{
                //    RecurringJob.AddOrUpdate(() => BackgroundJobCrawl(), Cron.Minutely);
                //}
            }

            using (var server = new BackgroundJobServer())
            {
                Console.ReadLine();
            }

            await host.RunAsync();
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
           Host.CreateDefaultBuilder(args)
               .ConfigureAppConfiguration((hostingContext, configuration) =>
               {
                   configuration.Sources.Clear();

                   IHostEnvironment env = hostingContext.HostingEnvironment;

                   configuration
                       .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                   IConfigurationRoot configurationRoot = configuration.Build();


                   //Initialize Logger
                   Log.Logger = new LoggerConfiguration()
                       .ReadFrom.Configuration(configurationRoot)
                       .CreateLogger();
               });


        public static async Task BackgroundJobCrawl()
        {
            var lastJobId = string.Empty;
            using (var connection = JobStorage.Current.GetConnection())
            {
                var recurring = connection.GetRecurringJobs().FirstOrDefault(x => x.Id == "Program.BackgroundJobCrawl");
                lastJobId = recurring.LastJobId == null ? "0" : recurring.LastJobId.ToString();
                Log.Information(string.Format(">>>>>>>>>>>>>>>>> Starting JobId {0} <<<<<<<<<<<<<<<<<<<", lastJobId));
            }

            Stopwatch sw = Stopwatch.StartNew();

            var listUri = new List<string> { "https://m.baomoi.com/tin-moi.epi", "https://m.baomoi.com" };

            //var existedCategory = CheckExistedCategory();
            //if (!existedCategory)
            //{
            //    await CrawlCategory("https://m.baomoi.com");
            //}
            //var existedPartner = CheckExistedPartner();
            //if (!existedPartner)
            //{
            //    await CrawlPartner("https://baomoi.com/");
            //}
            var result = await Crawler(listUri);

            //ReplaceSlug();
            //TestReplaceContent();
            // UpdateGroupCategory();

            sw.Stop();
            TimeSpan ts = sw.Elapsed;

            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}", ts.Minutes, ts.Seconds, ts.Milliseconds);
            Log.Information(string.Format("--------------------------------- Done, RunTime -> {0} ----------------------------------", elapsedTime));

            BackgroundJob.Enqueue(() => SendEmail(lastJobId, result,elapsedTime));
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

        private static async Task<int> Crawler(List<string> listUri)
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
                        foreach (var article in aricleList.Where(x => x.ClassName.Contains("rank1-stories") || x.ClassName.Contains("story")))
                        {
                            AngleSharp.Dom.IElement elementData = article;
                            var isRank1 = article.ClassName.Contains("rank1-stories");
                            if (isRank1)
                            {
                                elementData = article.QuerySelector(".story");
                            }

                            var aid = elementData.GetAttribute("data-aid");

                            var storyElement = elementData.QuerySelector(".story__link");
                            var existed = dataArticleCache.Any(x => x == aid);
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
                                List<string> listBreadcrumb = new List<string>();
                                foreach (var item in breadcrumb)
                                {
                                    listBreadcrumb.Add(item.TextContent.Trim());
                                }
                                var category = dataCategory.FirstOrDefault(x => x.Title == listBreadcrumb.Last()).Id;
                                var groupCategory = dataCategory.FirstOrDefault(x => x.Title == listBreadcrumb.First()).Id;

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
                                    var content = ReplaceContent(newAricleDetail.QuerySelector(".article__body").InnerHtml);
                                    var image = newAricleDetail.QuerySelectorAll(".body-image>img").FirstOrDefault()?.GetAttribute("src");

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
                                        ThumbImage = thumbImage?.Replace("https://photo-baomoi.zadn.vn/w300", "https://photo-baomoi.zadn.vn/w700"),
                                        Image = image,
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
                                        GroupCategoryId = groupCategory,
                                        ArticleCategoryId = category,
                                        IsHot = isHot,
                                        IsRank1 = isRank1,
                                        ViewCount = 0,
                                        CommentCount = 0,
                                        CreatedBy = "0bdd8200-ff66-46f3-bfb0-78a43e1124cb",
                                        CreatedOn = DateTime.Now,
                                        IsPublished = typeVideo ? false : true
                                    };
                                    listDataCrawl.Add(articleCrawled);
                                    dataArticleCache.Add(articleCrawled.Aid);
                                }
                            }
                        }
                    }
                }

                var total = listDataCrawl.Count;
                if (total > 0)
                {
                    //await cache.StringSetAsync(cacheName, JsonSerializer.Serialize(listDataCrawl));

                    using (var dbContext = new tintin247comContext())
                    {
                        await dbContext.Articles.AddRangeAsync(listDataCrawl);
                        await dbContext.SaveChangesAsync();
                    }
                }

                Log.Information(string.Format("----> Total: {0}", total));
                return total;

            }
            catch (Exception ex)
            {
                Log.Error("{crawledPage}", ex.Message);
            }
            return 0;

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

        private static List<string> GetDataCrawl()
        {
            using (var dbcontext = new tintin247comContext())
            {
                return dbcontext.Articles.AsNoTracking().Select(x => x.Aid).ToList<string>();
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
                            Order = order
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
                return dbcontext.ArticleCategories.Any();
            }
        }
        private static bool CheckExistedPartner()
        {
            using (var dbcontext = new tintin247comContext())
            {
                return dbcontext.Partners.Any();
            }
        }

        private static void ReplaceSlug()
        {
            var listDataReplace = new List<Article>();
            using (var dbcontext = new tintin247comContext())
            {
                var listData = dbcontext.Articles.ToList<Article>();
                foreach (var item in listData)
                {
                    item.Slug = StringHelper.ConvertShortName(item.Title);
                    listDataReplace.Add(item);
                }
                dbcontext.Articles.UpdateRange(listDataReplace);
                dbcontext.SaveChanges();
            }
        }

        private static string ReplaceContent(string content)
        {
            var parser = new HtmlParser(new HtmlParserOptions
            {
                IsNotConsumingCharacterReferences = true,
            });

            var document = parser.ParseDocument(content);
            var lazyImgae = document.QuerySelectorAll(".lazy-img");
            foreach (var item in lazyImgae)
            {
                var newElement = document.CreateElement("img");
                newElement.SetAttribute("class", item.ClassName);
                newElement.SetAttribute("src", item.GetAttribute("data-src"));
                newElement.SetAttribute("width", item.GetAttribute("width"));
                newElement.SetAttribute("height", item.GetAttribute("height"));
                item.Insert(AdjacentPosition.BeforeBegin, newElement.OuterHtml);
                item.Remove();
            }

            return document.QuerySelector("body").InnerHtml;
        }

        private static void TestReplaceContent()
        {
            using (var dbcontext = new tintin247comContext())
            {
                dbcontext.ChangeTracker.QueryTrackingBehavior = Microsoft.EntityFrameworkCore.QueryTrackingBehavior.NoTracking;
                var data = dbcontext.Articles.Where(x => x.CreatedOn.Day != 3);

                var parser = new HtmlParser(new HtmlParserOptions
                {
                    IsNotConsumingCharacterReferences = true,
                });
                foreach (var item in data)
                {
                    var newArtileString = JsonSerializer.Serialize(item);
                    var newArtile = JsonSerializer.Deserialize<Article>(newArtileString);

                    var document = parser.ParseDocument(newArtile.Content);
                    var lazyImgae = document.QuerySelectorAll(".lazy-img");
                    foreach (var subItem in lazyImgae)
                    {
                        var newElement = document.CreateElement("img");
                        newElement.SetAttribute("class", subItem.ClassName);
                        newElement.SetAttribute("src", subItem.GetAttribute("data-src"));
                        newElement.SetAttribute("width", subItem.GetAttribute("width"));
                        newElement.SetAttribute("height", subItem.GetAttribute("height"));
                        subItem.Insert(AdjacentPosition.BeforeBegin, newElement.OuterHtml);
                        subItem.Remove();
                    }
                    newArtile.Content = document.QuerySelector("body").InnerHtml;
                    newArtile.Image = document.QuerySelectorAll(".body-image>img").FirstOrDefault()?.GetAttribute("src");

                    dbcontext.Articles.Update(newArtile);
                }
                dbcontext.SaveChanges();
            }
        }

        private static async Task CrawlPartner(string Uri)
        {
            var crawledPage = await PageRequester(Uri);
            Log.Information("{crawledPage}", new { url = crawledPage.Uri, status = Convert.ToInt32(crawledPage.HttpResponseMessage?.StatusCode) });


            using (var dbContext = new tintin247comContext())
            {
                var order = 1;
                var listPartner = crawledPage.AngleSharpHtmlDocument.QuerySelectorAll(".box-content>.wrap>ul>li");
                foreach (var item in listPartner)
                {
                    var title = item.QuerySelector("span").TextContent.Trim();
                    var image = item.QuerySelector("img").GetAttribute("data-src");

                    var partner = new Partner
                    {
                        Title = title,
                        Slug = StringHelper.ConvertShortName(title),
                        CreatedBy = "0bdd8200-ff66-46f3-bfb0-78a43e1124cb",
                        CreatedOn = DateTime.Now,
                        Image = image,
                        Link = "",
                        Order = order,
                    };
                    await dbContext.Partners.AddAsync(partner);

                    order += 1;
                }
                await dbContext.SaveChangesAsync();
            }
        }

        private static void UpdateGroupCategory()
        {
            var listDataReplace = new List<Article>();
            using (var dbcontext = new tintin247comContext())
            {
                var listCategory = dbcontext.ArticleCategories.ToList<ArticleCategory>();
                var listData = dbcontext.Articles.ToList<Article>().Where(x => x.GroupCategoryId == 0);
                foreach (var item in listData)
                {
                    item.GroupCategoryId = listCategory.Where(x => x.Id == item.CategoryId).FirstOrDefault().ParentId.Value;
                    dbcontext.Articles.Update(item);
                }
                dbcontext.SaveChanges();
            }
        }

        public static async Task SendEmail(string jobId,int total, string runtime)
        {
            try
            {
                var email = new MimeMessage();
                email.Sender = MailboxAddress.Parse("tintin247.com");
                email.To.Add(MailboxAddress.Parse("hoangan007@gmail.com"));
                email.Subject = "Crawler success jobId: " + jobId;
                var builder = new BodyBuilder();
                builder.HtmlBody = string.Format("Success at: {0} - Total: {1} - Runtime: {2}", DateTime.Now.ToString("dd/MM/yyyy HH:mm"),total,runtime);
                email.Body = builder.ToMessageBody();
                using var smtp = new SmtpClient();
                smtp.Connect("smtp.gmail.com",587, SecureSocketOptions.StartTls);
                smtp.Authenticate("nguyenhoangan.dev@gmail.com", "h0@ng.@n");
                await smtp.SendAsync(email);
                smtp.Disconnect(true);
                Log.Information("Send Email Success !");
            }
            catch (Exception ex)
            {
                Log.Error(ex,"Send Email Error");
            }
        }
    }
}
