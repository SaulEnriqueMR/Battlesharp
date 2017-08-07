using BattlesharpInterfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
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
    /// Interacción lógica para Jugar.xaml
    /// </summary>

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public partial class Jugar : Window, IPartidaCallback
    {
        #region Atributos
        //Diccionario de los botones para poder accedes a ellos por medio de su clave
        Dictionary<string, Button> tablero = new Dictionary<string, Button>();
        Dictionary<string, Button> tableroEnemigo = new Dictionary<string, Button>();

        //Usuario que jugarán en la partida
        string Usuario { set; get; }
        string UsuarioEnemigo { set; get; }

        //Administrador de recursos que pondrá el texto en su idioma
        ResourceManager administradorDeRecursos;
        //Cultura que el administrador de recursos usará para saber que idioma escoger
        CultureInfo cultura;
        public string Lenguaje { set; get; }

        //Bandera para saber si los jugadores ya pusieron sus fichas
        public bool TableroFijado { set; get; }

        //Conexión del servidor
        //Canal para poder usar los métodos del servidor
        public DuplexChannelFactory<IPartidaService> canal { set; get; }
        //La interfaz de los métodos del servidor
        public IPartidaService proxy { set; get; }
        //Contexto
        public InstanceContext contexto { set; get; }

        //Lista de usuarios que se mostrarán para hacer una partida
        public ObservableCollection<string> ListaDeUsuariosConectados = new ObservableCollection<string>();

        //Bandera que controlará cuando se puede tirar y cuando no
        public bool esTurno;

        //Bandera que controla si se puede recibir una invitación de otro usuario, esto de pone en falso cuando se acepta una invitación o cuando le llega otra invitación al usuario
        public bool PuedeRecibirInvitacion { set; get; }
        #endregion

        /// <summary>
        /// Constructor de la ventana
        /// </summary>
        /// <param name="usuario">Usuario que está jugando</param>
        /// <param name="lenguaje">Lenguaje que se declaró</param>
        public Jugar(string usuario, string lenguaje)
        {
            InitializeComponent();
            //Se especifica el contexto para la lista de usuarios
            DataContext = this;
            //Se llama a los constructores de los diccionarios de casillas (propio y el enemigo)
            LlenarTablero();
            LlenarTableroEnemigo();
            //Se asigna el usuario y el lenguaje
            Usuario = usuario;
            Lenguaje = lenguaje;
            //Se asigna el administrador de recursos
            administradorDeRecursos = new ResourceManager("Battlesharp.lenguajes.Resource", typeof(Jugar).Assembly);
            //Se ponen los text block en el idioma indicado
            PonerTexto();
            //Se declara que se puede recibir invitación
            PuedeRecibirInvitacion = true;
            //El turno se declara falso al igual que los tableros para que no se puedan poner piezas ni disparar
            esTurno = false;
            gdTablero.IsEnabled = false;
            btnFijar.IsEnabled = false;
            gdTableroEnemigo.IsEnabled = false;
            try
            {
                //Se declara el contexto para el callback
                contexto = new InstanceContext(this);
                //Se declara un canal duplex para permitir los callbacks
                canal = new DuplexChannelFactory<IPartidaService>(contexto, "PartidaServiceEndpoint");
                //Se crea el canal
                proxy = canal.CreateChannel();
                //Se conecta al servidor para agregarlo al diccionario de usuarios
                proxy.Conectar(Usuario);
                //Se obtiene la lista de usuarios que están conectados en ese momento
                //NOTA: Sólo se obtienen los usuarios que se conectaron antes que este, para obtener los que se conectaron después se debe refrescar la lista
                List<string> usuariosConectados = proxy.RefrescarListaDeUsuarios();
                //Se muestran los usuarios
                MostrarUsuariosConectados(usuariosConectados);
                //El contexto de la lista pasa a ser la Observable Collection
                DataContext = ListaDeUsuariosConectados;
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
        
        /// <summary>
        /// Region destinada a mostrar lo siguiente:
        ///     -Usuarios conectados
        ///     -Mostrar mensajes
        ///     -Poner texto en el idioma especificado
        /// </summary>
        #region Mostrar
        
        private void ReiniciarVentana()
        {
            UsuarioEnemigo = "";
            ActualizarListaDeUsuarios();
            PuedeRecibirInvitacion = true;
            esTurno = false;
            gdTablero.IsEnabled = false;
            gdTableroEnemigo.IsEnabled = false;
            btnFijar.IsEnabled = false;
        }

        /// <summary>
        /// Muestra los usuarios que están conectados
        /// </summary>
        /// <param name="usuarios">Lista de usuarios conectados que se obtuvieron del proxy</param>
        private void MostrarUsuariosConectados(List<string> usuarios)
        {
            Cursor = Cursors.Wait;
            //Se limpia la Lista para que no se repitan nombres
            ListaDeUsuariosConectados.Clear();
            //Para cada usuario en la lista que se pasó en la lista como parámetro
            foreach (var usuario in usuarios)
            {
                //Se agrega a la Observable Collection
                ListaDeUsuariosConectados.Add(usuario);
            }
            Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// Muestra un mensaje que se solicite en el idioma indicado por la ventana
        /// </summary>
        /// <param name="causa">La causa que hizo el mostrar el mensaje</param>
        private void MostrarMensaje(string causa)
        {
            //Se crea el mensaje en el idioma que contiene esta ventana
            var mensaje = administradorDeRecursos.GetString(causa, cultura);
            //Se muestra el mensaje
            MessageBox.Show(mensaje);
        }

        /// <summary>
        /// Se pone la ventana en el idioma indicado
        /// </summary>
        private void PonerTexto()
        {
            txtblockTu.Text = Usuario;
            //Se asigna la cultura
            cultura = CultureInfo.CreateSpecificCulture(Lenguaje);
            //Se ponen los text block en el idioma especificado
            Title = administradorDeRecursos.GetString("Jugar", cultura);
            btnFijar.Content = administradorDeRecursos.GetString("Fijar", cultura);
            txtblIndicacion.Text = administradorDeRecursos.GetString("InvitaAAlguien", cultura);
        }
        #endregion

        /// <summary>
        /// Region destinada a implementar la interfaz del callback que es lo siguiente:
        ///     -Que hacer cuando llega una invitación para jugar
        ///     -Que pasa cuando se recibe un disparo
        ///     -Que pasa con el otro jugador cuando está listo
        /// </summary>
        #region Callback

        /// <summary>
        /// Lo que ocurre cuando se recibe una invitación de un usuario
        /// </summary>
        /// <param name="usuarioQueInvita">Nombre del usuario que invita a jugar</param>
        /// <returns>Si el usuario acepta la invitación regresa el usuario para poner su nombre en el tablero del otro jugador, si no regresá el mensaje "No"</returns>
        public string EnNuevaInvitacion(string usuarioQueInvita)
        {
            //Si el usuario puede recibir invitación
            if (PuedeRecibirInvitacion)
            {
                //Se cierra el poder recibir invitación
                PuedeRecibirInvitacion = false;
                //Se construyen los elementos del mensaje de invitación
                var titulo = administradorDeRecursos.GetString("Invitacion", cultura);
                var elJugador = administradorDeRecursos.GetString("ElJugador", cultura);
                var teHaInvitado = administradorDeRecursos.GetString("TeHaInvitado", cultura);
                var botones = MessageBoxButton.YesNo;
                var mensajeDeInvitacion = elJugador + usuarioQueInvita + teHaInvitado;
                MessageBoxResult resultado;
                //Se muestra el mensaje
                resultado = MessageBox.Show(mensajeDeInvitacion, titulo, botones);
                //Si el usuario acepta la invitación
                if (resultado == MessageBoxResult.Yes)
                {
                    //Se deshabilita la lista de usuarios conectados y el botón para actualizar
                    btnActualizar.IsEnabled = false;
                    lbxJugadoresActivos.IsEnabled = false;
                    //Se habilita el tablero para poner fichas
                    gdTablero.IsEnabled = true;
                    btnFijar.IsEnabled = true;
                    UsuarioEnemigo = usuarioQueInvita;
                    txtblockEnemigo.Text = UsuarioEnemigo;
                    //Se pone la indicación para poner fichas
                    txtblIndicacion.Text = administradorDeRecursos.GetString("PonPiezas", cultura);
                    //Regresa el usuario
                    return Usuario;
                }
                //Si el usuario rechaza la invitación
                else
                {
                    PuedeRecibirInvitacion = true;
                    return ("No");
                }
            }
            //Si el usuario estaba ocupado
            else{
                return ("UsuarioOcupado");
            }
        }

        /// <summary>
        /// Evento que ocurre cuando hay un nuevo disparo
        /// </summary>
        /// <param name="casillaDestino">Casilla a donde se dispara</param>
        /// <returns>Regresa el resultado del disparo</returns>
        public string EnNuevoDisparo(string casillaDestino)
        {
            if (TableroFijado) {
                //Se obtiene la casilla (el botón que se va a cambiar)
                var casillaPorDestruir = tablero.First(c => c.Key.Equals(casillaDestino)).Value;
                //Si en la casilla en la que nos disparan es de color "LightGray"
                if (casillaPorDestruir.Background.Equals(Brushes.LightGray))
                {
                    //Se cambia a color "Red"
                    casillaPorDestruir.Background = Brushes.Red;
                    //Se cuentan las casillas que hay de color rojo
                    if (tablero.Count(c => c.Value.Background.Equals(Brushes.Red)) == 7)
                    {
                        MessageBox.Show("Perdiste");
                    }
                    //Ahora es turno del que atacó ser atacado
                    esTurno = true;
                    //Regresa el mansaje "Tocado" para informar que si dio a un bote
                    return ("Tocado");
                }
                //Si la casilla es color "Lightblue"
                else
                {
                    esTurno = true;
                    return ("Agua");
                }
            }
            else
            {
                return ("UsuarioNoListo");
            }
        }
        #endregion

        /// <summary>
        /// Región donde se crean los diccionarios para los tableros y donde se implementan los métodos de click
        /// </summary>
        #region Llenar Tableros
        public void LlenarTablero()
        {
            tablero.Add("A1", A1);
            tablero.Add("A2", A2);
            tablero.Add("A3", A3);
            tablero.Add("A4", A4);
            tablero.Add("A5", A5);

            tablero.Add("B1", B1);
            tablero.Add("B2", B2);
            tablero.Add("B3", B3);
            tablero.Add("B4", B4);
            tablero.Add("B5", B5);

            tablero.Add("C1", C1);
            tablero.Add("C2", C2);
            tablero.Add("C3", C3);
            tablero.Add("C4", C4);
            tablero.Add("C5", C5);

            tablero.Add("D1", D1);
            tablero.Add("D2", D2);
            tablero.Add("D3", D3);
            tablero.Add("D4", D4);
            tablero.Add("D5", D5);

            tablero.Add("E1", E1);
            tablero.Add("E2", E2);
            tablero.Add("E3", E3);
            tablero.Add("E4", E4);
            tablero.Add("E5", E5);
        }

        #region Tablero
        private void A1_Click(object sender, RoutedEventArgs e)
        {
            PonerPiezaEnCasilla(A1);
        }

        private void A2_Click(object sender, RoutedEventArgs e)
        {
            PonerPiezaEnCasilla(A2);
        }

        private void A3_Click(object sender, RoutedEventArgs e)
        {
            PonerPiezaEnCasilla(A3);
        }

        private void A4_Click(object sender, RoutedEventArgs e)
        {
            PonerPiezaEnCasilla(A4);
        }

        private void A5_Click(object sender, RoutedEventArgs e)
        {
            PonerPiezaEnCasilla(A5);
        }

        private void B1_Click(object sender, RoutedEventArgs e)
        {
            PonerPiezaEnCasilla(B1);
        }

        private void B2_Click(object sender, RoutedEventArgs e)
        {
            PonerPiezaEnCasilla(B2);
        }

        private void B3_Click(object sender, RoutedEventArgs e)
        {
            PonerPiezaEnCasilla(B3);
        }

        private void B4_Click(object sender, RoutedEventArgs e)
        {
            PonerPiezaEnCasilla(B4);
        }

        private void B5_Click(object sender, RoutedEventArgs e)
        {
            PonerPiezaEnCasilla(B5);
        }

        private void C1_Click(object sender, RoutedEventArgs e)
        {
            PonerPiezaEnCasilla(C1);
        }

        private void C2_Click(object sender, RoutedEventArgs e)
        {
            PonerPiezaEnCasilla(C2);
        }

        private void C3_Click(object sender, RoutedEventArgs e)
        {
            PonerPiezaEnCasilla(C3);
        }

        private void C4_Click(object sender, RoutedEventArgs e)
        {
            PonerPiezaEnCasilla(C4);
        }

        private void C5_Click(object sender, RoutedEventArgs e)
        {
            PonerPiezaEnCasilla(C5);
        }

        private void D1_Click(object sender, RoutedEventArgs e)
        {
            PonerPiezaEnCasilla(D1);
        }


        private void D2_Click(object sender, RoutedEventArgs e)
        {
            PonerPiezaEnCasilla(D2);
        }

        private void D3_Click(object sender, RoutedEventArgs e)
        {
            PonerPiezaEnCasilla(D3);
        }

        private void D4_Click(object sender, RoutedEventArgs e)
        {
            PonerPiezaEnCasilla(D4);
        }

        private void D5_Click(object sender, RoutedEventArgs e)
        {
            PonerPiezaEnCasilla(D5);
        }

        private void E1_Click(object sender, RoutedEventArgs e)
        {
            PonerPiezaEnCasilla(E1);
        }

        private void E2_Click(object sender, RoutedEventArgs e)
        {
            PonerPiezaEnCasilla(E2);
        }

        private void E3_Click(object sender, RoutedEventArgs e)
        {
            PonerPiezaEnCasilla(E3);
        }

        private void E4_Click(object sender, RoutedEventArgs e)
        {
            PonerPiezaEnCasilla(E4);
        }

        private void E5_Click(object sender, RoutedEventArgs e)
        {
            PonerPiezaEnCasilla(E5);
        }
        #endregion

        public void LlenarTableroEnemigo()
        {
            tableroEnemigo.Add("A1", A1Enemigo);
            tableroEnemigo.Add("A2", A2Enemigo);
            tableroEnemigo.Add("A3", A3Enemigo);
            tableroEnemigo.Add("A4", A4Enemigo);
            tableroEnemigo.Add("A5", A5Enemigo);

            tableroEnemigo.Add("B1", B1Enemigo);
            tableroEnemigo.Add("B2", B2Enemigo);
            tableroEnemigo.Add("B3", B3Enemigo);
            tableroEnemigo.Add("B4", B4Enemigo);
            tableroEnemigo.Add("B5", B5Enemigo);

            tableroEnemigo.Add("C1", C1Enemigo);
            tableroEnemigo.Add("C2", C2Enemigo);
            tableroEnemigo.Add("C3", C3Enemigo);
            tableroEnemigo.Add("C4", C4Enemigo);
            tableroEnemigo.Add("C5", C5Enemigo);

            tableroEnemigo.Add("D1", D1Enemigo);
            tableroEnemigo.Add("D2", D2Enemigo);
            tableroEnemigo.Add("D3", D3Enemigo);
            tableroEnemigo.Add("D4", D4Enemigo);
            tableroEnemigo.Add("D5", D5Enemigo);

            tableroEnemigo.Add("E1", E1Enemigo);
            tableroEnemigo.Add("E2", E2Enemigo);
            tableroEnemigo.Add("E3", E3Enemigo);
            tableroEnemigo.Add("E4", E4Enemigo);
            tableroEnemigo.Add("E5", E5Enemigo);
        }

        #region Tablero Enemigo
        private void A1Enemigo_Click(object sender, RoutedEventArgs e)
        {
            Disparar(A1Enemigo);
        }

        private void A2Enemigo_Click(object sender, RoutedEventArgs e)
        {
            Disparar(A2Enemigo);
        }

        private void A3Enemigo_Click(object sender, RoutedEventArgs e)
        {
            Disparar(A3Enemigo);
        }

        private void A4Enemigo_Click(object sender, RoutedEventArgs e)
        {
            Disparar(A4Enemigo);
        }

        private void A5Enemigo_Click(object sender, RoutedEventArgs e)
        {
            Disparar(A5Enemigo);
        }

        private void B1Enemigo_Click(object sender, RoutedEventArgs e)
        {
            Disparar(B1Enemigo);
        }

        private void B2Enemigo_Click(object sender, RoutedEventArgs e)
        {
            Disparar(B2Enemigo);
        }

        private void B3Enemigo_Click(object sender, RoutedEventArgs e)
        {
            Disparar(B3Enemigo);
        }

        private void B4Enemigo_Click(object sender, RoutedEventArgs e)
        {
            Disparar(B4Enemigo);
        }

        private void B5Enemigo_Click(object sender, RoutedEventArgs e)
        {
            Disparar(B5Enemigo);
        }

        private void C1Enemigo_Click(object sender, RoutedEventArgs e)
        {
            Disparar(C1Enemigo);
        }

        private void C2Enemigo_Click(object sender, RoutedEventArgs e)
        {
            Disparar(C2Enemigo);
        }

        private void C3Enemigo_Click(object sender, RoutedEventArgs e)
        {
            Disparar(C3Enemigo);
        }

        private void C4Enemigo_Click(object sender, RoutedEventArgs e)
        {
            Disparar(C4Enemigo);
        }

        private void C5Enemigo_Click(object sender, RoutedEventArgs e)
        {
            Disparar(C5Enemigo);
        }

        private void D1Enemigo_Click(object sender, RoutedEventArgs e)
        {
            Disparar(D1Enemigo);
        }

        private void D2Enemigo_Click(object sender, RoutedEventArgs e)
        {
            Disparar(D2Enemigo);
        }

        private void D3Enemigo_Click(object sender, RoutedEventArgs e)
        {
            Disparar(D3Enemigo);
        }

        private void D4Enemigo_Click(object sender, RoutedEventArgs e)
        {
            Disparar(D4Enemigo);
        }

        private void D5Enemigo_Click(object sender, RoutedEventArgs e)
        {
            Disparar(D5Enemigo);
        }

        private void E1Enemigo_Click(object sender, RoutedEventArgs e)
        {
            Disparar(E1Enemigo);
        }

        private void E2Enemigo_Click(object sender, RoutedEventArgs e)
        {
            Disparar(E2Enemigo);
        }

        private void E3Enemigo_Click(object sender, RoutedEventArgs e)
        {
            Disparar(E3Enemigo);
        }

        private void E4Enemigo_Click(object sender, RoutedEventArgs e)
        {
            Disparar(E4Enemigo);
        }

        private void E5Enemigo_Click(object sender, RoutedEventArgs e)
        {
            Disparar(E5Enemigo);
        }
        #endregion
        #endregion

        /// <summary>
        /// Región donde se implementan las siguientes funciones
        ///     -Interacción lógica de lo que ocurre cuando se pulsa el botón "Actualizar"
        ///     -Actualizar la lista de usuarios conectados
        ///     -Interacción lógica de lo que ocurre cuando se pulsa el botón "Fijar"
        ///     -Interacción lógica de lo que ocurre cuando se pulsa el botón "Invitar"
        /// </summary>
        #region Botones

        /// <summary>
        /// Interacción lógica de todo lo que ocurre cuando se pulsa el botón Actualizar
        /// </summary>
        private void btnActualizar_Click(object sender, RoutedEventArgs e)
        {
            //Se actualiza la lista de usuarios, este es un método porque se requerirá en otros puntos del programa
            ActualizarListaDeUsuarios();
        }

        /// <summary>
        /// Interacción lógica de lo que ocurre cuando se pulsa el botón "Fijar"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFijar_Click(object sender, RoutedEventArgs e)
        {
            //Si ya no hay espacios disponibles para fichas
            if (!HayEspaciosDisponibles())
            {
                //El tablero fijado se pone en verdadero
                TableroFijado = true;
                gdTableroEnemigo.IsEnabled = true;
                txtblIndicacion.Text = administradorDeRecursos.GetString("Diviertete", cultura);
            }
            else
            {
                var aunQuedanPiezasPorPoner = administradorDeRecursos.GetString("QuedanPiezas", cultura);
                MessageBox.Show(aunQuedanPiezasPorPoner);
            }
        }

        /// <summary>
        /// Interacción lógica de lo que ocurre cuando se pulsa el botón "Invitar"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInvitar_Click(object sender, RoutedEventArgs e)
        {
            //El usuario que va a invitar debe de poder recibir invitación
            if (PuedeRecibirInvitacion)
            {
                //La bandera cambia a falso para que no le lleguen invitaciones
                PuedeRecibirInvitacion = false;
                //Se obtiene el usuario al que se quiere invitar
                var usuarioSeleccionado = lbxJugadoresActivos.SelectedItem;
                var usuarioElegido = usuarioSeleccionado.ToString();
                //Si el usuario elegido es distinto a uno mismo y distinto a null
                if (!usuarioElegido.Equals(Usuario) && !usuarioElegido.Equals(null))
                {
                    //Se obtiene el resultado de la invitación
                    var resultadoInvitacion = proxy.HacerInvitacion(Usuario, usuarioElegido);
                    //Si el resultado de la invitación es no
                    if (resultadoInvitacion.Equals("No"))
                    {
                        MostrarMensaje("UsuarioRechazoPartida");
                    }
                    else
                    {
                        //Si el usuario al que se invitó estaba ocupado
                        if (resultadoInvitacion.Equals("UsuarioOcupado"))
                        {
                            MostrarMensaje("UsuarioOcupado");
                            PuedeRecibirInvitacion = true;
                        }
                        else
                        {
                            if (resultadoInvitacion.Equals("UsuarioDesconectado"))
                            {
                                MostrarMensaje(resultadoInvitacion);
                            }
                            //Si el usuario aceptó la invitación
                            else
                            {
                                //Se pone la indicación para poner fichas
                                txtblIndicacion.Text = administradorDeRecursos.GetString("PonPiezas", cultura);
                                //Por ser el que invitó se le otorga el primer turno
                                esTurno = true;
                                btnFijar.IsEnabled = true;
                                //Se deshabilita la lista de usuarios
                                lbxJugadoresActivos.IsEnabled = false;
                                //Se habilita el tablero para poner fichas
                                gdTablero.IsEnabled = true;
                                UsuarioEnemigo = usuarioElegido;
                                txtblockEnemigo.Text = UsuarioEnemigo;
                            }
                        }
                    }
                }
                //Si no se seleccionó el usuario
                if (usuarioElegido.Equals(null))
                {
                    MostrarMensaje("SeleccionaUsuario");
                    PuedeRecibirInvitacion = true;
                }
                //Si se invita a uno mismo
                if (usuarioElegido.Equals(Usuario))
                {
                    MostrarMensaje("NoPuedesInvitarte");
                    PuedeRecibirInvitacion = true;
                }
            }

        }

        /// <summary>
        /// Actualiza la lista de usuarios conectados
        /// </summary>
        public void ActualizarListaDeUsuarios()
        {
            List<string> usuariosConectados = proxy.RefrescarListaDeUsuarios();
            MostrarUsuariosConectados(usuariosConectados);
        }
        #endregion

        /// <summary>
        /// Interacción lógica para cuando se esté cerrando la ventana
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //El usuario se desconecta del servidor
            proxy.Desconectar(Usuario);
        }

        /// <summary>
        /// Esta región describe toda la interacción para hacer un disparo y poner una ficha o quitarla
        /// </summary>
        #region Disparos y poner fichas

        /// <summary>
        /// Interacción lógica de lo que ocurre cuando se hace un disparo
        /// </summary>
        /// <param name="destino">Botón que se presionó para hacer el disparo</param>
        private void Disparar(Button destino)
        {
            //Si es su turno
            if (esTurno)
            {
                //Si aún no se ha disparado ahí
                if (destino.Background.Equals(Brushes.LightBlue))
                {
                    //Se obtiene el indice de la pieza para realizar el disparo
                    var casillaDestino = tableroEnemigo.First(c => c.Value.Equals(destino)).Key;
                    //Se guarda el resultado
                    var resultado = proxy.HacerDisparo(UsuarioEnemigo, casillaDestino);
                    //Si el resultado del disparo es "Tocado"
                    if (resultado.Equals("UsuarioDesconectado"))
                    {
                        MostrarMensaje(resultado);
                    }
                    else
                    {
                        if (resultado.Equals("Tocado"))
                        {
                            //Se pinta el de color rojo
                            destino.Background = Brushes.Red;
                            //Se cuenta si las piezas rojas son igual a 7
                            if (tableroEnemigo.Count(c => c.Value.Background.Equals(Brushes.Red)) == 7)
                            {
                                //Se muestra el mensaje de que ganó
                                MostrarMensaje("Ganaste");
                                var puntosPerdidos = ((tableroEnemigo.Count(c => c.Value.Background.Equals(Brushes.Gray))) * 2);
                                var puntuacion = 100 - puntosPerdidos;
                                RegistrarPuntuacion(Usuario, puntuacion);
                            }
                        }
                        //En caso de que haya fallado el tiro
                        else
                        {
                            destino.Background = Brushes.Gray;
                        }
                        //Ahora es turno del otro jugador
                        esTurno = false;
                    }
                }
                //En caso de que haga un disparo donde ya lo hizo
                else
                {
                    MostrarMensaje("DisparoRepetido");
                }
            }
            //En caso de que no sea su turno
            else
            {
                MostrarMensaje("NoEsTurno");
            }
        }

        private void RegistrarPuntuacion(string ganador, int puntuacion)
        {
            try
            {
                ChannelFactory<IRegistrarService> canalRegistro = new ChannelFactory<IRegistrarService>("RegistrarServiceEndpoint");
                IRegistrarService proxyRegistro = canalRegistro.CreateChannel();
                proxyRegistro.RegistrarPuntuacion(Usuario, puntuacion);
                MostrarMensaje("PuntuacionRegistrada");
            }
            catch (Exception)
            {
                MostrarMensaje("NoSePudoRegistrar");
            }
        }

        /// <summary>
        /// Interacción para poner una casilla
        /// </summary>
        /// <param name="casilla">Casilla en la que se pondrá o quitará la ficha</param>
        private void PonerPiezaEnCasilla(Button casilla)
        {
            //Si el tablero no está fijado
            if (!TableroFijado)
            {
                //Si la casilla tiene color "LightGray" (ya tiene una pieza)
                if (casilla.Background.Equals(Brushes.LightGray))
                {
                    //Se quita la pieza
                    casilla.Background = Brushes.LightBlue;
                }
                else
                {
                    //Si hay espacios disponibles
                    if (HayEspaciosDisponibles())
                    {
                        //La casilla cambia a color "LightGray" (se pone pieza)
                        casilla.Background = Brushes.LightGray;
                    }
                    //Si ya no hay espacios disponibles
                    else
                    {
                        var noSePuedenPonerMasPiezas = administradorDeRecursos.GetString("NoHayCasillas", cultura);
                        MessageBox.Show(noSePuedenPonerMasPiezas);
                    }
                }
            }
            //Si el tablero ya estaba fijado
            else
            {
                var elTableroEstaFijado = administradorDeRecursos.GetString("ElTableroEstaFijado", cultura);
                MessageBox.Show(elTableroEstaFijado);
            }
        }

        /// <summary>
        /// Verifica que haya espacios disponibles para poner fichas
        /// </summary>
        /// <returns>Regresa verdadero o flaso</returns>
        private bool HayEspaciosDisponibles()
        {
            //Se cuenta cuantas piezas hay con el color "LightGray" (tienen piezas)
            var piezasColocadas = tablero.Count(c => c.Value.Background.Equals(Brushes.LightGray));
            //Si es 7
            if (piezasColocadas == 7)
            {
                return false;
            }
            //Si es un número menor
            else
            {
                return true;
            }
        }
        #endregion
    }
}
