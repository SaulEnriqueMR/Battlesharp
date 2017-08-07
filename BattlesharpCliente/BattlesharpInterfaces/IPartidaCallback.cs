using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace BattlesharpInterfaces
{
    [ServiceContract]
    public interface IPartidaCallback
    {
        [OperationContract]
        string EnNuevaInvitacion(string usuarioQueInvita);
        [OperationContract]
        string EnNuevoDisparo(string casillaDestino);
    }
}
