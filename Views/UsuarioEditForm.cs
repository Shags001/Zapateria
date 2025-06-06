using System;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using ZapateriaWinForms.Utilities;

namespace ZapateriaWinForms.Views
{
    public class UsuarioEditForm : Form
    {
        private TextBox txtUsuario;
        private TextBox txtContrasena;
        private ComboBox cmbRol;
        private Button btnGuardar;
        private UsuarioLogin? usuarioEdicion;

        public UsuarioEditForm(UsuarioLogin? usuario)
        {
            usuarioEdicion = usuario;
            this.Text = usuario == null ? "Agregar Usuario" : "Editar Usuario";
            this.Width = 350;
            this.Height = 300;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = System.Drawing.Color.White;
            this.Font = new System.Drawing.Font("Segoe UI", 10);

            Label lblUsuario = new Label { Text = "Usuario:", Top = 30, Left = 20, Width = 80 };
            txtUsuario = new TextBox { Top = 30, Left = 110, Width = 180 };
            Label lblContrasena = new Label { Text = "Contraseña:", Top = 70, Left = 20, Width = 80 };
            txtContrasena = new TextBox { Top = 70, Left = 110, Width = 180, UseSystemPasswordChar = true };
            Label lblRol = new Label { Text = "Rol:", Top = 110, Left = 20, Width = 80 };
            cmbRol = new ComboBox { Top = 110, Left = 110, Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbRol.Items.AddRange(new string[] { "Administrador", "Empleado" });

            btnGuardar = new Button { Text = "Guardar", Top = 170, Left = 110, Width = 100 };
            btnGuardar.Click += BtnGuardar_Click;

            this.Controls.Add(lblUsuario);
            this.Controls.Add(txtUsuario);
            this.Controls.Add(lblContrasena);
            this.Controls.Add(txtContrasena);
            this.Controls.Add(lblRol);
            this.Controls.Add(cmbRol);
            this.Controls.Add(btnGuardar);

            if (usuario != null)
            {
                txtUsuario.Text = usuario.Nombre_Usuario;
                txtUsuario.Enabled = false;
                cmbRol.SelectedItem = usuario.Rol;
            }
        }

        private void BtnGuardar_Click(object? sender, EventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string contrasena = txtContrasena.Text.Trim();
            string rol = cmbRol.SelectedItem?.ToString() ?? "";
            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(rol) || (usuarioEdicion == null && string.IsNullOrEmpty(contrasena)))
            {
                MessageBox.Show("Completa todos los campos.");
                return;
            }
            string connectionString = ConfigHelper.GetConnectionString();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                if (usuarioEdicion == null)
                {
                    // Alta
                    string query = "INSERT INTO UsuariosLogin (Nombre_Usuario, Contrasena, Rol) VALUES (@usuario, @contrasena, @rol)";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@usuario", usuario);
                        cmd.Parameters.AddWithValue("@contrasena", contrasena);
                        cmd.Parameters.AddWithValue("@rol", rol);
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
                    string query = string.IsNullOrEmpty(contrasena)
                        ? "UPDATE UsuariosLogin SET Rol = @rol WHERE ID_Usuario = @id"
                        : "UPDATE UsuariosLogin SET Contrasena = @contrasena, Rol = @rol WHERE ID_Usuario = @id";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", usuarioEdicion.ID_Usuario);
                        if (!string.IsNullOrEmpty(contrasena))
                            cmd.Parameters.AddWithValue("@contrasena", contrasena);
                        cmd.Parameters.AddWithValue("@rol", rol);
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
