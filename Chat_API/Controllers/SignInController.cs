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
    public class SignInController : ControllerBase
    {
        private readonly UserService _loginService;
        public SignInController(UserService loginService) //Constructor
        {
            _loginService = loginService;
        }

        //Creación de un nuevo usuario
        [HttpPost]
        public ActionResult<Usuario> Create(Usuario user)
        {
            List<Usuario> usuarios = _loginService.Get(); //Lista usada para evitar que existan usuarios repetidos        
            if (!(usuarios.Exists(x => x.NickName == user.NickName)))
            {
                user.Code = usuarios.Count + 1;
                _loginService.Create(user);
                return StatusCode(201);
            }
            else
            {
                return StatusCode(409);
            }
            
        }

        [HttpDelete("{nombre}")]
        public IActionResult Delete(string nombre) //Borrar un mensaje para ambos
        {
            _loginService.Remove(nombre);
            return NoContent();
        }

        [HttpPut]
        public IActionResult Update([FromBody]Usuario user) //Editar alguna pizza ya creada
        {
            var NewData = _loginService.Get(user.NickName);
            if (NewData == null)
            {
                return NotFound();
            }
            user.Id = NewData.Id; //Poner la misma id al objeto nuevo para no tener problemas en la edición
            _loginService.Update(user.NickName, user);
            return NoContent();

        }

    }
}