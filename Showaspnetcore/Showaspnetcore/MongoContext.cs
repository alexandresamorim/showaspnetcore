using System;
using MongoDB.Driver;
using Showaspnetcore.Data;
using Showaspnetcore.Model;

namespace Showaspnetcore
{
    public class MongoContext : IDisposable
    {
        private const string StringDeConexao = "mongodb://192.168.229.129/EstudoMongoDB";
        public String DataBaseName = "ResultadoFacil";
        private IMongoDatabase _db;

        public MongoContext()
        {
            var client = new MongoClient(StringDeConexao);
            _db = client.GetDatabase(DataBaseName);

        }
        public IMongoCollection<Paciente> Pacientes
        {
            get
            {
                var paciente = _db.GetCollection<Paciente>("Pacientes");
                return paciente;
            }
        }
        public IMongoCollection<Paciente> ResultadoExamesCollection
        {
            get
            {
                var resultados = _db.GetCollection<Paciente>("ResultadoExames");
                return resultados;
            }
        }
        public void Dispose()
        {

        }
    }
}