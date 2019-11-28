using Chat_API.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Chat_API.Services
{
    public class ContactosService
    {
        private readonly IMongoCollection<Contactos> _usuario; //Todas las operaciones se harán en base a esta variable
        public ContactosService(IChatDatabaseSettings settings) //Connección con la base de datos 
        {
            var client = new MongoClient(settings.ConnectionString); //Conectarse al cluster
            settings.ChatCollectionName = "Contactos";
            var database = client.GetDatabase(settings.DatabaseName); //Obtener la base
            _usuario = database.GetCollection<Contactos>(settings.ChatCollectionName); //Obtener la colección en este caso la que contiene los datos de los usuarios

        }
        public Contactos Get(string nombre) =>
            _usuario.Find<Contactos>(usuario => usuario.OwnerNickName == nombre).FirstOrDefault();
        public Contactos Create(Contactos user) //Añadir nuevos usuarios al sistema
        {
            //AÑADIR EL CIFRADO DE LA CONTRASEÑA AQUÍ            
            _usuario.InsertOne(user);
            return user;
        }
        public void Update(string id, Contactos user) => //Actualizar registros
           _usuario.ReplaceOne(x => x.OwnerNickName == id, user);
    }
}
