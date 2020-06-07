﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json.Linq;
using RsMapper.Forms;
using System.Windows.Interop;
using Microsoft.Win32.SafeHandles;
using System.Reflection;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Diagnostics;
using RsMapper.Forms.Controls;

namespace RsMapper
{
    public partial class Form1 : Form
    {
        
        // LIST VARIABLES
        public RootObject compList { get; set; }
        ImageList imgl;
        List<PictureBox> PictureBoxes;
        List<PictureBox> RedoList;
        PictureBox picb;
        
        // MISC VARIABLES
        int indx;
        RsComponent CurSelectedComp;
        Point p;
        string path;

        bool unsavedChanges;


        AboutBox1 about;

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        }
        
        // Read components json and retrieve redstone components.
        public void GetComps()
        {
            using (StreamReader r = new StreamReader(PrgmSelfCheck.ComponentsJson))
            {
                string json = r.ReadToEnd();
                Console.Write(json);
                
                compList = JsonConvert.DeserializeObject<RootObject>(json);
                FillList();
                
            }
        }

        // Fill the component list with components from the json file,
        // and asign their properties.
        void FillList()
        {
            imgl = new ImageList();
            indx = 0;
            foreach (RsComponent rsc in compList.RsComponents)
            {
                // Setup the image list and add component images.
                Image image =  Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + rsc.image);
                imgl.Images.Add(image);
                listView1.LargeImageList = imgl;

                ListViewItem lvi = listView1.Items.Add(rsc.name); // Add the item.
                lvi.Group = listView1.Groups[rsc.type + "Group"]; // Asign it to a group.
                lvi.ToolTipText = rsc.name + "\n" + rsc.info;     // Set the tooltip.
                lvi.ImageIndex = indx;                            // Asign the component its image;
                indx++;                                           // Increment the image index.
            }

        }

        // Startup tasks.
        private void Form1_Load(object sender, EventArgs e)
        {
            about = new AboutBox1();
            GetComps(); // Load redstone components into the program.
            toolStripLabel.Text = about.AssemblyTitle + " " + about.AssemblyVersion; // Display name and version in the tool strip;
            PictureBoxes = new List<PictureBox>();
            RedoList = new List<PictureBox>();


            p = new Point();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            about = new AboutBox1();
            about.ShowDialog(); // Display about box.
        }

        #region UNUSED, TO BE REMOVED
        
        private void listView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            ListViewItem listvi = listView1.SelectedItems[0];
            Image compImg = imgl.Images[listvi.ImageIndex];

            
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            Cursor.Current = Cursors.Cross;
        }
        #endregion

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
             
            Graphics g = panel1.CreateGraphics();                    // Create graphics.             
            g.InterpolationMode = InterpolationMode.NearestNeighbor; // Maintain pixelated quality while zooming.
            Pen pen = new Pen(Color.Black);                          // Set pen color to black.
            panel1.Refresh();                                        // Force the panel to repaint.

            panel1.SuspendLayout();
            // Is a component selected?
            if (listView1.SelectedItems.Count > 0)
            {
                picb = new PictureBox();

                // Get the selected item.
                ListViewItem listvi = listView1.SelectedItems[0];
                Image compImg = imgl.Images[listvi.ImageIndex];
                Rectangle rect = new Rectangle();


                // Grid pattern:


                // Get nearest multiple of 50 and set this to the X and Y coordinates.
                p.X = NearestMultiple(panel1.PointToClient(MousePosition).X, 50);
                p.Y = NearestMultiple(panel1.PointToClient(MousePosition).Y, 50);

                // Set rect location to a grid location.
                rect.Location = p;

                // Set image height and width to 50.
                rect.Width = 50;
                rect.Height = 50;

                // Display an image of the component that follows the cursor.

                g.DrawImage(compImg, rect);


            }
            panel1.ResumeLayout();
        }
        
        /// <summary>
        /// Find the nearest multiple of an integer to another integer.
        /// </summary>
        /// <param name="value">The number being calculated.</param>
        /// <param name="factor">Number to find the nearest multiple of.</param>
        /// <returns>The nearest nultiple of a number.</returns>
        public int NearestMultiple(int value, int factor)
        {
             return (int)(Math.Round((value / (double)factor), MidpointRounding.AwayFromZero) * factor);
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            
        }

        // Exit the application.
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UnsavedChangesExit();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create and configure a save file dialog.
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.AddExtension = true;
            sfd.Filter = "PNG Images |*.png|Bitmap Images |*.bmp";
            sfd.DefaultExt = "png";

            // Show the dialog a save the image.
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                // Get panel size.
                int width = panel1.Size.Width;
                int height = panel1.Size.Height;

                // Configure bitmap.
                Bitmap bm = new Bitmap(width, height);
                panel1.DrawToBitmap(bm, new Rectangle(0, 0, width, height));
                bm.Save(sfd.FileName);
                path = sfd.FileName;

                // No unsaved changes.
                Form1.ActiveForm.Text = "RsMapper - " + path;
                unsavedChanges = false;
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save();
            
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Revert the window to new.
            Form1.ActiveForm.Text = "RsMapper - Untitled";
            path = null;
            unsavedChanges = false;
            foreach(PictureBox picb in PictureBoxes)
            {
                picb.Dispose();
            }

            // Disable undo/redo buttons.
            undoToolStripMenuItem.Enabled = false;
            redoToolStripMenuItem.Enabled = false;

        }

         /// <summary>
         /// Handles saving.
         /// If the file has not yet been saved, show the save file dialog.
         /// If there is alreasy a path set to a file, save to that file without a dialog.
         /// </summary>
        public void Save()
        {
            if(path == null)
            {

                // Create and configure a save file dialog.
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.AddExtension = true;
                sfd.Filter = "PNG Images |*.png|Bitmap Images |*.bmp";
                sfd.DefaultExt = "png";

                // Show the dialog a save the image.
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    // Get panel size.
                    int width = panel1.Size.Width;
                    int height = panel1.Size.Height;

                    // Configure bitmap.
                    Bitmap bm = new Bitmap(width, height);
                    panel1.DrawToBitmap(bm, new Rectangle(0, 0, width, height));
                    bm.Save(sfd.FileName);
                    path = sfd.FileName;

                    // No unsaved changes.
                    Form1.ActiveForm.Text = "RsMapper - " + path;
                    unsavedChanges = false;
                } 

            } else
            {

                // Get panel size.
                int width = panel1.Size.Width;
                int height = panel1.Size.Height;

                // Configure bitmap.
                Bitmap bm = new Bitmap(width, height);
                panel1.DrawToBitmap(bm, new Rectangle(0, 0, width, height));
                bm.Save(path);

                // No unsaved changes.
                Form1.ActiveForm.Text = "RsMapper - " + path;
                unsavedChanges = false;


            }


        }

        // Handle application exit.
        public bool CancelFormExit = false;
        public void UnsavedChangesExit()
        {
            CancelFormExit = false;
            UnsavedDialog unsavedDialog = new UnsavedDialog();
            
            unsavedDialog.Disposed += unsavedDialog_Disposed;

            // If there are unsaved changes, show a confirmation dialog and
            // handle the result accordingly.
            if (unsavedChanges == true)
            {

                switch (unsavedDialog.ShowDialog())
                {
                    // Save work before closing.
                    case DialogResult.OK:
                        CancelFormExit = false;
                        Save();
                        this.Close();
                        Application.Exit();                      
                        break;
                    
                    // Close without saving.
                    case DialogResult.No:
                        CancelFormExit = false;
                        
                         
                        break;
                    
                    // Keep the application open.
                    case DialogResult.Cancel:
                        
                        CancelFormExit = true;

                        break;

                }
                unsavedDialog.Dispose();


            } else
            {

                // If all changes are saved, just close the program
                // without confirmation.
                Application.Exit();

            }
                    

        }

        // Handle the main window closing.
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.ApplicationExitCall)
            {
                // Close the program like normal if it's being
                // closed by an application exit call.
            }
            else
            {
                // Show the unsaved changes dialog.
                e.Cancel = CancelFormExit;
                UnsavedChangesExit();
                e.Cancel = CancelFormExit;
                CancelFormExit = false;
            }
            
        }

        private void unsavedDialog_Disposed(object sender, EventArgs e)
        {
            
        }

        protected void panel1_Paint(object sender, PaintEventArgs e)
        {
            
            

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void documentationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/GreenJamesDev/RsMapper/wiki");
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {

                // Is a component selected?
                if (listView1.SelectedItems.Count > 0)
                {
                    picb = new NNPictureBox();


                    // Get the selected item.
                    ListViewItem listvi = listView1.SelectedItems[0];
                    Image compImg = imgl.Images[listvi.ImageIndex];

                    Rectangle rect = new Rectangle();

                    Bitmap src = (Bitmap)compImg;

                    Graphics graphics = Graphics.FromImage(src);
                    graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                    graphics.PixelOffsetMode = PixelOffsetMode.None;



                    // Get nearest multiple of 50 and set this to the X and Y coordinates.
                    p.X = NearestMultiple(panel1.PointToClient(MousePosition).X, 50);
                    p.Y = NearestMultiple(panel1.PointToClient(MousePosition).Y, 50);

                    // Set rect location to a grid location.
                    rect.Location = p;

                    // Set image height and width to 50.
                    rect.Width = 50;
                    rect.Height = 50;


                    picb.Parent = panel1;
                    picb.Location = p;

                    picb.Image = src;
                    picb.Size = rect.Size;
                    picb.SizeMode = PictureBoxSizeMode.StretchImage;
                    picb.Visible = true;

                    // Add component to the list.
                    PictureBoxes.Add(picb);

                    undoToolStripMenuItem.Enabled = true;

                    // Tell the user there are unsaved changes.
                    if (unsavedChanges == false)
                    {
                        Form1.ActiveForm.Text = ActiveForm.Text + "*";
                        unsavedChanges = true;
                    }
                }

            // Place Wires
            } else if (e.Button == MouseButtons.Right)
            {

                picb = new NNPictureBox();

                if (wireListView.SelectedItems.Count > 0)
                {
                    // Get the selected item.
                    ListViewItem listvi = wireListView.SelectedItems[0];
                    Image compImg = wireImageList.Images[listvi.ImageIndex];

                    Rectangle rect = new Rectangle();

                    Bitmap src = (Bitmap)compImg;

                    Graphics graphics = Graphics.FromImage(src);
                    graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                    graphics.PixelOffsetMode = PixelOffsetMode.None;



                    // Get nearest multiple of 50 and set this to the X and Y coordinates.
                    p.X = NearestMultiple(panel1.PointToClient(MousePosition).X, 50);
                    p.Y = NearestMultiple(panel1.PointToClient(MousePosition).Y, 50);

                    // Set rect location to a grid location.
                    rect.Location = p;

                    // Set image height and width to 50.
                    rect.Width = 50;
                    rect.Height = 50;


                    picb.Parent = panel1;
                    picb.Location = p;

                    picb.Image = src;
                    picb.Size = rect.Size;
                    picb.SizeMode = PictureBoxSizeMode.StretchImage;
                    picb.Visible = true;

                    // Add component to the list.
                    PictureBoxes.Add(picb);

                    undoToolStripMenuItem.Enabled = true;

                    // Tell the user there are unsaved changes.
                    if (unsavedChanges == false)
                    {
                        Form1.ActiveForm.Text = ActiveForm.Text + "*";
                        unsavedChanges = true;
                    }
                }

            } else if(e.Button == MouseButtons.Middle)
            {
                if (CursorControlLocate.FindControlAtCursor(panel1) is PictureBox)
                {

                    


                }
            }
        }

        // UNDO
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            PictureBoxes.Last().Visible = false; // Hide last change.
            RedoList.Add(PictureBoxes.Last());   // Add it to the redo list.
            PictureBoxes.Remove(PictureBoxes.Last()); // Remove it from the main list.
            redoToolStripMenuItem.Enabled = true; // Enable redo button.

            // If there are no more things to undo, disable button.
            if (PictureBoxes.Count == 0)
            {
                undoToolStripMenuItem.Enabled = false;
            } else
            {
                undoToolStripMenuItem.Enabled = true;
            }
        }

        // REDO
        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RedoList.Last().Visible = true; // Show the image.
            PictureBoxes.Add(RedoList.Last()); // Add it back to the main list.
            RedoList.Remove(RedoList.Last()); // Remove it from the redo list.
            undoToolStripMenuItem.Enabled = true; // Enable undo button.

            // If there are no more things to redo, disable button.
            if (RedoList.Count == 0)
            {
                redoToolStripMenuItem.Enabled = false;
            }
            else
            {
                redoToolStripMenuItem.Enabled = true;
            }
        }
    }
}
