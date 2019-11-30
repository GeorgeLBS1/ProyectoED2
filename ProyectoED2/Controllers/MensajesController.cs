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
using System.IO;

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
            if (GlobalData.Receptor == null || GlobalData.Receptor.NickName != id)
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
            int claveCifrado = dh.GenerarLlave(GlobalData.ActualUser.Code, GlobalData.Receptor.Code);
            mensajes.ForEach(x => x.Cuerpo = sdes.Desencriptar(claveCifrado, x.Cuerpo));
            
            
            return View(mensajes);
        }

        
        [HttpPost]
        public async Task <IActionResult> Buscar(string mensaje)
        {
            if (mensaje == null || mensaje == "")
            {
                return Content("No puede dejar el campo de búsqueda en blanco");                
            }
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
                int llave = dh.GenerarLlave(GlobalData.ActualUser.Code, llaves[item.Receptor]);
                ms.Cuerpo = sdes.Desencriptar(llave, item.Cuerpo);
                mios.Add(ms);

            }
            List<MensajesViewModel> recib = new List<MensajesViewModel>();
            foreach (var item in MensajesRecibidos)
            {
                MensajesViewModel ms = new MensajesViewModel();
                ms = item;
                int llave = dh.GenerarLlave(llaves[item.Emisor], GlobalData.ActualUser.Code);
                ms.Cuerpo = sdes.Desencriptar(llave, item.Cuerpo);
                recib.Add(ms);
            }

            mios = mios.FindAll(x => x.Cuerpo.Contains(mensaje));
            recib = recib.FindAll(x => x.Cuerpo.Contains(mensaje));
            var final = mios.Union(recib);
            List<MensajesViewModel> Lfinal = final.OrderBy(x => x.Date).ToList();
            Lfinal.RemoveAll(x => x.Archivo == true);
            return View(Lfinal);

        }

        [HttpPost]
        public IActionResult NuevoMensaje(string texto)
        {
            if (texto == null || texto == "")
            {
                return Content("No puede enviar un mensaje en blanco");
            }
            MensajesViewModel mensajesNuevo = new MensajesViewModel();

            int claveCifrado = dh.GenerarLlave(GlobalData.ActualUser.Code, GlobalData.Receptor.Code);
            texto = sdes.Encriptar(claveCifrado, texto);
            mensajesNuevo.Cuerpo = texto;
            mensajesNuevo.Date = DateTime.Now.AddHours(-6);
            mensajesNuevo.Archivo = false;
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

        
        public async Task<IActionResult> BorrarGLobal(string id)
        {
            var mensaje = new MensajesViewModel();
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.DeleteAsync($"api/Mensajes/{id}");

            return Redirect("http://localhost:61798/Mensajes/Index/" + GlobalData.para);
        }
        public async Task <IActionResult> Borrar(string id)
        {
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync($"api/Mensajes/{GlobalData.ActualUser.NickName}");
            List<MensajesViewModel> mensajesViews = new List<MensajesViewModel>();
            MensajesViewModel mensajeAborrar = new MensajesViewModel();
            if (res.IsSuccessStatusCode)
            {
                var results = res.Content.ReadAsStringAsync().Result;
                mensajesViews = JsonConvert.DeserializeObject<List<MensajesViewModel>>(results); //Obtener de los datos del usuario ingresado
                mensajeAborrar = mensajesViews.Find(x => x.Id == id);
                mensajeAborrar.Visible = GlobalData.ActualUser.NickName;
                var postTask = client.PutAsJsonAsync<MensajesViewModel>($"api/Mensajes/{id}", mensajeAborrar);
                postTask.Wait();
                if (postTask.Result.IsSuccessStatusCode)
                {
                    return Redirect("http://localhost:61798/Mensajes/Index/" + GlobalData.para);
                }
            }

            return Redirect("http://localhost:61798/Mensajes/Index/" + GlobalData.para);
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

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile Archivo)
        {
            if (Archivo == null || Archivo.Length == 0)
            {
                return Content("Arcivo no seleccionado");
            }
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", Archivo.FileName);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await Archivo.CopyToAsync(stream);
            }
            GlobalData.ArchivoEntrada = path;
            LZW Compresor = new LZW();
            string RutaSalida = "";
            Compresor.Comprimir(path, ref RutaSalida);
            GlobalData.ArchivoSalida = RutaSalida;
            FileInfo fileInfo = new FileInfo(path);
            Queue<byte> Texto = new Queue<byte>();
            LeerArchivo(ref Texto, GlobalData.ArchivoSalida);
            string salidaCompreso = "";
            while (Texto.Count > 0)
            {
                salidaCompreso += Convert.ToString(Texto.Dequeue()) + ",";
            }

            MensajesViewModel mensajesNuevo = new MensajesViewModel();
            mensajesNuevo.Cuerpo = salidaCompreso;
            mensajesNuevo.Date = DateTime.Now.AddHours(-6);
            mensajesNuevo.Archivo = true;
            mensajesNuevo.NombreArchivo = fileInfo.Name;
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

        void LeerArchivo(ref Queue<byte> TextoAleer, string rutaOrigen) //LEER TEXTO UTILIZANDO UN BUFFER Y TODO LEIDO EN BYTES
        {
            const int bufferLength = 1024;
            var buffer = new byte[bufferLength];
            using (var file = new FileStream(rutaOrigen, FileMode.Open))
            {
                using (var reader = new BinaryReader(file))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        buffer = reader.ReadBytes(bufferLength);
                        foreach (var item in buffer)
                        {
                            TextoAleer.Enqueue(item);
                        }
                    }

                }

            }


        }

        public async Task <IActionResult> Descargar_archivo(string id)
        {
            HttpClient client = _api.Initial();
            MensajesViewModel archivo = new MensajesViewModel();
            HttpResponseMessage res = await client.GetAsync($"api/Files/{id}");
            Queue<byte> textoEntrada = new Queue<byte>();
            if (res.IsSuccessStatusCode)
            {
                var results = res.Content.ReadAsStringAsync().Result;
                archivo = JsonConvert.DeserializeObject<MensajesViewModel>(results);
                HelperArchivos helperArchivos = new HelperArchivos();
                textoEntrada = helperArchivos.LeerCifrado(archivo.Cuerpo);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", archivo.NombreArchivo);
                path = path.Replace(".txt", ".lzw");
                helperArchivos.EscribirArchivo(textoEntrada, path);
                LZW lzw = new LZW();
                string archivoNormalRuta = "";
                lzw.Descomprimir(path, ref archivoNormalRuta);
                GlobalData.ArchivoSalida = archivoNormalRuta;
                return RedirectToAction("Download");
            }

            return Redirect("http://localhost:61798/Mensajes/Index/" + GlobalData.para);

        }

        public async Task<IActionResult> Download() //Método para realizar las descargas automaticas del archivo
        {
            var path = GlobalData.ArchivoSalida;

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(path), Path.GetFileName(path));
        }
        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".cif", "text/plain"},
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }

    }
}