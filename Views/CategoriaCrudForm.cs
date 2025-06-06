using System;
using System.Data;
using System.Windows.Forms;
using ZapateriaWinForms.Utilities;
using Microsoft.Data.SqlClient;

namespace ZapateriaWinForms.Views
{
    public class CategoriaCrudForm : Form
    {
        private DataGridView dgvCategorias;
        private BindingSource bindingSource;
        private DataTable categoriasTable = new DataTable();
        private TextBox txtBuscar;

        public CategoriaCrudForm()
        {
            this.Text = "Gestión de Categorías";
            this.Width = 500;
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
            dgvCategorias = new DataGridView {
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
            dgvCategorias.RowHeadersVisible = false;
            dgvCategorias.ColumnHeadersHeight = 36;
            dgvCategorias.AllowUserToResizeRows = false;
            dgvCategorias.AllowUserToResizeColumns = true;
            dgvCategorias.MultiSelect = false;
            dgvCategorias.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ID_Categoria", HeaderText = "ID" });
            dgvCategorias.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Nombre_Categoria", HeaderText = "Nombre" });

            bindingSource = new BindingSource();
            dgvCategorias.DataSource = bindingSource;

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
                if (dgvCategorias.CurrentRow != null)
                    AbrirEdicion(dgvCategorias.CurrentRow.DataBoundItem as DataRowView);
            };
            panelBotones.Controls.Add(btnAgregar, 0, 0);
            panelBotones.Controls.Add(btnEditar, 1, 0);

            // Ensamblar el layout
            mainPanel.Controls.Add(panelBusqueda, 0, 0);
            mainPanel.Controls.Add(dgvCategorias, 0, 1);
            mainPanel.Controls.Add(panelBotones, 0, 2);
            this.Controls.Clear();
            this.Controls.Add(mainPanel);

            // Filtro responsivo
            var dv = new DataView(categoriasTable);
            bindingSource.DataSource = dv;
            txtBuscar.TextChanged += (s, e) => RefrescarFiltroCategorias();
            // Al recargar datos, refrescar el DataView
            void RefrescarFiltroCategorias()
            {
                var filtro = txtBuscar.Text.Trim().ToLower();
                if (string.IsNullOrWhiteSpace(filtro))
                {
                    bindingSource.DataSource = categoriasTable;
                    return;
                }
                var tablaFiltrada = categoriasTable.Clone();
                foreach (DataRow row in categoriasTable.Rows)
                {
                    var nombre = row["Nombre_Categoria"]?.ToString()?.ToLower() ?? string.Empty;
                    if (nombre.Contains(filtro))
                        tablaFiltrada.ImportRow(row);
                }
                bindingSource.DataSource = tablaFiltrada;
            }

            CargarCategorias();
        }

        private void CargarCategorias()
        {
            categoriasTable = new DataTable();
            string connectionString = ConfigHelper.GetConnectionString();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT ID_Categoria, Nombre_Categoria FROM Categoria";
                using (var da = new SqlDataAdapter(query, conn))
                {
                    da.Fill(categoriasTable);
                }
            }
            bindingSource.DataSource = categoriasTable;
            if (txtBuscar != null)
                txtBuscar.Text = txtBuscar.Text; // Forzar evento TextChanged para refrescar filtro
        }

        private void AbrirEdicion(DataRowView? row)
        {
            var form = new CategoriaEditForm(row);
            if (form.ShowDialog() == DialogResult.OK)
                CargarCategorias();
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

    public class CategoriaEditForm : Form
    {
        private TextBox txtNombre;
        private DataRowView? rowEdicion;
        public CategoriaEditForm(DataRowView? row)
        {
            rowEdicion = row;
            this.Text = row == null ? "Agregar Categoría" : "Editar Categoría";
            this.Width = 350;
            this.Height = 180;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = System.Drawing.Color.White;
            this.Font = new System.Drawing.Font("Segoe UI", 10);

            var panel = new TableLayoutPanel {
                RowCount = 2,
                ColumnCount = 2,
                Dock = DockStyle.Fill,
                Padding = new Padding(15),
                AutoSize = true
            };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            Label lblNombre = new Label { Text = "Nombre:", Anchor = AnchorStyles.Right, TextAlign = System.Drawing.ContentAlignment.MiddleRight };
            txtNombre = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            if (row != null)
                txtNombre.Text = row["Nombre_Categoria"].ToString();
            var btnGuardar = new Button { Text = "Guardar", Anchor = AnchorStyles.None, Width = 120, Height = 32 };
            btnGuardar.Click += BtnGuardar_Click;

            panel.Controls.Add(lblNombre, 0, 0); panel.Controls.Add(txtNombre, 1, 0);
            panel.Controls.Add(btnGuardar, 0, 1); panel.SetColumnSpan(btnGuardar, 2);
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
                    string query = "INSERT INTO Categoria (Nombre_Categoria) VALUES (@nombre)";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@nombre", txtNombre.Text);
                        try { cmd.ExecuteNonQuery(); this.DialogResult = DialogResult.OK; }
                        catch (Exception ex) { MessageBox.Show("Error al guardar: " + ex.Message); }
                    }
                }
                else
                {
                    string query = "UPDATE Categoria SET Nombre_Categoria = @nombre WHERE ID_Categoria = @id";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", rowEdicion["ID_Categoria"]);
                        cmd.Parameters.AddWithValue("@nombre", txtNombre.Text);
                        try { cmd.ExecuteNonQuery(); this.DialogResult = DialogResult.OK; }
                        catch (Exception ex) { MessageBox.Show("Error al actualizar: " + ex.Message); }
                    }
                }
            }
        }
    }
}
