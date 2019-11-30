using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chat_API.Models;
using Chat_API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chat_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MensajesController : ControllerBase
    {
        private readonly MensajesService _mensajesService;
        public MensajesController(MensajesService mensajeService)
        {
            _mensajesService = mensajeService;
        }
        [HttpGet]
        public ActionResult<List<Mensajes>> Get() =>
           _mensajesService.Get();

       

        [HttpGet("{nombre}")] //Obtener todos los mensajes de cierto usuario
        public ActionResult<List<Mensajes>> Get(string nombre) =>
            _mensajesService.Get(nombre);

        [HttpPost]
        public ActionResult<Mensajes> Create(Mensajes msg) //Crear un mensaje
        {
            _mensajesService.Create(msg);
            return NoContent();
        }

        [HttpDelete("{texto}")]
        public IActionResult Delete(string texto) //Borrar un mensaje para ambos
        {
            var mensaje = _mensajesService.GetMensajes(texto);
            if (mensaje == null)
            {
                return NotFound();
            }
            _mensajesService.Remove(mensaje.Emisor);
            return NoContent();
        }

        [HttpPut("{eliminar}")]
        public IActionResult Update(string eliminar, Mensajes mensaje)
        {
            var msg = _mensajesService.GetMensajes(eliminar);
            if (msg == null)
            {
                return NotFound();
            }
            mensaje.Id = msg.Id;
            _mensajesService.BorrarParcial(eliminar, mensaje);
            return NoContent();
        }
        


    }
}