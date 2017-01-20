using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.MongoDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Net.Http.Headers;
using MongoDB.Driver;
using Showaspnetcore.Data;


// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Showaspnetcore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ResultadoController : Controller
    {
        private readonly IMongoCollection<ResultadoExame> resultadoCollection;
        private readonly IMongoCollection<Paciente> pacienteCollection;
        private UserManager<IdentityUser> _userManager;

        private IHostingEnvironment _env;

        public ResultadoController(MongoClient client, UserManager<IdentityUser> userManager,
            IHostingEnvironment env)
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
  
            var usuarioId = _userManager.GetUserName(User);

            var filter = Builders<ResultadoExame>.Filter.Regex("UsuarioId", usuarioId);
            var resultados = resultadoCollection.Find(filter).ToList();

            return View(resultados);
        }

        public ActionResult Create()
        {
            var filterPaciente = Builders<Paciente>.Filter.Regex("Name", "//");
            ViewBag.pacientesViewList =
                pacienteCollection.Find(filterPaciente).ToList().Select(x => new SelectListItem()
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
            person.Paciente = pacienteCollection.Find(filterPaciente).FirstOrDefault();

            person.UsuarioId = _userManager.GetUserName(User);
            
            await resultadoCollection.InsertOneAsync(person);
            return RedirectToAction("Edit", new { resultadoGuid = person.ResultadoExameGuid });
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
        public ActionResult Download(Guid imagemGuid, Guid resultadoGuid)
        {
            var filter = Builders<ResultadoExame>.Filter.Eq("ResultadoExameGuid", resultadoGuid);
            var resultado = resultadoCollection.Find(filter).FirstOrDefault();

            var imagem = resultado.Imagens.FirstOrDefault(x => x.ImagemGuid == imagemGuid);

            var pathFull = GetPathAndFilename(imagem.Local, resultado.ResultadoExameGuid.ToString());

            HttpContext.Response.ContentType = imagem.Formato;
            FileContentResult result = new FileContentResult(System.IO.File.ReadAllBytes(pathFull), imagem.Formato)
            {
                FileDownloadName = imagem.Local
            };

            return result;
        }

        [HttpPost]
        public async Task<ActionResult> UploadFiles(IList<IFormFile> files, ResultadoExame resultado)
        {
            var fals = Request.Form.Files;
            var filter = Builders<ResultadoExame>.Filter.Eq("ResultadoExameGuid", resultado.ResultadoExameGuid);
            var exame = resultadoCollection.Find(filter).FirstOrDefault();
            var descricao = Request.Form["txtDescricao"].ToString();

            foreach (IFormFile file in fals)
            {
                string filename = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');

                filename = this.EnsureCorrectFilename(filename);

                var pathInc = this.GetPathAndFilename(filename, resultado.ResultadoExameGuid.ToString());
                var pasta = GetPathFolter(resultado.ResultadoExameGuid.ToString());
                if (!Directory.Exists(pasta))
                {
                    Directory.CreateDirectory(pasta);
                }

                using (FileStream output = System.IO.File.Create(pathInc))
                    file.CopyTo(output);

                exame.Imagens.Add(new Imagem()
                {
                    ResultadoExameGuid = resultado.ResultadoExameGuid,
                    Descricao = descricao,
                    Local = "/exames/" + resultado.ResultadoExameGuid +"/"+ filename,
                    FileName = filename,
                    Length = file.Length,
                    Formato = file.ContentType
                });

                var filterUpdate = Builders<ResultadoExame>.Filter.Eq("ResultadoExameGuid", resultado.ResultadoExameGuid);
                var update = Builders<ResultadoExame>.Update
                    .Set("Imagens", exame.Imagens);

                var result = await resultadoCollection.UpdateOneAsync(filterUpdate, update);

            }
            return RedirectToAction("Edit", new {resultadoGuid = resultado.ResultadoExameGuid});
        }
        
        private string EnsureCorrectFilename(string filename)
        {
            if (filename.Contains("\\"))
                filename = filename.Substring(filename.LastIndexOf("\\") + 1);

            return filename;
        }

        private string GetPathAndFilename(string filename, string pathUser)
        {
            return _env.WebRootPath + "\\exames\\" + pathUser + "\\" + filename;
        }
        private string GetPathFolter(string pathUser)
        {
            return _env.WebRootPath + "\\exames\\" + pathUser;
        }
        public IActionResult Edit(Guid resultadoGuid)
        {
            var filter = Builders<ResultadoExame>.Filter.Eq("ResultadoExameGuid", resultadoGuid);
            var resultado = resultadoCollection.Find(filter).FirstOrDefault();

            ViewBag.PastaRoot = _env.WebRootPath;
            return View(resultado);
        }

        public IActionResult DeleteFile(Guid imagemGuid, Guid resultadoGuid)
        {
            var filter = Builders<ResultadoExame>.Filter.Eq("ResultadoExameGuid", resultadoGuid);
            var resultado = resultadoCollection.Find(filter).FirstOrDefault();

            var imagem = resultado.Imagens.FirstOrDefault(x => x.ImagemGuid == imagemGuid);

            resultado.Imagens.Remove(imagem);


            var filterUpdate = Builders<ResultadoExame>.Filter.Eq("ResultadoExameGuid", resultado.ResultadoExameGuid);
            var update = Builders<ResultadoExame>.Update
                .Set("Imagens", resultado.Imagens);

            resultadoCollection.UpdateOneAsync(filterUpdate, update);

            var pathFull = GetPathAndFilename(imagem.Local, resultado.ResultadoExameGuid.ToString());
            if (System.IO.File.Exists(pathFull))
                System.IO.File.Delete(pathFull);

            TempData["NossoErro"] = "Arquivo excluido com sucesso";


            return RedirectToAction("Edit", new {resultadoGuid = resultado.ResultadoExameGuid});
        }

        public IActionResult AlterarDescricao(Guid imagemGuid, Guid resultadoExameGuid, string descricao)
        {
            var filter = Builders<ResultadoExame>.Filter.Eq("ResultadoExameGuid", resultadoExameGuid);
            var resultado = resultadoCollection.Find(filter).FirstOrDefault();
            var imagemResult = resultado.Imagens.FirstOrDefault(x => x.ImagemGuid == imagemGuid);

            imagemResult.Descricao = descricao;

            return RedirectToAction("Edit", new { resultadoGuid = resultadoExameGuid });
        }
    }
}
