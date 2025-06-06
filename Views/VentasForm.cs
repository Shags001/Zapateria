using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ZapateriaWinForms.Utilities;
using Microsoft.Data.SqlClient;

namespace ZapateriaWinForms.Views
{
    public partial class VentasForm : Form
    {
        private DataGridView dgvVentas;
        private TextBox txtBuscar;
        private BindingSource bindingSource;
        private List<dynamic> ventasOriginal = new List<dynamic>();

        public VentasForm(string rol = "")
        {
            this.Text = "Ventas";
            this.Width = 850;
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
                Height = 60,
                BackColor = System.Drawing.Color.White,
                Padding = new Padding(10, 10, 10, 10),
                AutoSize = true
            };
            txtBuscar = new TextBox { PlaceholderText = "Buscar por método de pago o empleado...", Dock = DockStyle.Fill, Font = new System.Drawing.Font("Segoe UI", 10) };
            txtBuscar.TextChanged += (s, e) => Filtrar();
            panelBusqueda.Controls.Add(txtBuscar, 0, 0);

            // DataGridView responsivo y estilizado
            dgvVentas = new DataGridView
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 20, 0, 0),
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
            dgvVentas.Dock = DockStyle.Fill;
            dgvVentas.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvVentas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvVentas.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvVentas.MinimumSize = new System.Drawing.Size(400, 200);
            dgvVentas.MaximumSize = new System.Drawing.Size(2000, 2000);
            dgvVentas.Margin = new Padding(0);

            // Definir columnas manualmente
            var colFolio = new DataGridViewTextBoxColumn { DataPropertyName = "ID_Venta", HeaderText = "Folio" };
            var colFecha = new DataGridViewTextBoxColumn { DataPropertyName = "Hora_Fecha", HeaderText = "Fecha/Hora" };
            var colTotal = new DataGridViewTextBoxColumn { DataPropertyName = "Total_Precio", HeaderText = "Total" };
            var colIDEmpleado = new DataGridViewTextBoxColumn { DataPropertyName = "ID_Empleado", HeaderText = "ID Empleado" };
            var colEmpleado = new DataGridViewTextBoxColumn { DataPropertyName = "Nombre_Empleado", HeaderText = "Empleado" };
            var colMetodo = new DataGridViewTextBoxColumn { DataPropertyName = "Metodo_Pago", HeaderText = "Método de Pago" };
            dgvVentas.Columns.Clear();
            dgvVentas.Columns.AddRange(new DataGridViewColumn[] { colFolio, colFecha, colTotal, colIDEmpleado, colEmpleado, colMetodo });

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
                var form = new VentaEditForm(null);
                if (form.ShowDialog() == DialogResult.OK)
                    CargarVentas();
            };
            btnEditar.Click += (s, e) => {
                dynamic? venta = null;
                if (dgvVentas.CurrentRow != null && dgvVentas.CurrentRow.DataBoundItem != null)
                    venta = dgvVentas.CurrentRow.DataBoundItem;
                else if (dgvVentas.SelectedRows.Count > 0 && dgvVentas.SelectedRows[0].DataBoundItem != null)
                    venta = dgvVentas.SelectedRows[0].DataBoundItem;
                if (venta != null)
                {
                    var form = new VentaEditForm(venta);
                    if (form.ShowDialog() == DialogResult.OK)
                        CargarVentas();
                }
                else
                {
                    MessageBox.Show("Selecciona una venta para editar.");
                }
            };
            panelBotones.Controls.Add(btnAgregar, 0, 0);
            panelBotones.Controls.Add(btnEditar, 1, 0);

            // Ensamblar el layout
            mainPanel.Controls.Add(panelBusqueda, 0, 0);
            mainPanel.Controls.Add(dgvVentas, 0, 1);
            mainPanel.Controls.Add(panelBotones, 0, 2);
            this.Controls.Clear();
            this.Controls.Add(mainPanel);

            bindingSource = new BindingSource();
            dgvVentas.DataSource = bindingSource;

            CargarVentas();
        }

        private void VentasForm_Load(object sender, EventArgs e)
        {
            CargarVentas();
        }

        private void Filtrar()
        {
            if (ventasOriginal == null) return;
            var filtro = txtBuscar.Text.ToLower();
            if (string.IsNullOrWhiteSpace(filtro))
            {
                bindingSource.DataSource = ventasOriginal;
            }
            else
            {
                var filtrados = ventasOriginal.FindAll(v =>
                    v.Metodo_Pago.ToString().ToLower().Contains(filtro) ||
                    v.Nombre_Empleado.ToString().ToLower().Contains(filtro)
                );
                bindingSource.DataSource = filtrados;
            }
        }

        private void CargarVentas()
        {
            ventasOriginal = new List<dynamic>();
            string connectionString = ConfigHelper.GetConnectionString();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"SELECT v.ID_Venta, v.Hora_Fecha, v.Total_Precio, v.ID_Empleado, v.Metodo_Pago, e.Nombre_Empleado AS Nombre_Empleado FROM Ventas v INNER JOIN Empleados e ON v.ID_Empleado = e.ID_Empleado";
                using (var cmd = new SqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string fechaFormateada = "";
                        var valorFecha = reader["Hora_Fecha"];
                        if (valorFecha is DateTime dt)
                            fechaFormateada = dt.ToString("dd/MM/yyyy h:mm tt").Replace("AM", "a.m.").Replace("PM", "p.m.");
                        else if (DateTime.TryParse(valorFecha?.ToString(), out var dt2))
                            fechaFormateada = dt2.ToString("dd/MM/yyyy h:mm tt").Replace("AM", "a.m.").Replace("PM", "p.m.");
                        else
                            fechaFormateada = valorFecha?.ToString() ?? "";

                        ventasOriginal.Add(new {
                            ID_Venta = reader["ID_Venta"],
                            Hora_Fecha = fechaFormateada,
                            Total_Precio = reader["Total_Precio"],
                            ID_Empleado = reader["ID_Empleado"],
                            Nombre_Empleado = reader["Nombre_Empleado"],
                            Metodo_Pago = reader["Metodo_Pago"]
                        });
                    }
                }
            }
            bindingSource.DataSource = ventasOriginal;
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