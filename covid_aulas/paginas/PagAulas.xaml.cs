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
    /// Lógica de interacción para PagAulas.xaml
    /// </summary>
    public partial class PagAulas : Page
    {
        static HttpClient aulaHttp = new HttpClient();
        List<Aula> aulas = new List<Aula>();
        Aula aulaSeleccionada = new Aula();
        public PagAulas()
        {
            InitializeComponent();
            btnAulaBaja.IsEnabled = false;
            btnAulaMod.IsEnabled = false;
            cargarAulas();
        }

        public async void cargarAulas()
        {
            try
            {
                aulas = await GetAula("http://localhost:3000/aula/", 1);
                dtAulas.ItemsSource = aulas;
                dtAulas.Items.Refresh();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Environment.Exit(2);
            }
        }

        static async Task<List<Aula>> GetAula(String path, int numero)
        {
            Aula auxAula = null;
            List<Aula> aulas = null;
            HttpResponseMessage msg = await aulaHttp.GetAsync(path);
            if (msg.IsSuccessStatusCode)
            {
                var salida = await msg.Content.ReadAsStringAsync();
                if (numero == 0)
                {
                    aulas = new List<Aula>();
                    auxAula = JsonSerializer.Deserialize<Aula>(salida);
                    aulas.Add(auxAula);
                }
                else
                {
                    aulas = JsonSerializer.Deserialize<List<Aula>>(salida);
                }
            }
            return aulas;
        }

        private async Task<Aula> PostAula(Aula aula, String path)
        {
            Aula returnAula = null;
            var json = JsonSerializer.Serialize<Aula>(aula);
            var cabeceras = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage msg = await aulaHttp.PostAsync(path, cabeceras);
            if (msg.IsSuccessStatusCode)
            {
                var salida = await msg.Content.ReadAsStringAsync();
                returnAula = JsonSerializer.Deserialize<Aula>(salida);
            }
            return returnAula;
        }

        private async Task<Aula> PutAula(Aula aula, String path)
        {
            Aula returnAula = new Aula();
            var json = JsonSerializer.Serialize<Aula>(aula);
            var cabeceras = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage msg = await aulaHttp.PutAsync(path, cabeceras);
            if (msg.IsSuccessStatusCode)
            {
                var salida = await msg.Content.ReadAsStringAsync();
                returnAula = JsonSerializer.Deserialize<Aula>(salida);
            }
            else
            {
                returnAula = null;
            }
            return returnAula;
        }

        private async Task<Boolean> DeleteAula(String path)
        {
            Boolean salida = false;
            HttpResponseMessage msg = await aulaHttp.DeleteAsync(path);
            if (msg.IsSuccessStatusCode)
            {
                salida = true;
            }
            return salida;
        }

        public void LlenarForm()
        {
            try
            {
                if (aulaSeleccionada != null)
                {
                    txtNombre.Text = aulaSeleccionada.nombre;
                    txtPlanta.Text = aulaSeleccionada.planta.ToString();
                    txtCapacidad.Text = aulaSeleccionada.capacidad.ToString();
                }

            }
            
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Environment.Exit(1);
            }
        }

        private void limpiar_form()
        {
            txtNombre.Clear();
            txtCapacidad.Clear();
            txtPlanta.Clear();
            btnPostAula.IsEnabled = true;

        }

        private void Click_Limpiar_aula(object sender, RoutedEventArgs e)
        {
            limpiar_form();
            btnAulaBaja.IsEnabled = false;
            btnAulaMod.IsEnabled = false;
        }

        private async void Click_baja_aula(object sender, RoutedEventArgs e)
        {
            int aulaID = this.aulaSeleccionada.id;
            Boolean result = await DeleteAula("http://localhost:3000/aula/" + aulaID);
            if (result)
            {

                cargarAulas();
                limpiar_form();
            }
        }

        private async void Click_modificar_aula(object sender, RoutedEventArgs e)
        {
            Aula aula = new Aula();
            Aula result = new Aula();
            int aulaID = this.aulaSeleccionada.id;
            aula.nombre = txtNombre.Text;
            aula.planta = Int32.Parse(txtPlanta.Text);
            aula.capacidad = Int32.Parse(txtCapacidad.Text);
   
            result = await PutAula(aula, "http://localhost:3000/aula/" + aulaID);
            if (result != null)
            {

                cargarAulas();
                limpiar_form();
            }
        }

        private async void Click_PostAula(object sender, RoutedEventArgs e)
        {
            Aula aula = new Aula();
            Aula result = new Aula();
            aula.nombre = txtNombre.Text;
            aula.planta = Int32.Parse(txtPlanta.Text);
            aula.capacidad = Int32.Parse(txtCapacidad.Text);

            result = await PostAula(aula, "http://localhost:3000/aula");
            if (result != null)
            {

                cargarAulas();
                limpiar_form();
            }
        }

        private void Click_dt_aula(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dt = sender as DataGrid;
            this.aulaSeleccionada = dt.SelectedItem as Aula;
            this.LlenarForm();
            btnAulaBaja.IsEnabled = true;
            btnAulaMod.IsEnabled = true;
            btnPostAula.IsEnabled = false;
        }
    }
}
