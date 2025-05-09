using System.Windows.Forms;

namespace ZapateriaWinForms.Views
{
    public class MainForm : Form
    {
        public MainForm()
        {
            this.Text = "Zapatería";
            this.Width = 900;
            this.Height = 650;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Menú de navegación (navbar)
            var menu = new MenuStrip();
            var menuArchivo = new ToolStripMenuItem("Archivo");
            var menuSalir = new ToolStripMenuItem("Salir", null, (s, e) => this.Close());
            menuArchivo.DropDownItems.Add(menuSalir);

            var menuCatalogo = new ToolStripMenuItem("Catálogo", null, (s, e) => new CatalogoForm().ShowDialog());
            var menuInventario = new ToolStripMenuItem("Inventario", null, (s, e) => new InventarioForm().ShowDialog());
            var menuVentas = new ToolStripMenuItem("Ventas", null, (s, e) => new VentasForm().ShowDialog());
            var menuAcerca = new ToolStripMenuItem("Acerca de", null, (s, e) => new AcercaDeForm().ShowDialog());

            menu.Items.Add(menuArchivo);
            menu.Items.Add(menuCatalogo);
            menu.Items.Add(menuInventario);
            menu.Items.Add(menuVentas);
            menu.Items.Add(menuAcerca);

            this.MainMenuStrip = menu;
            this.Controls.Add(menu);

            // Encabezado
            var lblTitulo = new Label {
                Text = "Zapatería Elegante",
                Font = new System.Drawing.Font("Segoe UI", 28, System.Drawing.FontStyle.Bold),
                AutoSize = false,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 80
            };
            this.Controls.Add(lblTitulo);
            lblTitulo.BringToFront();

            // Eslogan
            var lblEslogan = new Label {
                Text = "Encuentra el calzado perfecto para cada ocasión",
                Font = new System.Drawing.Font("Segoe UI", 14, System.Drawing.FontStyle.Italic),
                AutoSize = false,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 40
            };
            this.Controls.Add(lblEslogan);
            lblEslogan.BringToFront();
        }
    }
}
