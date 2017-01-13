using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.Identity.MongoDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Net.Http.Headers;
using MongoDB.Driver;
using Showaspnetcore.Model;


// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Showaspnetcore.Controllers
{
    [Authorize]
    public class ResultadosController : Controller
    {
        private readonly IMongoCollection<ResultadoExame> resultadoCollection;
        private readonly IMongoCollection<Paciente> pacienteCollection;
        private UserManager<MongoIdentityUser> _userManager;

        private IHostingEnvironment _env;
        public ResultadosController(MongoClient client, UserManager<MongoIdentityUser> userManager, IHostingEnvironment env)
        {
            _userManager = userManager;
            var database = client.GetDatabase("resultadofacildb");
            resultadoCollection = database.GetCollection<ResultadoExame>("ResultadoExames");
            pacienteCollection = database.GetCollection<Paciente>("Pacientes");
            _env = env;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            var usuarioId = _userManager.GetUserId(User);
            
            var filter = Builders<ResultadoExame>.Filter.Regex("UsuarioId", usuarioId);
            var resultados = resultadoCollection.Find(filter).ToList();

            return View(resultados);
        }

        public ActionResult Create()
        {
            var filterPaciente = Builders<Paciente>.Filter.Regex("Name", "//");
            ViewBag.pacientesViewList = pacienteCollection.Find(filterPaciente).ToList().Select(x => new SelectListItem()
            {
                Value = x.PacienteGuid.ToString(),
                Text = x.Name
            });

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(ResultadoExame person)
        {
            var filterPaciente = Builders<Paciente>.Filter.Eq("PacienteGuid", person.PacienteGuid);
            person.Paciente =  pacienteCollection.Find(filterPaciente).FirstOrDefault();

            person.UsuarioId = _userManager.GetUserId(User);


            await resultadoCollection.InsertOneAsync(person);
            return View(Edit(person.ResultadoExameGuid));
        }

        public async void UploadMultiple(ICollection<IFormFile> files)
        {
            string uploads = Path.Combine("Data", "Imagens");

            
            string posts = Path.Combine("Data", "Posts");
            var _files = Directory.EnumerateFiles(posts);

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    await file.SaveAsAsync(Path.Combine(uploads, fileName));
                }
            }
        }

        [HttpGet]
        public FileStreamResult Download(int id)
        {
            var fileDescription = "";//fileRepository.GetFileDescription(id);

            var path = "";
            var stream = new FileStream(path, FileMode.Open);
            return File(stream, fileDescription);
        }

        [HttpPost]
        public async Task<ActionResult> UploadFiles(IList<IFormFile> files, ResultadoExame resultado)
        {
            var filter = Builders<ResultadoExame>.Filter.Eq("ResultadoExameGuid", resultado.ResultadoExameGuid);
            var exame = resultadoCollection.Find(filter).FirstOrDefault();

            foreach (IFormFile file in files)
            {
                string filename = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');

                filename = this.EnsureCorrectFilename(filename);

                var pathInc = this.GetPathAndFilename(filename);

                using (FileStream output = System.IO.File.Create(pathInc))
                    file.CopyTo(output);

                exame.Imagens.Add(new Imagem()
                {
                    ResultadoExameGuid = resultado.ResultadoExameGuid,
                    Local = filename,
                    Length = file.Length,
                    Formato = file.ContentType
                });

                var filterUpdate = Builders<ResultadoExame>.Filter.Eq("ResultadoExameGuid", resultado.ResultadoExameGuid);
                var update = Builders<ResultadoExame>.Update
                    .Set("Imagens", exame.Imagens);

                var result =  resultadoCollection.UpdateOneAsync(filterUpdate, update);

            }
            return View(Edit(resultado.ResultadoExameGuid));
        }
        private string EnsureCorrectFilename(string filename)
        {
            if (filename.Contains("\\"))
                filename = filename.Substring(filename.LastIndexOf("\\") + 1);

            return filename;
        }

        private string GetPathAndFilename(string filename)
        {

            return _env.WebRootPath + "\\exames\\" + filename;
        }

        public IActionResult Edit(Guid resultadoGuid)
        {
            var filter = Builders<ResultadoExame>.Filter.Eq("ResultadoExameGuid", resultadoGuid);
            var resultado = resultadoCollection.Find(filter).FirstOrDefault();

            return View(resultado);
        }
    }
}
