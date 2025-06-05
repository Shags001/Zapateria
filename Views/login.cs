using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

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
            lblTitulo = new Label()
            {
                Text = "Iniciar Sesión",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = Color.FromArgb(17, 24, 39),
                AutoSize = false,
                Size = new Size(350, 50),
                Location = new Point(50, 80),
                TextAlign = ContentAlignment.MiddleLeft
            };
            panelRight.Controls.Add(lblTitulo);

            lblSubtitulo = new Label()
            {
                Text = "Accede a tu cuenta para continuar",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(107, 114, 128),
                AutoSize = false,
                Size = new Size(350, 25),
                Location = new Point(50, 130)
            };
            panelRight.Controls.Add(lblSubtitulo);

            // Campo Email
            lblUsuario = new Label()
            {
                Text = "Correo electrónico",
                Location = new Point(50, 180),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(55, 65, 81)
            };
            panelRight.Controls.Add(lblUsuario);

            txtUsuario = new RoundedTextBox()
            {
                Location = new Point(50, 205),
                Width = 350,
                Height = 45,
                Font = new Font("Segoe UI", 11),
                BackColor = Color.FromArgb(249, 250, 251),
                ForeColor = Color.FromArgb(17, 24, 39),
                BorderStyle = BorderStyle.None,
                PlaceholderText = "nombre@ejemplo.com"
            };
            txtUsuario.Padding = new Padding(15, 12, 15, 12);
            panelRight.Controls.Add(txtUsuario);

            // Campo Contraseña
            lblContrasena = new Label()
            {
                Text = "Contraseña",
                Location = new Point(50, 270),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(55, 65, 81)
            };
            panelRight.Controls.Add(lblContrasena);

            // Panel para contraseña con botón mostrar/ocultar
            Panel panelPassword = new Panel()
            {
                Location = new Point(50, 295),
                Size = new Size(350, 45),
                BackColor = Color.FromArgb(249, 250, 251)
            };
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

            txtContrasena = new TextBox()
            {
                Location = new Point(15, 12),
                Width = 300,
                Height = 21,
                UseSystemPasswordChar = true,
                Font = new Font("Segoe UI", 11),
                BackColor = Color.FromArgb(249, 250, 251),
                ForeColor = Color.FromArgb(17, 24, 39),
                BorderStyle = BorderStyle.None
            };
            panelPassword.Controls.Add(txtContrasena);

            btnMostrarPassword = new Button()
            {
                Text = "👁",
                Size = new Size(30, 21),
                Location = new Point(315, 12),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(107, 114, 128),
                Font = new Font("Segoe UI", 10),
                Cursor = Cursors.Hand
            };
            btnMostrarPassword.FlatAppearance.BorderSize = 0;
            btnMostrarPassword.Click += BtnMostrarPassword_Click;
            panelPassword.Controls.Add(btnMostrarPassword);

            // Botón de inicio de sesión
            btnLogin = new RoundedButton()
            {
                Text = "Iniciar Sesión",
                Location = new Point(50, 370),
                Width = 350,
                Height = 50,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = Color.FromArgb(59, 130, 246),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                BorderRadius = 10
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.FlatAppearance.MouseOverBackColor = Color.FromArgb(37, 99, 235);
            btnLogin.Click += BtnLogin_Click;
            panelRight.Controls.Add(btnLogin);

            // Mensaje de estado
            lblMensaje = new Label()
            {
                Location = new Point(50, 440),
                Width = 350,
                Height = 30,
                ForeColor = Color.Red,
                Font = new Font("Segoe UI", 10),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            panelRight.Controls.Add(lblMensaje);

            // Animación de entrada
            this.Opacity = 0;
            Timer fadeInTimer = new Timer() { Interval = 20 };
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
            Timer timer = new Timer() { Interval = 3000 };
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
        private bool isPlaceholder = true;

        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
            using (Pen pen = new Pen(Color.FromArgb(209, 213, 219), 1))
            {
                GraphicsPath path = GetRoundedRectangle(rect, 8);
                e.Graphics.DrawPath(pen, path);
            }
            base.OnPaint(e);
        }

        protected override void OnEnter(EventArgs e)
        {
            if (isPlaceholder && this.Text == PlaceholderText)
            {
                this.Text = "";
                this.ForeColor = Color.FromArgb(17, 24, 39);
                isPlaceholder = false;
            }
            base.OnEnter(e);
        }

        protected override void OnLeave(EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.Text))
            {
                this.Text = PlaceholderText;
                this.ForeColor = Color.FromArgb(156, 163, 175);
                isPlaceholder = true;
            }
            base.OnLeave(e);
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