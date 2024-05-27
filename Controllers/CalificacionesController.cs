
using System.Web.Mvc;
using Desafio2_Cartelera_Cine.Models;

namespace Desafio2_Cartelera_Cine.Controllers
{
    public class CalificacionesController : Controller
    {
        private Conexion conexion = new Conexion();

        // GET: Calificaciones/Create
        public ActionResult Create(int id)
        {
            var calificacion = new Calificacion { IdPelicula = id };
            return View(calificacion);
        }

        // POST: Calificaciones/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdPelicula,calificacion,Usuario,Comentario")] Calificacion calificacion)
        {
            if (calificacion.Calificar < 1 || calificacion.Calificar > 5)
            {
                ModelState.AddModelError("calificacion", "La calificación debe estar entre 1 y 5.");
            }

            if (ModelState.IsValid)
            {
                bool result = conexion.AgregarCalificacion(calificacion);
                if (result)
                {
                    TempData["SuccessMessage"] = "Calificación agregada con éxito.";
                    return RedirectToAction("Index", "Peliculas");
                }
            }
            return View(calificacion);
        }
    }
}