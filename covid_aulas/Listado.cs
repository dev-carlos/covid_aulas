using System;
using System.Collections.Generic;
using System.Text;

namespace covid_aulas
{
    class Listado
    {
        public int id { get; set; }
        public int idAlumno { get; set; }
        public int idAula { get; set; }
        public string fecha { get; set; }
        public string hora { get; set; }

        public DateTime obtenerFechaNormal()
        {
            DateTime dtfecha = new DateTime(Int32.Parse(this.fecha.Substring(0, 4)), Int32.Parse(this.fecha.Substring(4, 2)), Int32.Parse(this.fecha.Substring(6, 2)));
            return dtfecha;
        }

    }
}
