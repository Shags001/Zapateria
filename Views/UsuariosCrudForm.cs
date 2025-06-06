using System;
using System.Data;
using System.Windows.Forms;
using ZapateriaWinForms.Utilities;
using Microsoft.Data.SqlClient;

namespace ZapateriaWinForms.Views
{
    public class UsuariosCrudForm : Form
    {
        private DataGridView dgvUsuarios;
        private BindingSource bindingSource;
        private DataTable usuariosTable = new DataTable();
        private TextBox txtBuscar;

        public UsuariosCrudForm()
        {
            this.Text = "Gestión de Usuarios";
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
            dgvUsuarios = new DataGridView {
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
            dgvUsuarios.RowHeadersVisible = false;
            dgvUsuarios.ColumnHeadersHeight = 36;
            dgvUsuarios.AllowUserToResizeRows = false;
            dgvUsuarios.AllowUserToResizeColumns = true;
            dgvUsuarios.MultiSelect = false;
            dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ID_Usuario", HeaderText = "ID" });
            dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Nombre_Usuario", HeaderText = "Usuario" });
            dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Rol", HeaderText = "Rol" });
            dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Ultimo_Inicio_Sesion", HeaderText = "Último Inicio" });

            bindingSource = new BindingSource();
            dgvUsuarios.DataSource = bindingSource;

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
                if (dgvUsuarios.CurrentRow != null)
                    AbrirEdicion(dgvUsuarios.CurrentRow.DataBoundItem as DataRowView);
            };
            panelBotones.Controls.Add(btnAgregar, 0, 0);
            panelBotones.Controls.Add(btnEditar, 1, 0);

            // Ensamblar el layout
            mainPanel.Controls.Add(panelBusqueda, 0, 0);
            mainPanel.Controls.Add(dgvUsuarios, 0, 1);
            mainPanel.Controls.Add(panelBotones, 0, 2);
            this.Controls.Clear();
            this.Controls.Add(mainPanel);
            mainPanel.SetRow(panelBusqueda, 0);
            mainPanel.SetColumn(panelBusqueda, 0);
            mainPanel.Controls.SetChildIndex(panelBusqueda, 0);

            CargarUsuarios();

            // Filtro responsivo
            var dv = new DataView(usuariosTable);
            bindingSource.DataSource = dv;
            txtBuscar.TextChanged += (s, e) => RefrescarFiltroUsuarios();
        }

        private void RefrescarFiltroUsuarios()
        {
            var filtro = txtBuscar.Text.Trim().ToLower();
            if (string.IsNullOrWhiteSpace(filtro))
            {
                bindingSource.DataSource = usuariosTable;
                return;
            }
            var tablaFiltrada = usuariosTable.Clone();
            foreach (DataRow row in usuariosTable.Rows)
            {
                var usuario = row["Nombre_Usuario"]?.ToString()?.ToLower() ?? string.Empty;
                var rol = row["Rol"]?.ToString()?.ToLower() ?? string.Empty;
                if (usuario.Contains(filtro) || rol.Contains(filtro))
                    tablaFiltrada.ImportRow(row);
            }
            bindingSource.DataSource = tablaFiltrada;
        }

        private void CargarUsuarios()
        {
            usuariosTable = new DataTable();
            string connectionString = ConfigHelper.GetConnectionString();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT ID_Usuario, Nombre_Usuario, Rol, Ultimo_Inicio_Sesion FROM UsuariosLogin";
                using (var da = new SqlDataAdapter(query, conn))
                {
                    da.Fill(usuariosTable);
                }
            }
            bindingSource.DataSource = usuariosTable;
            if (txtBuscar != null)
                txtBuscar.Text = txtBuscar.Text;
        }

        private void AbrirEdicion(DataRowView? row)
        {
            UsuarioLogin? usuario = null;
            if (row != null)
            {
                usuario = new UsuarioLogin
                {
                    ID_Usuario = Convert.ToInt32(row["ID_Usuario"]),
                    Nombre_Usuario = row["Nombre_Usuario"].ToString() ?? string.Empty,
                    Rol = row["Rol"].ToString() ?? string.Empty
                };
            }
            var form = new UsuarioEditForm(usuario);
            if (form.ShowDialog() == DialogResult.OK)
                CargarUsuarios();
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
}
