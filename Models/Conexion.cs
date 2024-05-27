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
        public List<Pelicula> ObtenerPeliculas(string titulo = null, string categoria = null)
        {
            List<Pelicula> peliculas = new List<Pelicula>();

            if (this.conectar())
            {
                // Construir la consulta SQL con filtros opcionales
                string query = @"
                                SELECT p.id_pelicula, p.titulo, p.genero, p.director, p.fecha_estreno, p.sinopsis, p.imagen,
                                       c.nombre_categoria
                                FROM peliculas p
                                LEFT JOIN Categorias c ON p.id_categoria = c.id_categoria
                                WHERE (@Titulo IS NULL OR p.titulo LIKE '%' + @Titulo + '%')
                                AND (@Categoria IS NULL OR c.nombre_categoria LIKE '%' + @Categoria + '%')";

                SqlCommand cmd = new SqlCommand(query, this.conexionSQL);
                cmd.Parameters.AddWithValue("@Titulo", string.IsNullOrEmpty(titulo) ? (object)DBNull.Value : titulo);
                cmd.Parameters.AddWithValue("@Categoria", string.IsNullOrEmpty(categoria) ? (object)DBNull.Value : categoria);
                SqlDataReader reader = cmd.ExecuteReader();

                Dictionary<int, Pelicula> peliculaDict = new Dictionary<int, Pelicula>();

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
                        Imagen = reader["imagen"] as byte[],
                        Categoria = reader["nombre_categoria"].ToString()
                    };
                    peliculas.Add(pelicula);
                    peliculaDict[pelicula.IdPelicula] = pelicula;
                }
                reader.Close();

                // Segundo query: obtener el promedio de calificaciones y unirlas a los datos principales
                query = @"
                        SELECT id_pelicula, ISNULL(AVG(calificacion), 0) AS PromedioCalificacion
                        FROM calificaciones
                        GROUP BY id_pelicula";

                cmd = new SqlCommand(query, this.conexionSQL);
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int idPelicula = Convert.ToInt32(reader["id_pelicula"]);
                    if (peliculaDict.ContainsKey(idPelicula))
                    {
                        peliculaDict[idPelicula].PromedioCalificacion = Convert.ToDouble(reader["PromedioCalificacion"]);
                    }
                }
                reader.Close();

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

        public bool AgregarCalificacion(Calificacion calificacion)
        {
            if (this.conectar())
            {
                string query = "INSERT INTO calificaciones (id_pelicula, calificacion, usuario, comentario) VALUES (@IdPelicula, @Calificacion, @Usuario, @Comentario)";
                SqlCommand cmd = new SqlCommand(query, this.conexionSQL);
                cmd.Parameters.AddWithValue("@IdPelicula", calificacion.IdPelicula);
                cmd.Parameters.AddWithValue("@Calificacion", calificacion.Calificar);
                cmd.Parameters.AddWithValue("@Usuario", calificacion.Usuario);
                cmd.Parameters.AddWithValue("@Comentario", calificacion.Comentario);

                //se debe crear una validacion para ver que la nota no sean mayor a 5//


                int result = cmd.ExecuteNonQuery();
                this.desconectar();
                return result > 0;
            }
            return false;
        }
    }

    public class RankingPelicula
    {
        public string Titulo { get; set; }
        public double PromedioCalificacion { get; set; }
    }
}