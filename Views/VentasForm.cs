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

        public VentasForm()
        {
            this.Text = "Ventas";
            this.Width = 850;
            this.Height = 500;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Font = new System.Drawing.Font("Segoe UI", 10);

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

            // Definir columnas manualmente
            var colFolio = new DataGridViewTextBoxColumn { DataPropertyName = "ID_Venta", HeaderText = "Folio" };
            var colFecha = new DataGridViewTextBoxColumn { DataPropertyName = "Hora_Fecha", HeaderText = "Fecha/Hora" };
            var colTotal = new DataGridViewTextBoxColumn { DataPropertyName = "Total_Precio", HeaderText = "Total" };
            var colIDEmpleado = new DataGridViewTextBoxColumn { DataPropertyName = "ID_Empleado", HeaderText = "ID Empleado" };
            var colEmpleado = new DataGridViewTextBoxColumn { DataPropertyName = "Nombre_Empleado", HeaderText = "Empleado" };
            var colMetodo = new DataGridViewTextBoxColumn { DataPropertyName = "Metodo_Pago", HeaderText = "Método de Pago" };
            dgvVentas.Columns.Clear();
            dgvVentas.Columns.AddRange(new DataGridViewColumn[] { colFolio, colFecha, colTotal, colIDEmpleado, colEmpleado, colMetodo });

            this.Controls.Clear();
            this.Controls.Add(dgvVentas);
            this.Controls.Add(panelBusqueda);

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
    }
}