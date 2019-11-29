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
    public class MensajesController : Controller
    {
        ChatAPI _api = new ChatAPI();
        // GET: Mensajes
        public async Task <IActionResult> Index()
        {
            List<MensajesViewModel> mensajes = new List<MensajesViewModel>();
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync($"api/Mensajes/{GlobalData.ActualUser.NickName}");
            if (res.IsSuccessStatusCode)
            {
                var results = res.Content.ReadAsStringAsync().Result;
                mensajes = JsonConvert.DeserializeObject<List<MensajesViewModel>>(results);

            }
            mensajes = mensajes.FindAll(x => ((x.Receptor == GlobalData.para && x.Emisor == GlobalData.ActualUser.NickName) || (x.Emisor == GlobalData.para && x.Receptor == GlobalData.ActualUser.NickName)) );
            mensajes = mensajes.OrderBy(x => x.Date).ToList();
            return View(mensajes);
        }

        [HttpPost]
        public IActionResult NuevoMensaje(string texto)
        {
            MensajesViewModel mensajesNuevo = new MensajesViewModel();
            mensajesNuevo.Cuerpo = texto;
            mensajesNuevo.Date = DateTime.Now.AddHours(-6);
            //DateTime HoraLocal = new DateTime(0, 0, 0, 6, 0, 0);
            //mensajesNuevo.Date.Subtract(HoraLocal);
            mensajesNuevo.Emisor = GlobalData.ActualUser.NickName;
            mensajesNuevo.Receptor = GlobalData.para;
            mensajesNuevo.Visible = "";
            HttpClient client = _api.Initial();
            var postTask = client.PostAsJsonAsync<MensajesViewModel>("api/Mensajes", mensajesNuevo);
            postTask.Wait();
            var result = postTask.Result;
            if (result.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Mensajes");
            }
            return RedirectToAction("Index", "Mensajes");
        }
        // GET: Mensajes/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Mensajes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Mensajes/Create
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

        // GET: Mensajes/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Mensajes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Mensajes/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Mensajes/Delete/5
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