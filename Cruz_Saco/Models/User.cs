using System.Collections.Generic;

namespace Cruz_Saco.Models
{
    public class User
    {
        public int Código { get; set; }
        public string Usuario { get; set; }
        public string Contraseña { get; set; }
        public int CodigoPerfil { get; set; }
        public string Perfil { get; set; }
        public string Estado { get; set; }
        public int Asignado { get; set; }   
        public List<Perfil> Perfiles { get; internal set; }
        //public Perfil Perfil { get; internal set; }
    }
}
