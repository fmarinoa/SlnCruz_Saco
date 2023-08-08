using Entidad.Seguridad;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datos.Usuario;
using Entidad.Usuario;
using Microsoft.Extensions.Configuration;

namespace Datos.Usuario
{
    public class daUsuario
    {
        //string cn = "server=DESKTOP-TO74OF5\\SQLEXPRESS;database=Cruz_Saco;integrated security=true;";

        private readonly IConfiguration _config;

        public daUsuario(IConfiguration config)
        {
            _config = config;
        }


        public List<enUsuario> ListarUsuario()
        {
            string cn = _config.GetConnectionString("cn");

            List<enUsuario> objUsuario = new List<enUsuario>();

            SqlConnection cone = new SqlConnection(cn);

            SqlCommand cmd = new SqlCommand("lista_usu", cone);


            cone.Open();

            SqlDataReader reader = cmd.ExecuteReader();
            if (reader != null)
            {
                while (reader.Read())
                {
                    objUsuario.Add(
                        new enUsuario
                        {
                            codigo = int.Parse(reader["Fila"].ToString()),
                            fila = int.Parse(reader["codigo"].ToString()),
                            usuario = reader["user_login"].ToString(),
                            perfil = reader["Perfil"].ToString(),
                            estado = reader["Estado"].ToString()
                        }
                    );
                }
            }
            cone.Close();
            return objUsuario;
        }
    }
}
