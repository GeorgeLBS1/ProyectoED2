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
using ProyectoED2.Utilities;
using AlgoritmosEDII;

namespace ProyectoED2.Controllers
{
    public class MensajesController : Controller
    {
        ChatAPI _api = new ChatAPI();
        SDES sdes = new SDES();
        GenerarClavesSeguras dh = new GenerarClavesSeguras(); //diffie-helfman
        // GET: Mensajes
        public async Task <IActionResult> Index(string id)
        {
            if (id != null || id != "")
            {
                GlobalData.para = id;
            }
            if (GlobalData.Receptor == null)
            {
                HttpClient client1 = _api.Initial();
                HttpResponseMessage respuesta = await client1.GetAsync($"api/Login/{GlobalData.para}");
                if (respuesta.IsSuccessStatusCode)
                {
                    var results = respuesta.Content.ReadAsStringAsync().Result;

                    GlobalData.Receptor = JsonConvert.DeserializeObject<UserData>(results); //Obtener de los datos del usuario ingresado 
                }
            }
            
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
            int claveCifrado = dh.GenerarLlave(17, 19);
            mensajes.ForEach(x => x.Cuerpo = sdes.Desencriptar(claveCifrado, x.Cuerpo));
            
            
            return View(mensajes);
        }

        
        [HttpPost]
        public async Task <IActionResult> Buscar(string mensaje)
        {
            List<UserData> keys = new List<UserData>();
            HttpClient client = _api.Initial();
            HttpResponseMessage usuarios = await client.GetAsync($"api/Login/");
            if (usuarios.IsSuccessStatusCode)
            {
                var results = usuarios.Content.ReadAsStringAsync().Result;
                keys = JsonConvert.DeserializeObject<List<UserData>>(results); //Obtener de los datos del usuario ingresado
            }

            List<MensajesViewModel> mensajes = new List<MensajesViewModel>();
            HttpResponseMessage res = await client.GetAsync($"api/Mensajes/{GlobalData.ActualUser.NickName}");
            if (res.IsSuccessStatusCode)
            {
                var results = res.Content.ReadAsStringAsync().Result;
                mensajes = JsonConvert.DeserializeObject<List<MensajesViewModel>>(results);

            }
            List<MensajesViewModel> MisMensajes = mensajes.FindAll(x => x.Emisor == GlobalData.ActualUser.NickName);
            List<MensajesViewModel> MensajesRecibidos = mensajes.FindAll(x => x.Receptor == GlobalData.ActualUser.NickName);
            Dictionary<string, int> llaves = new Dictionary<string, int>();
            foreach (var item in keys)
            {
                llaves.Add(item.NickName, item.Code);
            }
            List<MensajesViewModel> mios = new List<MensajesViewModel>();
            foreach (var item in MisMensajes)
            {
                MensajesViewModel ms = new MensajesViewModel();
                ms = item;
                int llave = dh.GenerarLlave(17, 19);
                ms.Cuerpo = sdes.Desencriptar(llave, item.Cuerpo);
                mios.Add(ms);

            }
            List<MensajesViewModel> recib = new List<MensajesViewModel>();
            foreach (var item in MensajesRecibidos)
            {
                MensajesViewModel ms = new MensajesViewModel();
                ms = item;
                int llave = dh.GenerarLlave(17, 19);
                ms.Cuerpo = sdes.Desencriptar(llave, item.Cuerpo);
                recib.Add(ms);
            }

            mios = mios.FindAll(x => x.Cuerpo.Contains(mensaje));
            recib = recib.FindAll(x => x.Cuerpo.Contains(mensaje));
            var final = mios.Union(recib);
            final = final.OrderBy(x => x.Date).ToList();

            return View(final);

        }

        [HttpPost]
        public IActionResult NuevoMensaje(string texto)
        {
            MensajesViewModel mensajesNuevo = new MensajesViewModel();

            int claveCifrado = dh.GenerarLlave(17, 19);
            texto = sdes.Encriptar(claveCifrado, texto);
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
                return Redirect("http://localhost:61798/Mensajes/Index/" + GlobalData.para);
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