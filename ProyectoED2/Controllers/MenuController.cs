using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
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

        // GET: Menu/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Menu/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Menu/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

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