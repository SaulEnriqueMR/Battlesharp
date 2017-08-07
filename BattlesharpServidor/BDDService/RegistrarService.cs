using BattlesharpInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace BDDService
{
    /// <summary>
    /// Implementa la interfaz IRegistrarService, sirve para registrar jugador y puntuación
    /// </summary>
    public class RegistrarService : IRegistrarService
    {
        /// <summary>
        /// Método que registra un nuevo jugador en el sistema.
        /// </summary>
        /// <remarks>
        /// El usuario con el que se registra el jugador no debe estar registrado previamente.
        /// </remarks>
        /// <param name="usuarioIngresado">Usuario que ingresó el jugador para que pueda iniciar sesión posteriormente.</param>
        /// <param name="contrasenaIngresada">Contraseña que ingresó el jugador para que pueda iniciar sesión posteriormente.</param>
        /// <param name="nombreIngresado">Nombre del jugador.</param>
        /// <returns>Mensaje de retroalimentación para dar a conocer el resultado del registro.</returns>
        public string RegistrarJugador(string usuarioIngresado, string contrasenaIngresada, string nombreIngresado)
        {
            Utilidad utilidad = new Utilidad();
            using (BattlesharpEntities BaseDeDatos = new BattlesharpEntities())
            {
                //Si no existe un jugador registrado con ese usuario procede a registrarlo
                if (!utilidad.ExisteUsuario(usuarioIngresado))
                {
                    var jugadoresRegistrados = BaseDeDatos.jugador.Count();
                    //Se crea el jugador con la información pasada por parámetros
                    jugador nuevoJugador = new jugador
                    {
                        idjugador = jugadoresRegistrados,
                        usuario = usuarioIngresado,
                        contrasena = utilidad.sha256(contrasenaIngresada),
                        nombre = nombreIngresado
                    };
                    //Se registra el jugador en la base de datos
                    BaseDeDatos.jugador.Add(nuevoJugador);
                    //Se guardan los cambios
                    BaseDeDatos.SaveChanges();
                    //Regresa el mensaje de éxito para ser mostrado en el cliente
                    return ("JugadorRegistrado");
                }
                //Si el jugador ya está registrado en la base de datos
                else
                {
                    return ("UsuarioEnUso");
                }
            }
        }

        /// <summary>
        /// Registra una nueva puntuación
        /// </summary>
        /// <param name="usuarioGanador">Usuario que ganó, se usa para saber el id del jugador</param>
        /// <param name="puntosObtenidos">Los puntos que tuvo al final de la partida</param>
        /// <returns>Mensaje de retroalimentación para dar a conocer el resultado del registro.</returns>
        public string RegistrarPuntuacion(string usuarioGanador, int puntosObtenidos)
        {
            Utilidad utilidad = new Utilidad();
            using (BattlesharpEntities BaseDeDatos = new BattlesharpEntities())
            {
                //Se obtiene el jugador ganador buscandolo por su usuario para obtener su ID
                var jugadorGanador = utilidad.BuscarJugador(usuarioGanador);
                var puntuacionesRegistradas = BaseDeDatos.puntuacion.Count();
                //Se crea la nueva puntuacion obteniendo el ID del jugador ganador
                puntuacion nuevaPuntuacion = new puntuacion()
                {
                    idpuntuacion = puntuacionesRegistradas,
                    idjugador = jugadorGanador.idjugador,
                    puntos = puntosObtenidos
                };
                //Se agrega la puntuación a la base de datos
                BaseDeDatos.puntuacion.Add(nuevaPuntuacion);
                //Se guardan los cambios en la base de datos
                BaseDeDatos.SaveChanges();
                //Regresa un mensaje de texto para ser mostrado en el cliente
                return ("Puntuación registrada");
            }
        }
    }
}
