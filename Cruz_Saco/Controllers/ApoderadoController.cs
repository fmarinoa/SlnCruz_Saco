using Cruz_Saco.Models;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data.SqlClient;
using static ClosedXML.Excel.XLPredefinedFormat;
using Microsoft.Extensions.Configuration;

namespace Cruz_Saco.Controllers
{
    public class ApoderadoController : Controller
    {
        //private readonly string connectionString = "server=DESKTOP-TO74OF5\\SQLEXPRESS;database=Cruz_Saco;integrated security=true;";

        private readonly IConfiguration _config;

        public ApoderadoController(IConfiguration config)
        {
            _config = config;
        }


        // GET: ApoderadoController
        public ActionResult Index()
        {
            string connectionString = _config.GetConnectionString("cn");


            List<Apoderado> apoderados = new List<Apoderado>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "sp_Listar_Clientes_Apoderados";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Apoderado apoderado = new Apoderado
                            {
                                Código = (int)reader["codigo"],
                                Apellido_Paterno = (string)reader["apellido_paterno"],
                                Apellido_Materno = (string)reader["apellido_materno"],
                                Nombre = (string)reader["nombre"],
                                Fecha_Nacimiento = (string)reader["fecha_nacimiento"],
                                Parentesco = (string)reader["parentesco"],
                                Tipo_Documento = (string)reader["tipo_documento"],
                                Número_Documento = (string)reader["numero_documento"],
                                Estado = (string)reader["estado"]
                            };

                            apoderados.Add(apoderado);
                        }
                    }
                }
            }

            return View(apoderados);
        }

        // GET: ApoderadoController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ApoderadoController/Create
        public ActionResult Create()
        {
            string connectionString = _config.GetConnectionString("cn");

            Apoderado obj = new Apoderado();

            // Consulta para obtener los tipos de documento 
            string sql = "select valor, nombre from Parametro_Sistema where codigo_padre = 1";
            List<TipoDocumento> tiposDocumento = new List<TipoDocumento>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(sql, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    // Agregar los perfiles activos a una lista
                    TipoDocumento tipoDoc = new TipoDocumento();
                    tipoDoc.valor = reader.GetInt32(0);
                    tipoDoc.nombre = reader.GetString(1);
                    tiposDocumento.Add(tipoDoc);
                }
                reader.Close();
            }

            // Agregar la lista de perfiles activos directamente al objeto User
            obj.Tipo_Doc = tiposDocumento;

            // Consulta para obtener los parentescos
            string sql2 = "select valor, nombre from Parametro_Sistema where codigo_padre = 8";
            List<Parentesco> parentescos = new List<Parentesco>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(sql2, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    // Agregar los perfiles activos a una lista
                    Parentesco parentesco = new Parentesco();
                    parentesco.valor = reader.GetInt32(0);
                    parentesco.nombre = reader.GetString(1);
                    parentescos.Add(parentesco);
                }
                reader.Close();
            }

            // Agregar la lista de perfiles activos directamente al objeto User
            obj.Parentescos = parentescos;

            // Consulta para obtener los usuarios disponibles
            string sql3 = "select u.codigo, u.user_login from Usuario u where u.asignado =0";
            List<User> usuarios = new List<User>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(sql3, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    // Agregar los perfiles activos a una lista
                    User user = new User();
                    user.Código = reader.GetInt32(0);
                    user.Usuario = reader.GetString(1);
                    usuarios.Add(user);
                }
                reader.Close();
            }

            // Agregar la lista de perfiles activos directamente al objeto User
            obj.Usuarios = usuarios;

            return View(obj);
        }

        // POST: ApoderadoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Apoderado apoderado)
        {
            string connectionString = _config.GetConnectionString("cn");

            try
            {
                apoderado.Estado = "A";
                
                string sql = "INSERT INTO Cliente_Apoderado (codigo_usuario, apellido_paterno, apellido_materno, nombre, fecha_nacimiento, parentesco, tipo_documento, numero_documento, estado)\r\n    VALUES (@codigo_usuario, @apellido_paterno, @apellido_materno, @nombre, @fecha_nacimiento, @parentesco, @tipo_documento, @numero_documento, @estado)\r\n";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(sql, connection);

                    // Agregar los parámetros a la consulta
                    command.Parameters.AddWithValue("@codigo_usuario", apoderado.Usuario);
                    command.Parameters.AddWithValue("@apellido_paterno", apoderado.Apellido_Paterno);
                    command.Parameters.AddWithValue("@apellido_materno", apoderado.Apellido_Materno);
                    command.Parameters.AddWithValue("@nombre", apoderado.Nombre);
                    command.Parameters.AddWithValue("@fecha_nacimiento", apoderado.Fecha_Nacimiento);
                    command.Parameters.AddWithValue("@parentesco", apoderado.Parentesco);
                    command.Parameters.AddWithValue("@tipo_documento", apoderado.Tipo_Documento);
                    command.Parameters.AddWithValue("@numero_documento", apoderado.Número_Documento);
                    command.Parameters.AddWithValue("@estado", apoderado.Estado);

                    // Ejecutar la consulta
                    int result = command.ExecuteNonQuery();
                }
                apoderado.Usuarios = new List<User>();
                User user = new User();
                user.Asignado = 1;
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ApoderadoController/Edit/5
        public ActionResult Edit(int id)
        {
            string connectionString = _config.GetConnectionString("cn");

            Apoderado obj = new Apoderado();
            string sql = "SELECT ca.codigo,ca.apellido_paterno,ca.apellido_materno,ca.nombre,convert(varchar(10),CA.fecha_nacimiento, 23)as fecha_nacimiento ,ca.parentesco,ca.tipo_documento,ca.numero_documento,ca.estado FROM Cliente_Apoderado ca where ca.codigo=@Codigo\r\n";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Codigo", id);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    obj.Código            = reader.GetInt32(0);
                    obj.Apellido_Paterno  = reader.GetString(1);
                    obj.Apellido_Materno  = reader.GetString(2);
                    obj.Nombre            = reader.GetString(3);
                    obj.Fecha_Nacimiento  = reader.GetString(4);
                    obj.Parentescoss      = reader.GetInt32(5);
                    obj.Tipo_Documentoo   = reader.GetInt32(6);
                    obj.Número_Documento  = reader.GetString(7);
                    obj.Estado            = reader.GetString(8);
                }

                reader.Close();
            }

            // Consulta para obtener los tipos de documento 
            string sql3 = "select valor, nombre from Parametro_Sistema where codigo_padre = 1";
            List<TipoDocumento> tiposDocumento = new List<TipoDocumento>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(sql3, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    // Agregar los perfiles activos a una lista
                    TipoDocumento tipoDoc = new TipoDocumento();
                    tipoDoc.valor = reader.GetInt32(0);
                    tipoDoc.nombre = reader.GetString(1);
                    tiposDocumento.Add(tipoDoc);
                }
                reader.Close();
            }

            // Agregar la lista de perfiles activos directamente al objeto User
            obj.Tipo_Doc = tiposDocumento;

            // Consulta para obtener los parentescos
            string sql2 = "select valor, nombre from Parametro_Sistema where codigo_padre = 8";
            List<Parentesco> parentescos = new List<Parentesco>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(sql2, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    // Agregar los perfiles activos a una lista
                    Parentesco parentesco = new Parentesco();
                    parentesco.valor = reader.GetInt32(0);
                    parentesco.nombre = reader.GetString(1);
                    parentescos.Add(parentesco);
                }
                reader.Close();
            }

            // Agregar la lista de perfiles activos directamente al objeto User
            obj.Parentescos = parentescos;

            // Consulta para obtener los usuarios disponibles
            string sql4 = "select u.codigo, u.user_login from Usuario u where u.asignado =0";
            List<User> usuarios = new List<User>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(sql4, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    // Agregar los perfiles activos a una lista
                    User user = new User();
                    user.Código = reader.GetInt32(0);
                    user.Usuario = reader.GetString(1);
                    usuarios.Add(user);
                }
                reader.Close();
            }

            // Agregar la lista de perfiles activos directamente al objeto User
            obj.Usuarios = usuarios;

            return View(obj);
        }

        // POST: ApoderadoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Apoderado apoderado)
        {
            string connectionString = _config.GetConnectionString("cn");

            try
            {
                apoderado.Estado = "A";
                string sql = "UPDATE Cliente_Apoderado SET codigo_usuario = @codigo_usuario, apellido_paterno = @apellido_paterno, apellido_materno = @apellido_materno, nombre = @nombre, fecha_nacimiento=@fecha_nacimiento,parentesco=@parentesco,tipo_documento=@tipo_documento,numero_documento=@numero_documento, estado = @estado WHERE codigo = @Codigo";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(sql, connection);

                    // Agregar los parámetros a la consulta
                    command.Parameters.AddWithValue("@Codigo", apoderado.Código);
                    command.Parameters.AddWithValue("@codigo_usuario", apoderado.Usuario);
                    command.Parameters.AddWithValue("@apellido_paterno", apoderado.Apellido_Paterno);
                    command.Parameters.AddWithValue("@apellido_materno", apoderado.Apellido_Materno);
                    command.Parameters.AddWithValue("@nombre", apoderado.Nombre);
                    command.Parameters.AddWithValue("@fecha_nacimiento",apoderado.Fecha_Nacimiento);
                    command.Parameters.AddWithValue("@parentesco", apoderado.Parentescoss);
                    command.Parameters.AddWithValue("@tipo_documento", apoderado.Tipo_Documentoo);
                    command.Parameters.AddWithValue("@numero_documento", apoderado.Número_Documento);
                    command.Parameters.AddWithValue("@estado", apoderado.Estado);

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

        // GET: ApoderadoController/Delete/5
        public ActionResult Delete(int id)
        {
            string connectionString = _config.GetConnectionString("cn");

            Apoderado apoderado = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Buscar el usuario por su ID
                string sql = "SELECT * FROM Cliente_Apoderado WHERE codigo = @Codigo";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Codigo", id);

                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    apoderado = new Apoderado();
                    apoderado.Código = (int)reader["codigo"];
                    apoderado.Apellido_Paterno = (string)reader["apellido_paterno"];
                    apoderado.Apellido_Materno = (string)reader["apellido_materno"];
                    apoderado.Nombre = (string)reader["nombre"];
                    apoderado.Número_Documento = (string)reader["numero_documento"];
                }
            }

            // Pasar los datos del usuario a la vista de eliminación
            return View(apoderado);
        }

        // POST: ApoderadoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            string connectionString = _config.GetConnectionString("cn");

            try
            {
                // Actualizar el estado del usuario a inactivo
                string sql = "UPDATE Cliente_Apoderado SET estado = 'I' WHERE codigo = @Codigo";

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
