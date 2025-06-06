using System;
using System.Data;
using System.Windows.Forms;
using ZapateriaWinForms.Utilities;
using Microsoft.Data.SqlClient;

namespace ZapateriaWinForms.Views
{
    public class EmpleadosCrudForm : Form
    {
        private DataGridView dgvEmpleados;
        private BindingSource bindingSource;
        private DataTable empleadosTable = new DataTable();
        private TextBox txtBuscar;

        public EmpleadosCrudForm()
        {
            this.Text = "Gestión de Empleados";
            this.Width = 900;
            this.Height = 500;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Font = new System.Drawing.Font("Segoe UI", 10);

            // Panel principal para responsividad
            var mainPanel = new TableLayoutPanel {
                Dock = DockStyle.Fill,
                RowCount = 3,
                ColumnCount = 1,
                BackColor = System.Drawing.Color.WhiteSmoke,
                Padding = new Padding(0),
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 60)); // Buscador
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Tabla
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 60)); // Botones
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            // Barra de búsqueda responsiva
            var panelBusqueda = new Panel {
                Dock = DockStyle.Fill,
                Height = 50,
                Padding = new Padding(10, 10, 10, 10),
                BackColor = System.Drawing.Color.White
            };
            txtBuscar = new TextBox {
                PlaceholderText = "Buscar...",
                Dock = DockStyle.Fill,
                Font = new System.Drawing.Font("Segoe UI", 11)
            };
            panelBusqueda.Controls.Add(txtBuscar);

            // DataGridView responsivo y estilizado
            dgvEmpleados = new DataGridView {
                ReadOnly = true,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = System.Drawing.Color.White,
                BorderStyle = BorderStyle.None,
                Dock = DockStyle.Fill,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells,
                MinimumSize = new System.Drawing.Size(300, 150),
                MaximumSize = new System.Drawing.Size(2000, 2000),
                Margin = new Padding(0),
                EnableHeadersVisualStyles = false,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle {
                    BackColor = System.Drawing.Color.FromArgb(44, 62, 80),
                    ForeColor = System.Drawing.Color.White,
                    Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                },
                DefaultCellStyle = new DataGridViewCellStyle {
                    BackColor = System.Drawing.Color.White,
                    ForeColor = System.Drawing.Color.Black,
                    SelectionBackColor = System.Drawing.Color.FromArgb(189, 195, 199),
                    SelectionForeColor = System.Drawing.Color.Black,
                    Font = new System.Drawing.Font("Segoe UI", 10)
                },
                RowTemplate = { Height = 32 },
                GridColor = System.Drawing.Color.LightGray
            };
            dgvEmpleados.RowHeadersVisible = false;
            dgvEmpleados.ColumnHeadersHeight = 36;
            dgvEmpleados.AllowUserToResizeRows = false;
            dgvEmpleados.AllowUserToResizeColumns = true;
            dgvEmpleados.MultiSelect = false;
            dgvEmpleados.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ID_Empleado", HeaderText = "ID" });
            dgvEmpleados.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Nombre_Empleado", HeaderText = "Nombre" });
            dgvEmpleados.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Apellido_Empleado", HeaderText = "Apellido" });
            dgvEmpleados.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Direccion", HeaderText = "Dirección" });
            dgvEmpleados.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Telefono", HeaderText = "Teléfono" });
            dgvEmpleados.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Correo", HeaderText = "Correo" });
            dgvEmpleados.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Posicion", HeaderText = "Posición" });
            dgvEmpleados.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Salario", HeaderText = "Salario" });

            bindingSource = new BindingSource();
            dgvEmpleados.DataSource = bindingSource;

            // Panel de botones
            var panelBotones = new TableLayoutPanel {
                Dock = DockStyle.Fill,
                RowCount = 1,
                ColumnCount = 2,
                BackColor = System.Drawing.Color.WhiteSmoke,
                Padding = new Padding(0),
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };
            panelBotones.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            panelBotones.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            var btnAgregar = new Button { Text = "Agregar", Dock = DockStyle.Fill };
            var btnEditar = new Button { Text = "Editar", Dock = DockStyle.Fill };
            EstilizarBoton(btnAgregar, "#3B82F6", "#2563EB"); // Azul
            EstilizarBoton(btnEditar, "#6366F1", "#4338CA"); // Morado
            btnAgregar.Click += (s, e) => AbrirEdicion(null);
            btnEditar.Click += (s, e) => {
                if (dgvEmpleados.CurrentRow != null)
                    AbrirEdicion(dgvEmpleados.CurrentRow.DataBoundItem as DataRowView);
            };
            panelBotones.Controls.Add(btnAgregar, 0, 0);
            panelBotones.Controls.Add(btnEditar, 1, 0);

            // Ensamblar el layout
            mainPanel.Controls.Add(panelBusqueda, 0, 0);
            mainPanel.Controls.Add(dgvEmpleados, 0, 1);
            mainPanel.Controls.Add(panelBotones, 0, 2);
            this.Controls.Clear();
            this.Controls.Add(mainPanel);

            // Filtro responsivo
            var dv = new DataView(empleadosTable);
            bindingSource.DataSource = dv;
            txtBuscar.TextChanged += (s, e) => RefrescarFiltroEmpleados();

            CargarEmpleados();
        }

        private void RefrescarFiltroEmpleados()
        {
            var filtro = txtBuscar.Text.Trim().ToLower();
            if (string.IsNullOrWhiteSpace(filtro))
            {
                bindingSource.DataSource = empleadosTable;
                return;
            }
            var tablaFiltrada = empleadosTable.Clone();
            foreach (DataRow row in empleadosTable.Rows)
            {
                var nombre = row["Nombre_Empleado"]?.ToString()?.ToLower() ?? string.Empty;
                var apellido = row["Apellido_Empleado"]?.ToString()?.ToLower() ?? string.Empty;
                var direccion = row["Direccion"]?.ToString()?.ToLower() ?? string.Empty;
                var telefono = row["Telefono"]?.ToString()?.ToLower() ?? string.Empty;
                var correo = row["Correo"]?.ToString()?.ToLower() ?? string.Empty;
                var posicion = row["Posicion"]?.ToString()?.ToLower() ?? string.Empty;
                if (nombre.Contains(filtro) || apellido.Contains(filtro) || direccion.Contains(filtro) || telefono.Contains(filtro) || correo.Contains(filtro) || posicion.Contains(filtro))
                    tablaFiltrada.ImportRow(row);
            }
            bindingSource.DataSource = tablaFiltrada;
        }

        private void CargarEmpleados()
        {
            empleadosTable = new DataTable();
            string connectionString = ConfigHelper.GetConnectionString();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT ID_Empleado, Nombre_Empleado, Apellido_Empleado, Direccion, Telefono, Correo, Posicion, Salario FROM Empleados";
                using (var da = new SqlDataAdapter(query, conn))
                {
                    da.Fill(empleadosTable);
                }
            }
            bindingSource.DataSource = empleadosTable;
            if (txtBuscar != null)
                txtBuscar.Text = txtBuscar.Text;
        }

        private void AbrirEdicion(DataRowView? row)
        {
            var form = new EmpleadoEditForm(row);
            if (form.ShowDialog() == DialogResult.OK)
                CargarEmpleados();
        }

        private void EstilizarBoton(Button btn, string colorHex, string hoverHex)
        {
            btn.BackColor = System.Drawing.ColorTranslator.FromHtml(colorHex);
            btn.ForeColor = System.Drawing.Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = new System.Drawing.Font("Segoe UI", 11, System.Drawing.FontStyle.Bold);
            btn.Height = 40;
            btn.MouseEnter += (s, e) => btn.BackColor = System.Drawing.ColorTranslator.FromHtml(hoverHex);
            btn.MouseLeave += (s, e) => btn.BackColor = System.Drawing.ColorTranslator.FromHtml(colorHex);
        }
    }

    public class EmpleadoEditForm : Form
    {
        private TextBox txtNombre, txtApellido, txtDireccion, txtTelefono, txtCorreo, txtPosicion, txtSalario;
        private DataRowView? rowEdicion;
        public EmpleadoEditForm(DataRowView? row)
        {
            rowEdicion = row;
            this.Text = row == null ? "Agregar Empleado" : "Editar Empleado";
            this.Width = 500;
            this.Height = 350;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = System.Drawing.Color.White;
            this.Font = new System.Drawing.Font("Segoe UI", 10);

            var panel = new TableLayoutPanel {
                RowCount = 8,
                ColumnCount = 2,
                Dock = DockStyle.Fill,
                Padding = new Padding(15),
                AutoSize = true
            };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            Label lblNombre = new Label { Text = "Nombre:", Anchor = AnchorStyles.Right, TextAlign = System.Drawing.ContentAlignment.MiddleRight };
            txtNombre = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            Label lblApellido = new Label { Text = "Apellido:", Anchor = AnchorStyles.Right, TextAlign = System.Drawing.ContentAlignment.MiddleRight };
            txtApellido = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            Label lblDireccion = new Label { Text = "Dirección:", Anchor = AnchorStyles.Right, TextAlign = System.Drawing.ContentAlignment.MiddleRight };
            txtDireccion = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            Label lblTelefono = new Label { Text = "Teléfono:", Anchor = AnchorStyles.Right, TextAlign = System.Drawing.ContentAlignment.MiddleRight };
            txtTelefono = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            Label lblCorreo = new Label { Text = "Correo:", Anchor = AnchorStyles.Right, TextAlign = System.Drawing.ContentAlignment.MiddleRight };
            txtCorreo = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            Label lblPosicion = new Label { Text = "Posición:", Anchor = AnchorStyles.Right, TextAlign = System.Drawing.ContentAlignment.MiddleRight };
            txtPosicion = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            Label lblSalario = new Label { Text = "Salario:", Anchor = AnchorStyles.Right, TextAlign = System.Drawing.ContentAlignment.MiddleRight };
            txtSalario = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right };

            if (row != null)
            {
                txtNombre.Text = row["Nombre_Empleado"].ToString();
                txtApellido.Text = row["Apellido_Empleado"].ToString();
                txtDireccion.Text = row["Direccion"].ToString();
                txtTelefono.Text = row["Telefono"].ToString();
                txtCorreo.Text = row["Correo"].ToString();
                txtPosicion.Text = row["Posicion"].ToString();
                txtSalario.Text = row["Salario"].ToString();
            }
            var btnGuardar = new Button { Text = "Guardar", Anchor = AnchorStyles.None, Width = 120, Height = 32 };
            btnGuardar.Click += BtnGuardar_Click;

            panel.Controls.Add(lblNombre, 0, 0); panel.Controls.Add(txtNombre, 1, 0);
            panel.Controls.Add(lblApellido, 0, 1); panel.Controls.Add(txtApellido, 1, 1);
            panel.Controls.Add(lblDireccion, 0, 2); panel.Controls.Add(txtDireccion, 1, 2);
            panel.Controls.Add(lblTelefono, 0, 3); panel.Controls.Add(txtTelefono, 1, 3);
            panel.Controls.Add(lblCorreo, 0, 4); panel.Controls.Add(txtCorreo, 1, 4);
            panel.Controls.Add(lblPosicion, 0, 5); panel.Controls.Add(txtPosicion, 1, 5);
            panel.Controls.Add(lblSalario, 0, 6); panel.Controls.Add(txtSalario, 1, 6);
            panel.Controls.Add(btnGuardar, 0, 7); panel.SetColumnSpan(btnGuardar, 2);
            this.Controls.Add(panel);
        }

        private void BtnGuardar_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtApellido.Text))
            {
                MessageBox.Show("Nombre y Apellido son obligatorios.");
                return;
            }
            decimal salario = 0;
            decimal.TryParse(txtSalario.Text, out salario);
            string connectionString = ConfigHelper.GetConnectionString();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                if (rowEdicion == null)
                {
                    string query = "INSERT INTO Empleados (Nombre_Empleado, Apellido_Empleado, Direccion, Telefono, Correo, Posicion, Salario) VALUES (@nombre, @apellido, @direccion, @telefono, @correo, @posicion, @salario)";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@nombre", txtNombre.Text);
                        cmd.Parameters.AddWithValue("@apellido", txtApellido.Text);
                        cmd.Parameters.AddWithValue("@direccion", txtDireccion.Text);
                        cmd.Parameters.AddWithValue("@telefono", txtTelefono.Text);
                        cmd.Parameters.AddWithValue("@correo", txtCorreo.Text);
                        cmd.Parameters.AddWithValue("@posicion", txtPosicion.Text);
                        cmd.Parameters.AddWithValue("@salario", salario);
                        try { cmd.ExecuteNonQuery(); this.DialogResult = DialogResult.OK; }
                        catch (Exception ex) { MessageBox.Show("Error al guardar: " + ex.Message); }
                    }
                }
                else
                {
                    string query = "UPDATE Empleados SET Nombre_Empleado = @nombre, Apellido_Empleado = @apellido, Direccion = @direccion, Telefono = @telefono, Correo = @correo, Posicion = @posicion, Salario = @salario WHERE ID_Empleado = @id";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", rowEdicion["ID_Empleado"]);
                        cmd.Parameters.AddWithValue("@nombre", txtNombre.Text);
                        cmd.Parameters.AddWithValue("@apellido", txtApellido.Text);
                        cmd.Parameters.AddWithValue("@direccion", txtDireccion.Text);
                        cmd.Parameters.AddWithValue("@telefono", txtTelefono.Text);
                        cmd.Parameters.AddWithValue("@correo", txtCorreo.Text);
                        cmd.Parameters.AddWithValue("@posicion", txtPosicion.Text);
                        cmd.Parameters.AddWithValue("@salario", salario);
                        try { cmd.ExecuteNonQuery(); this.DialogResult = DialogResult.OK; }
                        catch (Exception ex) { MessageBox.Show("Error al actualizar: " + ex.Message); }
                    }
                }
            }
        }
    }
}
