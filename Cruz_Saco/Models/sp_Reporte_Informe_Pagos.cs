using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cruz_Saco.Models
{
    public class sp_Reporte_Informe_Pagos
    {
        [Key]
        public int Cod_Est { get; set; }
        public string Nombres { get; set; }
        public int Cod_Pago { get; set; }
        public string Descripcion { get; set; }
        public string Monto { get; set; }
        public string Ajuste_manual { get; set; }
        public string Fecha_Pago { get; set; }
        public string tipo { get; set; }


        public Parametro_Sistema Parametro_Sistema { get;  set; }

    }
}
