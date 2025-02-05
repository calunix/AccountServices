using AccountServices.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using AccountServices.Classes;

namespace AccountServices.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration config;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            this.config = configuration;
        }

        public IActionResult Index()
        {
            HomePage homePageData = new(config);
            return View(homePageData);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}