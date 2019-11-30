using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AlgoritmosEDII;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProyectoED2.Helper;
using ProyectoED2.Models;

namespace ProyectoED2.Controllers
{
    public class LoginController : Controller
    {
        ChatAPI _api = new ChatAPI();
        
        // GET: Login
        public ActionResult Index()
        {
            //Limpiar variables globales
            GlobalData.Receptor = null;
            GlobalData.ActualUser = null;
            GlobalData.para = null;
            GlobalData.ArchivoEntrada = null;
            GlobalData.ArchivoSalida = null;
            return View();
        }
        // GET: Login/Details/5
        
        [HttpPost]
        public async Task<IActionResult> Ingresar(string username, string password)
        {
            if (password == null)
            {
                password = " ";
            }
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync($"api/Login/{username}");
            UserData user = new UserData();
            if (res.IsSuccessStatusCode)
            {
                var results = res.Content.ReadAsStringAsync().Result;
                user = JsonConvert.DeserializeObject<UserData>(results); //Obtener de los datos del usuario ingresado
                Cesar cesar = new Cesar();
                password = cesar.Cifrar(password, "1234567890'¿qwertyuiop");
                if (user.Password == password)
                {
                    GlobalData.ActualUser = user;
                    return RedirectToAction("Index","Menu");
                }
                else
                {
                    return Content("Error en la autentiacación, credenciales incorrectas o el usuario no existe");
                }

            }
            return Content("Error en la autentiacación, credenciales incorrectas o el usuario no existe");
        }

       
    }
}