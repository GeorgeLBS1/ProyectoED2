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
    public class FilesController : ControllerBase
    {
        private readonly MensajesService _mensajesService;
        public FilesController(MensajesService mensajeService)
        {
            _mensajesService = mensajeService;
        }
        [HttpGet("{nombre}")]
        public ActionResult<Mensajes> Get(string nombre)
        {
            var user = _mensajesService.GetFile(nombre);
            if (user == null)
            {
                return NotFound();
            }
            return user;
        }

    }
}