namespace MetaArt.Wpf {
    partial class SketchForm {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.skglControl1 = new MetaArt.Wpf.MyControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // skglControl1
            // 
            this.skglControl1.BackColor = System.Drawing.Color.Black;
            this.skglControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.skglControl1.Location = new System.Drawing.Point(0, 0);
            this.skglControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.skglControl1.Name = "skglControl1";
            this.skglControl1.Size = new System.Drawing.Size(700, 338);
            this.skglControl1.TabIndex = 0;
            this.skglControl1.VSync = true;
            this.skglControl1.PaintSurface += new System.EventHandler<SkiaSharp.Views.Desktop.SKPaintGLSurfaceEventArgs>(this.skglControl1_PaintSurface);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.skglControl1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(700, 338);
            this.panel1.TabIndex = 1;
            // 
            // SketchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 338);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "SketchForm";
            this.Text = "Form1";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private MyControl skglControl1;
        private Panel panel1;
    }
}
