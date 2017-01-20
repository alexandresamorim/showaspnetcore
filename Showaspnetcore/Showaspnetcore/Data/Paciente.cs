using System;
using MongoDB.Bson;

namespace Showaspnetcore.Data
{
    public class Paciente
    {
        public Paciente()
        {
            PacienteGuid = Guid.NewGuid();
            DataCadastro = DateTime.Now;
        }
        public ObjectId Id { get; set; }
        public Guid PacienteGuid { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string ChaveAcesso { get; set; }
        public string Responsavel { get; set; }
        public DateTime DataCadastro { get; set; }
    }
}