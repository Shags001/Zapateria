using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;
using ZapateriaWinForms.Utilities;

namespace ZapateriaWinForms.Views
{
    public class UsuariosForm : Form
    {
        private DataGridView dgvUsuarios;
        private BindingSource bindingSource;
        private Button btnAgregar;
        private Button btnEditar;
        private List<UsuarioLogin> usuarios = new List<UsuarioLogin>();

        public UsuariosForm()
        {
            this.Text = "Gestión de Usuarios";
            this.Width = 700;
            this.Height = 400;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Font = new System.Drawing.Font("Segoe UI", 10);

            dgvUsuarios = new DataGridView
            {
                Dock = DockStyle.Top,
                Height = 250,
                ReadOnly = true,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ID_Usuario", HeaderText = "ID" });
            dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Nombre_Usuario", HeaderText = "Usuario" });
            dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Rol", HeaderText = "Rol" });
            dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Ultimo_Inicio_Sesion", HeaderText = "Último Inicio de Sesión" });

            bindingSource = new BindingSource();
            dgvUsuarios.DataSource = bindingSource;

            btnAgregar = new Button { Text = "Agregar Usuario", Width = 150, Top = 270, Left = 30 };
            btnEditar = new Button { Text = "Editar Usuario", Width = 150, Top = 270, Left = 200 };
            btnAgregar.Click += (s, e) => AbrirFormularioUsuario(null);
            btnEditar.Click += (s, e) =>
            {
                if (dgvUsuarios.SelectedRows.Count > 0)
                {
                    var usuario = dgvUsuarios.SelectedRows[0].DataBoundItem as UsuarioLogin;
                    AbrirFormularioUsuario(usuario);
                }
            };

            this.Controls.Add(dgvUsuarios);
            this.Controls.Add(btnAgregar);
            this.Controls.Add(btnEditar);

            CargarUsuarios();
        }

        private void CargarUsuarios()
        {
            usuarios.Clear();
            string connectionString = ConfigHelper.GetConnectionString();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT ID_Usuario, Nombre_Usuario, Rol, Ultimo_Inicio_Sesion FROM UsuariosLogin";
                using (var cmd = new SqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        usuarios.Add(new UsuarioLogin
                        {
                            ID_Usuario = reader.GetInt32(0),
                            Nombre_Usuario = reader.GetString(1),
                            Rol = reader.GetString(2),
                            Ultimo_Inicio_Sesion = reader.IsDBNull(3) ? null : reader.GetDateTime(3).ToString()
                        });
                    }
                }
            }
            bindingSource.DataSource = null;
            bindingSource.DataSource = usuarios;
        }

        private void AbrirFormularioUsuario(UsuarioLogin? usuario)
        {
            var form = new UsuarioEditForm(usuario);
            if (form.ShowDialog() == DialogResult.OK)
            {
                CargarUsuarios();
            }
        }
    }

    public class UsuarioLogin
    {
        public int ID_Usuario { get; set; }
        public string Nombre_Usuario { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public string? Ultimo_Inicio_Sesion { get; set; }
    }
}
