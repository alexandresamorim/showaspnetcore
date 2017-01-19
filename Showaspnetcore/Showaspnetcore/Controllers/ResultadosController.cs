using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.MongoDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MongoDB.Driver;
using Showaspnetcore.Data;


// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Showaspnetcore.Controllers
{
    [Authorize]
    public class ResultadosController : Controller
    {
        private readonly IMongoCollection<ResultadoExame> resultadoCollection;
        private readonly IMongoCollection<Paciente> pacienteCollection;
        private UserManager<IdentityUser> _userManager;
        private readonly RoleManager<Role> _roleManager;

        private IHostingEnvironment _env;

        public ResultadosController(MongoClient client, UserManager<IdentityUser> userManager,
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

            person.UsuarioId = _userManager.GetUserId(User);
            
            await resultadoCollection.InsertOneAsync(person);
            return View(Edit(person.ResultadoExameGuid));
        }

        [HttpGet]
        public virtual  FileResult Download(Guid imagemGuid, Guid resultadoGuid)
        {
            var filter = Builders<ResultadoExame>.Filter.Eq("ResultadoExameGuid", resultadoGuid);
            var resultado = resultadoCollection.Find(filter).FirstOrDefault();

            var imagem = resultado.Imagens.FirstOrDefault(x => x.ImagemGuid == imagemGuid);

            var pathFull = GetPathAndFilename(imagem.FileName, resultadoGuid.ToString());

            return File(imagem.Local, imagem.Formato, imagem.FileName);

            /*
            FileContentResult result = new FileContentResult(System.IO.File.ReadAllBytes(pathFull), imagem.Formato)
            {
                FileDownloadName = imagem.FileName
            };

            return result;
            */
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

        public IActionResult Edit(Guid resultadoGuid)
        {
            var filter = Builders<ResultadoExame>.Filter.Eq("ResultadoExameGuid", resultadoGuid);
            var resultado = resultadoCollection.Find(filter).FirstOrDefault();

            ViewBag.PastaRoot = _env.WebRootPath;
            return View(resultado);
        }

    }
}
