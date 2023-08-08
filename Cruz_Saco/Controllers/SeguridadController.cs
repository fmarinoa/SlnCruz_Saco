using Cruz_Saco.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Datos.Seguridad;
using Entidad.Seguridad;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;

namespace Cruz_Saco.Controllers
{
    public class SeguridadController : Controller
    {
        private readonly IConfiguration _config;

        public SeguridadController(IConfiguration config)
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
            return View();
        }

        [HttpPost]
        public JsonResult Login([FromBody] User user)
        {
            daLogin daLogin = new daLogin(_config);
            enLogin enLogin = new enLogin();

            enLogin = daLogin.ValidarLogin(user.Usuario, user.Contraseña);

            // Obtengo el codigo de perfil del usuario logueado y lo guardo en la variable session
            HttpContext.Session.SetString("PerfilUsuario", enLogin.Perfil.ToString());
            HttpContext.Session.SetString("Usuario", enLogin.Usuario.ToString());

            return Json(enLogin);
        }

    
    }
}
