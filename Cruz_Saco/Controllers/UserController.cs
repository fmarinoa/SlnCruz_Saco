using Cruz_Saco.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Cruz_Saco.Controllers
{
    public class UserController : Controller
    {

        //private readonly string connectionString = "server=DESKTOP-TO74OF5\\SQLEXPRESS;database=Cruz_Saco;integrated security=true;";

        private readonly IConfiguration _config;

        public UserController(IConfiguration config)
        {
            _config = config;
        }

        public IActionResult Index()
        {
            string connectionString = _config.GetConnectionString("cn");


            List<User> usuarios = new List<User>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "sp_listar_usuarios";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            User usuario = new User
                            {
                                Código = (int)reader["Código"],
                                Usuario = (string)reader["Usuario"],
                                Contraseña = (string)reader["Contraseña"],
                                Perfil = (string)reader["Perfil"],
                                Estado = (string)reader["Estado"]
                            };

                            usuarios.Add(usuario);
                        }
                    }
                }
            }

            return View(usuarios);
        }
        public ActionResult Create()
        {
            string connectionString = _config.GetConnectionString("cn");

            User obj = new User();

            // Consulta para obtener los perfiles activos desde la tabla Perfil
            string sql = "SELECT codigo, nombre FROM Perfil WHERE estado = 'A'";
            List<Perfil> perfilesActivos = new List<Perfil>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(sql, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    // Agregar los perfiles activos a una lista
                    Perfil perfil = new Perfil();
                    perfil.Codigo = reader.GetInt32(0);
                    perfil.Nombre = reader.GetString(1);
                    perfilesActivos.Add(perfil);
                }
                reader.Close();
            }

            // Agregar la lista de perfiles activos directamente al objeto User
            obj.Perfiles = perfilesActivos;

            return View(obj);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User user)
        {
            string connectionString = _config.GetConnectionString("cn");


            try
            {
                // Establecer el valor predeterminado del Estado y asignado
                user.Estado = "A";
                user.Asignado = 0;

                // Insertar el nuevo usuario en la tabla Usuario
                string sql = "INSERT INTO Usuario (user_login, pass_login, codigo_Perfil, estado,asignado) \r\nVALUES (@UserLogin, @PassLogin, @CodigoPerfil, @Estado,@Asignado)";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(sql, connection);

                    // Agregar los parámetros a la consulta
                    command.Parameters.AddWithValue("@UserLogin", user.Usuario);
                    command.Parameters.AddWithValue("@PassLogin", user.Contraseña);
                    command.Parameters.AddWithValue("@CodigoPerfil", user.CodigoPerfil);
                    command.Parameters.AddWithValue("@Estado", user.Estado);
                    command.Parameters.AddWithValue("@Asignado", user.Asignado);
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



        // GET: UserController/Edit/5
        public ActionResult Edit(int id)
        {
            string connectionString = _config.GetConnectionString("cn");


            User user = new User();
            string sql = "SELECT codigo, user_login, pass_login, codigo_Perfil, estado FROM Usuario WHERE codigo = @Codigo";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Codigo", id);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    user.Código = reader.GetInt32(0);
                    user.Usuario = reader.GetString(1);
                    user.Contraseña = reader.GetString(2);
                    user.CodigoPerfil = reader.GetInt32(3);
                    user.Estado = reader.GetString(4);
                }

                reader.Close();
            }

            // Consulta para obtener los perfiles activos desde la tabla Perfil
            string sqlPerfiles = "SELECT codigo, nombre FROM Perfil WHERE estado = 'A'";
            List<Perfil> perfilesActivos = new List<Perfil>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(sqlPerfiles, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    // Agregar los perfiles activos a una lista
                    Perfil perfil = new Perfil();
                    perfil.Codigo = reader.GetInt32(0);
                    perfil.Nombre = reader.GetString(1);
                    perfilesActivos.Add(perfil);
                }

                reader.Close();
            }

            // Agregar la lista de perfiles activos directamente al objeto User
            user.Perfiles = perfilesActivos;

            return View(user);
        }


        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, User user)
        {
            string connectionString = _config.GetConnectionString("cn");


            try
            {
                user.Estado = "A";
                // Actualizar los datos del usuario en la tabla Usuario
                string sql = "UPDATE Usuario SET user_login = @UserLogin, pass_login = @PassLogin, codigo_perfil = @CodigoPerfil, estado = @Estado WHERE codigo = @Codigo";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(sql, connection);

                    // Agregar los parámetros a la consulta
                    command.Parameters.AddWithValue("@Codigo", user.Código);
                    command.Parameters.AddWithValue("@UserLogin", user.Usuario);
                    command.Parameters.AddWithValue("@PassLogin", user.Contraseña);
                    command.Parameters.AddWithValue("@CodigoPerfil", user.CodigoPerfil);
                    command.Parameters.AddWithValue("@Estado", user.Estado);

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

        // GET: UserController/Delete/5
        public ActionResult Delete(int id)
        {
            string connectionString = _config.GetConnectionString("cn");


            User user = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Buscar el usuario por su ID
                string sql = "SELECT * FROM Usuario WHERE codigo = @Codigo";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Codigo", id);

                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    user = new User();
                    user.Código = (int)reader["codigo"];
                    user.Usuario = (string)reader["user_login"];
                    user.Contraseña = (string)reader["pass_login"];
                    user.CodigoPerfil = (int)reader["codigo_perfil"];
                    user.Estado = (string)reader["estado"];
                }
            }

            // Pasar los datos del usuario a la vista de eliminación
            return View(user);
        }

        // POST: UserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            string connectionString = _config.GetConnectionString("cn");


            try
            {
                // Actualizar el estado del usuario a inactivo
                string sql = "UPDATE Usuario SET estado = 'I' WHERE codigo = @Codigo";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(sql, connection);

                    // Agregar el parámetro a la consulta
                    command.Parameters.AddWithValue("@Codigo", id);

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
