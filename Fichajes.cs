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
    public partial class Fichajes : Form
    {

        // objeto BD
        private ConectorSQL conector;
        private DataTable dtRecord;
        private SQLiteDataAdapter DataAdap;

        // objeto BD Equipo
        private DataTable dtRecordEquipo;
        private SQLiteDataAdapter DataAdapEquipo;

        // objeto BD Equipo
        private DataTable dtRecordJugador;
        private SQLiteDataAdapter DataAdapJugador;

        // para controlar cuando guardar algo
        bool salvado = true;//A true quiere decir que todo está guardado

        // colección para el textbox autocompletar de equipos
        private AutoCompleteStringCollection ColeccionParaAutocompletarEquipos = new AutoCompleteStringCollection();
        private AutoCompleteStringCollection ColeccionParaAutocompletarEquiposId = new AutoCompleteStringCollection();

        // para el calendario de la fecha de nacimiento
        private DateTimePicker calendario;
        private bool calendarioactivo;
        private int filafecha;
        private int columnafecha;


        // interfaz para relación con padre
        public Interfaz Opener { get; set; }

        public Fichajes(ConectorSQL con)
        {
            InitializeComponent();
            conector = con;
            string sql;
            /*
            sql = "SELECT fechaFichaje, precio, equipoProcedencia, tipoFichaje, equipoFicha, jugadorFichado, E.nombre AS nombreEquipo, J.nombre AS nombreJUGADOR " 
                + "FROM fichaje F, equipo E, Jugador J "
                + "WHERE F.equipoFicha = E.equipoId "
                + "AND F.jugadorFichado = J.dni";
            */
            sql = "SELECT * FROM fichaje";
            Inicializa_datagrid(sql);
        }

        private void llamaAlPadre_Click(object sender, EventArgs e)
        {
            this.Opener.activarBotones("fichajes");
        }

        // Inicializa y lista los datos de la tabla fichajes
        public void Inicializa_datagrid(string sql)
        {
            try
            {
                // cargo datos de bd
                SQLiteCommand consulta = conector.DameComando();
                consulta.CommandText = sql;
                DataAdap = new SQLiteDataAdapter(consulta);
                dtRecord = new DataTable();
                DataAdap.Fill(dtRecord);
                dataGridView1.DataSource = dtRecord;

                /**************************************************/
                /*** COMBOBOX DINÁMICOS CLAVE EXTERNAS 
                /**************************************************/

                // combo EQUIPOS
                DataGridViewComboBoxColumn comboEquipos = new DataGridViewComboBoxColumn();
                consulta.CommandText = "SELECT * FROM equipo";
                DataAdapEquipo = new SQLiteDataAdapter(consulta);
                dtRecordEquipo = new DataTable();
                DataAdapEquipo.Fill(dtRecordEquipo);
                comboEquipos.Name = "comboequipos";
                comboEquipos.HeaderText = "Equipo que ficha";
                comboEquipos.DataPropertyName = "equipoId";
                comboEquipos.DataSource = dtRecordEquipo;
                comboEquipos.DisplayMember = dtRecordEquipo.Columns["nombre"].ColumnName;
                comboEquipos.ValueMember = dtRecordEquipo.Columns["equipoId"].ColumnName;
                comboEquipos.DataPropertyName = "equipoFicha";
                dataGridView1.Columns.Insert(6, comboEquipos);
                
                

                // combo JUGADORES
                DataGridViewComboBoxColumn comboJugadores = new DataGridViewComboBoxColumn();
                consulta.CommandText = "SELECT * FROM jugador";
                DataAdapJugador = new SQLiteDataAdapter(consulta);
                dtRecordJugador = new DataTable();
                DataAdapJugador.Fill(dtRecordJugador);
                comboJugadores.Name = "combojugadores";
                comboJugadores.HeaderText = "Jugador fichado";
                comboJugadores.DataPropertyName = "dni";
                comboJugadores.DataSource = dtRecordJugador;
                comboJugadores.DisplayMember = dtRecordJugador.Columns["nombre"].ColumnName;
                comboJugadores.ValueMember = dtRecordJugador.Columns["dni"].ColumnName;
                comboJugadores.DataPropertyName = "jugadorFichado";
                dataGridView1.Columns.Insert(7, comboJugadores);

                /**************************************************/

                //Añado el combo para el tipo de fichaje
                DataGridViewComboBoxColumn comboTipoFichaje = new DataGridViewComboBoxColumn();
                comboTipoFichaje.Name = "tipofichaje";
                comboTipoFichaje.DataPropertyName = "tipoFichaje";//Campo de la base de datos con el que enlaza
                comboTipoFichaje.Items.Add("Traspaso");
                comboTipoFichaje.Items.Add("Cesion");
                dataGridView1.Columns.RemoveAt(3);
                dataGridView1.Columns.Insert(3, comboTipoFichaje);


                /**************************************************/
                /*** COLECCIONES PARA BUSCADOR AUTOCOMPLETAR    ***/
                /**************************************************/

                //Columna de búsqueda para Equipos
                DataGridViewTextBoxColumn busquedaEquipos = new DataGridViewTextBoxColumn();
                busquedaEquipos.Name = "busquedaE";//Para acceder a propiedades
                busquedaEquipos.HeaderText = "Búsqueda de equipos autocompletada";
                dataGridView1.Columns.Insert(6, busquedaEquipos);//Se inserta la recién creada en la posición 9
                //dataGridView1.Columns[10].Width = 80;
                //Aquí es donde relleno la colección para el desplegable 
                //Y es en el evento dataGridView1_EditingControlShowing cuando se muestra
                foreach (DataRow row in dtRecordEquipo.Rows)
                {
                    ColeccionParaAutocompletarEquipos.Add(row["nombre"].ToString());
                    ColeccionParaAutocompletarEquiposId.Add(row["equipoId"].ToString());
                    // PRUEBA CON INDEX OF!!!
                    Console.WriteLine("add autocompleta nombre => " + row["nombre"].ToString() + " con ID => "+ row["equipoId"].ToString());
                }
                    
                //Ahora podríamos enlazar este campo con la base de datos (DataPropertyName)
                //En este momento, tanto el combo como este campo están asociados al restaurante
                busquedaEquipos.DataPropertyName = "nombre";//Campo de la base de datos con el que enlaza

                // Modifico cabeceras de la tabla
                dataGridView1.Columns[0].Width = 120;
                dataGridView1.Columns[0].HeaderText = "Fecha fichaje";
                dataGridView1.Columns[1].Width = 120;
                dataGridView1.Columns[1].HeaderText = "Precio";
                dataGridView1.Columns[2].Width = 130;
                dataGridView1.Columns[2].HeaderText = "Equipo de procedencia";
                dataGridView1.Columns[3].Width = 130;
                dataGridView1.Columns[3].HeaderText = "Tipo de fichaje";
                dataGridView1.Columns[4].Width = 150;
                dataGridView1.Columns[4].HeaderText = "Equipo ID";
                dataGridView1.Columns[5].Width = 150;
                dataGridView1.Columns[5].HeaderText = "Jugador DNI";
                dataGridView1.Columns[6].Width = 160;
                dataGridView1.Columns[6].HeaderText = "Buscador de equipo que ficha";
                dataGridView1.Columns[7].Width = 140;
                dataGridView1.Columns[7].HeaderText = "Equipo que ficha";
                dataGridView1.Columns[8].Width = 140;
                dataGridView1.Columns[8].HeaderText = "Jugador fichado";

                // OCULTO COLUMNAS DE IDS (EQUIPOS Y JUGADORES)
                this.dataGridView1.Columns["equipoFicha"].Visible = false;
                this.dataGridView1.Columns["jugadorFichado"].Visible = false;

                //COMANDO INSERT ----------------------------------------------------------------
                SQLiteCommand comando_ins = new SQLiteCommand("INSERT INTO fichaje(fechaFichaje, precio, equipoProcedencia, tipoFichaje, equipoFicha, jugadorFichado) VALUES (@fechaFichaje, @precio, @equipoProcedencia, @tipoFichaje, @comboequipos, @combojugadores)", conector.DameConexion());
                //Se establece la anti-inyeccion SQL enlazando los parámetros 
                comando_ins.Parameters.Add(new SQLiteParameter("@fechaFichaje", DbType.String));
                comando_ins.Parameters.Add(new SQLiteParameter("@precio", DbType.Int16));
                comando_ins.Parameters.Add(new SQLiteParameter("@equipoProcedencia", DbType.String));
                comando_ins.Parameters.Add(new SQLiteParameter("@tipoFichaje", DbType.String));
                comando_ins.Parameters.Add(new SQLiteParameter("@comboequipos", DbType.Int16));
                comando_ins.Parameters.Add(new SQLiteParameter("@combojugadores", DbType.String));
                //Añado los nombres de los campos de la base de datos. Los números de columnas se corresponde con el orden de los parámetros anteriores
                comando_ins.Parameters[0].SourceColumn = "fechaFichaje";
                comando_ins.Parameters[1].SourceColumn = "precio";
                comando_ins.Parameters[2].SourceColumn = "equipoProcedencia";
                comando_ins.Parameters[3].SourceColumn = "tipoFichaje";
                comando_ins.Parameters[4].SourceColumn = "equipoFicha";
                comando_ins.Parameters[5].SourceColumn = "jugadorFichado";
                //Se actualiza el comando Insert y se le asocia la conexión
                DataAdap.InsertCommand = comando_ins;
                DataAdap.InsertCommand.Connection = conector.DameConexion();

                //COMANDO UPDATE ----------------------------------------------------------------
                SQLiteCommand comando_act = new SQLiteCommand("UPDATE fichaje SET precio=@precio, equipoProcedencia=@equipoProcedencia, tipoFichaje=@tipoFichaje, equipoFicha=@comboequipos, jugadorFichado=@combojugadores  WHERE fechaFichaje=@fechaFichaje", conector.DameConexion());
                //Dado que son los mismos parámmetros que para el comando insert puedo hacer lo siguiente: copiar parámetros y sourcecolumns 
                foreach (SQLiteParameter i in comando_ins.Parameters)
                    comando_act.Parameters.Add(i);

                for (int i = 0; i < 5; i++)
                    comando_act.Parameters[i].SourceColumn = comando_ins.Parameters[i].SourceColumn;

                DataAdap.UpdateCommand = comando_act;
                DataAdap.UpdateCommand.Connection = conector.DameConexion();

                //COMANDO DELETE ----------------------------------------------------------------
                SQLiteCommand comando_del = new SQLiteCommand("DELETE FROM fichaje WHERE fechaFichaje = @fechaFichaje", conector.DameConexion());
                //Se establece la anti-inyeccion SQL enlazando los parámetros 
                comando_del.Parameters.Add(new SQLiteParameter("@fechaFichaje", DbType.String));
                comando_del.Parameters[0].SourceColumn = "fechaFichaje";
                DataAdap.DeleteCommand = comando_del;
                DataAdap.DeleteCommand.Connection = conector.DameConexion();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        } // fin de inicializa datagridd

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
                int indicecol_fecha = dataGridView1.Columns["fechaFichaje"].Index;
                string fecha = dataGridView1.Rows[i].Cells[indicecol_fecha].Value.ToString();
                int indicecol_precio = dataGridView1.Columns["precio"].Index;
                string precio = dataGridView1.Rows[i].Cells[indicecol_precio].Value.ToString();
                int indicecol_equipoProc = dataGridView1.Columns["equipoProcedencia"].Index;
                string equipoProc = dataGridView1.Rows[i].Cells[indicecol_equipoProc].Value.ToString();
                int indicecol_tipo = dataGridView1.Columns["tipofichaje"].Index;
                string tipoFichaje = dataGridView1.Rows[i].Cells[indicecol_tipo].Value.ToString();
                int indicecol_equipoFicha = dataGridView1.Columns["equipoFicha"].Index;
                string equipoFicha = dataGridView1.Rows[i].Cells[indicecol_equipoFicha].Value.ToString();
                int indicecol_jugador = dataGridView1.Columns["jugadorFichado"].Index;
                string jugador = dataGridView1.Rows[i].Cells[indicecol_jugador].Value.ToString();
                int fila = i + 1;
                if(fecha.Equals(""))
                {
                    MessageBox.Show("La FECHA DE FICHAJE no puede queda vacía. Fila " + fila);
                    dataGridView1.CurrentCell = dataGridView1.Rows[i].Cells[indicecol_fecha];//Se coloca el foco
                    no_guardar = 1;
                    break;//Para que salga de la ejecución
                } else if (precio.Equals(""))
                {
                    MessageBox.Show("El PRECIO no puede queda vacío. Fila " + fila);
                    dataGridView1.CurrentCell = dataGridView1.Rows[i].Cells[indicecol_precio];//Se coloca el foco
                    no_guardar = 1;
                    break;//Para que salga de la ejecución
                } else if(equipoProc.Equals(""))
                {
                    MessageBox.Show("El EQUIPO DE PROCEDENCIA no puede queda vacío. Fila " + fila);
                    dataGridView1.CurrentCell = dataGridView1.Rows[i].Cells[indicecol_equipoProc];//Se coloca el foco
                    no_guardar = 1;
                    break;//Para que salga de la ejecución
                } else if(tipoFichaje.Equals(""))
                {
                    MessageBox.Show("El TIPO DE FICHAJE no puede queda vacío. Fila " + fila);
                    dataGridView1.CurrentCell = dataGridView1.Rows[i].Cells[indicecol_tipo];//Se coloca el foco
                    no_guardar = 1;
                    break;//Para que salga de la ejecución
                } else if(equipoFicha.Equals(""))
                {
                    MessageBox.Show("Debes seleccionar un EQUIPO QUE FICHA. Fila " + fila);
                    dataGridView1.CurrentCell = dataGridView1.Rows[i].Cells[indicecol_equipoFicha];//Se coloca el foco
                    no_guardar = 1;
                    break;//Para que salga de la ejecución
                } else if (jugador.Equals(""))
                {
                    MessageBox.Show("Debes seleccionar un JUGADOR FICHADO. Fila " + fila);
                    dataGridView1.CurrentCell = dataGridView1.Rows[i].Cells[indicecol_jugador];//Se coloca el foco
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
                        MessageBox.Show("Fichaje guardado en BBDD");

                        //Vuelvo a consultar en la base de datos para que me cargue los datos modificados
                        actualizaElDatagrid();
                    }
                }
                catch (SQLiteException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        // actualiza el datagrid
        private void actualizaElDatagrid()
        {
            //Vuelvo a consultar en la base de datos para que me cargue los datos modificados
            SQLiteCommand consulta = conector.DameComando();
            consulta.CommandText = "SELECT * FROM fichaje";
            dtRecord = new DataTable();
            DataAdap.Fill(dtRecord);
            dataGridView1.DataSource = dtRecord;
            salvado = true;
        }

        // Cuando borra una fila va a guardar en BD
        private void dataGridView1_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            this.Validate();
            this.guardarDatos();
            this.Refresh();
        }

        // Borrado en cascada antes de borrar un jugador de sus fichajes relacionados
        public void borraJugadoresCascada()
        {
            //COMANDO DELETE ----------------------------------------------------------------
            SQLiteCommand comando_del = new SQLiteCommand("DELETE FROM fichaje WHERE fechaFichaje = @fechaFichaje", conector.DameConexion());
            //Se establece la anti-inyeccion SQL enlazando los parámetros 
            comando_del.Parameters.Add(new SQLiteParameter("@fechaFichaje", DbType.String));
            comando_del.Parameters[0].SourceColumn = "fechaFichaje";
            DataAdap.DeleteCommand = comando_del;
            DataAdap.DeleteCommand.Connection = conector.DameConexion();
        }

        // activa botones al editar una celda del datagrid
        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            this.Opener.activarBotones("fichajes");
        }


        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            string tituloCol = dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].Name;
            //Obtiene el nombre de la columna perteneciente a la celda actual
            Console.WriteLine("en editing control showing => "+tituloCol);
            TextBox autoTextEquipos = e.Control as TextBox;//Se genera un textbox desplegable
            if (autoTextEquipos != null)
            {
                autoTextEquipos.AutoCompleteMode = AutoCompleteMode.Suggest;
                autoTextEquipos.AutoCompleteSource = AutoCompleteSource.CustomSource;
            }
            if (tituloCol.Equals("busquedaE"))
            {
                autoTextEquipos.AutoCompleteCustomSource = this.ColeccionParaAutocompletarEquipos;//Se asigna la colección al TexBox Desplegable
            }

            else if (tituloCol.Equals("comboequipos"))
            {
                //No hace nada, ya que si es combo e intenta modificar el autoText saltará una excepción
            }
            else
            {//Tengo que limpiar el objeto para que no visualice la lista si es otro campo
                //autoTextEquipos.AutoCompleteCustomSource = null;//Se asigna la colección al TexBox Desplegable
            }
        }

        // Para las columnas de AUTOCOMPLETAR coger el valor real que guarda (id equipo o dni jugador) 
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            Console.WriteLine("Terminada edición");
            if (dataGridView1.Columns[e.ColumnIndex].Name == "busquedaE") 
            {
                string idActual =  dataGridView1.Rows[e.RowIndex].Cells["equipoFicha"].Value.ToString();
                Console.WriteLine("ID DE EQUIPO ACTUAL AL SELECCIONAR AUTOCOMPLETAR => " + idActual);
                String idEquipo;
                int idPonerEnColumna=0;
                String equipo = this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                Console.WriteLine("Equipo escogido tras la busqueda => " + equipo);
                //String idEquipo = this.ColeccionParaAutocompletarEquiposId[this.ColeccionParaAutocompletarEquipos.IndexOf(equipo)];
                int indice = 0;
                foreach ( String equipos in ColeccionParaAutocompletarEquipos)
                {
                    indice++;
                    if (equipos == equipo)
                    {
                        idEquipo = ColeccionParaAutocompletarEquiposId[indice-1];
                        Console.WriteLine("ID de Equipo encontrado => "+idEquipo);
                        idPonerEnColumna = Int16.Parse(idEquipo);
                    }
                }

                Console.WriteLine("ID EQUIPO QUE VOY A PONER EN LA COLUMNA => " + idPonerEnColumna);
                if (idPonerEnColumna == 0)
                {
                    dataGridView1.Rows[e.RowIndex].Cells["equipoFicha"].Value = idActual;
                } else
                {
                    dataGridView1.Rows[e.RowIndex].Cells["equipoFicha"].Value = idPonerEnColumna;
                }
                
                /*for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    // (dataGridView1.Columns[e.ColumnIndex].Name == "dni") && (dataGridView1.Rows[e.RowIndex].Cells["dni"].Value.ToString().Equals(""))
                    int indicecol_equipo = dataGridView1.Columns["equipoFicha"].Index;
                    Console.WriteLine("INDICE DE COLUMNA EQUIPO FICHA => " + indicecol_equipo);
                    string equipoFicha = dataGridView1.Rows[i].Cells[indicecol_equipo].Value.ToString();
                    Console.WriteLine("ID EQUIPO FICHA ACTUAL => " + equipoFicha);
                    dataGridView1.Rows[e.ColumnIndex].Cells[indicecol_equipo].Value = idPonerEnColumna;
                }*/

               
            }
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            Console.WriteLine("Excepción producida en DATAGRID de FICHAJES");
        }

        // click sobre una fila para mostrar el menú contextual
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
                    string fechaFichaje = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
                    Console.WriteLine("FECHA fichaje para VER FICHA => " + fechaFichaje);
                    VerFichaje form_editar = new VerFichaje(this.conector, fechaFichaje);//Le paso el valor de la celda primera dni
                    form_editar.ShowDialog();
                    break;
                case "borrar":
                    if ((MessageBox.Show("¿Seguro que quiere borrar el fichaje seleccionado?", "Atención", MessageBoxButtons.YesNo) == DialogResult.Yes))
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
                        if ((MessageBox.Show("¿Seguro que quiere borrar el fichaje seleccionado?", "Atención", MessageBoxButtons.YesNo) == DialogResult.Yes))
                        {
                            dataGridView1.Rows.RemoveAt(dataGridView1.SelectedRows[0].Index);
                            this.dataGridView1_UserDeletedRow(null, null); ;//Actualiza el DataGrid con la base de datos
                        }
                        break;
                    case (Keys.V): //Mayúscula o minúscula siempre y cuando no vaya en combinación con otra tecla (x. ejemplo shift)
                        string fechaFichaje = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
                        Console.WriteLine("FECHA fichaje para VER FICHA => " + fechaFichaje);
                        VerFichaje form_editar = new VerFichaje(this.conector, fechaFichaje);//Le paso el valor de la celda primera dni
                        form_editar.ShowDialog();
                        //Si no tuviéramos bloqueada la primera columna, escribiría una e tras mostrar el form_editar
                        //para evitar esto simulamos que pulsamos la tecla ESCAPE para que, aunque escriba la letra en la celda, vuelva al valor que tenía antes
                        SendKeys.Send("{ESC}");
                        break;
                }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1 & e.ColumnIndex > -1)
            {
                if ((dataGridView1.Columns[e.ColumnIndex].Name == "fechaFichaje") & calendarioactivo == false)
                {
                    salvado = false;

                    //Inicializa el DateTimePicker Control  
                    this.calendario = new DateTimePicker();
                    //Añade el DateTimePicker control dentro del DataGridView   
                    dataGridView1.Controls.Add(calendario);
                    //Pone el formato sin hora  
                    calendario.Format = DateTimePickerFormat.Short;
                    // Esto me da el area rectangular del area de la celda
                    Rectangle oRectangle = dataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
                    //Le doy ese tamaño y localización 
                    calendario.Size = new Size(oRectangle.Width, oRectangle.Height);
                    calendario.Location = new Point(oRectangle.X, oRectangle.Y);
                    //Se crea un evento para que coloque la fecha en la celda
                    calendario.CloseUp += new EventHandler(DateTimePicker_ValueChange);
                    calendario.KeyDown += new KeyEventHandler(DateTimePicker_Tecla);//Para desactivar la edición de la caja
                    calendarioactivo = true; //Evita que active un calendario y al irme a otra celda rellene la fecha en la celda incorrecta
                    filafecha = e.RowIndex;
                    columnafecha = e.ColumnIndex;

                }
                else //Se ha pinchado en cualquier otra celda
                {
                    if (this.calendarioactivo)
                    {
                        MessageBox.Show("Debes terminar de rellenar la fecha");
                        dataGridView1.CurrentCell = dataGridView1.Rows[this.filafecha].Cells[this.columnafecha];//Se coloca el foco
                        calendarioactivo = false; //Desactiva la bandera
                    }
                }
            }
        }

        // Función asociada al calendario
        private void DateTimePicker_Tecla(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;//Evita la pulsación de teclas
        }

        // Función asociada al calendario
        private void DateTimePicker_ValueChange(object sender, EventArgs e)
        {
            //Oculta el control de la celda (pestaña)
            dataGridView1.CurrentCell.Value = calendario.Text.ToString();//Escribe el valor de la celda de fecha
            calendario.Visible = false;
            calendarioactivo = false; //Desactiva la bandera
        }

    }
}
