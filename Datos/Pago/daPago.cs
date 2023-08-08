using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Entidad.Pago;
using Microsoft.Extensions.Configuration;

namespace Datos.Pago
{
    public class daPago
    {
        //string cn = @"server=DESKTOP-TO74OF5\SQLEXPRESS;database=Cruz_Saco;integrated security=true;";

        private readonly IConfiguration _config;

        public daPago(IConfiguration config)
        {
            _config = config;
        }

        public List<enPago> Reporte_Informe_Pagos(string tipo, string f_ini, string f_fin)
        {
            string cn = _config.GetConnectionString("cn");

            List<enPago> objUsuario = new List<enPago>();

            SqlConnection cone = new SqlConnection(cn);

            SqlCommand cmd = new SqlCommand("sp_Reporte_Informe_Pagos @tipo,@fecha_inicio,@fecha_fin", cone);
            cmd.Parameters.AddWithValue("@tipo", tipo);
            cmd.Parameters.AddWithValue("@fecha_inicio", f_ini);
            cmd.Parameters.AddWithValue("@fecha_fin", f_fin);

            cone.Open();

            SqlDataReader reader = cmd.ExecuteReader();
            if (reader != null)
            {
                while (reader.Read())
                {
                    objUsuario.Add(
                        new enPago
                        {
                            Codigo_Estudiante = int.Parse(reader["Cod_Est"].ToString()),
                            Nombres_Estudiantes = reader["Nombres"].ToString(),
                            Codigo_Pago = int.Parse(reader["Cod_Pago"].ToString()),
                            Desc_Pago = reader["descripcion"].ToString(),
                            Monto = reader["Monto"].ToString(),
                            Ajuste_manual = reader["Ajuste_manual"].ToString(),
                            Fecha_Pago = reader["Fecha_Pago"].ToString()
                        }
                    );
                }
            }
            cone.Close();
            return objUsuario;

        }
    }
}
