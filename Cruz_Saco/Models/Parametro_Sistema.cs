using System;
using System.Collections.Generic;

namespace Cruz_Saco.Models
{
    public class Parametro_Sistema
    {
        public int codigo { get; set; } 
        public int codigo_padre { get;set; }
        public string nombre { get; set; }  
        public string descripcion { get; set; }
        public int valor { get; set; }
        public string estado { get; set; }

        public static implicit operator Parametro_Sistema(List<Parametro_Sistema> v)
        {
            throw new NotImplementedException();
        }

        /*public static implicit operator Parametro_Sistema(List<Parametro_Sistema> v)
        {
            throw new NotImplementedException();
        }*/
    }
}
