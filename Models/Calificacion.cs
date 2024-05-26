using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Desafio2_Cartelera_Cine.Models
{
    public class Calificacion
    {
        [Key]
        public int IdCalificacion { get; set; }
        public int IdPelicula { get; set; }
        public Pelicula Pelicula { get; set; }
        public int calificacion { get; set; }
        public string Usuario { get; set; }
        public string Comentario { get; set; }
    }
}