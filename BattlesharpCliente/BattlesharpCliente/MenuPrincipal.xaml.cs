using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Media;
using System.Resources;
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
    /// Interacción lógica para la ventana MenuPrincipal.xaml
    /// </summary>
    public partial class MenuPrincipal : Window
    {
        //Usuario del jugador que inició sesión
        public string Usuario { set; get; }
        //Administrador de recursos que pondrá el texto en su idioma
        ResourceManager administradorDeRecursos;
        //Cultura que el administrador de recursos usará para saber que idioma escoger
        CultureInfo cultura;
        public string Lenguaje { set; get; }

        /// <summary>
        /// Constructor de la ventana MenuPrincipal.xaml
        /// </summary>
        /// <param name="usuarioIniciado">Usuario que inicio sesión en el sistema</param>
        /// <param name="lenguaje">Lenguaje que se mostrará en esta ventana</param>
        public MenuPrincipal(string usuarioIniciado, string lenguaje)
        {
            InitializeComponent();
            //Se necesita el usuario para saber quien inicio sesión para posteriormente agregarlo a la lista de jugadores activos
            Usuario = usuarioIniciado;
            //Se asigna el administrador de recursos que es el que obtendrá las cadenas
            administradorDeRecursos = new ResourceManager("Battlesharp.lenguajes.Resource", typeof(MenuPrincipal).Assembly);
            //Lenguaje que se mostrará en la ventana
            Lenguaje = lenguaje;
            //Se ponen el texto de la ventana en el lenguaje que se ingresó
            PonerTexto();
            //Se muestra el usuario que ingresó sesión en la pantalla
            Usuario = usuarioIniciado;
            txtblUsuario.Text = Usuario;
        }

        /// <summary>
        /// Coloca los text block en el idioma que se requiere
        /// </summary>
        /// <remarks>Se ocupan text block porque así se puede alinear el texto y no se pierde formato</remarks>
        public void PonerTexto()
        {
            //Con el atributo Lenguaje se crea una cultura para especificar que archivo se ocupará
            cultura = CultureInfo.CreateSpecificCulture(Lenguaje);

            //Los text block se ponene en el texto en el idioma solicitado
            txtblockJugar.Text = administradorDeRecursos.GetString("Jugar", cultura);
            txtblockPuntuaciones.Text = administradorDeRecursos.GetString("PuntuacionesMasAltas", cultura);
            txtblockSalir.Text = administradorDeRecursos.GetString("Salir", cultura);
            Title = administradorDeRecursos.GetString("MenuPrincipal", cultura);
        }

        /// <summary>
        /// Interacción lógica que ocurre cuando se pulse la opción Jugar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtblockJugar_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Jugar nuevoJuego = new Jugar(Usuario, Lenguaje);
            nuevoJuego.Show();
        }

        /// <summary>
        /// Interacción lógica que ocurre cuando se pulse la opción Salir
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtblockSalir_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //Cierra la pantalla
            Close();
        }

        /// <summary>
        /// Interacción lógica que ocurre cuando se cierra la ventana
        /// </summary>
        /// <remarks>Hau dos formas de salir, pulsando la opción Salir y cerrando la ventana</remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindow ventanaPrincial = new MainWindow(Lenguaje);
            ventanaPrincial.Show();
        }

        /// <summary>
        /// Interacción lógica que ocurre cuando se selecciona la opción Puntuaciones
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtblockPuntuaciones_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //Se crea la pantalla puntuaciones con el lenguaje de esta ventana
            Puntuaciones puntuaciones = new Puntuaciones(Lenguaje);
            //Se muestra la ventana
            puntuaciones.Show();
            Cursor = Cursors.Arrow;
        }
    }
}
