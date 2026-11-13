using System;
using System.Windows.Forms;
using MunicipalReporter.Forms;

namespace MunicipalReporter
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            while (true)
            {
                using (LoginForm loginForm = new LoginForm())
                {
                    if (loginForm.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }

                    using (MainMenuForm mainMenu = new MainMenuForm(loginForm.AuthenticatedRole))
                    {
                        mainMenu.ShowDialog();
                        if (!mainMenu.SwitchUserRequested)
                        {
                            return;
                        }
                    }
                }
            }
        }
    }
}
