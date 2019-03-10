using Finisar.SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace registro_fichajes_de_futbol
{
    public partial class VerEquipo : Form
    {
        public VerEquipo(ConectorSQL conector, object equipo)
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            String idEquipo = Convert.ToString(equipo);
            SQLiteCommand consulta = conector.DameComando();
            SQLiteDataReader reader;
            consulta.CommandText = "SELECT * FROM equipo WHERE equipoId = '" + idEquipo + "'";
            reader = consulta.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine("Entro al READER => nombre de equipo es " + reader.GetString(1));
                textBoxNombre.Text = reader.GetString(1);
                textBoxCampeonato.Text = reader.GetString(2);
                textBoxCiudad.Text = reader.GetString(3);
                textBox1Pais.Text = reader.GetString(4);
                textBoxPresidente.Text = reader.GetString(5);
                textBoxId.Text = reader.GetString(0);
                try
                {
                    byte[] imageBytes = (System.Byte[])reader["escudo"];
                    MemoryStream ms = new MemoryStream(imageBytes);
                    pictureBoxEscudo.Image = Image.FromStream(ms, true);
                }
                catch (Exception ex) 
                {
                    Console.WriteLine(ex.Message);
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
