using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace ZapateriaWinForms.Views
{
    internal class login : Form
    {
        private Label lblUsuario;
        private TextBox txtUsuario;
        private Label lblContrasena;
        private TextBox txtContrasena;
        private Button btnLogin;
        private Label lblMensaje;

        private string connectionString = @"Server=TU_SERVIDOR;Database=ZapateriaBD;Trusted_Connection=True;";

        public login()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Login - Zapatería";
            this.Size = new Size(350, 250);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            lblUsuario = new Label()
            {
                Text = "Usuario (Correo):",
                Location = new Point(20, 30),
                AutoSize = true
            };
            this.Controls.Add(lblUsuario);

            txtUsuario = new TextBox()
            {
                Location = new Point(130, 25),
                Width = 170
            };
            this.Controls.Add(txtUsuario);

            lblContrasena = new Label()
            {
                Text = "Contraseña:",
                Location = new Point(20, 80),
                AutoSize = true
            };
            this.Controls.Add(lblContrasena);

            txtContrasena = new TextBox()
            {
                Location = new Point(130, 75),
                Width = 170,
                UseSystemPasswordChar = true
            };
            this.Controls.Add(txtContrasena);

            btnLogin = new Button()
            {
                Text = "Ingresar",
                Location = new Point(130, 120),
                Width = 100
            };
            btnLogin.Click += BtnLogin_Click;
            this.Controls.Add(btnLogin);

            lblMensaje = new Label()
            {
                Location = new Point(20, 160),
                Width = 300,
                ForeColor = Color.Red,
                AutoSize = false
            };
            this.Controls.Add(lblMensaje);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string contrasena = txtContrasena.Text;

            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(contrasena))
            {
                lblMensaje.Text = "Debe ingresar usuario y contraseña.";
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT Posicion 
                        FROM Empleados 
                        WHERE Correo = @correo AND Contrasena = @contrasena";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@correo", usuario);
                        cmd.Parameters.AddWithValue("@contrasena", contrasena);

                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            string rol = result.ToString();

                            lblMensaje.ForeColor = Color.Green;
                            lblMensaje.Text = $"Bienvenido {rol}!";

                            // Aquí puedes abrir otros formularios según rol
                        }
                        else
                        {
                            lblMensaje.ForeColor = Color.Red;
                            lblMensaje.Text = "Usuario o contraseña incorrectos.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblMensaje.ForeColor = Color.Red;
                lblMensaje.Text = "Error al conectar con la base de datos.";
                Console.WriteLine(ex.Message);
            }
        }
    }
}
