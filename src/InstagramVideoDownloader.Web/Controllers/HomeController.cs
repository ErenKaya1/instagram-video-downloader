using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using src.InstagramVideoDownloader.Web.Models;
using src.InstagramVideoDownloader.Web.Utils;

namespace InstagramVideoDownloader.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _hostEnvironment;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _hostEnvironment = hostEnvironment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetVideo(UrlViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Url) || !await GeneralFunctions.IsUrlInvalid(model.Url))
            {
                TempData["InvalidUrlError"] = "Invalid Url";
                return RedirectToAction("index");
            }

            string videoSource = await GeneralFunctions.GetVideoSource(model.Url);
            string fileName = Guid.NewGuid().ToString() + ".mp4";
            GeneralFunctions.SaveVideo(videoSource, fileName, _hostEnvironment);
            TempData["FileName"] = fileName;

            return RedirectToAction("index");
        }
    }
}
