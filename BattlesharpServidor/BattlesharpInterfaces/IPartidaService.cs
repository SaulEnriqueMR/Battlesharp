using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace BattlesharpInterfaces
{
    [ServiceContract(CallbackContract = typeof(IPartidaCallback))]
    public interface IPartidaService
    {
        [OperationContract]
        void Conectar(string usuario);
        [OperationContract]
        void Desconectar(string usuario);
        [OperationContract]
        string HacerDisparo(string usuarioDestino, string casillaDestino);
        [OperationContract]
        string HacerInvitacion(string usuarioQueInvita, string usuarioAInvitar);
        [OperationContract]
        List<string> RefrescarListaDeUsuarios();
    }
}
