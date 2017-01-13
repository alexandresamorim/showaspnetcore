using System;
using MongoDB.Bson;

namespace Showaspnetcore.Model
{
    public class Usuario
    {
        public ObjectId ObjectId { get; set; }
        public Guid EmpresaGuid { get; set; }
        public string Login { get; set; }
        public string Senha { get; set; }
    }
}