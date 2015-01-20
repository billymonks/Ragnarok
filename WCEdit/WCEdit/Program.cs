using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace WCEdit
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Form1 form = new Form1();
            form.Show();
            EditorGame game = new EditorGame();
            form.getContentManager(game.Content);
            form.getGraphicsDevice(game.GraphicsDevice);
            form.getEditor(game);
            game.Run();
        }
    }
}