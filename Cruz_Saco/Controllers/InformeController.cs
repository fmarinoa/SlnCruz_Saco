using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Cruz_Saco.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using ClosedXML.Excel;
using Microsoft.Extensions.Configuration;

namespace Cruz_Saco.Controllers
{
    public class InformeController : Controller
    {
        //private string connectionString = "server =DESKTOP-TO74OF5\\SQLEXPRESS; database = Cruz_Saco; integrated security = true; ";
        //private string connectionString = @"server =DESKTOP-QN8SLI9\SQLEXPRESS; database = Cruz_Saco; integrated security = true; ";

        private readonly IConfiguration _config;

        public InformeController(IConfiguration config)
        {
            _config = config;
        }

        public ActionResult InformePagos(int? tipos, string fechaInicio, string fechaFin)
        {
            string connectionString = _config.GetConnectionString("cn");


            // Conectarse a la base de datos
            List<Parametro_Sistema> tiposPago = new List<Parametro_Sistema>();
            string query = "SELECT valor, nombre FROM Parametro_Sistema WHERE codigo_padre = 11";

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand(query, cn);

                using (SqlDataReader read = cmd.ExecuteReader())
                {
                    while (read.Read())
                    {
                        tiposPago.Add(new Parametro_Sistema { codigo = read.GetInt32(0), nombre = read.GetString(1) });
                    }
                }
            }

            ViewBag.TiposPago = new SelectList(tiposPago, "codigo", "nombre");
            

            //valido si los parametros llegaron nulos o con valor
            tipos = tipos ?? 0;
            fechaInicio = (fechaInicio == null) ? "" : fechaInicio;
            fechaFin = (fechaFin == null) ? "" : fechaFin;

            // Devolver la lista de resultados como un modelo de vista
            List<sp_Reporte_Informe_Pagos> listaInformePagos = new List<sp_Reporte_Informe_Pagos>();
            listaInformePagos = Informe_Pagos_Lista(tipos.Value, fechaInicio, fechaFin);

            // Uso 2 ViewBag para mantener los valores ingresados en los filtros de fechas, porque cuando hago el boton filtrar recarga la pagina y se pedia los valores ingresados en las cajas de las fechas
            ViewBag.vfechaInicio = fechaInicio;
            ViewBag.vfechaFin = fechaFin;

            return View(listaInformePagos);
        }

        private List<sp_Reporte_Informe_Pagos> Informe_Pagos_Lista(int tipos, string fechaInicio, string fechaFin)
        {
            string connectionString = _config.GetConnectionString("cn");


            //string connectionString = "server =DESKTOP-TO74OF5\\SQLEXPRESS; database = Cruz_Saco; integrated security = true; ";
            //string connectionString = @"server =DESKTOP-QN8SLI9\SQLEXPRESS; database = Cruz_Saco; integrated security = true; ";

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            // Crear un objeto SqlCommand con el nombre del procedimiento almacenado
            SqlCommand command = new SqlCommand("sp_Reporte_Informe_Pagos", connection);
            command.CommandType = CommandType.StoredProcedure;

            // Agregar los parámetros necesarios
            command.Parameters.AddWithValue("@tipo", tipos);
            command.Parameters.AddWithValue("@fecha_inicio", fechaInicio);
            command.Parameters.AddWithValue("@fecha_fin", fechaFin);

            // Ejecutar el comando y obtener los resultados en un SqlDataReader
            SqlDataReader reader = command.ExecuteReader();

            // Crear una lista para almacenar los resultados
            List<sp_Reporte_Informe_Pagos> listaInformePagos = new List<sp_Reporte_Informe_Pagos>();

            // Leer los resultados y agregarlos a la lista
            while (reader.Read())
            {
                sp_Reporte_Informe_Pagos informePago = new sp_Reporte_Informe_Pagos();
                informePago.Cod_Est = reader.GetInt32(0);
                informePago.Nombres = reader.GetString(1);
                informePago.Cod_Pago = reader.GetInt32(2);
                informePago.Descripcion = reader.GetString(3);
                informePago.Monto = reader.GetString(4);
                informePago.Ajuste_manual = reader.GetString(5);
                informePago.Fecha_Pago = reader.GetString(6);
                listaInformePagos.Add(informePago);
            }

            // Cerrar la conexión y el SqlDataReader
            reader.Close();
            connection.Close();

            return listaInformePagos;
        }

       

        public IActionResult ExportarInforme(string tipos, string fechaInicio, string fechaFin)
        {
            // Valido los parametros si son nulos
            tipos = (tipos == null) ? "0" : tipos;
            fechaInicio = (fechaInicio == null) ? "" : fechaInicio;
            fechaFin = (fechaFin == null) ? "" : fechaFin;

            // Cargo un DataTable con el resultado del SP
            DataTable listaInformePagos = new DataTable();
            listaInformePagos = Informe_Pagos_Tabla(int.Parse(tipos), fechaInicio, fechaFin);

            // Creo un archivo excel en base al DataTable
            using (XLWorkbook xl = new XLWorkbook())
            {
                var hoja = xl.Worksheets.Add(listaInformePagos, "hoja1");
                hoja.Column(1).AdjustToContents();
                hoja.Column(2).AdjustToContents();
                hoja.Column(3).AdjustToContents();
                hoja.Column(4).AdjustToContents();
                hoja.Column(5).AdjustToContents();
                hoja.Column(6).AdjustToContents();
                hoja.Column(7).AdjustToContents();

                using (MemoryStream mstream = new MemoryStream())
                {
                    // Guardo el trabajo en memoria y lo envio al front
                    xl.SaveAs(mstream);
                    return File(mstream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reporte_Pagos.xlsx");
                }
            }
        }
        private DataTable Informe_Pagos_Tabla(int tipos, string fechaInicio, string fechaFin)
        {
            string connectionString = _config.GetConnectionString("cn");


            //string connectionString = "server =DESKTOP-TO74OF5\\SQLEXPRESS; database = Cruz_Saco; integrated security = true; ";
            //string connectionString = @"server =DESKTOP-QN8SLI9\SQLEXPRESS; database = Cruz_Saco; integrated security = true; ";

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            // Crear un objeto SqlCommand con el nombre del procedimiento almacenado
            SqlCommand command = new SqlCommand("sp_Reporte_Informe_Pagos", connection);
            command.CommandType = CommandType.StoredProcedure;

            // Agregar los parámetros necesarios
            command.Parameters.AddWithValue("@tipo", tipos);
            command.Parameters.AddWithValue("@fecha_inicio", fechaInicio);
            command.Parameters.AddWithValue("@fecha_fin", fechaFin);

            // Enviar la data del command hacia un DataTable
            SqlDataAdapter da = new SqlDataAdapter(command);
            DataTable dt = new DataTable();
            da.Fill(dt);

            connection.Close();

            return dt;
        }

    }
}
