using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidad.Seguridad
{
    public class enMenu
    {
        public int Codigo { get; set; }
        public int Orden { get; set; }
        public int Cod_Padre { get; set; }
        public string Nombre { get; set; }
        public string Ruta { get; set; }
    }
}
