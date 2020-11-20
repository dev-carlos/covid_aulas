using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace covid_aulas.paginas
{
    /// <summary>
    /// Lógica de interacción para PagInforme.xaml
    /// </summary>
    public partial class PagInforme : Page
    {
        static HttpClient listadoHttp = new HttpClient();
        static HttpClient alumnoHttp = new HttpClient();
        List<Alumno> lsAlumnos = new List<Alumno>();
        List<Listado> lsListado = new List<Listado>();
        List<Alumno> alumnosCurso = new List<Alumno>();
        List<Listado> listadoDef = new List<Listado>();
        Alumno m_alumno = new Alumno();

        public PagInforme()
        {
            cargarComboCursosAsync();
            InitializeComponent();
        }

        public async void cargarComboCursosAsync()
        {
            List<string> cursos = await ObtenerCursos();
            try
            {
                cbCursoInforme.ItemsSource = cursos;
                cbCursoInforme.Items.Refresh();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Environment.Exit(2);
            }
        }

        public async Task<List<string>> ObtenerCursos()
        {
            lsAlumnos = await GetAlumno("http://localhost:3000/alumno/");
            List<string> cursos = new List<string>();

            for (int i = 0; i < lsAlumnos.Count; i++)
            {
                if (!cursos.Contains(lsAlumnos[i].curso))
                {
                    cursos.Add(lsAlumnos[i].curso);
                }
            }

            return cursos;
        }

        static async Task<List<Alumno>> GetAlumno(String path)
        {

            List<Alumno> alumnos = null;
            HttpResponseMessage msg = await alumnoHttp.GetAsync(path);
            if (msg.IsSuccessStatusCode)
            {
                var salida = await msg.Content.ReadAsStringAsync();

                alumnos = JsonSerializer.Deserialize<List<Alumno>>(salida);

            }
            return alumnos;
        }

       

        private void Click_dt_informe_alu_curso(object sender, SelectionChangedEventArgs e)
        {
            this.ObtenerListado();
        }

        private void cbCursoInforme_change(object sender, SelectionChangedEventArgs e)
        {
            var comboboxSelect = sender as ComboBox;
            string curso = comboboxSelect.SelectedItem as string;
            obtenerAlumnosCurso(curso);
        }

        public void obtenerAlumnosCurso(string curso)
        {
            alumnosCurso.Clear();
            for (int i = 0; i < lsAlumnos.Count; i++)
            {
                if (lsAlumnos[i].curso == curso)
                {
                    alumnosCurso.Add(lsAlumnos[i]);
                }
            }
            dtInformeAluCurso.ItemsSource = alumnosCurso;
            dtInformeAluCurso.Items.Refresh();

        }


        public async void ObtenerListado()
        {
            List<Informe> coincidentes = new List<Informe>();
            //var idAlumno = from alumno in alumnosCurso select alumno;

            lsListado = await GetListado("http://localhost:3000/listado/");
            DateTime datetime = (DateTime)dpFechaInforme.SelectedDate;
            DateTime cincoAnteriores = datetime.AddDays(-5);

            foreach (var item in lsListado)
            {
                DateTime fecha = item.obtenerFechaNormal();
                foreach(var alumnoCurso in alumnosCurso)
                {
                    if(item.idAlumno == alumnoCurso.id && (fecha <= datetime && fecha >= cincoAnteriores)){
                        Informe informe = new Informe();
                        informe.idAlumno = item.idAlumno;
                        informe.nombreAlumno = alumnoCurso.nombre;
                        informe.apellidoAlumno = alumnoCurso.apellidos;
                        informe.fecha = fecha;
                        informe.hora = item.hora;
                        coincidentes.Add(informe);
                    }
                }
            }
            
            dtInformeAluCoincidente.ItemsSource = coincidentes;
            dtInformeAluCoincidente.Items.Refresh();
            
        }

        static async Task<List<Listado>> GetListado(String path)
        {

            List<Listado> lsListado = null;
            HttpResponseMessage msg = await listadoHttp.GetAsync(path);
            if (msg.IsSuccessStatusCode)
            {
                var salida = await msg.Content.ReadAsStringAsync();
                
                
                lsListado = JsonSerializer.Deserialize<List<Listado>>(salida);
                
            }
            return lsListado;
        }
    }
}
