using Cruz_Saco.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Cruz_Saco.Controllers
{
    public class ServicioController : Controller
    {
        //private readonly string connectionString = "server=DESKTOP-TO74OF5\\SQLEXPRESS;database=Cruz_Saco;integrated security=true;";

        private readonly IConfiguration _config;

        public ServicioController(IConfiguration config)
        {
            _config = config;
        }

        // GET: ServicioController1cs
        public ActionResult Index()
        {
            string connectionString = _config.GetConnectionString("cn");


            List<ServicioAdicional> servicios = new List<ServicioAdicional>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "sp_listar_servicio";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ServicioAdicional servicio = new ServicioAdicional
                            {
                                Código = (int)reader["codigo"],
                                Nombre = (string)reader["nombre"],
                                Descripcion = (string)reader["descripcion"],
                                Costo = (string)reader["precio"],
                                Estado = (string)reader["estado"]
                            };

                            servicios.Add(servicio);
                        }
                    }
                }
            }
            return View(servicios);
        }

        // GET: ServicioController1cs/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ServicioController1cs/Create
        public ActionResult Create()
        {
            ServicioAdicional obj = new ServicioAdicional();
            return View(obj);
        }

        // POST: ServicioController1cs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ServicioAdicional servicio)
        {
            string connectionString = _config.GetConnectionString("cn");


            try
            {
                servicio.Estado = "A";
                // Insertar el nuevo servicio en la tabla ServicioAdicional
                string sql = "INSERT INTO ServicioAdicional (nombre, descripcion, precio, estado) VALUES (@nombre, @descripcion, @precio, @estado)";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(sql, connection);

                    // Agregar los parámetros a la consulta
                    command.Parameters.AddWithValue("@nombre", servicio.Nombre);
                    command.Parameters.AddWithValue("@descripcion", servicio.Descripcion);
                    command.Parameters.AddWithValue("@precio", servicio.Precio);
                    command.Parameters.AddWithValue("@estado", servicio.Estado);

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

        // GET: ServicioController1cs/Edit/5
        public ActionResult Edit(int id)
        {
            string connectionString = _config.GetConnectionString("cn");


            ServicioAdicional obj = new ServicioAdicional();
            string sql = "SELECT codigo, nombre, descripcion, precio, estado FROM ServicioAdicional WHERE codigo = @Id";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Id", id);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    obj.Código = reader.GetInt32(0);
                    obj.Nombre = reader.GetString(1);
                    obj.Descripcion = reader.GetString(2);
                    obj.Precio = reader.GetDecimal(3);
                    obj.Estado = reader.GetString(4);
                }

                reader.Close();
            }
            return View(obj);
        }

        // POST: ServicioController1cs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, ServicioAdicional servicio)
        {
            string connectionString = _config.GetConnectionString("cn");


            try
            {
                servicio.Estado = "A";
                // Actualizar los datos del servicio en la tabla ServicioAdicional
                string sql = "UPDATE ServicioAdicional SET nombre = @nombre, descripcion = @descripcion, precio = @precio, estado = @estado WHERE codigo = @codigo";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(sql, connection);

                    // Agregar los parámetros a la consulta
                    command.Parameters.AddWithValue("@codigo", servicio.Código);
                    command.Parameters.AddWithValue("@nombre", servicio.Nombre);
                    command.Parameters.AddWithValue("@descripcion", servicio.Descripcion);
                    command.Parameters.AddWithValue("@precio", servicio.Precio);
                    command.Parameters.AddWithValue("@estado", servicio.Estado);

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

        // GET: ServicioController1cs/Delete/5
        public ActionResult Delete(int id)
        {
            string connectionString = _config.GetConnectionString("cn");


            ServicioAdicional servicio = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Buscar el servicio por su ID
                string sql = "SELECT * FROM ServicioAdicional WHERE codigo = @Id";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Id", id);

                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    servicio = new ServicioAdicional();
                    servicio.Código = (int)reader["codigo"];
                    servicio.Nombre = (string)reader["nombre"];
                    servicio.Descripcion = (string)reader["descripcion"];
                    servicio.Precio = (decimal)reader["precio"];
                    servicio.Estado = (string)reader["estado"];
                }
            }
            return View(servicio);
        }

        // POST: ServicioController1cs/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            string connectionString = _config.GetConnectionString("cn");


            try
            {
                // Actualizar el estado del servicio a inactivo
                string sql = "UPDATE ServicioAdicional SET estado = 'I' WHERE codigo = @Id";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(sql, connection);

                    // Agregar el parámetro a la consulta
                    command.Parameters.AddWithValue("@Id", id);

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
