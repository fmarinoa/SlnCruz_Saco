    using Cruz_Saco.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data.SqlClient;
using ClosedXML.Excel;
using System.Data;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Cruz_Saco.Controllers
{
    public class EstudianteController : Controller
    {
        //private readonly string connectionString = "server=DESKTOP-TO74OF5\\SQLEXPRESS;database=Cruz_Saco;integrated security=true;";
        //private readonly string connectionString = "server=DESKTOP-QN8SLI9\\SQLEXPRESS;database=Cruz_Saco;integrated security=true;";

        private readonly IConfiguration _config;

        public EstudianteController(IConfiguration config)
        {
            _config = config;
        }

        // GET: EstudianteController
        public ActionResult Index()
        {
            string connectionString = _config.GetConnectionString("cn");


            List<Estudiante> estudiantes = new List<Estudiante>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "sp_Listar_Clientes_Estudiantes";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Estudiante estudiante = new Estudiante
                            {
                                Código = (int)reader["codigo"],
                                Apoderado = (string)reader["apoderado"],
                                Apellido_Paterno = (string)reader["apellido_paterno"],
                                Apellido_Materno = (string)reader["apellido_materno"],
                                Nombre = (string)reader["nombre"],
                                Fecha_Nacimiento = (string)reader["fecha_nacimiento"],
                                Tipo_Documento = (string)reader["tipo_documento"],
                                Número_Documento = (string)reader["numero_documento"],
                                Estado = (string)reader["estado"]
                            };

                            estudiantes.Add(estudiante);
                        }
                    }
                }
            }

            return View(estudiantes);
        }

        // GET: EstudianteController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: EstudianteController/Create
        public ActionResult Create()
        {
            string connectionString = _config.GetConnectionString("cn");

            Estudiante obj = new Estudiante();

            // Consulta para obtener los apoderados disponibles
            string sql = "SELECT codigo, concat(apellido_paterno,' ',apellido_materno,', ',nombre) as nombre FROM Cliente_Apoderado WHERE estado = 'A'";
            List<Cod_Apoderado> apoderados = new List<Cod_Apoderado>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(sql, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    // Agregar los perfiles activos a una lista
                    Cod_Apoderado apoderado = new Cod_Apoderado();
                    apoderado.Codigo = reader.GetInt32(0);
                    apoderado.Nombre = reader.GetString(1);
                    apoderados.Add(apoderado);
                }
                reader.Close();
            }

            // Agregar la lista de perfiles activos directamente al objeto User
            obj.Cod_Apo = apoderados;
            //--------------------------------------------
            // Consulta para obtener los tipos de documeto
            string sql2 = "select valor, nombre from Parametro_Sistema where codigo_padre = 1";
            List<TipoDocumento> tiposDocumento = new List<TipoDocumento>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(sql2, connection);
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

            return View(obj);
        }

        // POST: EstudianteController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Estudiante estudiante)
        {
            string connectionString = _config.GetConnectionString("cn");

            try
            {
                estudiante.Estado = "A";
                string sql = "INSERT INTO Cliente_Estudiante(codigo_apoderado,apellido_paterno, apellido_materno, nombre, fecha_nacimiento,tipo_documento,numero_documento,estado) VALUES (@codigo_apoderado,@apellido_paterno, @apellido_materno, @nombre, @fecha_nacimiento,@tipo_documento,@numero_documento,@estado)";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(sql, connection);

                    // Agregar los parámetros a la consulta
                    command.Parameters.AddWithValue("@codigo_apoderado", estudiante.Codigo_apoderado);
                    command.Parameters.AddWithValue("@apellido_paterno", estudiante.Apellido_Paterno);
                    command.Parameters.AddWithValue("@apellido_materno", estudiante.Apellido_Materno);
                    command.Parameters.AddWithValue("@nombre", estudiante.Nombre);
                    command.Parameters.AddWithValue("@fecha_nacimiento", estudiante.Fecha_Nacimiento);
                    command.Parameters.AddWithValue("@tipo_documento", estudiante.Tipo_Documento);
                    command.Parameters.AddWithValue("@numero_documento", estudiante.Número_Documento);
                    command.Parameters.AddWithValue("@estado", estudiante.Estado);

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

        // GET: EstudianteController/Edit/5
        public ActionResult Edit(int id)
        {
            string connectionString = _config.GetConnectionString("cn");

            Estudiante obj = new Estudiante();
            string sql = "SELECT  ca.codigo,ca.codigo_apoderado,ca.apellido_paterno,ca.apellido_materno,ca.nombre,convert(varchar(10),CA.fecha_nacimiento, 23)as fecha_nacimiento ,ca.tipo_documento,ca.numero_documento,ca.estado FROM Cliente_Estudiante ca where ca.codigo=@Codigo";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Codigo", id);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    obj.Código = reader.GetInt32(0);
                    obj.Codigo_apoderado = reader.GetInt32(1);
                    obj.Apellido_Paterno = reader.GetString(2);
                    obj.Apellido_Materno = reader.GetString(3);
                    obj.Nombre = reader.GetString(4);
                    obj.Fecha_Nacimiento = reader.GetString(5);
                    obj.Tipo_Documentoo = reader.GetInt32(6);
                    obj.Número_Documento = reader.GetString(7);
                    obj.Estado = reader.GetString(8);
                }

                reader.Close();
            }

            // Consulta para obtener los apoderados disponibles
            string sql3 = "SELECT codigo, concat(apellido_paterno,' ',apellido_materno,', ',nombre) as nombre FROM Cliente_Apoderado WHERE estado = 'A'";
            List<Cod_Apoderado> apoderados = new List<Cod_Apoderado>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(sql3, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    // Agregar los perfiles activos a una lista
                    Cod_Apoderado apoderado = new Cod_Apoderado();
                    apoderado.Codigo = reader.GetInt32(0);
                    apoderado.Nombre = reader.GetString(1);
                    apoderados.Add(apoderado);
                }
                reader.Close();
            }

            // Agregar la lista de perfiles activos directamente al objeto User
            obj.Cod_Apo = apoderados;
            //--------------------------------------------
            // Consulta para obtener los tipos de documeto
            string sql2 = "select valor, nombre from Parametro_Sistema where codigo_padre = 1";
            List<TipoDocumento> tiposDocumento = new List<TipoDocumento>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(sql2, connection);
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

            return View(obj);
        }

        // POST: EstudianteController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Estudiante estudiante)
        {
            string connectionString = _config.GetConnectionString("cn");

            try
            {
                estudiante.Estado = "A";
                string sql = "UPDATE Cliente_Estudiante SET codigo_apoderado =@codigo_apoderado, apellido_paterno = @apellido_paterno, apellido_materno = @apellido_materno, nombre = @nombre, fecha_nacimiento=@fecha_nacimiento,tipo_documento=@tipo_documento,numero_documento=@numero_documento, estado = @estado WHERE codigo = @Codigo";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(sql, connection);

                    // Agregar los parámetros a la consulta
                    command.Parameters.AddWithValue("@Codigo", estudiante.Código);
                    command.Parameters.AddWithValue("@codigo_apoderado", estudiante.Codigo_apoderado);
                    command.Parameters.AddWithValue("@apellido_paterno", estudiante.Apellido_Paterno);
                    command.Parameters.AddWithValue("@apellido_materno", estudiante.Apellido_Materno);
                    command.Parameters.AddWithValue("@nombre", estudiante.Nombre);
                    command.Parameters.AddWithValue("@fecha_nacimiento", estudiante.Fecha_Nacimiento);
                    command.Parameters.AddWithValue("@tipo_documento", estudiante.Tipo_Documentoo);
                    command.Parameters.AddWithValue("@numero_documento", estudiante.Número_Documento);
                    command.Parameters.AddWithValue("@estado", estudiante.Estado);

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

        // GET: EstudianteController/Delete/5
        public ActionResult Delete(int id)
        {
            string connectionString = _config.GetConnectionString("cn");

            Estudiante estudiante = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Buscar el usuario por su ID
                string sql = "select ce.codigo , concat(ca.apellido_paterno,' ',ca.apellido_materno,', ',ca.nombre)as apoderado, concat(ce.apellido_paterno,' ',ce.apellido_materno,', ',ce.nombre) as estudiante,ce.numero_documento from Cliente_Apoderado ca inner join  Cliente_Estudiante CE on ca.codigo=ce.codigo_apoderado WHERE ce.codigo = @Codigo";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Codigo", id);

                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    estudiante = new Estudiante();
                    estudiante.Código = (int)reader["codigo"];
                    estudiante.Apoderado = (string)reader["apoderado"];
                    estudiante.Nombre = (string)reader["estudiante"];
                    estudiante.Número_Documento = (string)reader["numero_documento"];
                }
            }

            // Pasar los datos del usuario a la vista de eliminación
            return View(estudiante);
        }

        // POST: EstudianteController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            string connectionString = _config.GetConnectionString("cn");

            try
            {
                // Actualizar el estado del usuario a inactivo
                string sql = "UPDATE Cliente_Estudiante SET estado = 'I' WHERE codigo = @Codigo";

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

        public IActionResult ExportarExcel()
        {
            string connectionString = _config.GetConnectionString("cn");

            List<Estudiante> estudiantes = new List<Estudiante>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "sp_Listar_Clientes_Estudiantes";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Estudiante estudiante = new Estudiante
                            {
                                Código = (int)reader["codigo"],
                                Apoderado = (string)reader["apoderado"],
                                Apellido_Paterno = (string)reader["apellido_paterno"],
                                Apellido_Materno = (string)reader["apellido_materno"],
                                Nombre = (string)reader["nombre"],
                                Fecha_Nacimiento = (string)reader["fecha_nacimiento"],
                                Tipo_Documento = (string)reader["tipo_documento"],
                                Número_Documento = (string)reader["numero_documento"],
                                Estado = (string)reader["estado"]
                            };

                            estudiantes.Add(estudiante);
                        }
                    }
                }
            }

            using (FileStream fs = System.IO.File.OpenRead(@"C:\temp\plantilla_reporte_estudiantes.xlsx"))
            {
                using (XLWorkbook xl = new XLWorkbook(fs))
                {
                    var hoja = xl.Worksheet(1);
                    int nFilaInicio = 5;

                    for (int i = 0; i < estudiantes.Count; i++)
                    {
                        //hoja.Cell("B5").Value = "Test";
                        hoja.Cell("B" + (nFilaInicio + i).ToString()).Value = estudiantes[i].Código.ToString();
                        hoja.Cell("C" + (nFilaInicio + i).ToString()).Value = estudiantes[i].Apoderado;
                        hoja.Cell("D" + (nFilaInicio + i).ToString()).Value = estudiantes[i].Apellido_Paterno;
                        hoja.Cell("E" + (nFilaInicio + i).ToString()).Value = estudiantes[i].Apellido_Materno;
                        hoja.Cell("F" + (nFilaInicio + i).ToString()).Value = estudiantes[i].Nombre;
                        hoja.Cell("G" + (nFilaInicio + i).ToString()).Value = estudiantes[i].Fecha_Nacimiento;
                        hoja.Cell("H" + (nFilaInicio + i).ToString()).Value = estudiantes[i].Tipo_Documento;
                        hoja.Cell("I" + (nFilaInicio + i).ToString()).Value = estudiantes[i].Número_Documento;
                        hoja.Cell("J" + (nFilaInicio + i).ToString()).Value = estudiantes[i].Estado;

                    }

                    hoja.Column(1).AdjustToContents();
                    hoja.Column(2).AdjustToContents();
                    hoja.Column(3).AdjustToContents();
                    hoja.Column(4).AdjustToContents();
                    hoja.Column(5).AdjustToContents();
                    hoja.Column(6).AdjustToContents();
                    hoja.Column(7).AdjustToContents();
                    hoja.Column(8).AdjustToContents();

                    using (MemoryStream mstream = new MemoryStream())
                    {
                        // Guardo el trabajo en memoria y lo envio al front
                        xl.SaveAs(mstream);
                        return File(mstream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reporte_Estudiantes.xlsx");
                    }

                }
            }
        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult ExportarExcel()
        //{
        //    List<Estudiante> estudiantes = new List<Estudiante>();

        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        string sql = "sp_Listar_Clientes_Estudiantes";

        //        using (SqlCommand command = new SqlCommand(sql, connection))
        //        {
        //            connection.Open();

        //            using (SqlDataReader reader = command.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    Estudiante estudiante = new Estudiante
        //                    {
        //                        Código = (int)reader["codigo"],
        //                        Apoderado = (string)reader["apoderado"],
        //                        Apellido_Paterno = (string)reader["apellido_paterno"],
        //                        Apellido_Materno = (string)reader["apellido_materno"],
        //                        Nombre = (string)reader["nombre"],
        //                        Fecha_Nacimiento = (string)reader["fecha_nacimiento"],
        //                        Tipo_Documento = (string)reader["tipo_documento"],
        //                        Número_Documento = (string)reader["numero_documento"],
        //                        Estado = (string)reader["estado"]
        //                    };

        //                    estudiantes.Add(estudiante);
        //                }
        //            }
        //        }
        //    }

        //    DataTable dt = new DataTable("ExportExcel");
        //    for (int i = 0; i <= 9; i++)
        //    {
        //        dt.Columns.Add("Columna " + i.ToString(), System.Type.GetType("System.String"));
        //    }

        //    DataRow workRow;
        //    for (int i = 0; i < estudiantes.ToArray().Length; i++)
        //    {
        //        workRow = dt.NewRow();
        //        workRow[0] = estudiantes[i].Código.ToString();
        //        workRow[1] = estudiantes[i].Apoderado;
        //        workRow[2] = estudiantes[i].Apellido_Paterno;
        //        workRow[3] = estudiantes[i].Apellido_Materno;
        //        workRow[4] = estudiantes[i].Nombre;
        //        workRow[5] = estudiantes[i].Fecha_Nacimiento;
        //        workRow[6] = estudiantes[i].Tipo_Documento;
        //        workRow[7] = estudiantes[i].Número_Documento;
        //        workRow[8] = estudiantes[i].Estado;
        //        dt.Rows.Add(workRow);
        //    }

        //    MemoryStream ms = new MemoryStream();
        //    using (FileStream fs = System.IO.File.OpenRead(@"C:\temp\plantilla_reporte_estudiantes.xlsx"))
        //    using (ExcelPackage excelPackage = new ExcelPackage(fs))
        //    {
        //        ExcelWorkbook excelWorkBook = excelPackage.Workbook;
        //        ExcelWorksheet excelWorksheet = excelWorkBook.Worksheets[1];
        //        excelWorksheet.Cells["B5"].LoadFromDataTable(dt, false);
        //        excelPackage.SaveAs(ms); // This is the important part.
        //    }

        //    ms.Position = 0;
        //    return new FileStreamResult(ms, "application/xlsx")
        //    {
        //        FileDownloadName = "Tester.xlsx"
        //    };

        //}
    }
}
