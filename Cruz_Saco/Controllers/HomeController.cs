using Cruz_Saco.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Datos.Seguridad;
using Entidad.Seguridad;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;

namespace Cruz_Saco.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _config;

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

            public HomeController(IConfiguration config)
            {
                _config = config;
            }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            // Valida la nulidad de la variable de session
            if (HttpContext.Session.GetString("PerfilUsuario") == null)
            {
                HttpContext.Session.SetString("PerfilUsuario", "0");
            }
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public JsonResult ListarMenu()
        {
            // Obtengo lel valor de la variable session
            int nPerfil = int.Parse(HttpContext.Session.GetString("PerfilUsuario").ToString());
            int XUsuario = int.Parse(HttpContext.Session.GetString("Usuario").ToString());

            // Retorno la lista de menu/opciones a los cuales el perfil del usuario tenga acceso
            return Json(new daLogin(_config).ListarMenu(nPerfil));
        }


    }
}
