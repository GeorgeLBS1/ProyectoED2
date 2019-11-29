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
    public class ContactosController : ControllerBase
    {
        private readonly ContactosService _contactosService;
        public ContactosController(ContactosService contactosService)
        {
            _contactosService = contactosService;
        }
        [HttpGet("{nombre}")]
        public ActionResult<Contactos> Get(string nombre)
        {
            var user = _contactosService.Get(nombre);
            if (user == null)
            {
                return NotFound();
            }
            return user;
        }
        


        //Creación de un nuevo usuario
        [HttpPost]
        public ActionResult<Contactos> Create(Contactos contactos)
        {
            _contactosService.Create(contactos);
            return StatusCode(201);
        }

        [HttpPut("{Nombre}")]
        public IActionResult Update(string Nombre, Contactos contactos) //Editar alguna pizza ya creada
        {
            var contacts = _contactosService.Get(Nombre);
            if (contacts == null)
            {
                return NotFound();
            }
            contactos.Id = contacts.Id;
            _contactosService.Update(Nombre, contactos);
            return NoContent();
        }

            /*[HttpPost("{user}")]*/
            //public IActionResult Credate(string user, Usuario nuevoContacto)
            //{
            //    Contactos contactosDeEsteUsuario = _contactosService.Get(user); //Lista usada para evitar que existan usuarios repetidos        
            //    if (contactosDeEsteUsuario.ContactosAmigos.Count == 0)
            //    {
            //        Contactos nuevo = new Contactos();
            //        nuevo.OwnerNickName = user;
            //        nuevo.ContactosAmigos.Add(nuevoContacto.NickName);
            //        _contactosService.Create(nuevo);
            //        return StatusCode(201);

            //    }            
            //    else
            //    {
            //        if (!contactosDeEsteUsuario.ContactosAmigos.Contains(nuevoContacto.NickName))
            //        {
            //            contactosDeEsteUsuario.ContactosAmigos.Add(nuevoContacto.NickName);
            //            _contactosService.Update(user, contactosDeEsteUsuario);
            //            return StatusCode(201);
            //        }
            //        else
            //        {
            //            return StatusCode(400);
            //        }

            //    }


            //}
        }
}