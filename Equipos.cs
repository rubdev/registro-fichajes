using System;
using Finisar.SQLite;
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
    public partial class Equipos : Form
    {
        // objeto BD
        private ConectorSQL conector;
        private DataTable dtRecord;
        private SQLiteDataAdapter DataAdap;

        // para controlar cuando guardar algo
        bool salvado = true;//A true quiere decir que todo está guardado

        // interfaz para relación con padre
        public Interfaz Opener { get; set; }

        public Equipos(ConectorSQL con)
        {
            InitializeComponent();
            conector = con;
            string sql;
            sql = "SELECT * FROM equipo";
            Inicializa_datagrid(sql);
        }

        private void llamaAlPadre_Click(object sender, EventArgs e)
        {
            this.Opener.activarBotones("equipos");
        }

        // Inicializa y lista los datos de la tabla equipos
        public void Inicializa_datagrid(string sql)
        {
            try
            {
                SQLiteCommand consulta = conector.DameComando();
                consulta.CommandText = sql;
                DataAdap = new SQLiteDataAdapter(consulta);
                dtRecord = new DataTable();
                DataAdap.Fill(dtRecord);
                dataGridView1.DataSource = dtRecord;

                dataGridView1.Columns["equipoId"].DefaultCellStyle.BackColor = Color.PaleGreen;
                dataGridView1.Columns["equipoId"].ReadOnly = true;

                //Añado el combo para el campeonato
                DataGridViewComboBoxColumn comboCampeonato = new DataGridViewComboBoxColumn();
                comboCampeonato.Name = "campeonatoCombo";
                comboCampeonato.DataPropertyName = "campeonato";//Campo de la base de datos con el que enlaza
                comboCampeonato.Items.Add("La Liga");
                comboCampeonato.Items.Add("Premier League");
                comboCampeonato.Items.Add("Ligue 1");
                comboCampeonato.Items.Add("Serie A");
                comboCampeonato.Items.Add("Eredivisie");
                comboCampeonato.Items.Add("Primeira Liga");
                dataGridView1.Columns.RemoveAt(2);
                dataGridView1.Columns.Insert(2, comboCampeonato);

                //Añado el combo para el campeonato
                DataGridViewComboBoxColumn comboPais = new DataGridViewComboBoxColumn();
                comboPais.Name = "paisCombo";
                comboPais.DataPropertyName = "pais";//Campo de la base de datos con el que enlaza
                comboPais.Items.Add("España");
                comboPais.Items.Add("Reino Unido");
                comboPais.Items.Add("Francia");
                comboPais.Items.Add("Italia");
                comboPais.Items.Add("Holanda");
                comboPais.Items.Add("Portugal");
                dataGridView1.Columns.RemoveAt(4);
                dataGridView1.Columns.Insert(4, comboPais);

                // Modifico cabeceras de la tabla
                dataGridView1.Columns[0].Width = 80;
                dataGridView1.Columns[0].HeaderText = "ID de Equipo";
                dataGridView1.Columns[1].Width = 150;
                dataGridView1.Columns[1].HeaderText = "Nombre";
                dataGridView1.Columns[2].Width = 100;
                dataGridView1.Columns[2].HeaderText = "Campeonato";
                dataGridView1.Columns[3].Width = 120;
                dataGridView1.Columns[3].HeaderText = "Ciudad";
                dataGridView1.Columns[4].Width = 100;
                dataGridView1.Columns[4].HeaderText = "País";
                dataGridView1.Columns[5].Width = 120;
                dataGridView1.Columns[5].HeaderText = "Presidente";
                dataGridView1.Columns[6].Width = 170;
                dataGridView1.Columns[6].HeaderText = "Escudo";

                //Botón auxiliar para seleccionar imagen
                DataGridViewButtonColumn seleccionImagen = new DataGridViewButtonColumn();
                seleccionImagen.Name = "Imagen";//Para acceder a propiedades
                seleccionImagen.Text = "Selecciona Imagen";
                seleccionImagen.HeaderText = "";
                seleccionImagen.UseColumnTextForButtonValue = true;//Para que todos tomen el mismo nombre
                dataGridView1.Columns.Add(seleccionImagen);//Se inserta la recién creada

                //COMANDO INSERT ----------------------------------------------------------------
                SQLiteCommand comando_ins = new SQLiteCommand("INSERT INTO equipo(equipoId, nombre, campeonato, ciudad, pais, presidente, escudo) VALUES (@equipoId, @nombre, @campeonato, @ciudad, @pais, @presidente, @escudo)", conector.DameConexion());
                //Se establece la anti-inyeccion SQL enlazando los parámetros 
                comando_ins.Parameters.Add(new SQLiteParameter("@equipoId", DbType.Int16));
                comando_ins.Parameters.Add(new SQLiteParameter("@nombre", DbType.String));
                comando_ins.Parameters.Add(new SQLiteParameter("@campeonato", DbType.String));
                comando_ins.Parameters.Add(new SQLiteParameter("@ciudad", DbType.String));
                comando_ins.Parameters.Add(new SQLiteParameter("@pais", DbType.String));
                comando_ins.Parameters.Add(new SQLiteParameter("@presidente", DbType.String));
                comando_ins.Parameters.Add(new SQLiteParameter("@escudo", DbType.Binary));
                //Añado los nombres de los campos de la base de datos. Los números de columnas se corresponde con el orden de los parámetros anteriores
                comando_ins.Parameters[0].SourceColumn = "equipoId";
                comando_ins.Parameters[1].SourceColumn = "nombre";
                comando_ins.Parameters[2].SourceColumn = "campeonato";
                comando_ins.Parameters[3].SourceColumn = "ciudad";
                comando_ins.Parameters[4].SourceColumn = "pais";
                comando_ins.Parameters[5].SourceColumn = "presidente";
                comando_ins.Parameters[6].SourceColumn = "escudo";
                //Se actualiza el comando Insert y se le asocia la conexión
                DataAdap.InsertCommand = comando_ins;
                DataAdap.InsertCommand.Connection = conector.DameConexion();

                //COMANDO UPDATE ----------------------------------------------------------------
                SQLiteCommand comando_act = new SQLiteCommand("UPDATE equipo SET nombre=@nombre, campeonato=@campeonato, ciudad=@ciudad, pais=@pais, presidente=@presidente, escudo=@escudo  WHERE equipoId=@equipoId", conector.DameConexion());
                //Dado que son los mismos parámmetros que para el comando insert puedo hacer lo siguiente: copiar parámetros y sourcecolumns 
                foreach (SQLiteParameter i in comando_ins.Parameters)
                    comando_act.Parameters.Add(i);

                for (int i = 0; i < 6; i++)
                    comando_act.Parameters[i].SourceColumn = comando_ins.Parameters[i].SourceColumn;

                DataAdap.UpdateCommand = comando_act;
                DataAdap.UpdateCommand.Connection = conector.DameConexion();

                //COMANDO DELETE ----------------------------------------------------------------
                SQLiteCommand comando_del = new SQLiteCommand("DELETE FROM equipo WHERE equipoId = @equipoId", conector.DameConexion());
                //Se establece la anti-inyeccion SQL enlazando los parámetros 
                comando_del.Parameters.Add(new SQLiteParameter("@equipoId", DbType.Int16));
                comando_del.Parameters[0].SourceColumn = "equipoId";
                DataAdap.DeleteCommand = comando_del;
                DataAdap.DeleteCommand.Connection = conector.DameConexion();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //función para reescalar la imagen y que no pese tanto la base de datos
        private static Bitmap ResizeImage(Bitmap image, int width, int height)
        {
            Bitmap resizedImage = new Bitmap(width, height);
            using (Graphics gfx = Graphics.FromImage(resizedImage))
            {
                gfx.DrawImage(image, new Rectangle(0, 0, width, height),
                    new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
            }
            return resizedImage;
        }

        // Evento de click sobre el datagrid
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1 & e.ColumnIndex > -1) if (e.RowIndex > -1 & e.ColumnIndex > -1)
                {
                    if (dataGridView1.Columns[e.ColumnIndex].Name == "Imagen")
                    {
                        OpenFileDialog dlg = new OpenFileDialog();
                        dlg.Filter = "PNG Files(*.png)|*.png|JPG Files(*.jpg)|*.jpg|All Files(*.*)|*.*";
                        dlg.Title = "Seleccionar Imagen";
                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            Bitmap Imagen_orig = (Bitmap)Image.FromFile(dlg.FileName, true);//Se lee la imagen 
                            Bitmap imagen_reesc = ResizeImage(Imagen_orig, 180, 150);//Se reescala la imagen
                            ImageConverter converter = new ImageConverter();//Se convierte a bytes (BLOB)
                            byte[] bytes = (byte[])converter.ConvertTo(imagen_reesc, typeof(byte[]));

                            dataGridView1.Rows[e.RowIndex].Cells["escudo"].Value = bytes;
                            // si selecciono la imagen activo botón de guardar
                            this.Opener.activarBotones("equipos");
                        }
                    }
                }
            
        }

        public void addfila()
        {//Añadimos mediante código, se controla mejor que con el mecanismo del dataGridView
            DataRow nuevafila = dtRecord.NewRow();
            dtRecord.Rows.Add(nuevafila);//añade la nueva fila
            dataGridView1.DataSource = dtRecord;
            dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[2];//Se coloca el foco en la última fila
            salvado = false;
        }

        // Guarda nuevos datos añadidos
        public void guardarDatos()
        {
            int no_guardar = 0;
            // Ahora haría las comprobaciones...
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                int indicecol_nombre = dataGridView1.Columns["nombre"].Index;
                string nombre = dataGridView1.Rows[i].Cells[indicecol_nombre].Value.ToString();
                int indicecol_campeonato = dataGridView1.Columns["campeonatoCombo"].Index;
                string campeonato = dataGridView1.Rows[i].Cells[indicecol_campeonato].Value.ToString();
                int indicecol_ciudad = dataGridView1.Columns["ciudad"].Index;
                string ciudad = dataGridView1.Rows[i].Cells[indicecol_ciudad].Value.ToString();
                int indicecol_pais = dataGridView1.Columns["paisCombo"].Index;
                string pais = dataGridView1.Rows[i].Cells[indicecol_pais].Value.ToString();
                int indicecol_presidente = dataGridView1.Columns["presidente"].Index;
                string presidente = dataGridView1.Rows[i].Cells[indicecol_presidente].Value.ToString();
                int fila = i + 1;
                if (nombre.Equals(""))
                {
                    MessageBox.Show("El NOMBRE no puede queda vacío. Fila " + fila);
                    dataGridView1.CurrentCell = dataGridView1.Rows[i].Cells[indicecol_nombre];//Se coloca el foco
                    no_guardar = 1;
                    break;//Para que salga de la ejecución
                } else if (campeonato.Equals(""))
                {
                    MessageBox.Show("El CAMPEONATO no puede queda vacío. Fila " + fila);
                    dataGridView1.CurrentCell = dataGridView1.Rows[i].Cells[indicecol_campeonato];//Se coloca el foco
                    no_guardar = 1;
                    break;//Para que salga de la ejecución
                } else if(ciudad.Equals(""))
                {
                    MessageBox.Show("La CIUDAD no puede queda vacío. Fila " + fila);
                    dataGridView1.CurrentCell = dataGridView1.Rows[i].Cells[indicecol_ciudad];//Se coloca el foco
                    no_guardar = 1;
                    break;//Para que salga de la ejecución
                } else if(pais.Equals(""))
                {
                    MessageBox.Show("El PAÍS no puede queda vacío. Fila " + fila);
                    dataGridView1.CurrentCell = dataGridView1.Rows[i].Cells[indicecol_pais];//Se coloca el foco
                    no_guardar = 1;
                    break;//Para que salga de la ejecución
                } else if(presidente.Equals(""))
                {
                    MessageBox.Show("El PRESIDENTE no puede queda vacío. Fila " + fila);
                    dataGridView1.CurrentCell = dataGridView1.Rows[i].Cells[indicecol_presidente];//Se coloca el foco
                    no_guardar = 1;
                    break;//Para que salga de la ejecución
                }
            }
                //guardo si todo ok
                if (no_guardar == 0)
            {
                try
                {
                    //dataGridView1.CancelEdit();
                    bool resp = this.dataGridView1.EndEdit();//Se fuerza a que termine la edición de todos los campos
                    //dataGridView1.
                    //Si la edición no acaba es porque hay algún campo a medio editar y no guarda
                    Console.WriteLine(resp);
                    if (resp)
                    {
                        this.DataAdap.Update(this.dtRecord);//SE LLAMA AL ADAPTER PARA QUE ACTÚE SEGÚN: INSERCIÓN, BORRADO O ACTUALIZACIÓN.
                        MessageBox.Show("Equipo guardado en BBDD");

                        //Vuelvo a consultar en la base de datos para que me cargue los datos modificados
                        actualizaDatagrid();
                    }
                }
                catch (SQLiteException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        // Actualiza los datos del datagrid actual y el de la tabla intermedia
        private void actualizaDatagrid()
        {
            SQLiteCommand consulta = conector.DameComando();
            consulta.CommandText = "SELECT * FROM equipo";
            dtRecord = new DataTable();
            DataAdap.Fill(dtRecord);
            dataGridView1.DataSource = dtRecord;
            salvado = true;
            // despues actualizo la tabla intermedia (fichajes)
            this.Opener.activarBotones("borraEnCascada");
        }

        // Cuando borra una fila va a guardar en BD
        private void dataGridView1_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            this.Validate();
            this.guardarDatos();
            this.Refresh();
        }

        // activa botones al editar una celda del datagrid
        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            this.Opener.activarBotones("equipos");
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            Console.WriteLine("Excepción producida en DATAGRID de EQUIPOS");
        }

        // click del raton sobre una fila para ver el menú contextual
        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right & salvado)
            {

                dataGridView1.ClearSelection();//Deselecciono fila
                int currentMouseOverRow = dataGridView1.HitTest(e.X, e.Y).RowIndex;

                if (currentMouseOverRow >= 0)//Solo si está sobre las filas
                {
                    ContextMenuStrip m = new ContextMenuStrip();
                    m.Items.Add("Ver Ficha").Name = "verFicha";
                    m.Items.Add("Borrar").Name = "borrar";
                    dataGridView1.Rows[currentMouseOverRow].Selected = true;//Activo la línea donde se ha pulsado, luego será necesaria para saber donde estamos
                    m.ItemClicked += new ToolStripItemClickedEventHandler(funcion_menucontextual);
                    m.Show(dataGridView1, new Point(e.X, e.Y));

                }

            }
            else if (e.Button == MouseButtons.Right & salvado == false)
            {
                MessageBox.Show("El menú contextual se activa cuando todo está guardado");
            }
        }

        //Esta función controla las opciones del menú contextual, se hace pública porque los eventos de teclado lo llaman desde el padre
        public void funcion_menucontextual(object sender, ToolStripItemClickedEventArgs e)
        {

            switch (e.ClickedItem.Name.ToString())
            {
                case "verFicha":
                    //La variable form_editar no hace falta que sea de clase ya que la uso solamente aquí
                    string idEquipo = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
                    Console.WriteLine("ID Equipo para VER FICHA => " + idEquipo);
                    VerEquipo form_editar = new VerEquipo(this.conector, idEquipo);//Le paso el valor de la celda primera dni
                    form_editar.ShowDialog();
                    break;
                case "borrar":
                    if ((MessageBox.Show("¿Seguro que quiere borrar el equipo seleccionado?", "Atención", MessageBoxButtons.YesNo) == DialogResult.Yes))
                    {
                        dataGridView1.Rows.RemoveAt(dataGridView1.SelectedRows[0].Index);
                        this.dataGridView1_UserDeletedRow(null, null); ;//Actualiza el DataGrid con la base de datos
                    }
                    break;
            }
            dataGridView1.ClearSelection();//Deselecciono fila
        }

        //Funciones de teclado para ver ficha y borrar
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (dataGridView1.SelectedRows.Count == 1) //Si hay una fila seleccionada
                switch (keyData)
                {
                    case (Keys.B): //Mayúscula o minúscula siempre y cuando no vaya en combinación con otra tecla (x. ejemplo shift)
                        if ((MessageBox.Show("¿Seguro que quiere borrar el equipo seleccionado?", "Atención", MessageBoxButtons.YesNo) == DialogResult.Yes))
                        {
                            dataGridView1.Rows.RemoveAt(dataGridView1.SelectedRows[0].Index);
                            this.dataGridView1_UserDeletedRow(null, null); ;//Actualiza el DataGrid con la base de datos
                        }
                        break;
                    case (Keys.V): //Mayúscula o minúscula siempre y cuando no vaya en combinación con otra tecla (x. ejemplo shift)
                        string idEquipo = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
                        Console.WriteLine("ID Equipo para VER FICHA => " + idEquipo);
                        VerEquipo form_editar = new VerEquipo(this.conector, idEquipo);//Le paso el valor de la celda primera dni
                        form_editar.ShowDialog();
                        //Si no tuviéramos bloqueada la primera columna, escribiría una e tras mostrar el form_editar
                        //para evitar esto simulamos que pulsamos la tecla ESCAPE para que, aunque escriba la letra en la celda, vuelva al valor que tenía antes
                        SendKeys.Send("{ESC}");
                        break;
                }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
