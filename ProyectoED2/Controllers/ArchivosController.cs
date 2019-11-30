using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AlgoritmosEDII;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace ProyectoED2.Controllers
{
    public class ArchivosController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        

    }
}