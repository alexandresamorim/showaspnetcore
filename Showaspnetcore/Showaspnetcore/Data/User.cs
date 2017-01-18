using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.MongoDB;
using Showaspnetcore.Models.Enums;

namespace Showaspnetcore.Data
{
    public class User : IdentityUser
    {
        public User() : base()
        {

            UserPermissions = new HashSet<UserPermission>();

        }

        public UserType UserType { get; set; }

        public string Nome { get; set; }
        public string Cep { get; set; }
        public string Logradouro { get; set; }
        public string Bairro { get; set; }
        public string Numero { get; set; }
        public string Cidade { get; set; }
        public string Uf { get; set; }
        public HashSet<UserPermission> UserPermissions { get; set; }
    }
}
