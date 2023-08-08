using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Cruz_Saco.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System;

namespace Cruz_Saco.Controllers
{
    public class ConsultaPagosClienteController : Controller
    {
        private readonly IConfiguration _config;

        public ConsultaPagosClienteController(IConfiguration config)
        {
            _config = config;
        }
        public IActionResult PagoCliente()
        {
            string _connectionString = _config.GetConnectionString("cn");

            int XUsuario = int.Parse(HttpContext.Session.GetString("Usuario").ToString());

            int codigo_apode = 0;

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var command = new SqlCommand("sp_obtener_codigo_apoderado", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@xUsuario", XUsuario);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        codigo_apode = Convert.ToInt32(reader.GetValue(0));
                    }
                }


                // Obtener la lista de pagos del apoderado 
                var pagos = new List<PagosClienteClass>();

                using (var connection2 = new SqlConnection(_connectionString))
                {
                    connection2.Open();

                    var command2 = new SqlCommand("sp_VerPagosPagados", connection);
                    command2.CommandType = CommandType.StoredProcedure;
                    command2.Parameters.AddWithValue("@apoderado", codigo_apode);

                    using (var reader2 = command2.ExecuteReader())
                    {
                        while (reader2.Read())
                        {
                            var pago = new PagosClienteClass();

                            pago.Apoderado = reader2.GetString(0);
                            pago.Parentesco = reader2.GetString(1);
                            pago.Estudiante = reader2.GetString(2);
                            pago.Concepto = reader2.GetString(3);
                            pago.FechaPago = reader2.GetString(4);
                            pago.HoraPago = reader2.GetString(5);
                            pago.Monto = reader2.GetDecimal(6);
                            pago.Descuento = reader2.GetDecimal(7);
                            pago.AjusteManual = reader2.GetString(8);

                            pagos.Add(pago);
                        }
                    }
                }

                return View(pagos);
            }

        }

        public IActionResult Historial()
        {
            string _connectionString = _config.GetConnectionString("cn");
            int XUsuario = int.Parse(HttpContext.Session.GetString("Usuario").ToString());
            int codigo_apode = 0;

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var command = new SqlCommand("sp_obtener_codigo_apoderado_estudiante", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@xUsuario", XUsuario);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        codigo_apode = Convert.ToInt32(reader.GetValue(0));
                    }
                }

                var historial = new List<Historial>();
                using (var connection2 = new SqlConnection(_connectionString))
                {
                    connection2.Open();

                    var command2 = new SqlCommand("sp_VerPagosPendientes", connection2);
                    command2.CommandType = CommandType.StoredProcedure;
                    command2.Parameters.AddWithValue("@anio", 2023);
                    command2.Parameters.AddWithValue("@estudiante", codigo_apode);
                    

                    using (var reader2 = command2.ExecuteReader())
                    {
                        while (reader2.Read())
                        {
                            var historial1 = new Historial();

                            historial1.Nro_Cuota = Convert.ToInt32(reader2.GetValue(0));
                            historial1.Mes = reader2.GetString(1);
                            historial1.Fecha_Vencimiento = reader2.GetDateTime(2);
                            historial1.Fecha_Pago = reader2.GetString(3);
                            historial1.Hora_Pago = reader2.GetString(4);
                            historial1.Monto = reader2.GetDecimal(5);
                            historial1.Estado = reader2.GetString(6);

                            historial.Add(historial1);
                        }
                    }
                }
                return View(historial);
            } 
        }

        public IActionResult HistorialAdmin(int estudiante)
        {
            string _connectionString = _config.GetConnectionString("cn");

            // Obtener la lista de apoderados
            List<Apoderado> apoderados = new List<Apoderado>();
            string query = "select\r\nca.codigo,\r\nconcat(ca.apellido_paterno,' ',ca.apellido_materno,', ',ca.nombre) as nombre\r\nfrom Cliente_Apoderado ca";

            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand(query, cn);

                using (SqlDataReader read = cmd.ExecuteReader())
                {
                    while (read.Read())
                    {
                        apoderados.Add(new Apoderado { Código = read.GetInt32(0), Nombre = read.GetString(1) });
                    }
                }
            }

            // Agregar la lista de apoderados al ViewBag para enviarla a la vista
            ViewBag.Apoderados = new SelectList(apoderados, "Código", "Nombre");

            // Obtener la lista de pagos del apoderado seleccionado
            var historial = new List<Historial>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var command = new SqlCommand("sp_VerPagosPendientes", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@anio", 2023);
                command.Parameters.AddWithValue("@estudiante", estudiante);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var pago = new Historial();

                        pago.Nro_Cuota = Convert.ToInt32(reader.GetValue(0));
                        pago.Mes = reader.GetString(1);
                        pago.Fecha_Vencimiento = reader.GetDateTime(2);
                        pago.Fecha_Pago = reader.GetString(3);
                        pago.Hora_Pago = reader.GetString(4);
                        pago.Monto = reader.GetDecimal(5);
                        pago.Estado = reader.GetString(6);

                        historial.Add(pago);
                    }
                }
            }

            return View(historial);
        }

    }
}
