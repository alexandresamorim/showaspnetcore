using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MongoDB.Bson;
using Showaspnetcore.Models.Enums;

namespace Showaspnetcore.Data
{
    public class ResultadoExame
    {
        public ResultadoExame()
        {
            ResultadoExameGuid = Guid.NewGuid();
            Data = DateTime.Now.Date;
            Imagens = new Collection<Imagem>();
        }
        public ObjectId Id { get; set; }
        public Guid ResultadoExameGuid { get; set; }
        public string UsuarioId { get; set; }
        public string Exame { get; set; }
        public string Resultado { get; set; }
        public DateTime Data { get; set; }
        public Guid PacienteGuid { get; set; }
        public Paciente Paciente { get; set; }
        public DateTime DataPrevista { get; set; }
        public ResultadoStatusEnumView Status { get; set; }
        public ICollection<Imagem> Imagens { get; set; }
    }
}