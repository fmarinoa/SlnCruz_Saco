using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Datos.Pago;
using Entidad.Pago;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Cruz_Saco.Controllers
{
    public class PagoController : Controller
    {
        private readonly IConfiguration _config;

        public PagoController(IConfiguration config)
        {
            _config = config;
        }


        public ActionResult Index(string tipo = "0", string f_ini = "", string f_fin = "")
        {
            tipo = (tipo == null) ? "0" : tipo;
            f_ini = (f_ini == null) ? "" : f_ini;
            f_fin = (f_fin == null) ? "" : f_fin;

            daPago daPago = new daPago(_config);
            List<enPago> lista = new List<enPago>();

            lista = daPago.Reporte_Informe_Pagos(tipo, f_ini, f_fin);

            return View(lista);
        }


    }
}
