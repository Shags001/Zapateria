using System;
using System.Data;
using System.Windows.Forms;
using ZapateriaWinForms.Utilities;
using Microsoft.Data.SqlClient;

namespace ZapateriaWinForms.Views
{
    public class ProveedoresCrudForm : Form
    {
        private DataGridView dgvProveedores;
        private BindingSource bindingSource;
        private DataTable proveedoresTable = new DataTable();
        private TextBox txtBuscar;

        public ProveedoresCrudForm()
        {
            this.Text = "Gestión de Proveedores";
            this.Width = 700;
            this.Height = 400;
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
            dgvProveedores = new DataGridView {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = System.Drawing.Color.White,
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells,
                MinimumSize = new System.Drawing.Size(300, 100),
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
            dgvProveedores.RowHeadersVisible = false;
            dgvProveedores.ColumnHeadersHeight = 36;
            dgvProveedores.AllowUserToResizeRows = false;
            dgvProveedores.AllowUserToResizeColumns = true;
            dgvProveedores.MultiSelect = false;
            dgvProveedores.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ID_Proveedor", HeaderText = "ID" });
            dgvProveedores.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Nombre_Proveedor", HeaderText = "Nombre" });
            dgvProveedores.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Direccion", HeaderText = "Dirección" });
            dgvProveedores.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Telefono", HeaderText = "Teléfono" });
            dgvProveedores.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Correo_Proveedor", HeaderText = "Correo" });

            bindingSource = new BindingSource();
            dgvProveedores.DataSource = bindingSource;

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
                if (dgvProveedores.CurrentRow != null)
                    AbrirEdicion(dgvProveedores.CurrentRow.DataBoundItem as DataRowView);
            };
            panelBotones.Controls.Add(btnAgregar, 0, 0);
            panelBotones.Controls.Add(btnEditar, 1, 0);

            // Ensamblar el layout
            mainPanel.Controls.Add(panelBusqueda, 0, 0);
            mainPanel.Controls.Add(dgvProveedores, 0, 1);
            mainPanel.Controls.Add(panelBotones, 0, 2);
            this.Controls.Clear();
            this.Controls.Add(mainPanel);

            // Mejorar responsividad de la ventana y tabla
            this.MinimumSize = new System.Drawing.Size(900, 400);
            this.Width = 1000;
            this.Height = 500;
            this.SizeGripStyle = SizeGripStyle.Show;
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.AutoSize = false;
            mainPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            mainPanel.RowStyles[0].SizeType = SizeType.Absolute;
            mainPanel.RowStyles[0].Height = 60;
            mainPanel.RowStyles[1].SizeType = SizeType.Percent;
            mainPanel.RowStyles[1].Height = 100;
            mainPanel.RowStyles[2].SizeType = SizeType.Absolute;
            mainPanel.RowStyles[2].Height = 60;

            panelBusqueda.Dock = DockStyle.Top;
            panelBusqueda.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panelBusqueda.AutoSize = false;
            panelBusqueda.Height = 50;

            panelBotones.Dock = DockStyle.Bottom;
            panelBotones.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelBotones.AutoSize = false;
            panelBotones.Height = 60;

            // DataGridView responsivo
            // Elimina restricciones de tamaño máximo para permitir expansión
            dgvProveedores.Dock = DockStyle.Fill;
            dgvProveedores.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvProveedores.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvProveedores.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvProveedores.MinimumSize = new System.Drawing.Size(300, 100);
            dgvProveedores.Margin = new Padding(0);

            // Filtro responsivo
            var dv = new DataView(proveedoresTable);
            bindingSource.DataSource = dv;
            txtBuscar.TextChanged += (s, e) => RefrescarFiltroProveedores();

            CargarProveedores();
        }

        private void RefrescarFiltroProveedores()
        {
            var filtro = txtBuscar.Text.Trim().ToLower();
            if (string.IsNullOrWhiteSpace(filtro))
            {
                bindingSource.DataSource = proveedoresTable;
                return;
            }
            var tablaFiltrada = proveedoresTable.Clone();
            foreach (DataRow row in proveedoresTable.Rows)
            {
                var nombre = row["Nombre_Proveedor"]?.ToString()?.ToLower() ?? string.Empty;
                var direccion = row["Direccion"]?.ToString()?.ToLower() ?? string.Empty;
                var telefono = row["Telefono"]?.ToString()?.ToLower() ?? string.Empty;
                var correo = row["Correo_Proveedor"]?.ToString()?.ToLower() ?? string.Empty;
                if (nombre.Contains(filtro) || direccion.Contains(filtro) || telefono.Contains(filtro) || correo.Contains(filtro))
                    tablaFiltrada.ImportRow(row);
            }
            bindingSource.DataSource = tablaFiltrada;
        }

        private void CargarProveedores()
        {
            proveedoresTable = new DataTable();
            string connectionString = ConfigHelper.GetConnectionString();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT ID_Proveedor, Nombre_Proveedor, Direccion, Telefono, Correo_Proveedor FROM Proveedores";
                using (var da = new SqlDataAdapter(query, conn))
                {
                    da.Fill(proveedoresTable);
                }
            }
            bindingSource.DataSource = proveedoresTable;
            if (txtBuscar != null)
                txtBuscar.Text = txtBuscar.Text;
        }

        private void AbrirEdicion(DataRowView? row)
        {
            var form = new ProveedorEditForm(row);
            if (form.ShowDialog() == DialogResult.OK)
                CargarProveedores();
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

    public class ProveedorEditForm : Form
    {
        private TextBox txtNombre, txtDireccion, txtTelefono, txtCorreo;
        private DataRowView? rowEdicion;
        public ProveedorEditForm(DataRowView? row)
        {
            rowEdicion = row;
            this.Text = row == null ? "Agregar Proveedor" : "Editar Proveedor";
            this.Width = 400;
            this.Height = 300;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = System.Drawing.Color.White;
            this.Font = new System.Drawing.Font("Segoe UI", 10);

            var panel = new TableLayoutPanel {
                RowCount = 5,
                ColumnCount = 2,
                Dock = DockStyle.Fill,
                Padding = new Padding(15),
                AutoSize = true
            };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            Label lblNombre = new Label { Text = "Nombre:", Anchor = AnchorStyles.Right, TextAlign = System.Drawing.ContentAlignment.MiddleRight };
            txtNombre = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            Label lblDireccion = new Label { Text = "Dirección:", Anchor = AnchorStyles.Right, TextAlign = System.Drawing.ContentAlignment.MiddleRight };
            txtDireccion = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            Label lblTelefono = new Label { Text = "Teléfono:", Anchor = AnchorStyles.Right, TextAlign = System.Drawing.ContentAlignment.MiddleRight };
            txtTelefono = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            Label lblCorreo = new Label { Text = "Correo:", Anchor = AnchorStyles.Right, TextAlign = System.Drawing.ContentAlignment.MiddleRight };
            txtCorreo = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right };

            if (row != null)
            {
                txtNombre.Text = row["Nombre_Proveedor"].ToString();
                txtDireccion.Text = row["Direccion"].ToString();
                txtTelefono.Text = row["Telefono"].ToString();
                txtCorreo.Text = row["Correo_Proveedor"].ToString();
            }
            var btnGuardar = new Button { Text = "Guardar", Anchor = AnchorStyles.None, Width = 120, Height = 32 };
            btnGuardar.Click += BtnGuardar_Click;

            panel.Controls.Add(lblNombre, 0, 0); panel.Controls.Add(txtNombre, 1, 0);
            panel.Controls.Add(lblDireccion, 0, 1); panel.Controls.Add(txtDireccion, 1, 1);
            panel.Controls.Add(lblTelefono, 0, 2); panel.Controls.Add(txtTelefono, 1, 2);
            panel.Controls.Add(lblCorreo, 0, 3); panel.Controls.Add(txtCorreo, 1, 3);
            panel.Controls.Add(btnGuardar, 0, 4); panel.SetColumnSpan(btnGuardar, 2);
            this.Controls.Add(panel);
        }

        private void BtnGuardar_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre es obligatorio.");
                return;
            }
            string connectionString = ConfigHelper.GetConnectionString();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                if (rowEdicion == null)
                {
                    string query = "INSERT INTO Proveedores (Nombre_Proveedor, Direccion, Telefono, Correo_Proveedor) VALUES (@nombre, @direccion, @telefono, @correo)";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@nombre", txtNombre.Text);
                        cmd.Parameters.AddWithValue("@direccion", txtDireccion.Text);
                        cmd.Parameters.AddWithValue("@telefono", txtTelefono.Text);
                        cmd.Parameters.AddWithValue("@correo", txtCorreo.Text);
                        try { cmd.ExecuteNonQuery(); this.DialogResult = DialogResult.OK; }
                        catch (Exception ex) { MessageBox.Show("Error al guardar: " + ex.Message); }
                    }
                }
                else
                {
                    string query = "UPDATE Proveedores SET Nombre_Proveedor = @nombre, Direccion = @direccion, Telefono = @telefono, Correo_Proveedor = @correo WHERE ID_Proveedor = @id";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", rowEdicion["ID_Proveedor"]);
                        cmd.Parameters.AddWithValue("@nombre", txtNombre.Text);
                        cmd.Parameters.AddWithValue("@direccion", txtDireccion.Text);
                        cmd.Parameters.AddWithValue("@telefono", txtTelefono.Text);
                        cmd.Parameters.AddWithValue("@correo", txtCorreo.Text);
                        try { cmd.ExecuteNonQuery(); this.DialogResult = DialogResult.OK; }
                        catch (Exception ex) { MessageBox.Show("Error al actualizar: " + ex.Message); }
                    }
                }
            }
        }
    }
}
