﻿using System;
using MongoDB.Bson;

namespace Showaspnetcore.Model
{
    public class Paciente
    {
        public Paciente()
        {
            PacienteGuid = Guid.NewGuid();
        }
        public ObjectId Id { get; set; }
        public Guid PacienteGuid { get; set; }
        public string Name { get; set; }
        public string Proprietario { get; set; }
        public DateTime DataCadastro { get; set; }
    }
}