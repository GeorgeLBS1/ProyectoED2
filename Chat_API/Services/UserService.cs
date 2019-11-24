using Chat_API.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat_API.Services
{
    public class UserService
    {
        private readonly IMongoCollection<Usuario> _usuario; //Todas las operaciones se harán en base a esta variable
        public UserService(IChatDatabaseSettings settings) //Connección con la base de datos 
        {
            var client = new MongoClient(settings.ConnectionString); //Conectarse al cluster
            settings.ChatCollectionName = "UsersData";
            var database = client.GetDatabase(settings.DatabaseName); //Obtener la base
            _usuario = database.GetCollection<Usuario>(settings.ChatCollectionName); //Obtener la colección en este caso la que contiene los datos de los usuarios
                        
        }
        public Usuario Get(string nombre) =>        
            _usuario.Find<Usuario>(usuario => usuario.NickName == nombre).FirstOrDefault();
        public List<Usuario> Get() =>
            _usuario.Find(usuario => true).ToList();
        public Usuario Create(Usuario user) //Añadir nuevos usuarios al sistema
        {
            //AÑADIR EL CIFRADO DE LA CONTRASEÑA AQUÍ            
            _usuario.InsertOne(user);
            return user;

        }
        public void Update(string id, Usuario user) => //Actualizar registros
            _usuario.ReplaceOne(x => x.NickName == id, user);

        public void Remove(Usuario user) => //Eliminar usuarioi
            _usuario.DeleteOne(x => x.NickName == user.NickName);

        public void Remove(string id) => //Eliminar buscando el Id
            _usuario.DeleteOne(x => x.NickName == id);

    }
}
