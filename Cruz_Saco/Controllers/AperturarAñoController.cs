using Cruz_Saco.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Cruz_Saco.Controllers
{
    public class AperturarAñoController : Controller
    {
        //private readonly string connectionString = "server=DESKTOP-TO74OF5\\SQLEXPRESS;database=Cruz_Saco;integrated security=true;";
        //private readonly string connectionString = @"server =DESKTOP-QN8SLI9\SQLEXPRESS; database = Cruz_Saco; integrated security = true; ";

        private readonly IConfiguration _config;

        public AperturarAñoController(IConfiguration config)
        {
            _config = config;
        }


        public IActionResult Index()
        {
            string connectionString = _config.GetConnectionString("cn");


            List<Matricula> matri = new List<Matricula>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "sp_listar_matricula";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Matricula matricu = new Matricula
                            {
                                Año = (int)reader["Año"],
                                PrimerDíaDeClases = (string)reader["Fecha"],
                                Precio = (string)reader["Monto"],
                                DíaDePago = (int)reader["Día_de_pago"],
                                Estado= (string)reader["Estado"]
                            };

                            matri.Add(matricu);
                        }
                    }
                }
            }
            return View(matri);
        }

        public ActionResult Create()
        {
    
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Matricula matri)
        {
            string connectionString = _config.GetConnectionString("cn");

            try
            {
                // Insertar el nuevo usuario en la tabla Usuario
                string sql = "sp_aperturar_Nuevo_año @año,@incioClases,@monto";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(sql, connection);

                    // Agregar los parámetros a la consulta
                    command.Parameters.AddWithValue("@año", matri.Año);
                    command.Parameters.AddWithValue("@incioClases", matri.PrimerDíaDeClases);
                    command.Parameters.AddWithValue("@monto", matri.Costo);

                    // Ejecutar la consulta
                    int result = command.ExecuteNonQuery();
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

    }
}
