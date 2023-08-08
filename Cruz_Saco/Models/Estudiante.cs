using System.Collections.Generic;

namespace Cruz_Saco.Models
{
    public class Estudiante
    {
        public int Código { get; set; }
        public int Codigo_apoderado { get; set; }
        public string Apoderado { get; set; }
        public string Apellido_Paterno { get; set; }
        public string Apellido_Materno { get; set; }
        public string Nombre { get; set; }
        public string Fecha_Nacimiento { get; set; }
        public string Tipo_Documento { get; set; }
        public int Tipo_Documentoo { get; set; }
        public string Número_Documento { get; set; }
        public string Estado { get; set; }
        public List<TipoDocumento> Tipo_Doc { get; internal set; }
        public List<Cod_Apoderado> Cod_Apo { get; internal set; }
    }
}
