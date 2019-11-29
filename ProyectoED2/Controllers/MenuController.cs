using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Chat_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProyectoED2.Helper;
using ProyectoED2.Models;

namespace ProyectoED2.Controllers
{
    public class MenuController : Controller
    {
        ChatAPI _api = new ChatAPI();
        // GET: Menu
        public ActionResult Index()
        {
            return View();
        }

        // GET: Menu/Create
        public static List<Contactos> lista = new List<Contactos>();
        public async Task<IActionResult> MisContact()
        {
            lista.Clear();
            HttpClient client = _api.Initial();
            var res = await client.GetAsync($"api/Contactos/{GlobalData.ActualUser.NickName}");
            if (res.IsSuccessStatusCode)
            {
                var resultas = res.Content.ReadAsStringAsync().Result;
                var contactosUser = JsonConvert.DeserializeObject<Contactos>(resultas); //Obtener de los datos del usuario ingresado
                lista.Add(contactosUser);
            }
            return RedirectToAction("MisContactos", "Menu", lista);
        }
        [HttpGet]
        public ActionResult MisContactos(List<Contactos> collection)
        {
            try
            {
                // TODO: Add insert logic here

                return View(lista);
            }
            catch
            {
                return View();
            }
        }

        public ActionResult AddContacto()
        {
            return View();
        }

        [HttpPost]
        public async Task <IActionResult> AddConta(string contact)
        {
            HttpClient client = _api.Initial();
            if (contact != "" && contact != null)
            {
                HttpResponseMessage res = await client.GetAsync($"api/Login/{contact}");
                UserData user = new UserData();
                if (res.IsSuccessStatusCode)
                {
                    var results = res.Content.ReadAsStringAsync().Result;
                    user = JsonConvert.DeserializeObject<UserData>(results); //Obtener de los datos del usuario ingresado
                    res = await client.GetAsync($"api/Contactos/{GlobalData.ActualUser.NickName}");
                    if (res.IsSuccessStatusCode)
                    {
                        var resultas = res.Content.ReadAsStringAsync().Result;
                        var contactosUser = JsonConvert.DeserializeObject<Contactos>(resultas); //Obtener de los datos del usuario ingresado
                        if (!(contactosUser.ContactosAmigos.Contains(user.NickName)))
                        {
                            contactosUser.ContactosAmigos.Add(user.NickName);
                            var postTask = client.PutAsJsonAsync<Contactos>($"api/Contactos/{GlobalData.ActualUser.NickName}", contactosUser);
                            postTask.Wait();
                            if (postTask.Result.IsSuccessStatusCode)
                            {
                                return RedirectToAction("Index", "Menu");
                            }
                        }

                    }
                    else
                    {
                        Contactos nuevoContacto = new Contactos();
                        nuevoContacto.OwnerNickName = GlobalData.ActualUser.NickName;
                        nuevoContacto.ContactosAmigos = new List<string>();
                        nuevoContacto.ContactosAmigos.Add(user.NickName);

                        var postTask = client.PostAsJsonAsync<Contactos>("api/Contactos", nuevoContacto);
                        postTask.Wait();

                        if (postTask.Result.IsSuccessStatusCode)
                        {
                            return RedirectToAction("Index", "Menu");
                        }
                    }
                }
                


                //var result = post.Result;
                //if (result.IsSuccessStatusCode)
                //{
                //    return RedirectToAction("Index", "Menu");
                //}

                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");

        }
        // POST: Menu/Create
        
       

        // GET: Menu/Edit/5
        public async Task <IActionResult> Edit()
        {
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync($"api/Login/{GlobalData.ActualUser.NickName}");
            UserData user = new UserData();
            if (res.IsSuccessStatusCode)
            {
                var results = res.Content.ReadAsStringAsync().Result;
                user = JsonConvert.DeserializeObject<UserData>(results); //Obtener de los datos del usuario ingresado
                return View(user);
            }
                return View();
        }

        // POST: Menu/Edit/5
        [HttpPost]
        public ActionResult Edit(UserData userData)
        {
            try
            {
                GlobalData.ActualUser.Name = userData.Name;
                GlobalData.ActualUser.Password = userData.Password;
                HttpClient client = _api.Initial();
                var res = client.PutAsJsonAsync($"api/SignIn/", GlobalData.ActualUser);
                res.Wait();
                if (res.Result.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                return View();
            }
            catch
            {
                return View();
            }
        }

        // GET: Menu/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Menu/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}