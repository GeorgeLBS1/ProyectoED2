using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Chat_API.Models
{
    public class Usuario
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public int Code { get; set; } //Para diffie helfman
        public string NickName { get; set; } //Nombre del usuario o nickname
        public string Name { get; set; } //identificador para el registro
        public string Password { get; set; } //contraseña a guardar en la base de datos
    }
}
