using Finisar.SQLite;
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace registro_fichajes_de_futbol
{
    public class ConectorSQL
    {
        private SQLiteConnection sqlite_con; 
        private SQLiteCommand consulta; 

        public ConectorSQL()
        {
            sqlite_con = new SQLiteConnection();
            consulta = sqlite_con.CreateCommand();
            sqlite_con.ConnectionString = "Data Source=fichajes.db;Version=3;New=False;Compress=True;";
            try
            {
                sqlite_con.Open();
                Console.WriteLine("Abro BASE DE DATOS");
            }
            catch (Finisar.SQLite.SQLiteException)
            {
                sqlite_con.ConnectionString = "Data Source=fichajes.db;Version=3;New=True;Compress=True;";
                sqlite_con.Open();
                CrearBBDD();
            }

        }

        public SQLiteConnection DameConexion()
        {
            return sqlite_con;
        }

        public SQLiteCommand DameComando()
        {
            return this.consulta;
        }

        public void CrearBBDD()
        {
            // Creación de tabla equipo
            consulta.CommandText = "CREATE TABLE equipo ("
                + "equipoId INTEGER PRIMARY KEY,"
                + "nombre TEXT,"
                + "campeonato TEXT CHECK(campeonato IN('La Liga', 'Premier League', 'Ligue 1', 'Bundesliga', 'Serie A', 'Eredivisie', 'Primeira Liga')),"
                + "ciudad TEXT,"
                + "pais TEXT CHECK(pais IN('España', 'Reino Unido', 'Francia', 'Alemania', 'Italia', 'Holanda', 'Portugal')),"
                + "presidente TEXT,"
                + "escudo BLOB"
                + ")";
            consulta.ExecuteNonQuery();
            // Creación de tabla jugador
            consulta.CommandText = "CREATE TABLE jugador("
                + "dni TEXT PRIMARY KEY,"
                + "nombre TEXT,"
                + "posicion TEXT CHECK(posicion IN('Portero', 'Defensa', 'Centrocampista', 'Delantero')),"
                + "dorsal INTEGER,"
                + "fechaNacimiento TEXT,"
                + "nacionalidad TEXT,"
                + "lesionado INTEGER"
                + ")";
            consulta.ExecuteNonQuery();
            // creación tabla fichajes (intermedia)
            consulta.CommandText = "CREATE TABLE fichaje("
                + "fechaFichaje TEXT PRIMARY KEY,"
                + "precio INTEGER,"
                + "equipoProcedencia TEXT,"
                + "tipoFichaje TEXT CHECK(tipoFichaje IN('Traspaso', 'Cesion')),"
                + "equipoFicha INTEGER,"
                + "jugadorFichado TEXT,"
                + "FOREIGN KEY(equipoFicha) REFERENCES equipo(equipoId),"
                + "FOREIGN KEY(jugadorFichado) REFERENCES jugador(dni)"
                + ")";
            consulta.ExecuteNonQuery();
            // trigger para borrar en cascada jugadores fichados
            consulta.CommandText = "CREATE TRIGGER Delete_fichaje_jugador"
                + "AFTER DELETE ON jugador "
                + "FOR EACH ROW "
                + "BEGIN "
                + "DELETE FROM fichaje WHERE jugadorFichado = OLD.dni; "
                + "END";
            consulta.ExecuteNonQuery();
            // trigger para borrar en cascada equipos fichados
            consulta.CommandText = "CREATE TRIGGER Delete_fichaje_equipo "
                + "AFTER DELETE ON equipo "
                + "FOR EACH ROW "
                + "BEGIN "
                + "DELETE FROM fichaje WHERE equipoFicha = OLD.equipoId; "
                + "END";
            consulta.ExecuteNonQuery();
            Console.WriteLine("BASE DE DATOS CREADA");
        }

        public void ReiniciarBBDD()
        {
            this.consulta.CommandText = "DROP TABLE fichaje";
            consulta.ExecuteNonQuery();
            this.consulta.CommandText = "DROP TABLE jugador";
            consulta.ExecuteNonQuery();
            this.consulta.CommandText = "DROP TABLE equipo";
            consulta.ExecuteNonQuery();
            CrearBBDD();
        }


        public void CerrarBBDD()
        {
            sqlite_con.Close();
        }

    }
}
