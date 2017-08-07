using BattlesharpInterfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Media;
using System.Resources;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Battlesharp
{
    /// <summary>
    /// Interacción lógica para Puntuaciones.xaml
    /// </summary>
    public partial class Puntuaciones : Window
    {
        //Canal para poder usar los métodos del servidor
        public ChannelFactory<IConsultarService> canal { set; get; }
        //La interfaz de los métodos del servidor
        public IConsultarService proxy { set; get; }
        //Collección observable que se usa para el mostrar en pantalla las puntuaciones
        public ObservableCollection<Tuple<string, int>> puntuaciones = new ObservableCollection<Tuple<string, int>>();
        //Administrador de recursos que pondrá el texto en su idioma
        ResourceManager administradorDeRecursos;
        //Cultura que el administrador de recursos usará para saber que idioma escoger
        CultureInfo cultura;
        public string Lenguaje { set; get; }

        public Puntuaciones(string lenguaje)
        {
            DataContext = this;
            InitializeComponent();
            //Se asigna el canal al Endpoint
            canal = new ChannelFactory<IConsultarService>("ConsultarServiceEndpoint");
            //Se crea el canal para que las demás funciones tengan los métodos del proxy
            proxy = canal.CreateChannel();
            //Se asigna el administrador de recursos
            administradorDeRecursos = new ResourceManager("Battlesharp.lenguajes.Resource", typeof(Puntuaciones).Assembly);
            //Se asigna el lenguaje de la pantalla
            Lenguaje = lenguaje;
            //Se muestra el texto en el lenguaje correspondiente
            PonerTexto();
            try
            {
                //Se obtienen las puntuaciones del servidor
                List<Tuple<string, int>> puntuacionesObtenidas = proxy.ObtenerMejoresPuntuaciones();
                //Se llena la lista que se va a mostrar
                LlenarLista(puntuacionesObtenidas);
                //Se asigna el contexto para que se muestren las puntuaciones
                DataContext = puntuaciones;
            }
            //En caso de que no se pueda conectar
            catch (EndpointNotFoundException)
            {
                var problemaConexion = administradorDeRecursos.GetString("ProblemaConexion", cultura);
                Cursor = Cursors.Arrow;
                //Se muestra el mensaje
                MessageBox.Show(problemaConexion);
            }
            //En caso de que el servidor tarde mucho en responder
            catch (TimeoutException)
            {
                var servidorNoResponde = administradorDeRecursos.GetString("ServidorNoResponde", cultura);
                Cursor = Cursors.Arrow;
                //Se muestra el mensaje
                MessageBox.Show(servidorNoResponde);
            }
        }

        /// <summary>
        /// Método que pone el texto en el lenguaje seleccionado
        /// </summary>
        public void PonerTexto()
        {
            //Se asigna la cultura
            cultura = CultureInfo.CreateSpecificCulture(Lenguaje);
            //Se ponen los text block en el idioma especificado
            txtblUsuario.Text = administradorDeRecursos.GetString("Usuario", cultura);
            txtblPuntos.Text = administradorDeRecursos.GetString("Puntos", cultura);
        }

        /// <summary>
        /// Metodo que llena la lista para mostrar
        /// </summary>
        /// <param name="puntuacionesObtenidas">Lista que se obtuvo del servidor</param>
        private void LlenarLista(List<Tuple<string, int>> puntuacionesObtenidas)
        {
            //Para cada elemento en la lista de las puntuaciones obtenidas por el proxy
            foreach (var puntuacion in puntuacionesObtenidas)
            {
                //Se agrega a la tabla que contiene esta ventana
                puntuaciones.Add(new Tuple<string, int>(puntuacion.Item1, puntuacion.Item2));
            }
        }
    }
}
