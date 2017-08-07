using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace BattlesharpInterfaces
{
    [ServiceContract]
    public interface IConsultarService
    {
        [OperationContract]
        string IniciarSesion(string usuario, string contrasena);
        [OperationContract]
        List<Tuple<string, int>> ObtenerMejoresPuntuaciones();
    }
}
