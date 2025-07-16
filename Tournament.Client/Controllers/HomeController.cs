using System.Diagnostics;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Tournament.Client.Client;
using Tournament.Client.Models;

namespace Tournament.Client.Controllers;

public class HomeController : Controller
{
    private const string json = "application/json";

    private readonly HttpClient httpClient;
    private readonly ITournamentClient tournamentClient;

    //private readonly ILogger<HomeController> _logger;
    //public HomeController(ILogger<HomeController> logger)
    //{
    //    _logger = logger;
    //}
    public HomeController(IHttpClientFactory httpClientFactory, ITournamentClient tCli)
    {
        //Utan inject - ny instans varje gång=farlig!
        //var httpClient = new HttpClient();

        //Mindre farlig
        //httpClient = httpClientFactory.CreateClient();
        //httpClient.BaseAddress = new Uri("https://localhost:7273");

        //Namngiven
        //httpClient = httpClientFactory.CreateClient("CompaniesClient");

        //Bästa metoden.
        tournamentClient = tCli;

    }

    public IActionResult Index()
    {
        return View();
    }

    //public IActionResult Privacy()
    //{
    //    return View();
    //}

    //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    //public IActionResult Error()
    //{
    //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    //}
}
