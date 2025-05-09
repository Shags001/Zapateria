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

        public CatalogoForm()
        {
            this.Text = "Catálogo de Productos";
            this.Width = 1200;
            this.Height = 500;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Font = new System.Drawing.Font("Segoe UI", 10);

            var panelBusqueda = new TableLayoutPanel {
                RowCount = 1,
                ColumnCount = 1,
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = System.Drawing.Color.White,
                Padding = new Padding(10, 10, 10, 10),
                AutoSize = true
            };

            txtBuscar = new TextBox
            {
                Dock = DockStyle.Fill,
                PlaceholderText = "Buscar..."
            };

            panelBusqueda.Controls.Add(txtBuscar, 0, 0);

            dgvProductos = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                BackgroundColor = System.Drawing.Color.White,
                BorderStyle = BorderStyle.None,
                ColumnHeadersDefaultCellStyle = { BackColor = System.Drawing.Color.FromArgb(44, 62, 80), ForeColor = System.Drawing.Color.White, Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold) },
                DefaultCellStyle = { BackColor = System.Drawing.Color.White, ForeColor = System.Drawing.Color.Black, SelectionBackColor = System.Drawing.Color.FromArgb(189, 195, 199), SelectionForeColor = System.Drawing.Color.Black },
                RowHeadersVisible = false,
                EnableHeadersVisualStyles = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersVisible = true
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

            panelBusqueda.Dock = DockStyle.Top;
            dgvProductos.Dock = DockStyle.Fill;

            this.Controls.Clear();
            this.Controls.Add(dgvProductos);
            this.Controls.Add(panelBusqueda);

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
    }
}