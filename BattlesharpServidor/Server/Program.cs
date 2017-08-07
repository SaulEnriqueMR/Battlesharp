using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using BDDService;
using JugarService;

namespace Server
{
    class Program
    {
        /// <summary>
        /// Aquí se inicializan y abren los host para ser utilizados por los clientes
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //Se crean los host
            ServiceHost hostRegistro = null;
            ServiceHost hostConsulta = null;
            ServiceHost hostJugar = null;

            //Se asignan los host a los servicios
            //Originalmente era un try y un finally pero según el estándar de codificación de C# es más recomendable el utilizar un using por las siguientes razones:
            //  -Es básicamente el mismo código pero con un buen chequeo automático de nulls y un alcance adicional para la variable.
            //  -Asegura el uso correcto del objeto IDisposable "por lo que también podría obtener un mejor soporte de framework para cualquier caso oscuro en el futuro.

            using (hostRegistro = new ServiceHost(typeof(RegistrarService)))
            using (hostConsulta = new ServiceHost(typeof(ConsultarService)))
            using (hostJugar = new ServiceHost(typeof(PartidaService)))
            {
                //Se abren los host
                hostRegistro.Open();
                hostConsulta.Open();
                hostJugar.Open();

                Console.WriteLine();
                Console.WriteLine("Presione <ENTER> para cerrar el host");
                Console.ReadLine();

                //En caso de que es estado de comunicación del host de registro halla fallado
                if (hostRegistro.State == CommunicationState.Faulted)
                {
                    hostRegistro.Abort();
                    hostRegistro.Open();
                }
                //Si no falla, solamente se cierra el host de registro
                else
                {
                    hostRegistro.Close();
                }
                //En caso de que es estado de consulta del host de registro halla fallado
                if (hostConsulta.State == CommunicationState.Faulted)
                {
                    hostConsulta.Abort();
                    hostConsulta.Open();
                }
                //Si no falla, solamente se cierra el host de consulta
                else
                {
                    hostConsulta.Close();
                }
                //En caso de que es estado de comunicación del host de registro halla fallado
                if (hostJugar.State == CommunicationState.Faulted)
                {
                    hostJugar.Abort();
                    hostJugar.Open();
                }
                //Si no falla, solamente se cierra el host de registro
                else
                {
                    hostJugar.Close();
                }
            }
        }
    }
}
