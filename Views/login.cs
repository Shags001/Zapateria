using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace ZapateriaWinForms.Views
{
    internal class login : Form
    {
        private Label lblTitulo;
        private Label lblSubtitulo;
        private Label lblUsuario;
        private TextBox txtUsuario;
        private Label lblContrasena;
        private TextBox txtContrasena;
        private Button btnLogin;
        private Button btnMostrarPassword;
        private Label lblMensaje;
        private Panel panelMain;
        private Panel panelLeft;
        private Panel panelRight;
        private PictureBox picLogo;
        private string connectionString = @"Server=TU_SERVIDOR;Database=ZapateriaBD;Trusted_Connection=True;";
        private bool passwordVisible = false;

        public login()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Zapatería - Sistema de Gestión";
            this.Size = new Size(900, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(240, 244, 248);
            this.SetStyle(ControlStyles.ResizeRedraw, true);

            // Panel principal con esquinas redondeadas
            panelMain = new RoundedPanel()
            {
                Size = new Size(850, 500),
                Location = new Point(25, 25),
                BackColor = Color.White,
                BorderRadius = 15
            };
            this.Controls.Add(panelMain);

            // Panel izquierdo (imagen/branding)
            panelLeft = new Panel()
            {
                Size = new Size(400, 500),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(45, 55, 72),
                Dock = DockStyle.None
            };
            panelMain.Controls.Add(panelLeft);

            // Gradiente para panel izquierdo
            panelLeft.Paint += (s, e) =>
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    panelLeft.ClientRectangle,
                    Color.FromArgb(45, 55, 72),
                    Color.FromArgb(74, 85, 104),
                    LinearGradientMode.Vertical))
                {
                    e.Graphics.FillRectangle(brush, panelLeft.ClientRectangle);
                }
            };

            // Logo/Icono
            picLogo = new PictureBox()
            {
                Size = new Size(120, 120),
                Location = new Point(140, 100),
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.CenterImage
            };
            // Crear un icono simple de zapato
            CreateShoeIcon();
            panelLeft.Controls.Add(picLogo);

            // Título en panel izquierdo
            Label lblBrand = new Label()
            {
                Text = "ZAPATERÍA\nPRO",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(400, 100),
                Location = new Point(0, 230)
            };
            panelLeft.Controls.Add(lblBrand);

            Label lblSlogan = new Label()
            {
                Text = "Sistema de Gestión Integral",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(203, 213, 224),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(400, 30),
                Location = new Point(0, 330)
            };
            panelLeft.Controls.Add(lblSlogan);

            // Panel derecho (formulario)
            panelRight = new Panel()
            {
                Size = new Size(450, 500),
                Location = new Point(400, 0),
                BackColor = Color.White
            };
            panelMain.Controls.Add(panelRight);

            // Botón cerrar
            Button btnClose = new Button()
            {
                Text = "✕",
                Size = new Size(30, 30),
                Location = new Point(410, 10),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(107, 114, 128),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(254, 226, 226);
            btnClose.Click += (s, e) => this.Close();
            panelRight.Controls.Add(btnClose);

            // Título del formulario
            lblTitulo = new Label();
            lblTitulo.Text = "Iniciar Sesión";
            lblTitulo.Font = new Font("Segoe UI", 28, FontStyle.Bold);
            lblTitulo.ForeColor = Color.FromArgb(17, 24, 39);
            lblTitulo.AutoSize = false;
            lblTitulo.Size = new Size(350, 50);
            lblTitulo.Location = new Point(50, 80);
            lblTitulo.TextAlign = ContentAlignment.MiddleLeft;
            panelRight.Controls.Add(lblTitulo);

            lblSubtitulo = new Label();
            lblSubtitulo.Text = "Accede a tu cuenta para continuar";
            lblSubtitulo.Font = new Font("Segoe UI", 11);
            lblSubtitulo.ForeColor = Color.FromArgb(107, 114, 128);
            lblSubtitulo.AutoSize = false;
            lblSubtitulo.Size = new Size(350, 25);
            lblSubtitulo.Location = new Point(50, 130);
            panelRight.Controls.Add(lblSubtitulo);

            // Campo Email
            lblUsuario = new Label();
            lblUsuario.Text = "Correo electrónico";
            lblUsuario.Location = new Point(50, 180);
            lblUsuario.AutoSize = true;
            lblUsuario.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            lblUsuario.ForeColor = Color.FromArgb(55, 65, 81);
            panelRight.Controls.Add(lblUsuario);

            txtUsuario = new RoundedTextBox();
            txtUsuario.Location = new Point(50, 205);
            txtUsuario.Width = 350;
            txtUsuario.Height = 45;
            txtUsuario.Font = new Font("Segoe UI", 11);
            txtUsuario.BackColor = Color.FromArgb(249, 250, 251);
            txtUsuario.ForeColor = Color.FromArgb(17, 24, 39);
            txtUsuario.BorderStyle = BorderStyle.None;
            txtUsuario.PlaceholderText = "nombre@ejemplo.com";
            panelRight.Controls.Add(txtUsuario);

            // Campo Contraseña
            lblContrasena = new Label();
            lblContrasena.Text = "Contraseña";
            lblContrasena.Location = new Point(50, 270);
            lblContrasena.AutoSize = true;
            lblContrasena.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            lblContrasena.ForeColor = Color.FromArgb(55, 65, 81);
            panelRight.Controls.Add(lblContrasena);

            // Panel para contraseña con botón mostrar/ocultar
            Panel panelPassword = new Panel();
            panelPassword.Location = new Point(50, 295);
            panelPassword.Size = new Size(350, 45);
            panelPassword.BackColor = Color.FromArgb(249, 250, 251);
            panelPassword.Paint += (s, e) =>
            {
                using (Pen pen = new Pen(Color.FromArgb(209, 213, 219), 1))
                {
                    Rectangle rect = new Rectangle(0, 0, panelPassword.Width - 1, panelPassword.Height - 1);
                    GraphicsPath path = GetRoundedRectangle(rect, 8);
                    e.Graphics.DrawPath(pen, path);
                }
            };
            panelRight.Controls.Add(panelPassword);

            txtContrasena = new TextBox();
            txtContrasena.Location = new Point(15, 12);
            txtContrasena.Width = 300;
            txtContrasena.Height = 21;
            txtContrasena.UseSystemPasswordChar = true;
            txtContrasena.Font = new Font("Segoe UI", 11);
            txtContrasena.BackColor = Color.FromArgb(249, 250, 251);
            txtContrasena.ForeColor = Color.FromArgb(17, 24, 39);
            txtContrasena.BorderStyle = BorderStyle.None;
            panelPassword.Controls.Add(txtContrasena);

            btnMostrarPassword = new Button();
            btnMostrarPassword.Text = "👁";
            btnMostrarPassword.Size = new Size(30, 21);
            btnMostrarPassword.Location = new Point(315, 12);
            btnMostrarPassword.FlatStyle = FlatStyle.Flat;
            btnMostrarPassword.BackColor = Color.Transparent;
            btnMostrarPassword.ForeColor = Color.FromArgb(107, 114, 128);
            btnMostrarPassword.Font = new Font("Segoe UI", 10);
            btnMostrarPassword.Cursor = Cursors.Hand;
            btnMostrarPassword.FlatAppearance.BorderSize = 0;
            btnMostrarPassword.Click += BtnMostrarPassword_Click;
            panelPassword.Controls.Add(btnMostrarPassword);

            // Botón de inicio de sesión
            btnLogin = new RoundedButton();
            btnLogin.Text = "Iniciar Sesión";
            btnLogin.Location = new Point(50, 370);
            btnLogin.Width = 350;
            btnLogin.Height = 50;
            btnLogin.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnLogin.BackColor = Color.FromArgb(59, 130, 246);
            btnLogin.ForeColor = Color.White;
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.Cursor = Cursors.Hand;
            ((RoundedButton)btnLogin).BorderRadius = 10;
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.FlatAppearance.MouseOverBackColor = Color.FromArgb(37, 99, 235);
            btnLogin.Click += BtnLogin_Click;
            panelRight.Controls.Add(btnLogin);

            // Mensaje de estado
            lblMensaje = new Label();
            lblMensaje.Location = new Point(50, 440);
            lblMensaje.Width = 350;
            lblMensaje.Height = 30;
            lblMensaje.ForeColor = Color.Red;
            lblMensaje.Font = new Font("Segoe UI", 10);
            lblMensaje.TextAlign = ContentAlignment.MiddleCenter;
            lblMensaje.BackColor = Color.Transparent;
            lblMensaje.Text = "";
            panelRight.Controls.Add(lblMensaje);

            // Animación de entrada
            this.Opacity = 0;
            Timer fadeInTimer = new Timer();
            fadeInTimer.Interval = 20;
            fadeInTimer.Tick += (s, e) =>
            {
                if (this.Opacity < 1)
                    this.Opacity += 0.05;
                else
                    fadeInTimer.Stop();
            };
            fadeInTimer.Start();
        }

        private void CreateShoeIcon()
        {
            Bitmap icon = new Bitmap(120, 120);
            using (Graphics g = Graphics.FromImage(icon))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                // Dibujar icono simple de zapato
                using (SolidBrush brush = new SolidBrush(Color.White))
                {
                    // Cuerpo del zapato
                    Rectangle shoe = new Rectangle(20, 50, 80, 30);
                    g.FillEllipse(brush, shoe);

                    // Suela
                    Rectangle sole = new Rectangle(15, 75, 90, 15);
                    g.FillEllipse(brush, sole);

                    // Detalle
                    using (Pen pen = new Pen(Color.FromArgb(203, 213, 224), 2))
                    {
                        g.DrawArc(pen, 30, 55, 20, 20, 0, 180);
                        g.DrawArc(pen, 55, 55, 20, 20, 0, 180);
                    }
                }
            }
            picLogo.Image = icon;
        }

        private void BtnMostrarPassword_Click(object sender, EventArgs e)
        {
            passwordVisible = !passwordVisible;
            txtContrasena.UseSystemPasswordChar = !passwordVisible;
            btnMostrarPassword.Text = passwordVisible ? "🙈" : "👁";
        }

        private GraphicsPath GetRoundedRectangle(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.X + rect.Width - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.X + rect.Width - radius, rect.Y + rect.Height - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Y + rect.Height - radius, radius, radius, 90, 90);
            path.CloseAllFigures();
            return path;
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string contrasena = txtContrasena.Text;

            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(contrasena))
            {
                ShowMessage("Por favor, complete todos los campos.", false);
                return;
            }

            // Animación de loading
            btnLogin.Text = "Verificando...";
            btnLogin.Enabled = false;

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
                            ShowMessage($"¡Bienvenido {rol}!", true);
                        }
                        else
                        {
                            ShowMessage("Correo o contraseña incorrectos.", false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error de conexión con la base de datos.", false);
                Console.WriteLine(ex.Message);
            }
            finally
            {
                btnLogin.Text = "Iniciar Sesión";
                btnLogin.Enabled = true;
            }
        }

        private void ShowMessage(string message, bool success)
        {
            lblMensaje.Text = message;
            lblMensaje.ForeColor = success ? Color.FromArgb(34, 197, 94) : Color.FromArgb(239, 68, 68);

            // Animación del mensaje
            lblMensaje.Visible = true;
            Timer timer = new Timer();
            timer.Interval = 3000;
            timer.Tick += (s, e) =>
            {
                lblMensaje.Text = "";
                timer.Stop();
            };
            timer.Start();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Sombra del formulario
            Rectangle shadowRect = new Rectangle(5, 5, this.Width - 10, this.Height - 10);
            using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
            {
                e.Graphics.FillRectangle(shadowBrush, shadowRect);
            }
            base.OnPaint(e);
        }
    }

    // Clases auxiliares para componentes redondeados
    public class RoundedPanel : Panel
    {
        public int BorderRadius { get; set; } = 10;

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
            GraphicsPath path = GetRoundedRectangle(rect, BorderRadius);

            using (SolidBrush brush = new SolidBrush(this.BackColor))
                e.Graphics.FillPath(brush, path);

            using (Pen pen = new Pen(Color.FromArgb(229, 231, 235), 1))
                e.Graphics.DrawPath(pen, path);
        }

        private GraphicsPath GetRoundedRectangle(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.X + rect.Width - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.X + rect.Width - radius, rect.Y + rect.Height - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Y + rect.Height - radius, radius, radius, 90, 90);
            path.CloseAllFigures();
            return path;
        }
    }

    public class RoundedTextBox : TextBox
    {
        public string PlaceholderText { get; set; } = "";
        private bool isPlaceholder = false;

        public RoundedTextBox()
        {
            this.SetStyle(ControlStyles.UserPaint, true);
            this.BorderStyle = BorderStyle.None;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
            using (SolidBrush brush = new SolidBrush(this.BackColor))
            {
                GraphicsPath path = GetRoundedRectangle(rect, 8);
                e.Graphics.FillPath(brush, path);
            }

            using (Pen pen = new Pen(Color.FromArgb(209, 213, 219), 1))
            {
                GraphicsPath path = GetRoundedRectangle(rect, 8);
                e.Graphics.DrawPath(pen, path);
            }

            // Dibujar el texto manualmente
            Rectangle textRect = new Rectangle(15, (this.Height - this.Font.Height) / 2, this.Width - 30, this.Font.Height);
            string displayText = string.IsNullOrEmpty(this.Text) && !this.Focused ? PlaceholderText : this.Text;
            Color textColor = (string.IsNullOrEmpty(this.Text) && !this.Focused) ? Color.FromArgb(156, 163, 175) : this.ForeColor;

            TextRenderer.DrawText(e.Graphics, displayText, this.Font, textRect, textColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
        }

        private GraphicsPath GetRoundedRectangle(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.X + rect.Width - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.X + rect.Width - radius, rect.Y + rect.Height - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Y + rect.Height - radius, radius, radius, 90, 90);
            path.CloseAllFigures();
            return path;
        }
    }

    public class RoundedButton : Button
    {
        public int BorderRadius { get; set; } = 10;

        protected override void OnPaint(PaintEventArgs pevent)
        {
            GraphicsPath path = new GraphicsPath();
            Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
            path.AddArc(rect.X, rect.Y, BorderRadius, BorderRadius, 180, 90);
            path.AddArc(rect.X + rect.Width - BorderRadius, rect.Y, BorderRadius, BorderRadius, 270, 90);
            path.AddArc(rect.X + rect.Width - BorderRadius, rect.Y + rect.Height - BorderRadius, BorderRadius, BorderRadius, 0, 90);
            path.AddArc(rect.X, rect.Y + rect.Height - BorderRadius, BorderRadius, BorderRadius, 90, 90);
            path.CloseAllFigures();
            this.Region = new Region(path);
            base.OnPaint(pevent);
        }
    }
}