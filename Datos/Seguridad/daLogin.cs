using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entidad.Seguridad;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace Datos.Seguridad
{
    public class daLogin
    {
        //string cn = "server=DESKTOP-TO74OF5\\SQLEXPRESS;database=Cruz_Saco;integrated security=true;";
        //string cn = @"server=DESKTOP-QN8SLI9\SQLEXPRESS;database=Cruz_Saco;integrated security=true;";

        private readonly IConfiguration _config;

        public daLogin(IConfiguration config)
        {
            _config = config;
        }



        public enLogin ValidarLogin(string usuario, string clave)
        {
            string cn = _config.GetConnectionString("cn");


            enLogin objLog = new enLogin();

            SqlConnection cone = new SqlConnection(cn);

            SqlCommand cmd = new SqlCommand("sp_validar_login @user, @pass", cone);

            SqlParameter userParam = new SqlParameter("@user", SqlDbType.VarChar, 50);
            userParam.Value = usuario;
            cmd.Parameters.Add(userParam);

            SqlParameter passParam = new SqlParameter("@pass", SqlDbType.VarChar, 50);
            passParam.Value = clave;
            cmd.Parameters.Add(passParam);


            cone.Open();

            SqlDataReader reader = cmd.ExecuteReader();
            if (reader != null)
            {
                while (reader.Read())
                {
                    //objLog.Codigo = int.Parse(reader["Código de respuesta"].ToString());
                    //objLog.Mensaje = reader["Mensaje de respuesta"].ToString();
                    //objLog.Perfil = int.Parse(reader["Perfil"].ToString());
                    objLog.Codigo = int.Parse(reader["codigo"].ToString());
                    objLog.Mensaje = reader["mensaje"].ToString();
                    objLog.Perfil = int.Parse(reader["Perfil"].ToString());
                    objLog.Usuario = int.Parse(reader["Usuario"].ToString());

                }
            }
            cone.Close();
            return objLog;
        }

        public List<enMenu> ListarMenu(int perfil)
        {
            string cn = _config.GetConnectionString("cn");

            List<enMenu> oLista = new List<enMenu>();

            SqlConnection cone = new SqlConnection(cn);

            SqlCommand cmd = new SqlCommand("sp_Listar_Opciones_Menu @perfil", cone);

            SqlParameter userParam = new SqlParameter("@perfil", SqlDbType.Int);
            userParam.Value = perfil;
            cmd.Parameters.Add(userParam);

            cone.Open();

            SqlDataReader reader = cmd.ExecuteReader();
            if (reader != null)
            {
                while (reader.Read())
                {
                    oLista.Add(
                        new enMenu()
                        {
                            Codigo = int.Parse(reader["codigo"].ToString()),
                            Orden = int.Parse(reader["orden"].ToString()),
                            Cod_Padre = int.Parse(reader["codigo_padre"].ToString()),
                            Nombre = reader["nombre"].ToString(),
                            Ruta = reader["ruta"].ToString()
                        }
                    );
                }
            }
            cone.Close();
            return oLista;
        }

    }
}
