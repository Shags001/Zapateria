using System;
using System.Windows.Forms;
using ZapateriaWinForms.Models;
using ZapateriaWinForms.Utilities;
using Microsoft.Data.SqlClient;

namespace ZapateriaWinForms.Views
{
    public class ProductoEditForm : Form
    {
        private TextBox txtNombre;
        private TextBox txtTalla;
        private TextBox txtModelo;
        private TextBox txtMarca;
        private TextBox txtColor;
        private TextBox txtPrecio;
        private Button btnGuardar;
        private ComboBox cmbCategoria;
        private ComboBox cmbProveedor;
        private Producto? productoEdicion;
        private TextBox txtStock;

        public ProductoEditForm(Producto? producto)
        {
            productoEdicion = producto;
            this.Text = producto == null ? "Agregar Producto" : "Editar Producto";
            this.Width = 400;
            this.Height = 420;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = System.Drawing.Color.White;
            this.Font = new System.Drawing.Font("Segoe UI", 10);

            var panel = new TableLayoutPanel {
                RowCount = 10,
                ColumnCount = 2,
                Dock = DockStyle.Fill,
                Padding = new Padding(15, 15, 15, 15),
                AutoSize = true
            };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            Label lblNombre = new Label { Text = "Nombre:", Anchor = AnchorStyles.Right, TextAlign = System.Drawing.ContentAlignment.MiddleRight };
            txtNombre = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            Label lblTalla = new Label { Text = "Talla:", Anchor = AnchorStyles.Right, TextAlign = System.Drawing.ContentAlignment.MiddleRight };
            txtTalla = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            Label lblModelo = new Label { Text = "Modelo:", Anchor = AnchorStyles.Right, TextAlign = System.Drawing.ContentAlignment.MiddleRight };
            txtModelo = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            Label lblMarca = new Label { Text = "Marca:", Anchor = AnchorStyles.Right, TextAlign = System.Drawing.ContentAlignment.MiddleRight };
            txtMarca = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            Label lblColor = new Label { Text = "Color:", Anchor = AnchorStyles.Right, TextAlign = System.Drawing.ContentAlignment.MiddleRight };
            txtColor = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            Label lblPrecio = new Label { Text = "Precio:", Anchor = AnchorStyles.Right, TextAlign = System.Drawing.ContentAlignment.MiddleRight };
            txtPrecio = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            Label lblCategoria = new Label { Text = "Categoría:", Anchor = AnchorStyles.Right, TextAlign = System.Drawing.ContentAlignment.MiddleRight };
            cmbCategoria = new ComboBox { Anchor = AnchorStyles.Left | AnchorStyles.Right, DropDownStyle = ComboBoxStyle.DropDownList };
            Label lblProveedor = new Label { Text = "Proveedor:", Anchor = AnchorStyles.Right, TextAlign = System.Drawing.ContentAlignment.MiddleRight };
            cmbProveedor = new ComboBox { Anchor = AnchorStyles.Left | AnchorStyles.Right, DropDownStyle = ComboBoxStyle.DropDownList };
            Label lblStock = new Label { Text = "Stock:", Anchor = AnchorStyles.Right, TextAlign = System.Drawing.ContentAlignment.MiddleRight };
            txtStock = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right };

            btnGuardar = new Button { Text = "Guardar", Anchor = AnchorStyles.None, Width = 120, Height = 32 };
            btnGuardar.Click += BtnGuardar_Click;

            panel.Controls.Add(lblNombre, 0, 0); panel.Controls.Add(txtNombre, 1, 0);
            panel.Controls.Add(lblTalla, 0, 1); panel.Controls.Add(txtTalla, 1, 1);
            panel.Controls.Add(lblModelo, 0, 2); panel.Controls.Add(txtModelo, 1, 2);
            panel.Controls.Add(lblMarca, 0, 3); panel.Controls.Add(txtMarca, 1, 3);
            panel.Controls.Add(lblColor, 0, 4); panel.Controls.Add(txtColor, 1, 4);
            panel.Controls.Add(lblPrecio, 0, 5); panel.Controls.Add(txtPrecio, 1, 5);
            panel.Controls.Add(lblCategoria, 0, 6); panel.Controls.Add(cmbCategoria, 1, 6);
            panel.Controls.Add(lblProveedor, 0, 7); panel.Controls.Add(cmbProveedor, 1, 7);
            panel.Controls.Add(lblStock, 0, 8); panel.Controls.Add(txtStock, 1, 8);
            panel.Controls.Add(btnGuardar, 0, 9); panel.SetColumnSpan(btnGuardar, 2);

            this.Controls.Clear();
            this.Controls.Add(panel);

            if (producto != null)
            {
                txtNombre.Text = producto.Nombre_Producto;
                txtTalla.Text = producto.Talla;
                txtModelo.Text = producto.Modelo;
                txtMarca.Text = producto.Marca;
                txtColor.Text = producto.Color;
                txtPrecio.Text = producto.Precio_Unitario.ToString();
                txtStock.Text = producto.Stock.ToString();
                if (producto.ID_Categoria > 0)
                    cmbCategoria.SelectedValue = producto.ID_Categoria;
                if (producto.ID_Proveedor > 0)
                    cmbProveedor.SelectedValue = producto.ID_Proveedor;
            }

            CargarCategorias();
            CargarProveedores();
        }

        private void CargarCategorias()
        {
            string connectionString = ConfigHelper.GetConnectionString();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT ID_Categoria, Nombre_Categoria FROM Categoria";
                using (var cmd = new SqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    var dt = new System.Data.DataTable();
                    dt.Load(reader);
                    cmbCategoria.DataSource = dt;
                    cmbCategoria.DisplayMember = "Nombre_Categoria";
                    cmbCategoria.ValueMember = "ID_Categoria";
                }
            }
        }

        private void CargarProveedores()
        {
            string connectionString = ConfigHelper.GetConnectionString();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT ID_Proveedor, Nombre_Proveedor FROM Proveedores";
                using (var cmd = new SqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    var dt = new System.Data.DataTable();
                    dt.Load(reader);
                    cmbProveedor.DataSource = dt;
                    cmbProveedor.DisplayMember = "Nombre_Proveedor";
                    cmbProveedor.ValueMember = "ID_Proveedor";
                }
            }
        }

        private void BtnGuardar_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtPrecio.Text))
            {
                MessageBox.Show("Completa todos los campos obligatorios.");
                return;
            }
            if (!decimal.TryParse(txtPrecio.Text, out decimal precio))
            {
                MessageBox.Show("Precio inválido.");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtStock.Text) || !int.TryParse(txtStock.Text, out int stock) || stock < 0)
            {
                MessageBox.Show("Stock inválido.");
                return;
            }
            string connectionString = ConfigHelper.GetConnectionString();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                if (productoEdicion == null)
                {
                    // Alta
                    string query = "INSERT INTO Productos (Nombre_Producto, Talla, Modelo, Marca, Color, Precio_Unitario, ID_Categoria, ID_Proveedor, Stock) VALUES (@nombre, @talla, @modelo, @marca, @color, @precio, @categoria, @proveedor, @stock)";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@nombre", txtNombre.Text);
                        cmd.Parameters.AddWithValue("@talla", txtTalla.Text);
                        cmd.Parameters.AddWithValue("@modelo", txtModelo.Text);
                        cmd.Parameters.AddWithValue("@marca", txtMarca.Text);
                        cmd.Parameters.AddWithValue("@color", txtColor.Text);
                        cmd.Parameters.AddWithValue("@precio", precio);
                        cmd.Parameters.AddWithValue("@categoria", cmbCategoria.SelectedValue ?? 1);
                        cmd.Parameters.AddWithValue("@proveedor", cmbProveedor.SelectedValue ?? 1);
                        cmd.Parameters.AddWithValue("@stock", stock);
                        try
                        {
                            cmd.ExecuteNonQuery();
                            this.DialogResult = DialogResult.OK;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error al guardar: " + ex.Message);
                        }
                    }
                }
                else
                {
                    // Modificación
                    string query = "UPDATE Productos SET Nombre_Producto = @nombre, Talla = @talla, Modelo = @modelo, Marca = @marca, Color = @color, Precio_Unitario = @precio, ID_Categoria = @categoria, ID_Proveedor = @proveedor, Stock = @stock WHERE ID_Producto = @id";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", productoEdicion.ID_Producto);
                        cmd.Parameters.AddWithValue("@nombre", txtNombre.Text);
                        cmd.Parameters.AddWithValue("@talla", txtTalla.Text);
                        cmd.Parameters.AddWithValue("@modelo", txtModelo.Text);
                        cmd.Parameters.AddWithValue("@marca", txtMarca.Text);
                        cmd.Parameters.AddWithValue("@color", txtColor.Text);
                        cmd.Parameters.AddWithValue("@precio", precio);
                        cmd.Parameters.AddWithValue("@categoria", cmbCategoria.SelectedValue ?? 1);
                        cmd.Parameters.AddWithValue("@proveedor", cmbProveedor.SelectedValue ?? 1);
                        cmd.Parameters.AddWithValue("@stock", stock);
                        try
                        {
                            cmd.ExecuteNonQuery();
                            this.DialogResult = DialogResult.OK;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error al actualizar: " + ex.Message);
                        }
                    }
                }
            }
        }
    }
}
