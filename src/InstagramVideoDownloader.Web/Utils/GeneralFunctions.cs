using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp;
using Microsoft.AspNetCore.Hosting;

namespace src.InstagramVideoDownloader.Web.Utils
{
    public class GeneralFunctions
    {
        public async static Task<bool> IsUrlInvalid(string url)
        {
            // regex for instagram post url
            var regex = @"(https?:\/\/(?:www\.)?instagram\.com\/p\/([^/?#&]+)).*";
            var match = Regex.Match(url, regex, RegexOptions.IgnoreCase);
            if (!match.Success) return false;

            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(url);

            // if it is a error page, it is not contains og:video meta tag
            var urlIsAPost = document
                                    .GetElementsByTagName("meta")
                                    .Where(x => x.GetAttribute("property") == "og:video")
                                    .FirstOrDefault();

            if (urlIsAPost == null) return false;

            return true;
        }

        public async static Task<string> GetVideoSource(string url)
        {
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(url);

            var videoSource = document
                                    .GetElementsByTagName("meta")
                                    .Where(x => x.GetAttribute("property") == "og:video")
                                    .FirstOrDefault()
                                    .GetAttribute("content");

            return videoSource;
        }

        public async static void SaveVideo(string url, string fileName, IWebHostEnvironment hostEnvironment)
        {
            var path = Path.Combine(hostEnvironment.WebRootPath + "/video", fileName);

            using (var client = new HttpClient())
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                var response = await client.GetAsync(url);
                await response.Content.CopyToAsync(fileStream);
            }
        }
    }
}