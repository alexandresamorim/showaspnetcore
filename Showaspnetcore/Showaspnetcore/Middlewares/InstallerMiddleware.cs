using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;
using Showaspnetcore.Data;

namespace Showaspnetcore.Middlewares
{
    public class InstallerMiddleware
    {
        private readonly RequestDelegate _next;

        public InstallerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context, MongoClient client)
        {
            var dbContext = client.GetDatabase("resultadofacildb");
            var userCollection = dbContext.GetCollection<IdentityUser>("users");

            var filter = new BsonDocument();
            var users = userCollection.Find(filter).ToList();
            if (users.Count == 0)
            {
                if (!context.Request.Path.ToString().Contains("/Install"))
                {
                    context.Request.Path = "/Install";
                }
            }
            else
            {
                if (context.Request.Path.ToString().Contains("/Install"))
                {
                    context.Request.Path = "/";
                }
            }

            return _next(context);
        }
    }
}
