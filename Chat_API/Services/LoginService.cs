using Chat_API.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat_API.Services
{
    public class LoginService
    {
        private readonly IMongoCollection<Usuario> _usuario; //Todas las operaciones se harán en base a esta variable
        public LoginService(IChatDatabaseSettings settings) //Connección con la base de datos 
        {
            var client = new MongoClient(settings.ConnectionString); //Conectarse al cluster
            settings.ChatCollectionName = "UsersData";
            var database = client.GetDatabase(settings.DatabaseName); //Obtener la base
            _usuario = database.GetCollection<Usuario>(settings.ChatCollectionName); //Obtener la colección en este caso la que contiene los datos de los usuarios
                        
        }
        public Usuario Get(string nombre) =>        
            _usuario.Find<Usuario>(usuario => usuario.Nombre == nombre).FirstOrDefault();     


    }
}
