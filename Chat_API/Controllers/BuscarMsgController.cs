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
    public class BuscarMsgController : ControllerBase
    {
        private readonly MensajesService _mensajesService;
        public BuscarMsgController(MensajesService mensajeService)
        {
            _mensajesService = mensajeService;
        }
        [HttpGet("{nombre}")] //Obtener todos los mensajes de cierto usuario
        public ActionResult<List<Mensajes>> Get(string nombre) =>
           _mensajesService.BuscarMensaje(nombre);
    }
}