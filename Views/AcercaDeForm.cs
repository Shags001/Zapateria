using System.Windows.Forms;

namespace ZapateriaWinForms.Views
{
    public class AcercaDeForm : Form
    {
        public AcercaDeForm()
        {
            this.Text = "Acerca de la Zapatería";
            this.Width = 500;
            this.Height = 300;
            this.StartPosition = FormStartPosition.CenterParent;

            var lblInfo = new Label
            {
                Text = "Sistema de Gestión de Zapatería\n\nDesarrollado por Grupo 4\n\nVersión 1.0\n\n© 2025 Zapatería Elegante",
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Font = new System.Drawing.Font("Segoe UI", 14, System.Drawing.FontStyle.Regular)
            };
            this.Controls.Add(lblInfo);
        }
    }
}
