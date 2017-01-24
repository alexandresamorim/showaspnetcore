using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Showaspnetcore.Data;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Showaspnetcore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "ADMINISTRATOR")]
    public class ExamesController : Controller
    {
        private IMongoCollection<Exame> examesCollection;

        public ExamesController(MongoClient client)
        {
            var database = client.GetDatabase("resultadofacildb");
            examesCollection = database.GetCollection<Exame>("Exames");
        }

        // GET: /<controller>/
        public IActionResult Index(string pesquisa, int? page)
        {
            if (string.IsNullOrEmpty(pesquisa))
                pesquisa = string.Empty;

            var pageNumber = page ?? 1;
            var pageSize = 10;

            var filter = Builders<Exame>.Filter.Regex("Descricao", "/" + pesquisa + "/");
            var totalItemCount = examesCollection.Find(filter).Count();
            var exames = examesCollection.Find(filter).Skip(pageNumber > 0 ? ((pageNumber - 1) * pageSize) : 0).Limit(pageSize).ToList();

            ViewData["PageNumber"] = pageNumber;
            ViewData["PageSize"] = pageSize;
            ViewData["TotalItemCount"] = totalItemCount;

            return View(exames);
        }

        public IActionResult Create()
        {
            return View();
        }
        // POST: Exames/Create
        [HttpPost]
        public async Task<ActionResult> Create(Exame exame)
        {
            await examesCollection.InsertOneAsync(exame);
            return RedirectToAction(nameof(Index));
        }
    }
}
