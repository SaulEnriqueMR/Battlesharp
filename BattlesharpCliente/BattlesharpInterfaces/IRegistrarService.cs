using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace BattlesharpInterfaces
{
    [ServiceContract]
    public interface IRegistrarService
    {
        [OperationContract]
        string RegistrarJugador(string usuario, string contrasena, string nombre);
        [OperationContract]
        string RegistrarPuntuacion(string usuarioGanador, int puntosObtenidos);
    }
}
