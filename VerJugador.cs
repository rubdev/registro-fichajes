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
    public partial class VerJugador : Form
    {
        public VerJugador(ConectorSQL conector, object jugadorDni)
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            string dni = Convert.ToString(jugadorDni);
            Console.WriteLine("VENTANA DE FICHA => DNI JUGADOR: " + dni);
            SQLiteCommand consulta = conector.DameComando();
            SQLiteDataReader reader;
            consulta.CommandText = "SELECT * FROM jugador WHERE dni= '" + dni+"'";
            reader = consulta.ExecuteReader();
            while (reader.Read())
            {
                textBoxDni.Text = reader.GetString(0);
                textBoxNombre.Text = reader.GetString(1);
                textBoxPosicion.Text = reader.GetString(2);
                textBoxDorsal.Text = reader.GetInt16(3).ToString();
                textBoxFecha.Text = reader.GetString(4);
                textBoxNacionalidad.Text = reader.GetString(5);
                if (reader.GetInt16(6) == 1) checkBoxLesionado.Checked = true;
            }
            reader.Close();
        }

        private void buttonCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
