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
    public partial class Jugadores : Form
    {
        // objeto BD
        private ConectorSQL conector;
        private DataTable dtRecord;
        private SQLiteDataAdapter DataAdap;

        // para el calendario de la fecha de nacimiento
        private DateTimePicker calendario;
        private bool calendarioactivo;
        private int filafecha;
        private int columnafecha;

        // para controlar cuando guardar algo
        bool salvado = true;//A true quiere decir que todo está guardado

        // interfaz para relación con padre
        public Interfaz Opener { get; set; }

        public Jugadores(ConectorSQL con)
        {
            InitializeComponent();
            conector = con;
            string sql;
            sql = "SELECT * FROM jugador";
            Inicializa_datagrid(sql);
        }

        private void llamaAlPadre_Click(object sender, EventArgs e)
        {
            this.Opener.activarBotones("jugadores");
        }

        // Inicializa y lista los datos de la tabla jugadores
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

               
                //dataGridView1.Columns["dni"].ReadOnly = true;
                dataGridView1.Columns["dni"].DefaultCellStyle.BackColor = Color.PaleGreen;
                
                

                //Añado el combo para la posición
                DataGridViewComboBoxColumn comboPosicion = new DataGridViewComboBoxColumn();
                comboPosicion.Name = "tipocombo";
                comboPosicion.DataPropertyName = "posicion";//Campo de la base de datos con el que enlaza
                comboPosicion.Items.Add("Portero");
                comboPosicion.Items.Add("Defensa");
                comboPosicion.Items.Add("Centrocampista");
                comboPosicion.Items.Add("Delantero");
                dataGridView1.Columns.RemoveAt(2);
                dataGridView1.Columns.Insert(2, comboPosicion);

                // Checkbox para el campo lesionado
                DataGridViewCheckBoxColumn checkLesion = new DataGridViewCheckBoxColumn();
                checkLesion.HeaderText = "Lesionado";
                checkLesion.Name = "activocheck";
                checkLesion.DataPropertyName = "lesionado";//Campo de la base de datos con el que enlaza
                checkLesion.FalseValue = 0;
                checkLesion.TrueValue = 1;
                dataGridView1.Columns.RemoveAt(6);//Borro lo columna de activo para que no aparezca. Podemos hacer igual que antes, comentar y ejecutar a ver qué pasa
                dataGridView1.Columns.Insert(6, checkLesion);//Se inserta la recién creada

                // Modifico cabeceras de la tabla
                dataGridView1.Columns[0].Width = 140;
                dataGridView1.Columns[0].HeaderText = "Dni";
                dataGridView1.Columns[1].Width = 190;
                dataGridView1.Columns[1].HeaderText = "Nombre";
                dataGridView1.Columns[2].Width = 150;
                dataGridView1.Columns[2].HeaderText = "Posición";
                dataGridView1.Columns[3].Width = 80;
                dataGridView1.Columns[3].HeaderText = "Dorsal";
                dataGridView1.Columns[4].Width = 150;
                dataGridView1.Columns[4].HeaderText = "F. Nacimiento";
                dataGridView1.Columns[5].Width = 150;
                dataGridView1.Columns[5].HeaderText = "Nacionalidad";
                dataGridView1.Columns[6].Width = 80;
                dataGridView1.Columns[6].HeaderText = "Lesionado";

                // Pongo clave primaria solo lectura a los jugadores que lista
                dataGridView1.Columns[0].ReadOnly = true;

                //COMANDO INSERT ----------------------------------------------------------------
                SQLiteCommand comando_ins = new SQLiteCommand("INSERT INTO jugador(dni, nombre, posicion, dorsal, fechaNacimiento, nacionalidad, lesionado) VALUES (@dni, @nombre, @posicion, @dorsal, @fechaNacimiento, @nacionalidad, @lesionado)", conector.DameConexion());
                //Se establece la anti-inyeccion SQL enlazando los parámetros 
                comando_ins.Parameters.Add(new SQLiteParameter("@dni", DbType.String));
                comando_ins.Parameters.Add(new SQLiteParameter("@nombre", DbType.String));
                comando_ins.Parameters.Add(new SQLiteParameter("@posicion", DbType.String));
                comando_ins.Parameters.Add(new SQLiteParameter("@dorsal", DbType.Int16));
                comando_ins.Parameters.Add(new SQLiteParameter("@fechaNacimiento", DbType.String));
                comando_ins.Parameters.Add(new SQLiteParameter("@nacionalidad", DbType.String));
                comando_ins.Parameters.Add(new SQLiteParameter("@lesionado", DbType.Int16));
                //Añado los nombres de los campos de la base de datos. Los números de columnas se corresponde con el orden de los parámetros anteriores
                comando_ins.Parameters[0].SourceColumn = "dni";
                comando_ins.Parameters[1].SourceColumn = "nombre";
                comando_ins.Parameters[2].SourceColumn = "posicion";
                comando_ins.Parameters[3].SourceColumn = "dorsal";
                comando_ins.Parameters[4].SourceColumn = "fechaNacimiento";
                comando_ins.Parameters[5].SourceColumn = "nacionalidad";
                comando_ins.Parameters[6].SourceColumn = "lesionado";
                //Se actualiza el comando Insert y se le asocia la conexión
                DataAdap.InsertCommand = comando_ins;
                DataAdap.InsertCommand.Connection = conector.DameConexion();

                //COMANDO UPDATE ----------------------------------------------------------------
                SQLiteCommand comando_act = new SQLiteCommand("UPDATE jugador SET nombre=@nombre, posicion=@posicion, dorsal=@dorsal, fechaNacimiento=@fechaNacimiento, nacionalidad=@nacionalidad, lesionado=@lesionado  WHERE dni=@dni", conector.DameConexion());
                //Dado que son los mismos parámmetros que para el comando insert puedo hacer lo siguiente: copiar parámetros y sourcecolumns 
                foreach (SQLiteParameter i in comando_ins.Parameters)
                    comando_act.Parameters.Add(i);

                for (int i = 0; i < 6; i++)
                    comando_act.Parameters[i].SourceColumn = comando_ins.Parameters[i].SourceColumn;

                DataAdap.UpdateCommand = comando_act;
                DataAdap.UpdateCommand.Connection = conector.DameConexion();

                //COMANDO DELETE ----------------------------------------------------------------
                SQLiteCommand comando_del = new SQLiteCommand("DELETE FROM jugador WHERE dni = @dni", conector.DameConexion());
                //Se establece la anti-inyeccion SQL enlazando los parámetros 
                comando_del.Parameters.Add(new SQLiteParameter("@dni", DbType.String));
                comando_del.Parameters[0].SourceColumn = "dni";
                DataAdap.DeleteCommand = comando_del;
                DataAdap.DeleteCommand.Connection = conector.DameConexion();

            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // Evento click sobre el datagridview
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1 & e.ColumnIndex > -1)
            { //Para que no falle al pulsar en la columna de selección (izquierda)
                // Console.WriteLine("Hago click en la celda del DATAGRIDVIEW");

                //Si se ha pulsado en fecha
                if ((dataGridView1.Columns[e.ColumnIndex].Name == "fechaNacimiento") & calendarioactivo == false)
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

                // PARA EVITAR EDITAR UN DNI (CLAVE PRIMARIA) PERO SI AÑADIR UNO NUEVO
                if((dataGridView1.Columns[e.ColumnIndex].Name == "dni") && (dataGridView1.Rows[e.RowIndex].Cells["dni"].Value.ToString().Equals("")))
                {
                    dataGridView1.Columns["dni"].ReadOnly = false;
                } else
                {
                    dataGridView1.Columns["dni"].ReadOnly = true;
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
                int indicecol_dni = dataGridView1.Columns["dni"].Index;
                string dni = dataGridView1.Rows[i].Cells[indicecol_dni].Value.ToString();
                int indicecol_nombre = dataGridView1.Columns["nombre"].Index;
                string nombre = dataGridView1.Rows[i].Cells[indicecol_nombre].Value.ToString();
                int indicecol_posicion = dataGridView1.Columns["tipocombo"].Index;
                string posicion = dataGridView1.Rows[i].Cells[indicecol_posicion].Value.ToString();
                int indicecol_dorsal = dataGridView1.Columns["dorsal"].Index;
                string dorsal = dataGridView1.Rows[i].Cells[indicecol_dorsal].Value.ToString();
                int indicecol_fecha = dataGridView1.Columns["fechaNacimiento"].Index;
                string fecha = dataGridView1.Rows[i].Cells[indicecol_fecha].Value.ToString();
                int indicecol_nacionalidad = dataGridView1.Columns["nacionalidad"].Index;
                string nacionalidad = dataGridView1.Rows[i].Cells[indicecol_nacionalidad].Value.ToString();
                int indicecol_lesionado = dataGridView1.Columns["activocheck"].Index;
                string lesionado = dataGridView1.Rows[i].Cells[indicecol_lesionado].Value.ToString();
                //Console.WriteLine("Compruebo guardar a => " + dni + " - " + nombre);
                int fila = i + 1;
                if(dni.Equals(""))
                {
                    MessageBox.Show("El DNI no puede queda vacío. Fila " + fila);
                    dataGridView1.CurrentCell = dataGridView1.Rows[i].Cells[indicecol_dni];//Se coloca el foco
                    no_guardar = 1;
                    break;//Para que salga de la ejecución
                }
                else if(nombre.Equals(""))
                {
                    MessageBox.Show("El NOMBRE no puede queda vacío. Fila " + fila);
                    dataGridView1.CurrentCell = dataGridView1.Rows[i].Cells[indicecol_nombre];//Se coloca el foco
                    no_guardar = 1;
                    break;//Para que salga de la ejecución
                }
                else if (posicion.Equals(""))
                {
                    MessageBox.Show("Debes seleccionar una POSICIÓN. Fila " + fila);
                    dataGridView1.CurrentCell = dataGridView1.Rows[i].Cells[indicecol_nombre];//Se coloca el foco
                    no_guardar = 1;
                    break;//Para que salga de la ejecución
                }
                else if(dorsal.Equals(""))
                {
                    MessageBox.Show("El DORSAL no puede queda vacío. Fila " + fila);
                    dataGridView1.CurrentCell = dataGridView1.Rows[i].Cells[indicecol_dorsal];//Se coloca el foco
                    no_guardar = 1;
                    break;//Para que salga de la ejecución
                } else if (fecha.Equals(""))
                {
                    MessageBox.Show("La FECHA DE NACIMIENTO no puede queda vacíA. Fila " + fila);
                    dataGridView1.CurrentCell = dataGridView1.Rows[i].Cells[indicecol_fecha];//Se coloca el foco
                    no_guardar = 1;
                    break;//Para que salga de la ejecución
                } else if(nacionalidad.Equals(""))
                {
                    MessageBox.Show("La NACIONALIDAD no puede queda vacía. Fila " + fila);
                    dataGridView1.CurrentCell = dataGridView1.Rows[i].Cells[indicecol_nacionalidad];//Se coloca el foco
                    no_guardar = 1;
                    break;//Para que salga de la ejecución
                }
                // compruebo que dorsal es un número
                Boolean esEntero = int.TryParse(dorsal, out int dorsalNum);
                Console.WriteLine("EL BOOLEAN DORSAL => " + esEntero);
                if (!esEntero)
                {
                    MessageBox.Show("El DORSAL debería ser un NÚMERO ENTERO. Fila " + fila);
                    dataGridView1.CurrentCell = dataGridView1.Rows[i].Cells[indicecol_dorsal];//Se coloca el foco
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
                        MessageBox.Show("Jugador guardado en BBDD");

                        //Vuelvo a consultar en la base de datos para que me cargue los datos modificados
                        actualizaDatagrid();
                    }
                } catch(SQLiteException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        // Actualiza los datos del datagrid actual y el de la tabla intermedia
        private void actualizaDatagrid()
        {
            SQLiteCommand consulta = conector.DameComando();
            consulta.CommandText = "SELECT * FROM jugador";
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
        public void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            this.Opener.activarBotones("jugadores");
        }

        // Control de excepciones
        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            Console.WriteLine("Excepción producida en DATAGRID de JUGADORES");
        }

        //Crea el menú contextual siempre y cuando no haya datos por salvar
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
                    string dniJugador = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                    Console.WriteLine("DNI Jugador para VER FICHA => " + dniJugador);
                    VerJugador form_editar = new VerJugador(this.conector,dniJugador);//Le paso el valor de la celda primera dni
                    form_editar.ShowDialog();
                    break;
                case "borrar":
                    if ((MessageBox.Show("¿Seguro que quiere borrar el jugador seleccionado?", "Atención", MessageBoxButtons.YesNo) == DialogResult.Yes))
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
                        if ((MessageBox.Show("¿Seguro que quiere borrar el jugador seleccionado?", "Atención", MessageBoxButtons.YesNo) == DialogResult.Yes))
                        {
                            dataGridView1.Rows.RemoveAt(dataGridView1.SelectedRows[0].Index);
                            this.dataGridView1_UserDeletedRow(null, null); ;//Actualiza el DataGrid con la base de datos
                        }
                        break;
                    case (Keys.V): //Mayúscula o minúscula siempre y cuando no vaya en combinación con otra tecla (x. ejemplo shift)
                        string dniJugador = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                        Console.WriteLine("DNI Jugador para VER FICHA => " + dniJugador);
                        VerJugador form_editar = new VerJugador(this.conector, dniJugador);//Le paso el valor de la celda primera dni
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
