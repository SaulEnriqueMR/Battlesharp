//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BDDService
{
    using System;
    using System.Collections.Generic;
    
    public partial class jugador
    {
        public jugador()
        {
            this.puntuacion = new HashSet<puntuacion>();
        }
    
        public int idjugador { get; set; }
        public string usuario { get; set; }
        public string contrasena { get; set; }
        public string nombre { get; set; }
    
        public virtual ICollection<puntuacion> puntuacion { get; set; }
    }
}
