using DocumentFormat.OpenXml.Drawing.Charts;
using System;

namespace Cruz_Saco.Models
{
    public class Historial
    {
        public int Nro_Cuota { get; set; }
        public string Mes { get; set; }
        public DateTime Fecha_Vencimiento { get; set; }
        public string Fecha_Pago { get; set; }
        public string Hora_Pago { get; set; }
        public decimal Monto { get; set; }
        public string Estado { get; set; }
    }
}
