using System;
using MongoDB.Bson;
using Showaspnetcore.Interface;

namespace Showaspnetcore.Model
{
    public class Paciente
    {
        public Paciente()
        {
            PacienteGuid = Guid.NewGuid();

        }
        public object Id { get; set; }
        public Guid PacienteGuid { get; set; }
        public string Name { get; set; }
        public Cliente Cliente { get; set; }
        public string Proprietario { get; set; }
        public DateTime DataCadastro { get; set; }
    }
}