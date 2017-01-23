using MongoDB.Bson;

namespace Showaspnetcore.Data
{
    public class Exame
    {
        public ObjectId Id { get; set; }
        public string Descricao { get; set; }
    }
}