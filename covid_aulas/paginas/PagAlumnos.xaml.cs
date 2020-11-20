using System;
using System.Collections.Generic;
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
    /// Lógica de interacción para PagAlumnos.xaml
    /// </summary>
    public partial class PagAlumnos : Page
    {
        static HttpClient alumnoHttp = new HttpClient();
        List<Alumno> alumnos = new List<Alumno>();
        Alumno alumnoSeleccionado = new Alumno();

        public PagAlumnos()
        {
            InitializeComponent();
            btnAlumnoBaja.IsEnabled = false;
            btnAlumnoMod.IsEnabled = false;
            cargarAlumnos();
        }

        public async void cargarAlumnos()
        {
            try
            {
                alumnos = await GetAlumno("http://localhost:3000/alumno/", 1);
                dtAlumnos.ItemsSource = alumnos;
                dtAlumnos.Items.Refresh();

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                Environment.Exit(2);
            }
        }

        static async Task<List<Alumno>> GetAlumno(String path, int numero)
        {
            Alumno auxAlum = null;
            List<Alumno> alumnos = null;
            HttpResponseMessage msg = await alumnoHttp.GetAsync(path);
            if (msg.IsSuccessStatusCode)
            {
                var salida = await msg.Content.ReadAsStringAsync();
                if(numero == 0)
                {
                    alumnos = new List<Alumno>();
                    auxAlum = JsonSerializer.Deserialize<Alumno>(salida);
                    alumnos.Add(auxAlum);
                }
                else
                {
                    alumnos = JsonSerializer.Deserialize<List<Alumno>>(salida);
                }
            }
            return alumnos;
        }

        private async Task<Alumno> PostAlumno(Alumno alumno, String path)
        {
            Alumno returnAl = null;
            var json = JsonSerializer.Serialize<Alumno>(alumno);
            var cabeceras = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage msg = await alumnoHttp.PostAsync(path, cabeceras);
            if (msg.IsSuccessStatusCode)
            {
                var salida = await msg.Content.ReadAsStringAsync();
                returnAl = JsonSerializer.Deserialize<Alumno>(salida);
            }
            return returnAl;
        }

        private async Task<Alumno> PutAlumno(Alumno alumno, String path)
        {
            Alumno returnAl = new Alumno();
            var json = JsonSerializer.Serialize<Alumno>(alumno);
            var cabeceras = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage msg = await alumnoHttp.PutAsync(path, cabeceras);
            if (msg.IsSuccessStatusCode)
            {
                var salida = await msg.Content.ReadAsStringAsync();
                returnAl = JsonSerializer.Deserialize<Alumno>(salida);
            }
            else
            {
                returnAl = null;
            }
            return returnAl;
        }

        private  async Task<Boolean> DeleteAlumno(String path)
        {
            Boolean salida = false;
            HttpResponseMessage msg = await alumnoHttp.DeleteAsync(path);
            if (msg.IsSuccessStatusCode)
            {
                salida = true;
            }
            return salida;
        }

        private async void Click_PostAlumno(object sender, RoutedEventArgs e)
        {
            Alumno alumno = new Alumno();
            Alumno result = new Alumno();
            alumno.nombre = txtNombre.Text;
            alumno.apellidos = txtApellidos.Text;
            DateTime datetime = (DateTime)dpNacimiento.SelectedDate;
            String fecha = datetime.ToString("yyyyMMdd");
            alumno.fecha_nacimiento = fecha.Replace("-", "");
            alumno.telefono = txtTelefono.Text;
            ComboBoxItem ele = cbSexo.SelectedValue as ComboBoxItem;
            alumno.sexo = ele.Content.ToString();
            alumno.curso = txtCursoAlumno.Text;
            
            result = await PostAlumno(alumno, "http://localhost:3000/alumno");
            if(result != null)
            {
                alumnos.Add(alumno);
                cargarAlumnos();
                
            }
            
        }

        private void Click_dt_alumno(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dt = sender as DataGrid;
            this.alumnoSeleccionado = dt.SelectedItem as Alumno;
            this.LlenarForm();
            btnAlumnoBaja.IsEnabled = true;
            btnAlumnoMod.IsEnabled = true;
            btnPostAlumno.IsEnabled = false;
        }

        public void LlenarForm()
        {
            try
            {
                if(alumnoSeleccionado != null)
                {
                    txtNombre.Text = alumnoSeleccionado.nombre;
                    txtApellidos.Text = alumnoSeleccionado.apellidos;
                    DateTime dtfecha = alumnoSeleccionado.obtenerFechaNormal();
                    dpNacimiento.SelectedDate = dtfecha;
                    txtTelefono.Text = alumnoSeleccionado.telefono;
                    txtCursoAlumno.Text = alumnoSeleccionado.curso;
                    cbSexo.Text = alumnoSeleccionado.sexo;
                }
                
            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Environment.Exit(1);
            }
        }

        private void Click_Limpiar_alumno(object sender, RoutedEventArgs e)
        {
            limpiar_form();
            btnAlumnoBaja.IsEnabled = false;
            btnAlumnoMod.IsEnabled = false;
            
        }

        private void limpiar_form()
        {
            txtNombre.Clear();
            txtApellidos.Clear();
            DateTime dtfecha = DateTime.Now;
            dpNacimiento.SelectedDate = dtfecha;
            txtTelefono.Clear();
            txtCursoAlumno.Clear();
            btnPostAlumno.IsEnabled = true;
            
        }

        private async void Click_modificar_alumno(object sender, RoutedEventArgs e)
        {
            Alumno alumno = new Alumno();
            Alumno result = new Alumno();
            int alumnoID = this.alumnoSeleccionado.id;
            alumno.nombre = txtNombre.Text;
            alumno.apellidos = txtApellidos.Text;
            DateTime datetime = (DateTime)dpNacimiento.SelectedDate;
            String fecha = datetime.ToString("yyyyMMdd");
            alumno.fecha_nacimiento = fecha.Replace("-", "");
            alumno.telefono = txtTelefono.Text;
            ComboBoxItem ele = cbSexo.SelectedValue as ComboBoxItem;
            alumno.sexo = ele.Content.ToString();
            alumno.curso = txtCursoAlumno.Text;

            result = await PutAlumno(alumno, "http://localhost:3000/alumno/" + alumnoID);
            if (result != null)
            {

                cargarAlumnos();
                limpiar_form();
            }
        }

        private async void Click_baja_alumno(object sender, RoutedEventArgs e)
        {
            int alumnoID = this.alumnoSeleccionado.id;
            Boolean result = await DeleteAlumno("http://localhost:3000/alumno/" + alumnoID);
            if (result)
            {
                cargarAlumnos();
                limpiar_form();
            }
        }
    }
}
