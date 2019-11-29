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
    public class LoginController : ControllerBase
    {
        private readonly UserService _loginService;
        public LoginController(UserService loginService)
        {
            _loginService = loginService;
        }
        [HttpGet("{nombre}")]
        public ActionResult<Usuario> Get(string nombre)
        {
            var user = _loginService.Get(nombre);
            if (user == null)
            {
                return NotFound();
            }
            return user;
        }

        [HttpGet]
        public List<Usuario> Get()
        {
            return _loginService.Get();
        }
    }
}