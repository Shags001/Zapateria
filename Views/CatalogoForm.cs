using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;
using ZapateriaWinForms.Models;
using ZapateriaWinForms.Utilities;

namespace ZapateriaWinForms.Views
{
    public partial class CatalogoForm : Form
    {
        private DataGridView dgvProductos;
        private TextBox txtBuscar;
        private BindingSource bindingSource;
        private List<Producto> productosOriginal = new List<Producto>();

        public CatalogoForm(string rol = "")
        {
            this.Text = "Catálogo de Productos";
            this.Width = 1200;
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

            var panelBusqueda = new TableLayoutPanel {
                RowCount = 1,
                ColumnCount = 1,
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = System.Drawing.Color.White,
                Padding = new Padding(10, 10, 10, 10),
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Margin = new Padding(0)
            };

            txtBuscar = new TextBox
            {
                Dock = DockStyle.Fill,
                PlaceholderText = "Buscar..."
            };
            panelBusqueda.Controls.Add(txtBuscar, 0, 0);

            // DataGridView responsivo y estilizado
            dgvProductos = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                BackgroundColor = System.Drawing.Color.White,
                BorderStyle = BorderStyle.None,
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
                GridColor = System.Drawing.Color.LightGray,
                RowHeadersVisible = false,
                ColumnHeadersHeight = 36,
                AllowUserToResizeRows = false,
                AllowUserToResizeColumns = true,
                MultiSelect = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells,
                MinimumSize = new System.Drawing.Size(400, 200),
                MaximumSize = new System.Drawing.Size(2000, 2000),
                Margin = new Padding(0)
            };

            // Definir columnas manualmente
            var colNombre = new DataGridViewTextBoxColumn { DataPropertyName = "Nombre_Producto", HeaderText = "Nombre" };
            var colTalla = new DataGridViewTextBoxColumn { DataPropertyName = "Talla", HeaderText = "Talla" };
            var colModelo = new DataGridViewTextBoxColumn { DataPropertyName = "Modelo", HeaderText = "Modelo" };
            var colMarca = new DataGridViewTextBoxColumn { DataPropertyName = "Marca", HeaderText = "Marca" };
            var colColor = new DataGridViewTextBoxColumn { DataPropertyName = "Color", HeaderText = "Color" };
            var colPrecio = new DataGridViewTextBoxColumn { DataPropertyName = "Precio_Unitario", HeaderText = "Precio" };
            var colMaterial = new DataGridViewTextBoxColumn { DataPropertyName = "Material", HeaderText = "Material" };
            var colStock = new DataGridViewTextBoxColumn { DataPropertyName = "Stock", HeaderText = "Stock" };
            dgvProductos.Columns.AddRange(new DataGridViewColumn[] { colNombre, colTalla, colModelo, colMarca, colColor, colPrecio, colMaterial, colStock });

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
            btnAgregar.Click += (s, e) => {
                var form = new CatalogoEditForm(null);
                if (form.ShowDialog() == DialogResult.OK)
                    CargarProductos();
            };
            btnEditar.Click += (s, e) => {
                Producto? producto = null;
                if (dgvProductos.CurrentRow != null && dgvProductos.CurrentRow.DataBoundItem is Producto p)
                    producto = p;
                else if (dgvProductos.SelectedRows.Count > 0 && dgvProductos.SelectedRows[0].DataBoundItem is Producto p2)
                    producto = p2;
                if (producto != null)
                {
                    var form = new CatalogoEditForm(producto);
                    if (form.ShowDialog() == DialogResult.OK)
                        CargarProductos();
                }
                else
                {
                    MessageBox.Show("Selecciona un producto para editar.");
                }
            };
            panelBotones.Controls.Add(btnAgregar, 0, 0);
            panelBotones.Controls.Add(btnEditar, 1, 0);

            // Ensamblar el layout
            mainPanel.Controls.Add(panelBusqueda, 0, 0);
            mainPanel.Controls.Add(dgvProductos, 0, 1);
            mainPanel.Controls.Add(panelBotones, 0, 2);
            this.Controls.Clear();
            this.Controls.Add(mainPanel);

            this.Padding = new Padding(0, 0, 0, 10);
            this.PerformLayout();

            bindingSource = new BindingSource();
            dgvProductos.DataSource = bindingSource;

            txtBuscar.TextChanged += (s, e) => Filtrar();

            CargarProductos();
        }

        private void Filtrar()
        {
            if (productosOriginal == null) return;
            var filtro = txtBuscar.Text.ToLower();
            if (string.IsNullOrWhiteSpace(filtro))
            {
                bindingSource.DataSource = productosOriginal;
            }
            else
            {
                var filtrados = productosOriginal.FindAll(p =>
                    p.Nombre_Producto.ToLower().Contains(filtro) ||
                    p.Marca.ToLower().Contains(filtro) ||
                    p.Modelo.ToLower().Contains(filtro)
                );
                bindingSource.DataSource = filtrados;
            }
        }

        private void CargarProductos()
        {
            productosOriginal = new List<Producto>();
            string connectionString = ConfigHelper.GetConnectionString();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Productos";
                using (var cmd = new SqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        productosOriginal.Add(new Producto
                        {
                            ID_Producto = reader.GetInt32(reader.GetOrdinal("ID_Producto")),
                            Nombre_Producto = reader["Nombre_Producto"]?.ToString() ?? string.Empty,
                            Talla = reader["Talla"]?.ToString() ?? string.Empty,
                            Modelo = reader["Modelo"]?.ToString() ?? string.Empty,
                            Marca = reader["Marca"]?.ToString() ?? string.Empty,
                            Color = reader["Color"]?.ToString() ?? string.Empty,
                            Precio_Unitario = reader.GetDecimal(reader.GetOrdinal("Precio_Unitario")),
                            Material = reader["Material"]?.ToString() ?? string.Empty,
                            Stock = reader.GetInt32(reader.GetOrdinal("Stock")),
                        });
                    }
                }
            }
            bindingSource.DataSource = productosOriginal;
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