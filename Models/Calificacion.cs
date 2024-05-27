using DataAnnotationsExtensions;
using System.ComponentModel.DataAnnotations;

namespace Desafio2_Cartelera_Cine.Models
{
    public class Calificacion
    {
        [Key]
        public int IdCalificacion { get; set; }

        [Required]
        [Min(1, ErrorMessage = "La calificación debe ser al menos 1.")]
        [Max(5, ErrorMessage = "La calificación no puede ser mayor a 5.")]
        public int Calificar { get; set; }

        public int IdPelicula { get; set; }

        [Required]
        public string Usuario { get; set; }

        [Required]
        public string Comentario { get; set; }

        // Relaciones
        public virtual Pelicula Pelicula { get; set; }
    }
}