﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.MongoDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Showaspnetcore.Models;

namespace Showaspnetcore.Controllers
{
    public class InstallController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly ILogger _logger;

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private readonly MongoClient _client;
        public InstallController(
            MongoClient client,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILoggerFactory loggerFactory)
        {
            _client = client;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = loggerFactory.CreateLogger<InstallController>();
        }

        public IActionResult Index()
        {

            var dbContext = _client.GetDatabase("resultadofacildb");
            var userCollection = dbContext.GetCollection<IdentityUser>("users");

            var filter = new BsonDocument();
            var users = userCollection.Find(filter).ToList();

            if (users.Count > 0)
                return RedirectToAction("~/Home/Index");


            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(InstallationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var roles = new List<IdentityRole>()
                {
                    new IdentityRole() {Name = "Administrator", NormalizedName = "Administrador"},
                    new IdentityRole() {Name = "Cliente", NormalizedName = "Cliente"},

                    new IdentityRole() {Name = "CreateExame", NormalizedName = "Direitos para criar exames"},
                    new IdentityRole() {Name = "AlterExame", NormalizedName = "Direitos para alterar exames"}
                };

                foreach (var role in roles)
                {
                    await  _roleManager.CreateAsync(role);
                }

                //_context.SaveChanges();

                //var roles = new List<Role>()
                //{
                //    new Role() { Name = "Administrator" }
                //};

                var user = new IdentityUser()
                {
                    UserName = model.UserName,
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    var roleAdm = roles.FirstOrDefault(x => x.Name == "Administrator");

                    await _userManager.AddToRoleAsync(user, roleAdm.Name);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    AddErrors(result);
                }
            }

            return View(model);
        }
    }
}