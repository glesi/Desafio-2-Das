using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Desafio2_Cartelera_Cine.Models;

namespace Desafio2_Cartelera_Cine.Controllers
{
    public class PeliculasController : Controller
    {
        private readonly Conexion db = new Conexion();

        // GET: Peliculas
        public ActionResult Index()
        {
            List<Pelicula> peliculas = db.ObtenerPeliculas();
            return View(peliculas);
        }

        // GET: Peliculas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pelicula pelicula = db.ObtenerPeliculas().Find(p => p.IdPelicula == id);
            if (pelicula == null)
            {
                return HttpNotFound();
            }
            return View(pelicula);
        }

        // GET: Peliculas/Ranking
        public ActionResult Ranking()
        {
            var ranking = db.ObtenerRankingPeliculas();
            return View(ranking);
        }
    }
}