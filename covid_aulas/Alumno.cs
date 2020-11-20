using System;
using System.Collections.Generic;
using System.Text;

namespace covid_aulas
{
    class Alumno
    {
        public int id { get; set; }
        public String nombre { get; set; }
        public String apellidos { get; set; }
        public String fecha_nacimiento { get; set; }
        public String telefono { get; set; }
        public String sexo { get; set; }
        public String curso { get; set; }


        public DateTime obtenerFechaNormal()
        {
            DateTime dtfecha = new DateTime(Int32.Parse(this.fecha_nacimiento.Substring(0, 4)), Int32.Parse(this.fecha_nacimiento.Substring(4, 2)), Int32.Parse(this.fecha_nacimiento.Substring(6, 2)));
            return dtfecha;
        }

    }

    
}
