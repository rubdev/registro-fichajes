using Finisar.SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace registro_fichajes_de_futbol
{
    public partial class VerFichaje : Form
    {
        public VerFichaje(ConectorSQL conector, object fichaje)
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            String fechaFichaje = Convert.ToString(fichaje);
            Console.WriteLine("FECHA DE FICHAJE QUE ME LLEGA => " +fechaFichaje);
            SQLiteCommand consulta = conector.DameComando();
            SQLiteDataReader reader;
            string sqlConsulta = "SELECT fechaFichaje, precio, equipoProcedencia, tipoFichaje, equipoFicha, jugadorFichado, E.nombre AS nombreEquipo, J.nombre AS nombreJUGADOR "
                + "FROM fichaje F, equipo E, Jugador J "
                + "WHERE F.equipoFicha = E.equipoId "
                + "AND F.jugadorFichado = J.dni "
                + "AND fechaFichaje='" + fechaFichaje + "'";
            consulta.CommandText = sqlConsulta;
            reader = consulta.ExecuteReader();
            while (reader.Read())
            {
                textBoxEquipo.Text = reader["nombreEquipo"].ToString();
                textBoxJugador.Text = reader["nombreJugador"].ToString();
                textBoxFechaFichaje.Text = reader.GetString(0);
                textBoxPrecio.Text = reader["precio"].ToString()+" €";
                textBoxEquipoProcedencia.Text = reader.GetString(2);
                string tipoFichaje = reader.GetString(3);
                if (tipoFichaje.Equals("Traspaso"))
                {
                    radioButtonTransferencia.Checked = true;
                } else
                {
                    radioButtonCesion.Checked = true;
                }
            }
            reader.Close();
        }

        private void buttonCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
