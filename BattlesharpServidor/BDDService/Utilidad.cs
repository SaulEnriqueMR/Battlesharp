using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDDService
{
    /// <summary>
    /// Esta clase contiene los siguientes métodos para que los servicios lo puedan utilizar:
    ///     -Verificar si un jugador existe.
    ///     -Buscar un usuario.
    ///     -Codificación de constraseña.
    /// </summary>
    class Utilidad
    {
        /// <summary>
        /// Verifica si un jugador existe.
        /// </summary>
        /// <remarks>
        /// Esta búsqueda se hace por su usuario.
        /// </remarks>
        /// <param name="usuarioPorBuscar">Usuario del jugador que se va a buscar.</param>
        /// <returns>Regresa si es cierto o falso si el jugador existe.</returns>
        public bool ExisteUsuario(string usuarioPorBuscar)
        {
            using (BattlesharpEntities BaseDeDatos = new BattlesharpEntities())
            {
                //Cuenta jugadores registrados con ese usuario hay
                var jugadoresConEseUsuario = (from u in BaseDeDatos.jugador
                                              where u.usuario.Equals(usuarioPorBuscar)
                                              select u).Count();
                //Si no hay jugadores con ese usuario
                if (jugadoresConEseUsuario == 0)
                {
                    return false;
                }
                //Si hay un jugador con ese usuario
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// Busca un jugador registrado.
        /// </summary>
        /// <param name="usuarioPorBuscar">El jugador será buscado por su usuario.</param>
        /// <returns>Regresa el jugador que se encontró.</returns>
        public jugador BuscarJugador(string usuarioPorBuscar)
        {
            using (BattlesharpEntities BaseDeDatos = new BattlesharpEntities())
            {
                //Se busca al jugador que tenga un usuario que coincida con el usuario ingresado
                var jugadorCoincidente = (from j in BaseDeDatos.jugador
                                          where j.usuario.Equals(usuarioPorBuscar)
                                          select j).SingleOrDefault();
                //Regresa el jugador encontrado
                return jugadorCoincidente;
            }
        }

        /// <summary>
        /// Códifica una constraseña ingresada por el usuario.
        /// </summary>
        /// <param name="contrasena">Contraseña que se va a codificar.</param>
        /// <returns>Regresa la contraseña ingresada pero códificada.</returns>
        public string sha256(string contrasena)
        {
            System.Security.Cryptography.SHA256Managed crypt = new System.Security.Cryptography.SHA256Managed();
            StringBuilder hash = new StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(contrasena), 0, Encoding.UTF8.GetByteCount(contrasena));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }
    }
}
