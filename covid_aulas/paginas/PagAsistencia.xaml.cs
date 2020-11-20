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
    /// Lógica de interacción para PagAsistencia.xaml
    /// </summary>
    public partial class PagAsistencia : Page
    {
        static HttpClient listadoHttp = new HttpClient();
        static HttpClient alumnoHttp = new HttpClient();
        static HttpClient aulaHttp = new HttpClient();
        List<Alumno> lsAlumnos = new List<Alumno>();
        List<Alumno> alumnosCurso = new List<Alumno>();
        List<Alumno> lsPresentes = new List<Alumno>();
        List<Aula> lsAula = new List<Aula>();
        Alumno m_alumno = new Alumno();
        Aula m_aula = new Aula();

        public PagAsistencia()
        {
            cargarComboCursosAsync();
            cargarComboAulasAsync();
            InitializeComponent();
            llenarComboHoras();
        }

        private async void cargarComboAulasAsync()
        {
           lsAula = await GetAula("http://localhost:3000/aula/");
            foreach(Aula aula in lsAula)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = aula.nombre;
                item.Tag = aula.id;
                cbAulaAsistencia.Items.Add(item);
            }
        }

        static async Task<List<Aula>> GetAula(String path)
        {
            List<Aula> aulas = null;
            HttpResponseMessage msg = await aulaHttp.GetAsync(path);
            if (msg.IsSuccessStatusCode)
            {
                var salida = await msg.Content.ReadAsStringAsync();

                aulas = JsonSerializer.Deserialize<List<Aula>>(salida);

            }
            return aulas;
        }

        public async void cargarComboCursosAsync()
        {
            List<string> cursos = await ObtenerCursos();
            try
            {
                cbCursoAsistencia.ItemsSource = cursos;
                cbCursoAsistencia.Items.Refresh();
                
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

            for(int i = 0; i<lsAlumnos.Count; i++)
            {
                if (!cursos.Contains(lsAlumnos[i].curso))
                {
                    cursos.Add(lsAlumnos[i].curso);
                }
            }

            return cursos;
        }

        public void obtenerAlumnosCurso(string curso)
        {
            alumnosCurso.Clear();
            
            for(int i = 0; i<lsAlumnos.Count; i++)
            {
                if(lsAlumnos[i].curso == curso)
                {
                    alumnosCurso.Add(lsAlumnos[i]);
                }
            }
            dtAsistenciaAluCurso.ItemsSource = alumnosCurso;
            dtAsistenciaAluCurso.Items.Refresh();
            
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



        private void Click_dt_asistencia_alu_curso(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dt = sender as DataGrid;
            if(dt.SelectedItem != null)
            {
                m_alumno = dt.SelectedItem as Alumno;
                dtAsistenciaAluPresente.ItemsSource = lsPresentes;
                lsPresentes.Add(m_alumno);
                alumnosCurso.Remove(m_alumno);


                dtAsistenciaAluCurso.Items.Refresh();
                dtAsistenciaAluPresente.Items.Refresh();
            }
           

        }

        private void Click_dt_asistencia_alu_presente(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dt = sender as DataGrid;
            if (dt.SelectedItem != null)
            {
                m_alumno = dt.SelectedItem as Alumno;
                lsPresentes.Remove(m_alumno);
                alumnosCurso.Add(m_alumno);


                dtAsistenciaAluCurso.Items.Refresh();
                dtAsistenciaAluPresente.Items.Refresh();
            }

        }

        private async void Click_PostAsistencia(object sender, RoutedEventArgs e)
        {
            if(lsPresentes.Count > 0 && cbHoras.SelectedItem != null && cbAulaAsistencia.SelectedItem != null && dpAsistencia.SelectedDate != null)
            {
                string hora = cbHoras.Text;
                btnPostAsistencia.Content = "Procesando....";
                var selectedAula = ((ComboBoxItem)cbAulaAsistencia.SelectedItem).Tag.ToString();
                int aula = Int32.Parse(selectedAula);
                DateTime fecha = (DateTime)dpAsistencia.SelectedDate;
                String fechaString = fecha.ToString("yyyyMMdd");
                Boolean seguir = true;
                foreach (Alumno alumno in lsPresentes)
                {
                    if (seguir)
                    {
                        Listado newItemListado = new Listado();
                        newItemListado.fecha = fechaString;
                        newItemListado.hora = hora;
                        newItemListado.idAlumno = alumno.id;
                        newItemListado.idAula = aula;
                        seguir = await PostListado(newItemListado, "http://localhost:3000/listado");
                    }
                }
                if (seguir)
                {
                    btnPostAsistencia.Content = "Grabar";
                    lsPresentes.Clear();
                    dtAsistenciaAluPresente.Items.Refresh();
                }
                    
            }
        }

        private async Task<Boolean> PostListado(Listado listado, String path)
        {
            Boolean salida = false;
            var json = JsonSerializer.Serialize<Listado>(listado);
            var cabeceras = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage msg = await listadoHttp.PostAsync(path, cabeceras);
            if (msg.IsSuccessStatusCode)
            {
                salida = true;
            }
            return salida;
        }

        private void Click_Limpiar_asistencia(object sender, RoutedEventArgs e)
        {

        }

        private void cbCursoAsistencia_change(object sender, SelectionChangedEventArgs e)
        {
            var comboboxSelect = sender as ComboBox;
            string curso = comboboxSelect.SelectedItem as string;
            obtenerAlumnosCurso(curso);
        }

        private void cbHoras_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        public void llenarComboHoras()
        {
            for(int i = 9; i<=21; i++)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = i + ":00";
                cbHoras.Items.Add(item);
            }
        }
    }
}
