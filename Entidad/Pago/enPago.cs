using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidad.Pago
{
    public class enPago
    {
        public int Codigo_Estudiante { get; set; }
        public string Nombres_Estudiantes { get; set; }
        public int Codigo_Pago { get; set; }
        public string Desc_Pago { get; set; }
        public string Monto { get; set; }

        public string Ajuste_manual { get; set; }
        public string Fecha_Pago { get; set; }

    }
}
