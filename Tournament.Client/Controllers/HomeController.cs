using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Tournament.Client.Client;
using Tournament.Client.Models;
using Tournament.Shared.DTOs;

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
        httpClient = httpClientFactory.CreateClient("TournamentClient");
        httpClient.BaseAddress = new Uri("https://localhost:7006");

        //Bästa metoden.
        tournamentClient = tCli;

    }

    public async Task<IActionResult> Index()
    {
        var result = await SimpleGetAsync();
        var result2 = await SimpleGetAsync2();
        var result3 = await GetWithRequestMessageAsync();
        var result4 = await PostWithRequestMessageAsync();
        await PatchWithRequestMessageAsync();

        return View();
    }


    private async Task<IEnumerable<TournamentDto>> SimpleGetAsync()
    {
        var response = await httpClient.GetAsync("api/TournamentDetails");
        response.EnsureSuccessStatusCode();
        var res = await response.Content.ReadAsStringAsync();

        var companies = JsonSerializer.Deserialize<IEnumerable<TournamentDto>>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        return companies!;

    }

    private async Task<IEnumerable<TournamentDto>> SimpleGetAsync2()
    {
        return await httpClient.GetFromJsonAsync<IEnumerable<TournamentDto>>("api/TournamentDetails", new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
    }

    private async Task<IEnumerable<TournamentDto>> GetWithRequestMessageAsync()
    {
        //var request = new HttpRequestMessage(HttpMethod.Get, "api/TournamentDetails");
        //request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(json));
        //var response = await httpClient.SendAsync(request);
        //response.EnsureSuccessStatusCode();
        //var res = await response.Content.ReadAsStringAsync();
        //var companies = JsonSerializer.Deserialize<IEnumerable<CompanyDto>>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        //return companies!;

        return await tournamentClient.GetAsync<IEnumerable<TournamentDto>>("api/TournamentDetails");
    }

    private async Task<TournamentDto> PostWithRequestMessageAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "api/TournamentDetails");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(json));
        var tournamentToCreate = new TournamentCreateDto
        {
            Title="Qwerty",
            StartDate=DateTime.Now
            //Name = "AB Skrot och korn",
            //Address = "Storgatan 5",
            //Country = "Sweden",
            //Employees = null
        };
        var jsonCompany = JsonSerializer.Serialize(tournamentToCreate);
        request.Content = new StringContent(jsonCompany);
        request.Content.Headers.ContentType = new MediaTypeHeaderValue(json);
        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var res = await response.Content.ReadAsStringAsync();
        var tournamentDto = JsonSerializer.Deserialize<TournamentDto>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        var location = response.Headers.Location;


        return tournamentDto;
    }

    private async Task PatchWithRequestMessageAsync()
    {
        var patchDocument = new JsonPatchDocument<TournamentUpdateDto>();
        patchDocument.Replace(e => e.Title, "Kilroy was here");
        patchDocument.Replace(e => e.StartDate, DateTime.Now );

        var serializedPatchDoc = Newtonsoft.Json.JsonConvert.SerializeObject(patchDocument);

        var request = new HttpRequestMessage(HttpMethod.Patch, "api/TournamentDetails/1");

        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(json));

        request.Content = new StringContent(serializedPatchDoc);

        request.Content.Headers.ContentType = new MediaTypeHeaderValue(json);

        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

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
