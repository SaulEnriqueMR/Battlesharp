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
    
    public partial class puntuacion
    {
        public int idpuntuacion { get; set; }
        public int idjugador { get; set; }
        public int puntos { get; set; }
    
        public virtual jugador jugador { get; set; }
    }
}
