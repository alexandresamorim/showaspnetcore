using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using MongoDB.Driver;
using Showaspnetcore.Model;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Showaspnetcore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PacientesController : Controller
    {
        private IMongoCollection<Paciente> paciente;

        public PacientesController(MongoClient client)
        {
            var database = client.GetDatabase("resultadofacildb");
            paciente = database.GetCollection<Paciente>("Pacientes");
        }

        // GET: /<controller>/
        public IActionResult Index(string pesquisa)
        {
            if (string.IsNullOrEmpty(pesquisa))
                pesquisa = "";

            var filter = Builders<Paciente>.Filter.Regex("Name", "/" + pesquisa + "/");
            var pacientes = paciente.Find(filter).ToList();

            //var pacientes = paciente.Find(FilterDefinition<Paciente>.Empty).ToList();
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

        public ActionResult Detail(Guid pacienteGuid)
        {
            var filterBuilder = Builders<Paciente>.Filter;
            var filterOn = filterBuilder.Eq("PacienteGuid", pacienteGuid);

            var pacientes = paciente.Find(filterOn).FirstOrDefault();

            return View(pacientes);
        }

        // POST: People/Create
        [HttpPost]
        public async Task<ActionResult> Detail(Paciente person)
        {

            var filter = Builders<Paciente>.Filter.Eq("PacienteGuid", person.PacienteGuid);
            var update = Builders<Paciente>.Update
                .Set("Name", person.Name)
                .Set("Proprietario", person.Proprietario);

            var result = await paciente.UpdateOneAsync(filter, update);
            if (result.ModifiedCount > 0)
            {
                return RedirectToAction(nameof(Index));
            }

            ViewData["Messagem"] = "Erro ao alterar o cadastro";
            return View();

        }

        public async Task<ActionResult> Delete(Guid pacienteGuid)
        {
            var filter = Builders<Paciente>.Filter.Eq("PacienteGuid", pacienteGuid);
            var deleted = await paciente.DeleteOneAsync(filter);

            return RedirectToAction(nameof(Index));
        }

        public async Task<ActionResult> Pesquisa(string pesquisa)
        {
            var filter = Builders<Paciente>.Filter.Regex("Name", pesquisa);
            var find = await paciente.Find(filter).ToListAsync();

            return Json(find);
        }



        [HttpPost]
        public async Task<IActionResult> UploadMultiple(ICollection<IFormFile> files)
        {
            var uploads = Path.Combine("\\var\\www\\", "uploads");
            foreach (var file in files) 
            {
                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    await file.SaveAsAsync(Path.Combine(uploads, fileName));
                }
            }


            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public FileStreamResult Download(int id)
        {
            var fileDescription = "";//fileRepository.GetFileDescription(id);

            var path = "";
            var stream = new FileStream(path, FileMode.Open);
            return File(stream, fileDescription);
        }
    }
}
