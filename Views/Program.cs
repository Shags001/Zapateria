using System;
using System.Windows.Forms;
using ZapateriaWinForms.Views;

namespace ZapateriaWinForms
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new login()); // Aquí llama a tu formulario login
        }
    }
}

