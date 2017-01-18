using System;

namespace Showaspnetcore.Data
{
    public class Cliente
    {
        public Cliente()
        {
            ClienteGuid = Guid.NewGuid();
        }
        public object Id { get; set; }
        public Guid ClienteGuid { get; set; }
        public string Nome { get; set; }
    }
}