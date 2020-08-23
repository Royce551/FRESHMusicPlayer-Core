using System;
using Gtk;

namespace GtkSharpTest
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Application.Init();

            var app = new Application("org.GtkSharpTest.GtkSharpTest", GLib.ApplicationFlags.None);
            app.Register(GLib.Cancellable.Current);

            var win = new MainWindow();
            app.AddWindow(win);

            win.Show();
            win.DeleteEvent += (o, e) => Application.Quit();
            Application.Run();
        }
    }
}
