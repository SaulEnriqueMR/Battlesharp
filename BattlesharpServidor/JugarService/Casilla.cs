using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JugarService
{
    class Casilla
    {
        public string ID { set; get; }
        public string Estado { set; get; }

        public Casilla(string id)
        {
            ID = id;
            Estado = "Desocupada";
        }

        public void PonerBote()
        {
            Estado = "Ocupada";
        }

        public string DispararEnCasilla()
        {
            string resultado;
            if (Estado.Equals("Ocupada"))
            {
                resultado = "¡Tocado!";
            }
            else
            {
                resultado = "¡Agua!";
            }
            Estado = "Destruido";
            return resultado;
        }
    }
}
