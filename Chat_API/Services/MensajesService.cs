using Chat_API.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat_API.Services
{
    public class MensajesService
    {
        private readonly IMongoCollection<Mensajes> _mensajes; //Todas las operaciones se harán en base a esta variable
        public MensajesService(IChatDatabaseSettings settings) //Connección con la base de datos 
        {
            var client = new MongoClient(settings.ConnectionString); //Conectarse al cluster
            settings.ChatCollectionName = "Mensajes";
            var database = client.GetDatabase(settings.DatabaseName); //Obtener la base
            _mensajes = database.GetCollection<Mensajes>(settings.ChatCollectionName); //Obtener la colección en este caso la que contiene los datos de los usuarios

        }

        public Mensajes GetMensajes(string mensaje) =>
            _mensajes.Find<Mensajes>(x => x.Cuerpo == mensaje).FirstOrDefault();

        public Mensajes GetFile(string name) =>
            _mensajes.Find<Mensajes>(x => x.NombreArchivo == name).FirstOrDefault();

        public List<Mensajes> Get(string nombre) => //Obtener los mensajes de cierta conversación
            _mensajes.Find<Mensajes>(usuario => usuario.Emisor == nombre || usuario.Receptor == nombre).ToList();

        public List<Mensajes> BuscarMensaje(string text) =>
            _mensajes.Find(msg => msg.Cuerpo.Contains(text)).ToList();

        public List<Mensajes> Get() =>
            _mensajes.Find(usuario => true).ToList();

        public Mensajes Create(Mensajes msj) //Añadir nuevos usuarios al sistema
        {
            //AÑADIR EL CIFRADO DE LA CONTRASEÑA AQUÍ            
            _mensajes.InsertOne(msj);
            return msj;
        }

        public void BorrarParcial(string cuerpo, Mensajes user) => //Borrar los mensajes solo para una persona
            _mensajes.ReplaceOne(x => x.Cuerpo == cuerpo && x.Emisor == user.Emisor && x.Receptor == user.Receptor, user);

        public void Remove(Mensajes mensajes) => //Eliminar buscando el emisor
            _mensajes.DeleteOne(x => x.Emisor == mensajes.Emisor);

        public void Remove(string Emisor) => //Eliminar buscando el emisor
            _mensajes.DeleteOne(x => x.Emisor == Emisor);
    }
}
