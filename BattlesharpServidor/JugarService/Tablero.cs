using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JugarService
{
    class Tablero
    {
        List<Casilla> Board = new List<Casilla>();
        public int DisparosAcertados { set; get; }

        public Tablero()
        {
            Board.Add(new Casilla("A1"));
            Board.Add(new Casilla("A2"));
            Board.Add(new Casilla("A3"));
            Board.Add(new Casilla("A4"));
            Board.Add(new Casilla("A5"));

            Board.Add(new Casilla("B1"));
            Board.Add(new Casilla("B2"));
            Board.Add(new Casilla("B3"));
            Board.Add(new Casilla("B4"));
            Board.Add(new Casilla("B5"));

            Board.Add(new Casilla("C1"));
            Board.Add(new Casilla("C2"));
            Board.Add(new Casilla("C3"));
            Board.Add(new Casilla("C4"));
            Board.Add(new Casilla("C5"));

            Board.Add(new Casilla("D1"));
            Board.Add(new Casilla("D2"));
            Board.Add(new Casilla("D3"));
            Board.Add(new Casilla("D4"));
            Board.Add(new Casilla("D5"));

            Board.Add(new Casilla("E1"));
            Board.Add(new Casilla("E2"));
            Board.Add(new Casilla("E3"));
            Board.Add(new Casilla("E4"));
            Board.Add(new Casilla("E5"));

            DisparosAcertados = 0;
        }

        public void PonerBotes(string[] portaaviones, string[] submarino, string[] destructores, string[] fragata)
        {
            foreach (var idCasilla in portaaviones)
            {
                Board.Find(c => c.ID.Equals(idCasilla)).PonerBote();
            }
            foreach (var idCasilla in submarino)
            {
                Board.Find(c => c.ID.Equals(idCasilla)).PonerBote();
            }
            foreach (var idCasilla in destructores)
            {
                Board.Find(c => c.ID.Equals(idCasilla)).PonerBote();
            }
            foreach (var idCasilla in fragata)
            {
                Board.Find(c => c.ID.Equals(idCasilla)).PonerBote();
            }
        }

        public string HacerDisparo(string casillaDestino)
        {
            var resultado = Board.Find(c => c.ID.Equals(casillaDestino)).DispararEnCasilla();
            if (resultado.Equals("¡Tocado!"))
            {
                DisparosAcertados = DisparosAcertados + 1;
                if (DisparosAcertados == 10)
                {
                    resultado = "Partida terminada";
                    return resultado;
                }
                else
                {
                    return resultado;
                }
            }
            else
            {
                return resultado;
            }
        }
    }
}
