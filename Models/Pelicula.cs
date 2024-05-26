using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Desafio2_Cartelera_Cine.Models
{
    public class Pelicula
    {
        [Key]
        public int IdPelicula { get; set; }
        public string Titulo { get; set; }
        public string Genero { get; set; }
        public string Director { get; set; }
        public DateTime FechaEstreno { get; set; }
        public string Sinopsis { get; set; }
        public byte[] Imagen { get; set; }
    }
}