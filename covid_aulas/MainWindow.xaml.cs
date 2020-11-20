using covid_aulas.paginas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace covid_aulas
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Page> misPaginas = new List<Page>();
        public MainWindow()
        {
            misPaginas.Add(new PagAulas());
            misPaginas.Add(new PagAlumnos());
            misPaginas.Add(new PagAsistencia());
            misPaginas.Add(new PagInforme());
            InitializeComponent();
            cambiarPagina(2);
        }

        private void cambiarPagina(int p)
        {
            framePrincipal.Content = misPaginas[p];
        }

        private void NavAlumno_Click(object sender, RoutedEventArgs e)
        {
            
            cambiarPagina(1);
        }

        private void NavAula_Click(object sender, RoutedEventArgs e)
        {
            
            cambiarPagina(0);
        }

        private void NavAsistencia_Click(object sender, RoutedEventArgs e)
        {
            cambiarPagina(2);
        }

        private void NavInforme_Click(object sender, RoutedEventArgs e)
        {
            cambiarPagina(3);
        }
    }
}
