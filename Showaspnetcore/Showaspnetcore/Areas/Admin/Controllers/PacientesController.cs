using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using MongoDB.Driver;
using Showaspnetcore.Data;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Showaspnetcore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "ADMINISTRATOR")]
    public class PacientesController : Controller
    {
        private readonly IMongoCollection<Paciente> _pacienteCollection;

        public PacientesController(MongoClient client)
        {
            var database = client.GetDatabase("resultadofacildb");
            _pacienteCollection = database.GetCollection<Paciente>("Pacientes");
        }

        // GET: /<controller>/
        public IActionResult Index(string pesquisa, int? page)
        {
            if (string.IsNullOrEmpty(pesquisa))
                pesquisa = string.Empty;

            var pageNumber = page ?? 1;
            var pageSize = 10;

            var filter = Builders<Paciente>.Filter.Regex("Name", "/" + pesquisa + "/");
            var totalItemCount = _pacienteCollection.Find(filter).Count();
            var pacientes = _pacienteCollection.Find(filter).Skip(pageNumber > 0 ? ((pageNumber - 1) * pageSize) : 0).Limit(pageSize).ToList();
            
            ViewData["PageNumber"] = pageNumber;
            ViewData["PageSize"] = pageSize;
            ViewData["TotalItemCount"] = totalItemCount;

            //var pacientes = paciente.Find(FilterDefinition<Paciente>.Empty).ToList();
            return View(pacientes);
        }

        public ActionResult Create()
        {
            var paciente = new Paciente();
            paciente.ChaveAcesso = AlfanumericoAleatorio(8);
            return View(paciente);
        }

        // POST: People/Create
        [HttpPost]
        public async Task<ActionResult> Create(Paciente person)
        {
            await _pacienteCollection.InsertOneAsync(person);
            return RedirectToAction(nameof(Index));
        }

        public ActionResult Detail(Guid pacienteGuid)
        {
            var filterBuilder = Builders<Paciente>.Filter;
            var filterOn = filterBuilder.Eq("PacienteGuid", pacienteGuid);

            var pacientes = _pacienteCollection.Find(filterOn).FirstOrDefault();

            return View(pacientes);
        }

        // POST: People/Create
        [HttpPost]
        public async Task<ActionResult> Detail(Paciente person)
        {

            var filter = Builders<Paciente>.Filter.Eq("PacienteGuid", person.PacienteGuid);
            var update = Builders<Paciente>.Update
                .Set("Name", person.Name)
                .Set("Proprietario", person.Responsavel);

            var result = await _pacienteCollection.UpdateOneAsync(filter, update);
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
            var deleted = await _pacienteCollection.DeleteOneAsync(filter);

            return RedirectToAction(nameof(Index));
        }

        public async Task<ActionResult> Pesquisa(string pesquisa)
        {
            var filter = Builders<Paciente>.Filter.Regex("Name", pesquisa);
            var find = await _pacienteCollection.Find(filter).ToListAsync();

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

        public static string AlfanumericoAleatorio(int tamanho)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, tamanho)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
            return result;
        }
    }
}
