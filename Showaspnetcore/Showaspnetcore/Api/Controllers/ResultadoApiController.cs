using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.MongoDB;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Showaspnetcore.Data;
using Showaspnetcore.Model;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Showaspnetcore.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize]
    public class ResultadoApiController : Controller
    {
        private readonly IMongoCollection<ResultadoExame> resultadoCollection;
        private UserManager<IdentityUser> _userManager;
        public ResultadoApiController(MongoClient client, UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            var database = client.GetDatabase("resultadofacildb");
            resultadoCollection = database.GetCollection<ResultadoExame>("ResultadoExames");
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var usuarioId = _userManager.GetUserId(User);

            var filter = Builders<ResultadoExame>.Filter.Regex("UsuarioId", usuarioId);
            var resultados = resultadoCollection.Find(filter).ToList();

            return new JsonResult(resultados);
        }
    }
}
