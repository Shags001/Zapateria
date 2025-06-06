using System.Windows.Forms;

namespace ZapateriaWinForms.Views
{
    public class MainForm : Form
    {
        private readonly string userRol;
        private readonly string userName;
        public MainForm(string rol = "", string nombreUsuario = "")
        {
            userRol = rol;
            userName = nombreUsuario;
            this.Width = 900;
            this.Height = 650;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // --- BARRA DE TÍTULO PERSONALIZADA ---
            this.Text = string.Empty;
            this.ControlBox = false;
            var panelTitulo = new Panel {
                Dock = DockStyle.Top,
                Height = 36,
                BackColor = System.Drawing.Color.FromArgb(44, 62, 80)
            };
            var lblTitulo = new Label {
                Text = "Zapatería PRO - Sistema de Gestión",
                Dock = DockStyle.Left,
                Width = 350,
                Font = new System.Drawing.Font("Segoe UI", 13, System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.Color.White,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Padding = new Padding(16, 0, 0, 0)
            };
            var btnMin = new Button {
                Text = "_",
                Width = 36,
                Height = 36,
                Dock = DockStyle.Right,
                FlatStyle = FlatStyle.Flat,
                BackColor = System.Drawing.Color.FromArgb(44, 62, 80),
                ForeColor = System.Drawing.Color.White,
                Font = new System.Drawing.Font("Segoe UI", 14, System.Drawing.FontStyle.Bold),
                TabStop = false
            };
            btnMin.FlatAppearance.BorderSize = 0;
            btnMin.Click += (s, e) => this.WindowState = FormWindowState.Minimized;
            var btnClose = new Button {
                Text = "✕",
                Width = 36,
                Height = 36,
                Dock = DockStyle.Right,
                FlatStyle = FlatStyle.Flat,
                BackColor = System.Drawing.Color.FromArgb(44, 62, 80),
                ForeColor = System.Drawing.Color.White,
                Font = new System.Drawing.Font("Segoe UI", 14, System.Drawing.FontStyle.Bold),
                TabStop = false
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();
            panelTitulo.Controls.Add(btnClose);
            panelTitulo.Controls.Add(btnMin);
            panelTitulo.Controls.Add(lblTitulo);
            this.Controls.Add(panelTitulo);
            panelTitulo.BringToFront();
            // Permitir mover la ventana arrastrando el panel de título y el label de título
            // --- BARRA DE TÍTULO MÓVIL ---
            void HacerArrastrable(Control ctrl)
            {
                ctrl.MouseDown += (s, e) => {
                    if (e.Button == MouseButtons.Left)
                    {
                        ReleaseCapture();
                        SendMessage(this.Handle, 0xA1, new IntPtr(2), IntPtr.Zero);
                    }
                };
            }
            HacerArrastrable(panelTitulo);
            HacerArrastrable(lblTitulo);

            // Menú de navegación (navbar)
            var menu = new MenuStrip();
            var menuArchivo = new ToolStripMenuItem("Archivo");
            var menuSalir = new ToolStripMenuItem("Salir", null, (s, e) => Application.Exit());
            menuArchivo.DropDownItems.Add(menuSalir);
            menu.Items.Insert(0, menuArchivo); // Insertar al inicio
            var menuCatalogo = new ToolStripMenuItem("Catálogo", null, (s, e) => new CatalogoForm(userRol).ShowDialog());
            if (userRol == "Administrador")
            {
            var menuCategorias = new ToolStripMenuItem("Categorías", null, (s, e) => new CategoriaCrudForm().ShowDialog());
            var menuProveedores = new ToolStripMenuItem("Proveedores", null, (s, e) => new ProveedoresCrudForm().ShowDialog());
            var menuEmpleados = new ToolStripMenuItem("Empleados", null, (s, e) => new EmpleadosCrudForm().ShowDialog());
            var menuUsuariosCrud = new ToolStripMenuItem("Usuarios", null, (s, e) => new UsuariosCrudForm().ShowDialog());
            menu.Items.Add(menuCategorias);
            menu.Items.Add(menuProveedores);
            menu.Items.Add(menuEmpleados);
            menu.Items.Add(menuUsuariosCrud);
            }
            var menuVentas = new ToolStripMenuItem("Ventas", null, (s, e) => new VentasForm(userRol).ShowDialog());
            var menuAcerca = new ToolStripMenuItem("Acerca de", null, (s, e) => new AcercaDeForm().ShowDialog());

            menu.Items.Add(menuCatalogo);
            menu.Items.Add(menuVentas);
            menu.Items.Add(menuAcerca);

            this.MainMenuStrip = menu;
            this.Controls.Add(menu);

            // --- PANEL DE BIENVENIDA MEJORADO ---
            var panelBienvenida = new Panel {
                Dock = DockStyle.Top,
                Height = 320, // Mucho más alto para el zapato
                BackColor = System.Drawing.Color.FromArgb(236, 240, 241),
                Padding = new Padding(0, 20, 0, 20), // Más padding arriba y abajo
            };
            // Sombra sutil animada
            panelBienvenida.Paint += (s, e) => {
                var g = e.Graphics;
                using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                    new System.Drawing.Rectangle(0, panelBienvenida.Height - 16, panelBienvenida.Width, 16),
                    System.Drawing.Color.FromArgb(80, 44, 62, 80),
                    System.Drawing.Color.Transparent,
                    System.Drawing.Drawing2D.LinearGradientMode.Vertical))
                {
                    g.FillRectangle(brush, 0, panelBienvenida.Height - 16, panelBienvenida.Width, 16);
                }
            };
            // Ícono de zapato (más alto y margen superior)
            var lblIcono = new Label {
                Text = "👞",
                Dock = DockStyle.Top,
                Height = 120,
                Font = new System.Drawing.Font("Segoe UI Emoji", 90, System.Drawing.FontStyle.Regular),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Padding = new Padding(0, 0, 0, 0) // Sin padding superior
            };
            // Mensaje de bienvenida
            var lblBienvenido = new Label {
                Text = $"¡Bienvenido, {userName}!",
                Dock = DockStyle.Top,
                Height = 60,
                Font = new System.Drawing.Font("Segoe UI", 20, System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(44, 62, 80),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };
            // Slogan
            var lblSlogan = new Label {
                Text = "Encuentra el calzado perfecto para cada ocasión",
                Dock = DockStyle.Top,
                Height = 28,
                Font = new System.Drawing.Font("Segoe UI", 13, System.Drawing.FontStyle.Italic),
                ForeColor = System.Drawing.Color.FromArgb(52, 73, 94),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };
            // Fecha y hora actual (reloj responsivo)
            var lblFecha = new Label {
                Text = DateTime.Now.ToString("dddd, dd 'de' MMMM 'de' yyyy, HH:mm:ss"),
                Dock = DockStyle.Top,
                Height = 24,
                Font = new System.Drawing.Font("Segoe UI", 11, System.Drawing.FontStyle.Regular),
                ForeColor = System.Drawing.Color.FromArgb(127, 140, 141),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };
            // Usar Timer de WinForms explícitamente
            var timerReloj = new System.Windows.Forms.Timer { Interval = 1000 };
            timerReloj.Tick += (s, e) => lblFecha.Text = DateTime.Now.ToString("dddd, dd 'de' MMMM 'de' yyyy, HH:mm:ss");
            timerReloj.Start();

            // Asegura el orden correcto de los controles
            panelBienvenida.Controls.Clear();
            panelBienvenida.Controls.Add(lblBienvenido);
            panelBienvenida.Controls.Add(lblSlogan);
            panelBienvenida.Controls.Add(lblFecha);
            panelBienvenida.Controls.Add(lblIcono);
            panelBienvenida.Controls.SetChildIndex(lblIcono, 0);
            panelBienvenida.Controls.SetChildIndex(lblBienvenido, 1);
            panelBienvenida.Controls.SetChildIndex(lblSlogan, 2);
            panelBienvenida.Controls.SetChildIndex(lblFecha, 3);
            this.Controls.Add(panelBienvenida);
            panelBienvenida.BringToFront();

            // --- OPCIÓN CERRAR SESIÓN EN ARCHIVO ---
            if (menu.Items.Count > 0 && menu.Items[0] is ToolStripMenuItem archivoMenu)
            {
                bool existeCerrarSesion = false;
                foreach (ToolStripItem item in archivoMenu.DropDownItems)
                    if (item.Text == "Cerrar sesión") existeCerrarSesion = true;
                if (!existeCerrarSesion)
                {
                    var menuCerrarSesion = new ToolStripMenuItem("Cerrar sesión", null, (s, e) => {
                        // Simplemente cerrar el MainForm, el control vuelve al login principal
                        this.Close();
                    });
                    archivoMenu.DropDownItems.Insert(0, menuCerrarSesion);
                }
            }

            // Estilizar menú
            EstilizarMenu(menu);

            // --- ANIMACIÓN AL INICIAR SESIÓN EN MainForm ---
            this.Opacity = 0;
            var fadeInMain = new System.Windows.Forms.Timer { Interval = 15 };
            fadeInMain.Tick += (s, e) => {
                if (this.Opacity < 1)
                    this.Opacity += 0.05;
                else
                    fadeInMain.Stop();
            };
            fadeInMain.Start();
        }

        // --- ESTILO GLOBAL DE BOTONES ---
        void EstilizarBoton(Button btn, string colorHex, string hoverHex)
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

        // --- ESTILO GLOBAL DE MENÚ ---
        void EstilizarMenu(MenuStrip menu)
        {
            menu.BackColor = System.Drawing.Color.FromArgb(44, 62, 80); // Azul oscuro
            menu.ForeColor = System.Drawing.Color.White;
            menu.Font = new System.Drawing.Font("Segoe UI", 12, System.Drawing.FontStyle.Bold);
            menu.Renderer = new ToolStripProfessionalRenderer(new CustomMenuColorTable());
            foreach (ToolStripMenuItem item in menu.Items)
            {
                item.ForeColor = Color.White;
                item.BackColor = Color.Transparent;
                item.Padding = new Padding(16, 6, 16, 6);
                item.Margin = new Padding(4, 0, 4, 0);
                item.MouseEnter += (s, e) => item.BackColor = Color.FromArgb(52, 152, 219);
                item.MouseLeave += (s, e) => item.BackColor = Color.Transparent;
                foreach (ToolStripItem subItem in item.DropDownItems)
                {
                    subItem.ForeColor = Color.White;
                    subItem.BackColor = Color.FromArgb(44, 62, 80);
                    subItem.Font = new Font("Segoe UI", 11, FontStyle.Bold);
                }
            }
        }

        class CustomMenuColorTable : ProfessionalColorTable
        {
            public override Color MenuItemSelected => Color.FromArgb(52, 152, 219); // Azul claro
            public override Color MenuItemBorder => Color.FromArgb(41, 128, 185);
            public override Color MenuItemSelectedGradientBegin => Color.FromArgb(52, 152, 219);
            public override Color MenuItemSelectedGradientEnd => Color.FromArgb(41, 128, 185);
            public override Color MenuItemPressedGradientBegin => Color.FromArgb(41, 128, 185);
            public override Color MenuItemPressedGradientEnd => Color.FromArgb(52, 152, 219);
            public override Color ToolStripDropDownBackground => Color.FromArgb(44, 62, 80);
            public override Color ImageMarginGradientBegin => Color.FromArgb(44, 62, 80);
            public override Color ImageMarginGradientMiddle => Color.FromArgb(44, 62, 80);
            public override Color ImageMarginGradientEnd => Color.FromArgb(44, 62, 80);
        }

        // --- USO EN CADA FORMULARIO ---
        // Ejemplo para CatalogoForm:
        // EstilizarBoton(btnAgregar, "#3B82F6", "#2563EB"); // Azul
        // EstilizarBoton(btnEditar, "#6366F1", "#4338CA"); // Morado
        //
        // Para ProveedoresCrudForm, EmpleadosCrudForm, CategoriaCrudForm, UsuariosCrudForm:
        // EstilizarBoton(btnAgregar, "#10B981", "#059669"); // Verde
        // EstilizarBoton(btnEditar, "#F59E42", "#D97706"); // Naranja
        // ...
        // Aplica la función EstilizarBoton a todos los botones principales de acción en cada formulario después de crearlos.

        // --- BARRA DE TÍTULO MÓVIL (declarar DllImport a nivel de clase, fuera de métodos) ---
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
    }
}
