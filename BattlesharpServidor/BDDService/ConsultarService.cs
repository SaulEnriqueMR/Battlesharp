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
    /// 
    /// </summary>
    public class ConsultarService : IConsultarService
    {
        /// <summary>
        /// Inicia sesión en el sistema.
        /// </summary>
        /// <param name="usuario">Usuario con el que fue registrado el jugador.</param>
        /// <param name="contrasena">Contraseña con la que se registró el jugador.</param>
        /// <returns>Si existe un jugador con ese usuario y contraseña regresa el usuario, sino regresa un mensaje de retroalimentación</returns>
        public string IniciarSesion(string usuarioIngresado, string contrasenaIngresada)
        {
            using (BattlesharpEntities BaseDeDatos = new BattlesharpEntities())
            {
                Utilidad utilidad = new Utilidad();
                //Primero se verifica que el jugador exista en la base de datos
                if (utilidad.ExisteUsuario(usuarioIngresado))
                {
                    try
                    {
                        //Se busca el jugador que coincida su usario y su contraseña
                        contrasenaIngresada = utilidad.sha256(contrasenaIngresada);
                        var jugadorCoincidente = (from j in BaseDeDatos.jugador
                                                  where j.usuario.Equals(usuarioIngresado)
                                                        && j.contrasena.Equals(contrasenaIngresada)
                                                  select j).SingleOrDefault();
                        //Se regresa el usuario del jugador para que se muestre en el menú principal
                        return (jugadorCoincidente.usuario);
                    }
                    catch (NullReferenceException)
                    {
                        return ("ContrasenaIncorrecta");
                    }
                }
                //Sí no existe el usuario regresa el siguiente mensaje que se usará para mostrarla al cliente
                else
                {
                    return ("UsuarioNoExiste");
                }
            }
        }

        /// <summary>
        /// Consigue la lista de puntuaciones ordenada de mayor a menor
        /// </summary>
        /// <returns>Regresa una lista de Tuplas para poder ser mostrado</returns>
        public List<Tuple<string, int>> ObtenerMejoresPuntuaciones()
        {
            List<Tuple<string, int>> puntuaciones = new List<Tuple<string, int>>();
            using (BattlesharpEntities BaseDeDatos = new BattlesharpEntities())
            {
                //Se obtienen todos los elementos de la vista de mejores
                var puntuacionesOrdenadas = (from j in BaseDeDatos.mejores
                                             select j);
                //Cada elementos del resultado de la busqueda se agrega a la lista
                foreach (var elemento in puntuacionesOrdenadas)
                {
                    //Como la tupla solo tiene formato string e int primero se agrega
                    //el usuario ganador y después los puntos obtenidos
                    puntuaciones.Add(new Tuple<string, int>(elemento.usuario, elemento.puntos));
                }
                //Regresa la lista de tuplas
                return puntuaciones;
            }
        }
    }
}
