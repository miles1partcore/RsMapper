﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace RsMapper.Forms.Controls
{
    public class DrawPanel : Panel
    {
        public DrawPanel()
        {
            // Enable double buffering.
            this.DoubleBuffered = true;

            // Set styles.
            this.SetStyle(ControlStyles.UserPaint |
              ControlStyles.AllPaintingInWmPaint |
              ControlStyles.ResizeRedraw |
              ControlStyles.ContainerControl |
              ControlStyles.OptimizedDoubleBuffer |
              ControlStyles.SupportsTransparentBackColor
              , true);

            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Call the OnPaint method of the base class.
            base.OnPaint(e);

            // Draw a grid.
            Graphics g = e.Graphics;
            Pen pen = new Pen(Color.Black);


            for (int y = 0; y < 50; ++y)
            {
                g.DrawLine(pen, 0, y * 50, 100 * 50, y * 50);
            }

            for (int x = 0; x < 50; ++x)
            {
                g.DrawLine(pen, x * 50, 0, x * 50, 100 * 50);
            }
        }
    }
}