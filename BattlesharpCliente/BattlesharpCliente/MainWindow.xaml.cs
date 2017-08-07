using BattlesharpInterfaces;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Battlesharp
{
    /// <summary>
    /// Interacción lógica para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Canal para poder usar los métodos del servidor
        public ChannelFactory<IConsultarService> canal { set; get; }
        //La interfaz de los métodos del servidor
        public IConsultarService proxy { set; get; }
        //Administrador de recursos que pondrá el texto en su idioma
        ResourceManager administradorDeRecursos;
        //Cultura que el administrador de recursos usará para saber que idioma escoger
        CultureInfo cultura;
        public string Lenguaje { set; get; }
        

        /// <summary>
        /// Constructor de la ventana MainWindow.xaml que a su vez también es la primera ventana que se muestra del sistema 
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            //Se asigna el canal al Endpoint
            canal = new ChannelFactory<IConsultarService>("ConsultarServiceEndpoint");
            //Se crea el canal para que las demás funciones tengan los métodos del proxy
            proxy = canal.CreateChannel(); 
            //Se asigna el administrador de recursos
            administradorDeRecursos = new ResourceManager("Battlesharp.lenguajes.Resource", typeof(MainWindow).Assembly);
            //Cuando se inicia la ventana por primera vez se asigna el idioma en español
            Lenguaje = "es-MX";
            //Se ponen los text block en el idioma por defecto
            PonerTexto();
        }

        /// <summary>
        /// Constructor sobrecargado de la ventana MainWindow.xaml, esta sobrecarga se usa cuando se regresa de otra ventana como Registrar Jugador o el Menú Principal
        /// </summary>
        /// <param name="lenguaje">Lenguaje que se estableció previamente</param>
        public MainWindow(string lenguaje)
        {
            InitializeComponent();
            //Se asigna el canal al Endpoint
            canal = new ChannelFactory<IConsultarService>("ConsultarServiceEndpoint");
            //Se crea el canal para que las demás funciones tengan los métodos del proxy
            proxy = canal.CreateChannel();
            //Se asigna el administrador de recursos
            administradorDeRecursos = new ResourceManager("Battlesharp.lenguajes.Resource", typeof(MainWindow).Assembly);
            //Se asigna el lenguaje que se pasó por parámetro
            Lenguaje = lenguaje;
            //Se ponen el texto de la ventana en el lenguaje que se ingresó
            PonerTexto();
            
        }

        /// <summary>
        /// Valida la información ingresada antes del registro
        /// </summary>
        /// <param name="usuario">Se verifica si lo contenido en el text box usuario contiene texto</param>
        /// <param name="contrasena">Se verifica si lo contenido en el text box contraseña contiene texto</param>
        /// <returns></returns>
        public bool HayCamposNulos(string usuario, string contrasena)
        {
            //Verifica que la información no sea vacía
            if (usuario.Equals("") || contrasena.Equals(""))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Interacción lógica que ocurre cuando se selecciona la opción Registrarse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblRegistrarse_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //Se crea una nueva ventana RegistrarJugardor, con parámetro de lenguaje que está configurado.
            RegistrarJugador registrarJugador = new RegistrarJugador(Lenguaje);
            //Se muestra la ventana de Registrar Jugador
            registrarJugador.Show();
            //Se cierra esta ventana
            Close();
        }

        /// <summary>
        /// Interacción lógica que ocurre cuando se selecciona el botón iniciar sesión
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnIniciarSesion_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            //Se obtienen el texto que hay en el text box de usuario y lo que hay en el password box
            var usuario = txtbUsuario.Text;
            var contrasena = pwdContrasena.Password;

            //Se verifica que los dos campos contengan algo
            if (!HayCamposNulos(usuario, contrasena))
            {
                try
                {
                    //El proxy llama al método Iniciar Sesión del BDDService del servidor y se guarda el resultado en esta cadena
                    var resultadoInicioSesion = proxy.IniciarSesion(usuario, contrasena);
                    //Si el resultado del método Iniciar Sesión es distinto a UsuarioNoExiste
                    if (!resultadoInicioSesion.Equals("UsuarioNoExiste"))
                    {
                        //Se verifica que el resultado no sea ContrasenaIncorrecta
                        if (!resultadoInicioSesion.Equals("ContrasenaIncorrecta"))
                        {
                            Cursor = Cursors.Arrow;
                            //Se crea la ventana MenuPrincipal con el resultado de inicio de sesión (el nombre del usuario) y el lenguaje que contiene esta ventana
                            MenuPrincipal menu = new MenuPrincipal(resultadoInicioSesion, Lenguaje);
                            //Se muestra la ventana
                            menu.Show();
                            //Esta ventana se cierra
                            Close();
                        }
                        //En caso de que el resultado sea ContrasenaIncorrecta
                        else
                        {
                            MostrarMensaje(resultadoInicioSesion);
                            //Se limpia el password box para que se vuelva a intentar
                            pwdContrasena.Clear();
                        }
                    }
                    //En caso de que el usuario no exista
                    else
                    {
                        MostrarMensaje(resultadoInicioSesion);
                        //Solo se limpia el text box de usuario por si se ingresó una contraseña incorrecta
                        txtbUsuario.Clear();
                    }
                }
                //En caso de que no se pueda conectar
                catch (EndpointNotFoundException)
                {
                    MostrarMensaje("ProblemaConexion");
                }
                //En caso de que el servidor tarde mucho en responder
                catch (TimeoutException)
                {
                    MostrarMensaje("ServidorNoResponde");
                }
            }
            //En caso de que haya campos vacíos
            else
            {
                MostrarMensaje("CamposVacios");
            }
        }

        /// <summary>
        /// Método que crea mensajes para ser mostrados como retroalimentación
        /// </summary>
        /// <param name="causa">Que causó este mensaje</param>
        public void MostrarMensaje(string causa)
        {
            //Se crea el mensaje en el idioma que contiene esta ventana
            var mensaje = administradorDeRecursos.GetString(causa, cultura);
            Cursor = Cursors.Arrow;
            //Se muestra el mensaje
            MessageBox.Show(mensaje);
        }

        /// <summary>
        /// Interacción lógica que ocurre cuando se quiere cambiar el idioma a Español de México
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblEsMX_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //Cambian de visibilidad los text block
            txtblEsMX.Visibility = Visibility.Collapsed;
            txtblEnUS.Visibility = Visibility.Visible;
            //El lenguaje se asigna a la ventana
            Lenguaje = "es-MX";
            //Se pone el texto
            PonerTexto();
        }

        /// <summary>
        /// Interacción lógica que ocurre cuando se quiere cambiar el idioma a Inglés de Estados Unidos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblEnUS_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //Cambian de visibilidad los text block
            txtblEnUS.Visibility = Visibility.Collapsed;
            txtblEsMX.Visibility = Visibility.Visible;
            //El lenguaje se asigna a la ventana
            Lenguaje = "en-US";
            //Se pone el texto
            PonerTexto();
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
            txtblContrasena.Text = administradorDeRecursos.GetString("Contrasena", cultura);
            btnIniciarSesion.Content = administradorDeRecursos.GetString("IniciarSesion", cultura);
            txtbRegistrarse.Text = administradorDeRecursos.GetString("Registrarse", cultura);
            Title = administradorDeRecursos.GetString("Battlesharp", cultura);
        }
    }
}
