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
using System.Windows.Shapes;
using System.ServiceModel;
using System.Media;
using BattlesharpInterfaces;
using System.Globalization;
using System.Resources;

namespace Battlesharp
{
    /// <summary>
    /// Interacción lógica para RegistrarJugador.xaml
    /// </summary>
    public partial class RegistrarJugador : Window
    {
        //Canal para poder usar los métodos del servidor
        public ChannelFactory<IRegistrarService> canal { set; get; }
        //La interfaz de los métodos del servidor
        public IRegistrarService proxy { set; get; }
        //Administrador de recursos que pondrá el texto en su idioma
        ResourceManager administradorDeRecursos;
        //Cultura que el administrador de recursos usará para saber que idioma escoger
        CultureInfo cultura;
        public string Lenguaje { set; get; }

        public RegistrarJugador(string lenguaje)
        {
            InitializeComponent();
            //Se asigna el canal al Endpoint
            canal = new ChannelFactory<IRegistrarService>("RegistrarServiceEndpoint");
            //Se crea el canal para que las demás funciones tengan los métodos del proxy
            proxy = canal.CreateChannel();

            administradorDeRecursos = new ResourceManager("Battlesharp.lenguajes.Resource", typeof(RegistrarJugador).Assembly);
            Lenguaje = lenguaje;
            PonerTexto();
        }

        public void PonerTexto()
        {
            //Se asigna la cultura
            cultura = CultureInfo.CreateSpecificCulture(Lenguaje);
            //Se ponen los text block en el idioma especificado
            txtblockContrasena.Text = administradorDeRecursos.GetString("Contrasena", cultura);
            txtblockConfirmacionContrasena.Text = administradorDeRecursos.GetString("ConfirmacionContrasena", cultura);
            txtblockUsuario.Text = administradorDeRecursos.GetString("Usuario", cultura);
            txtblockNombre.Text = administradorDeRecursos.GetString("Nombre", cultura);
            btnCancelar.Content = administradorDeRecursos.GetString("Cancelar", cultura);
            btnRegistrar.Content = administradorDeRecursos.GetString("Registrarse", cultura);
            Title = administradorDeRecursos.GetString("Registrarse", cultura);
        }

        //Evento que ocurre cuando una ventana se cierra
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Se llama a la ventana principal
            MainWindow ventanaPrincipal = new MainWindow(Lenguaje);
            //Se muestra la ventana
            ventanaPrincipal.Show();
        }

        //Valida la información ingresada antes del registro
        public string ValidarInformacion(string nombre, string usuario, 
                                       string contrasena, string confirmacionDeContrasena)
        {
            //Verifica que la información no sea nula
            if (!nombre.Equals("") && !usuario.Equals("") && 
                !contrasena.Equals("") && !confirmacionDeContrasena.Equals(""))
            {
                //Si no hay información nula, verifica que contraseña y confirmación de contraseña sean iguales
                if (contrasena.Equals(confirmacionDeContrasena))
                {
                    //Si son iguales regresa información válida
                    return ("InformacionValida");
                }
                else
                {
                    //Si no son iguales regresa retroalimentación para el usuario
                    return ("ContrasenasNoIguales");
                }
            }
            else
            {
                //Si hay información vacía regresa retroalimentación para el usuario
                return ("CamposVacios");
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

        private void btnRegistrar_Click(object sender, RoutedEventArgs e)
        {
            //El cursor cambia a espera mientras se hacen las validaciones y se registra información
            Cursor = Cursors.Wait;
            //Se asignan la información de los campos a variables
            var nombre = txtbNombre.Text;
            var usuario = txtbUsuario.Text;
            var contrasena = pwdbContrasena.Password;
            var confirmacionContrasena = pwdbContrasenaRepetida.Password;
            //Se hace la validación de la información (no campos vacíos y contraseña y confirmación de contraseña iguales)
            var resultadoDeValidacion = ValidarInformacion(nombre, usuario, contrasena, confirmacionContrasena);
            if (resultadoDeValidacion.Equals("InformacionValida"))
            {
                try
                {
                    //Se manda a llamar a la función registrar jugador, se asigna en una variable para saber si el registro fue éxitoso o no
                    var resultadoDeRegistro = proxy.RegistrarJugador(usuario, contrasena, nombre);
                    MostrarMensaje(resultadoDeRegistro);
                    if (resultadoDeRegistro.Equals("JugadorRegistrado"))
                    {
                        Close();
                    }
                    else
                    {
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
            else
            {
                MostrarMensaje(resultadoDeValidacion);
            }
        }

        //En caso de que no se quiere el cancelar, siempre y cuando no se haya seleccionado el botón registrar
        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
