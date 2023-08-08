using System.Collections.Generic;

namespace Cruz_Saco.Models
{
    public class Apoderado
    {
        public int Código { get; set; }
        public string Usuario { get; set; }
        public int Usuarioss { get; set; }
        public string Apellido_Paterno { get; set; }
        public string Apellido_Materno { get; set; }
        public string Nombre { get; set; }
        public string Fecha_Nacimiento { get; set; }
        public string Número_Documento { get; set; }
        public string Estado { get; set; }
        public string Tipo_Documento { get; set; }
        public int Tipo_Documentoo { get; set; }
        public string Parentesco { get; set; }
        public int Parentescoss { get; set; }
        public List<TipoDocumento> Tipo_Doc { get; internal set; }
        public List<Parentesco> Parentescos { get; internal set; }
        public List <User> Usuarios { get; internal set; }
    }
}
