using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using BattlesharpInterfaces;

namespace JugarService
{
    /// <summary>
    /// Clase que implementa la interfaz IPartidaService, esta tiene un contexto de instancia "Single" para que todos los usuarios se conecten a la misma instancia
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class PartidaService : IPartidaService
    {
        //Diccionario de los usuarios conectados con los valores string como llave y su interfaz callback
        public Dictionary<string, IPartidaCallback> UsuariosConectados = new Dictionary<string, IPartidaCallback>();

        /// <summary>
        /// Se conecta el usuario a la lista de usuarios conectados
        /// </summary>
        /// <param name="usuario">Usuario que se va a conectar</param>
        public void Conectar(string usuario)
        {
            //Se crea un contexto por medio del canal
            IPartidaCallback contexto = OperationContext.Current.GetCallbackChannel<IPartidaCallback>();

            //Si el usuario no está en el diccionario
            if (!UsuariosConectados.ContainsKey(usuario))
            {
                //Se agrega
                UsuariosConectados.Add(usuario, contexto);
            }
            //Si el usuario ya está en el diccionario
            else
            {
                //Se quita y se vuelve a agregar
                UsuariosConectados.Remove(usuario);
                UsuariosConectados.Add(usuario, contexto);
            }
        }

        /// <summary>
        /// Desconecta al usuario
        /// </summary>
        /// <param name="usuario">El usuario que se va a quitar</param>
        public void Desconectar(string usuario)
        {
            //Se quita el callback por el identificador (el usuario)
            UsuariosConectados.Remove(usuario);
        }

        public List<string> ObtenerUsuariosConectados()
        {
            List<string> usuarios = new List<string>();
            foreach(var cliente in UsuariosConectados)
            {
                usuarios.Add(cliente.Key);
            }
            return usuarios;
        }

        public string HacerDisparo(string usuarioDestino, string casillaDestino)
        {
            try
            {
                var callbackUsuarioDestino = UsuariosConectados.First(u => u.Key.Equals(usuarioDestino)).Value;
                string resultadoDisparo = callbackUsuarioDestino.EnNuevoDisparo(casillaDestino);
                return resultadoDisparo;
            }
            catch(Exception)
            {
                return ("UsuarioDesconectado");
            }
        }

        public string HacerInvitacion(string usuarioQueInvita, string usuarioAInvitar)
        {
            try
            {
                var callbackUsuarioAInvitar = UsuariosConectados.First(u => u.Key.Equals(usuarioAInvitar)).Value;
                string resultadoInvitacion = callbackUsuarioAInvitar.EnNuevaInvitacion(usuarioQueInvita);
                return resultadoInvitacion;
            }
            catch (Exception)
            {
                return ("UsuarioDesconectado");
            }
        }

        public List<string> RefrescarListaDeUsuarios()
        {
            List<string> usuariosConectados = new List<string>();
            foreach (var usuario in UsuariosConectados)
            {
                usuariosConectados.Add(usuario.Key);
            }
            return usuariosConectados;
        }
    }
}
