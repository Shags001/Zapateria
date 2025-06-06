using System;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using ZapateriaWinForms.Utilities;

namespace ZapateriaWinForms.Views
{
    public class VentaEditForm : Form
    {
        private TextBox txtTotal;
        private ComboBox cmbMetodo;
        private DateTimePicker dtpFecha;
        private TextBox txtIDEmpleado;
        private Button btnGuardar;
        private dynamic? ventaEdicion;

        public VentaEditForm(dynamic? venta)
        {
            ventaEdicion = venta;
            this.Text = venta == null ? "Agregar Venta" : "Editar Venta";
            this.Width = 350;
            this.Height = 320;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = System.Drawing.Color.White;
            this.Font = new System.Drawing.Font("Segoe UI", 10);

            Label lblTotal = new Label { Text = "Total:", Top = 30, Left = 20, Width = 80 };
            txtTotal = new TextBox { Top = 30, Left = 110, Width = 180 };
            Label lblMetodo = new Label { Text = "Método de Pago:", Top = 70, Left = 20, Width = 80 };
            cmbMetodo = new ComboBox { Top = 70, Left = 110, Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbMetodo.Items.AddRange(new string[] { "Efectivo", "Tarjeta", "Transferencia" });
            Label lblFecha = new Label { Text = "Fecha/Hora:", Top = 110, Left = 20, Width = 80 };
            dtpFecha = new DateTimePicker { Top = 110, Left = 110, Width = 180, Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy HH:mm" };
            Label lblIDEmpleado = new Label { Text = "ID Empleado:", Top = 150, Left = 20, Width = 80 };
            txtIDEmpleado = new TextBox { Top = 150, Left = 110, Width = 180 };
            btnGuardar = new Button { Text = "Guardar", Top = 200, Left = 110, Width = 100 };
            btnGuardar.Click += BtnGuardar_Click;

            this.Controls.Add(lblTotal);
            this.Controls.Add(txtTotal);
            this.Controls.Add(lblMetodo);
            this.Controls.Add(cmbMetodo);
            this.Controls.Add(lblFecha);
            this.Controls.Add(dtpFecha);
            this.Controls.Add(lblIDEmpleado);
            this.Controls.Add(txtIDEmpleado);
            this.Controls.Add(btnGuardar);

            if (venta != null)
            {
                txtTotal.Text = venta.Total_Precio.ToString();
                cmbMetodo.SelectedItem = venta.Metodo_Pago.ToString();
                DateTime fecha;
                if (DateTime.TryParse(venta.Hora_Fecha.ToString(), out fecha))
                    dtpFecha.Value = fecha;
                else
                    dtpFecha.Value = DateTime.Now;
                txtIDEmpleado.Text = venta.ID_Empleado.ToString();
            }
        }

        private void BtnGuardar_Click(object? sender, EventArgs e)
        {
            if (!decimal.TryParse(txtTotal.Text, out decimal total) || string.IsNullOrEmpty(cmbMetodo.Text) || string.IsNullOrEmpty(txtIDEmpleado.Text))
            {
                MessageBox.Show("Completa todos los campos correctamente.");
                return;
            }
            string connectionString = ConfigHelper.GetConnectionString();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                if (ventaEdicion == null)
                {
                    // Alta
                    string query = "INSERT INTO Ventas (Hora_Fecha, Total_Precio, ID_Empleado, Metodo_Pago) VALUES (@fecha, @total, @idempleado, @metodo)";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@fecha", dtpFecha.Value);
                        cmd.Parameters.AddWithValue("@total", total);
                        cmd.Parameters.AddWithValue("@idempleado", txtIDEmpleado.Text);
                        cmd.Parameters.AddWithValue("@metodo", cmbMetodo.Text);
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
                    string query = "UPDATE Ventas SET Hora_Fecha = @fecha, Total_Precio = @total, ID_Empleado = @idempleado, Metodo_Pago = @metodo WHERE ID_Venta = @id";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", ventaEdicion.ID_Venta);
                        cmd.Parameters.AddWithValue("@fecha", dtpFecha.Value);
                        cmd.Parameters.AddWithValue("@total", total);
                        cmd.Parameters.AddWithValue("@idempleado", txtIDEmpleado.Text);
                        cmd.Parameters.AddWithValue("@metodo", cmbMetodo.Text);
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
