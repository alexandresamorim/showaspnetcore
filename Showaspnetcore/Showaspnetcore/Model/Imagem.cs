using System;
using MongoDB.Bson;

namespace Showaspnetcore.Model
{
    public class Imagem
    {
        public Imagem()
        {
            ImagemGuid = Guid.NewGuid();
        }

        public ObjectId ObjectId { get; set; }
        public Guid ImagemGuid { get; set; }
        public Guid ResultadoExameGuid { get; set; }
        public string Local { get; set; }
        public long Length { get; set; }
        public string Formato { get; set; }
        public string Descricao { get; set; }
    }
}