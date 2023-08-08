using Cruz_Saco.Models;
using Microsoft.AspNetCore.Mvc;
using Datos.Usuario;
using Entidad.Usuario;
using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Datos.Seguridad;
using Entidad.Seguridad;

namespace Cruz_Saco.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IConfiguration _config;

        public UsuarioController(IConfiguration config)
        {
            _config = config;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            // Inicializa la variable de sesión
            if (HttpContext.Session.GetString("PerfilUsuario") == null)
            {
                HttpContext.Session.SetString("PerfilUsuario", "0");
            }
        }

        public IActionResult Index()
        {
            //int nPerfil = int.Parse(HttpContext.Session.GetString("PerfilUsuario").ToString());
            //daLogin daLogin = new daLogin(_config);
            //ViewBag.MenuItems = daLogin.ListarMenu(nPerfil);

            return View();
        }


        //[HttpPost]
        //public JsonResult ListarUsuario()

        public IActionResult ListarUsuario()
        {
            daUsuario daUsuario = new daUsuario(_config);
            List<enUsuario> data = new List<enUsuario>();

            data = daUsuario.ListarUsuario();

            // Agrega mensajes de depuración para asegurarte de que se están devolviendo datos válidos
            Console.WriteLine("Total de registros devueltos: " + data.Count());
            foreach (var usuario in data)
            {
                Console.WriteLine("Usuario devuelto: " + usuario.codigo + " - " + usuario.usuario);
            }

            return Json(new { data });
        }

        [HttpPost]
        public JsonResult ListarMenu()
        {
            int nPerfil = int.Parse(HttpContext.Session.GetString("PerfilUsuario").ToString());

            // Obtengo la lista de menu/opciones a los cuales el perfil del usuario tenga acceso
            daLogin daLogin = new daLogin(_config);
            List<enMenu> oLista = new List<enMenu>();

            oLista = daLogin.ListarMenu(nPerfil);

            return Json(oLista);
        }


    }
}
