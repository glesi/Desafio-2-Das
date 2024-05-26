using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Desafio2_Cartelera_Cine.Models
{
    public class Conexion
    {
        private string cadenaConexion { get; set; }

        private SqlConnection conexionSQL;

        public Conexion()
        {
            cadenaConexion = @"Data source=(local)\SQLEXPRESS;Initial Catalog=peliculas_db;Integrated Security=True";
        }

        public bool conectar()
        {
            try
            {
                this.conexionSQL = new SqlConnection(this.cadenaConexion);
                this.conexionSQL.Open();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public bool estadoConexion()
        {
            switch (this.conexionSQL.State)
            {
                case System.Data.ConnectionState.Broken:
                    return true;
                case System.Data.ConnectionState.Open:
                    return true;
                default:
                    return false;
            }
        }

        public void desconectar()
        {
            this.conexionSQL.Close();
        }

        //Obtenemos todas las peliculas de la tabla peliculas, sin filtros
        public List<Pelicula> ObtenerPeliculas()
        {
            List<Pelicula> peliculas = new List<Pelicula>();

            if (this.conectar())
            {
                string query = "SELECT * FROM peliculas";
                SqlCommand cmd = new SqlCommand(query, this.conexionSQL);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Pelicula pelicula = new Pelicula
                    {
                        IdPelicula = Convert.ToInt32(reader["id_pelicula"]),
                        Titulo = reader["titulo"].ToString(),
                        Genero = reader["genero"].ToString(),
                        Director = reader["director"].ToString(),
                        FechaEstreno = Convert.ToDateTime(reader["fecha_estreno"]),
                        Sinopsis = reader["sinopsis"].ToString(),
                        Imagen = (byte[])reader["imagen"]
                    };
                    peliculas.Add(pelicula);
                }
                this.desconectar();
            }

            return peliculas;
        }

        // Obtener el ranking de las peliculas basadas en las calificaciones promedio
        public List<RankingPelicula> ObtenerRankingPeliculas()
        {
            List<RankingPelicula> rankingPeliculas = new List<RankingPelicula>();

            if (this.conectar())
            {
                string query = @"
                SELECT p.titulo, AVG(c.calificacion) AS PromedioCalificacion
                FROM peliculas p
                JOIN calificaciones c ON p.id_pelicula = c.id_pelicula
                GROUP BY p.titulo
                ORDER BY PromedioCalificacion DESC";

                SqlCommand cmd = new SqlCommand(query, this.conexionSQL);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    RankingPelicula ranking = new RankingPelicula
                    {
                        Titulo = reader["titulo"].ToString(),
                        PromedioCalificacion = Convert.ToDouble(reader["PromedioCalificacion"])
                    };
                    rankingPeliculas.Add(ranking);
                }
                this.desconectar();
            }

            return rankingPeliculas;
        }

    }

    public class RankingPelicula
    {
        public string Titulo { get; set; }
        public double PromedioCalificacion { get; set; }
    }
}