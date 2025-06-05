using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace ZapateriaWinForms.Views
{
    internal class login : Form
    {
        private Label lblTitulo;
        private Label lblUsuario;
        private TextBox txtUsuario;
        private Label lblContrasena;
        private TextBox txtContrasena;
        private Button btnLogin;
        private Label lblMensaje;
        private Panel panelContainer;
        private string connectionString = @"Server=TU_SERVIDOR;Database=ZapateriaBD;Trusted_Connection=True;";

        public login()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Zapatería - Inicio de Sesión";
            this.Size = new Size(420, 380);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(244, 236, 223); // Beige claro (tono cálido)

            // Panel contenedor (centro visual)
            panelContainer = new Panel()
            {
                Size = new Size(360, 300),
                Location = new Point(30, 30),
                BackColor = Color.FromArgb(255, 248, 235),
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(panelContainer);

            // Título
            lblTitulo = new Label()
            {
                Text = "ZAPATERÍA",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.SaddleBrown,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(360, 40),
                Location = new Point(0, 10)
            };
            panelContainer.Controls.Add(lblTitulo);

            // Etiqueta usuario
            lblUsuario = new Label()
            {
                Text = "Usuario:",
                Location = new Point(30, 60),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(90, 45, 30)
            };
            panelContainer.Controls.Add(lblUsuario);

            // TextBox usuario
            txtUsuario = new TextBox()
            {
                Location = new Point(30, 85),
                Width = 300,
                Font = new Font("Segoe UI", 10),
                BackColor = Color.White
            };
            panelContainer.Controls.Add(txtUsuario);

            // Etiqueta contraseña
            lblContrasena = new Label()
            {
                Text = "Contraseña:",
                Location = new Point(30, 125),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(90, 45, 30)
            };
            panelContainer.Controls.Add(lblContrasena);

            // TextBox contraseña
            txtContrasena = new TextBox()
            {
                Location = new Point(30, 150),
                Width = 300,
                UseSystemPasswordChar = true,
                Font = new Font("Segoe UI", 10),
                BackColor = Color.White
            };
            panelContainer.Controls.Add(txtContrasena);

            // Botón ingresar
            btnLogin = new Button()
            {
                Text = "Iniciar Sesión",
                Location = new Point(105, 200),
                Width = 150,
                Height = 35,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.SaddleBrown,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += BtnLogin_Click;
            panelContainer.Controls.Add(btnLogin);

            // Mensaje
            lblMensaje = new Label()
            {
                Location = new Point(30, 250),
                Width = 300,
                ForeColor = Color.Red,
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                TextAlign = ContentAlignment.MiddleCenter
            };
            panelContainer.Controls.Add(lblMensaje);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string contrasena = txtContrasena.Text;

            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(contrasena))
            {
                lblMensaje.ForeColor = Color.Red;
                lblMensaje.Text = "Por favor, complete todos los campos.";
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

                            
                        }
                        else
                        {
                            lblMensaje.ForeColor = Color.Red;
                            lblMensaje.Text = "Correo o contraseña incorrectos.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblMensaje.ForeColor = Color.Red;
                lblMensaje.Text = "Error de conexión con la base de datos.";
                Console.WriteLine(ex.Message);
            }
        }
    }
}

