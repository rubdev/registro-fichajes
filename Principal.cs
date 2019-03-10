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
    public partial class Principal : Form, Interfaz
    {
        // objetos formularios
        Equipos equipo_listado;
        Jugadores jugador_listado;
        Fichajes fichajes_listado;

        // objeto BD
        public ConectorSQL con;

        // Formulario informes
        form_informe ventana_informe;

        public Principal()
        {
            InitializeComponent();
            con = new ConectorSQL();
            jugador_listado = JugadoresListado();
            equipo_listado = EquiposListado();
            fichajes_listado = FichajesListado();
            AbreFormFichajes();
            this.GuardarDatos.Enabled = false;
            ventana_informe = new form_informe();
        }

        // inicializa listado de jugadores
        private Jugadores JugadoresListado()
        {
            jugador_listado = new Jugadores(this.con);
            jugador_listado.MdiParent = this;
            jugador_listado.FormBorderStyle = FormBorderStyle.None;
            jugador_listado.Dock = DockStyle.Fill;
            jugador_listado.Opener = this;
            return jugador_listado;
        }

        // inicializa listado de equipo
        private Equipos EquiposListado()
        {
            equipo_listado = new Equipos(this.con);
            equipo_listado.MdiParent = this;
            equipo_listado.FormBorderStyle = FormBorderStyle.None;
            equipo_listado.Dock = DockStyle.Fill;
            equipo_listado.Opener = this;
            return equipo_listado;
        }

        // inicializa listado de fichajes
        private Fichajes FichajesListado()
        {
            fichajes_listado = new Fichajes(this.con);
            fichajes_listado.MdiParent = this;
            fichajes_listado.FormBorderStyle = FormBorderStyle.None;
            fichajes_listado.Dock = DockStyle.Fill;
            fichajes_listado.Opener = this;
            return fichajes_listado;
        }

        // Al abrir el formulario hijo oculta si hay otro abierto y abre el de jugadores
        public void AbreFormJugadores()
        {
            //Si tiene hijo lo oculta, no tendría sentido cerrarlo con Close 
            //ya que se tendría que crear objetos hijo una y otra vez
            if (this.ActiveMdiChild != null)
                this.ActiveMdiChild.Hide();

            //Si no hay un hijo activo lo crea. Esto evita que se creen múltiples instancias del mismo hijo
            if (this.ActiveMdiChild == null)
            {
                //Se desactivan botones y se muestra
                toolStripButton1.Enabled = true;
                toolStripButton2.Enabled = true;
                aJugadores.Enabled = false;
                añadeFila.Enabled = true;
                GuardarDatos.Enabled = false;
                jugador_listado.Show();
            }
        }

        // Al abrir el formulario hijo oculta si hay otro abierto y abre el de equipos
        public void AbreFormEquipos()
        {
            if (this.ActiveMdiChild != null)
                this.ActiveMdiChild.Hide();
            
            if (this.ActiveMdiChild == null)
            {
                aJugadores.Enabled = true;
                toolStripButton2.Enabled = true;
                toolStripButton1.Enabled = false;
                GuardarDatos.Enabled = false;
                añadeFila.Enabled = true;
                equipo_listado.Show();
            }
        }

        // Al abrir el formulario hijo oculta si hay otro abierto y abre el de fichajes
        public void AbreFormFichajes()
        {
            if (this.ActiveMdiChild != null)
                this.ActiveMdiChild.Hide();

            if (this.ActiveMdiChild == null)
            {
                toolStripButton1.Enabled = true;
                aJugadores.Enabled = true;
                toolStripButton2.Enabled = false;
                añadeFila.Enabled = true;
                GuardarDatos.Enabled = true;
                fichajes_listado.Show();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
 
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void acercaDeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Registro de fichajes App\nDesarrollada por Rubén Segura Romo\nVersión 1.0", "Acerca de", MessageBoxButtons.OK);
        }

        private void reiniciarBDToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void manualDeUsuarioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("manual.pdf");
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void jugadores_Click(object sender, EventArgs e)
        {
            AbreFormJugadores();
        }

        private void equipos_Click(object sender, EventArgs e)
        {
            AbreFormEquipos();
        }

        private void fichajes_Click(object sender, EventArgs e)
        {
            AbreFormFichajes();
        }

        private void verDatos_Click(object sender, EventArgs e)
        {

        }

        private void borrarElemento_Click(object sender, EventArgs e)
        {

        }

        public void activarBotones(string arg1)
        {
            // pongo un boton al formulario y coojo el sender para saber que boton ha pulsado
            switch (arg1)
            {
                case "jugadores":
                    Console.WriteLine("LLamo al padre desde el hijo => JUGADORES");
                    this.GuardarDatos.Enabled = true;
                    break;
                case "equipos":
                    Console.WriteLine("LLamo al padre desde el hijo => EQUIPOS");
                    this.GuardarDatos.Enabled = true;
                    break;
                case "fichajes":
                    Console.WriteLine("LLamos al padre desde el hijo => FICHAJES");
                    this.GuardarDatos.Enabled = true;
                    break;
                case "borraEnCascada":
                    this.fichajes_listado.Validate();
                    this.fichajes_listado.guardarDatos();
                    this.fichajes_listado.Refresh();
                    break;
            }
        }

        // Función que añade nueva fila al datagrid dependiendo del que esté activo
        private void addFilaNueva(object sender, EventArgs e)
        {
            string dataGridActual = this.ActiveMdiChild.Text;
            Console.WriteLine("GRID ACTUAL => " + dataGridActual);
            switch (dataGridActual)
            {
                case "Jugadores":
                    this.jugador_listado.addfila();
                    this.GuardarDatos.Enabled = true;
                    this.añadeFila.Enabled = false;
                    break;
                case "Equipos":
                    this.equipo_listado.addfila();
                    this.GuardarDatos.Enabled = true;
                    this.añadeFila.Enabled = false;
                    break;
                case "Fichajes":
                    this.fichajes_listado.addfila();
                    this.GuardarDatos.Enabled = true;
                    this.añadeFila.Enabled = false;
                    break;
            }
        }

        // Guarda datos en BD dependiendo del datagrid en el que estés
        private void GuardarDatos_Click(object sender, EventArgs e)
        {
            string dataGridActual = this.ActiveMdiChild.Text;
            switch (dataGridActual)
            {
                case "Jugadores":
                    this.jugador_listado.Validate();
                    this.jugador_listado.guardarDatos();
                    this.jugador_listado.Refresh();
                    this.GuardarDatos.Enabled = false;
                    this.añadeFila.Enabled = true;
                    break;
                case "Equipos":
                    this.equipo_listado.Validate();
                    this.equipo_listado.guardarDatos();
                    this.equipo_listado.Refresh();
                    this.GuardarDatos.Enabled = false;
                    this.añadeFila.Enabled = true;
                    break;
                case "Fichajes":
                    this.fichajes_listado.Validate();
                    this.fichajes_listado.guardarDatos();
                    this.fichajes_listado.Refresh();
                    this.GuardarDatos.Enabled = false;
                    this.añadeFila.Enabled = true;
                    break;
            }
        }

        // Al pulsar en GENERAR INFORME
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            crearInforme("SELECT * FROM fichaje", "SELECT * FROM equipo", "SELECT * FROM jugador");
            //crearInforme("SELECT fechaFichaje, precio, equipoProcedencia, tipoFichaje, equipoFicha, jugadorFichado, E.nombre AS nombreEquipo, J.nombre AS nombreJUGADOR FROM fichaje F, equipo E, Jugador J WHERE F.equipoFicha = E.equipoId ");
        }

        private void crearInforme(string sql, string sql2, string sql3)
        {
            SQLiteCommand consulta = con.DameComando();
            consulta.CommandText = sql;
            SQLiteDataAdapter DataAdap = new SQLiteDataAdapter(consulta);//Hace de intermediario entre la base de datos y el DataGrid
            DataSet Ds = new DataSet();//Se crea un DataSet
            DataAdap.Fill(Ds, "DataTableFichaje");//Se enlaza con el que hemos creado desde la interfaz gráfica

            //Se aplica la otra select EQUIPO
            consulta.CommandText = sql2;
            DataAdap.Fill(Ds, "DataTableEquipo");//Se enlaza con el que hemos creado desde la interfaz gráfica

            //Se aplica la otra select JUGADOR
            consulta.CommandText = sql3;
            DataAdap.Fill(Ds, "DataTableJugador");//Se enlaza con el que hemos creado desde la interfaz gráfica


            if (Ds.Tables[0].Rows.Count == 0)
            {
                MessageBox.Show("No hay datos que mostrar, revisar la SQL", "Informe");
                return;
            }

            CrystalReport1 informe = new CrystalReport1(); ;//Se crea el objeto informe

            informe.Load("..\\..\\CrystalReport1.rpt"); //Dado que el directorio es debug, he de salir a la raiz
            informe.SetDataSource(Ds);//Se toma el origen de datos del informe
            /*
            if (pidedatos == false)
                informe.SetParameterValue("topecalorias", 100000);//Se pone un parámetro muy alto para que muestre todo
            */

            //informe.SetParameterValue("mi_parametro", "Cesion");

            ventana_informe.crystalReportViewer1.ReportSource = informe;//Añadimos al Viewer el informe que vamos a mostrar
            ventana_informe.crystalReportViewer1.Refresh();//Se actualiza el informe
            ventana_informe.ShowDialog();//Muestro la ventana de diálogo
        }
    }
}
