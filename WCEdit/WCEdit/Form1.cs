using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using wickedcrush.screen;

namespace WCEdit
{
    public partial class Form1 : Form
    {
        ContentManager _content;
        GraphicsDevice _graphics;
        EditorGame _editor;

        public Form1()
        {
            InitializeComponent();
        }

        //public IntPtr getDrawSurface()
        //{
            //return pctSurface.Handle;
        //}

        public void getContentManager(ContentManager cm)
        {
            _content = cm;
        }
        public void getGraphicsDevice(GraphicsDevice gd)
        {
            _graphics = gd;
        }
        public void getEditor(EditorGame ed) //going to hell
        {
            _editor = ed;
        }

        private void gridToggleButton_Click(object sender, EventArgs e)
        {
            _editor.ToggleGrid();
        }

        private void loadMap_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "XML Files|*.xml";
            openFileDialog1.Title = "Select a Map File";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialog1.FileName != "")
                {
                    //_editor.mapManager.LoadMap(
                }
            }
        }
    }
}
