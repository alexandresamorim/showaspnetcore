using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Showaspnetcore.Model;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Showaspnetcore.Controllers
{
    public class PacientesController : Controller
    {
        private IMongoCollection<Paciente> paciente;
        public PacientesController(MongoClient client)
        {
            var database = client.GetDatabase("resultadofacildb");
            paciente = database.GetCollection<Paciente>("Pacientes");
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            var pacientes = paciente.Find(FilterDefinition<Paciente>.Empty).ToList();
            return View(pacientes);
        }

        public ActionResult Create()
        {
            return View();
        }
        // POST: People/Create
        [HttpPost]
        public async Task<ActionResult> Create(Paciente person)
        {
            await paciente.InsertOneAsync(person);
            return RedirectToAction(nameof(Index));
        }
    }
}
