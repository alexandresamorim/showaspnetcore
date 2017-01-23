using MongoDB.Bson;

namespace Showaspnetcore.Data
{
    public class Servico
    {
        public ObjectId Id { get; set; }
        public string Descricao { get; set; }
        public string Conteudo { get; set; }
        public string LocalIcon { get; set; }
        public string Type { get; set; }
    }
}
