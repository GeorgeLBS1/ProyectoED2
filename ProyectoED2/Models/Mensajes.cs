using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Chat_API.Models
{
    public class Mensajes
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } //Id que maneja mongodb
        public string Emisor { get; set; } //Emisor del mensaje
        public string Receptor { get; set; } //Receptor del mensaje
        public string Cuerpo { get; set; } //El mensaje en sí 
        public string Visible { get; set; } //identificador para borrar o no el mensaje
        public DateTime Date { get; set; }
    }
}
