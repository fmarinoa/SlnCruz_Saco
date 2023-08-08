using System;
using System.Collections.Generic;

#nullable disable

namespace Cruz_Saco.Models
{
    public class Usuario
    {

        public int Codigo { get; set; }
        public string UserLogin { get; set; }
        public string PassLogin { get; set; }
        public int CodigoPerfil { get; set; }
        public string Estado { get; set; }

      
    }
}
