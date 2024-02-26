using CsvHelper;
using DataGenerator.Models;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using DataGenerator.Service.Helpers;
using DataGenerator.Domain.Entities;
using DataGenerator.Service.Interfaces;

namespace DataGenerator.Controllers
{
    public class HomeController : Controller
    {
        private const int PageSize = 20;

        private readonly IUserDataService _userDataService;

        public HomeController(IUserDataService userDataService)
        {
            _userDataService = userDataService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetData(DataRequestModel request)
        {
            var data = _userDataService.GetUserData(request.Seed, request.MistakesRate, request.Region.ToString());
            return Json(data.Skip((request.PageNumber - 1) * PageSize).Take(PageSize));
        }

        [HttpPost]
        public async Task<IActionResult> CreateCsv([FromBody] IEnumerable<User> persons)
        {
            var path = $"{Directory.GetCurrentDirectory()}{DateTime.Now.Ticks}.csv";
            await using var writer = new StreamWriter(path);

            await using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csvWriter.Context.RegisterClassMap<DataCsvMap>();
                await csvWriter.WriteRecordsAsync(persons);
            }

            return PhysicalFile(path, "text/csv");
        }

    }
}
