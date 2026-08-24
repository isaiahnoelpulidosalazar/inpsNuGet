namespace inpsNuGetTestForm
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            verticalFlowLayoutPanel1 = new inpsNuGet.VerticalFlowLayoutPanel();
            verticalFlowLayoutPanel2 = new VerticalFlowLayoutPanel();
            SuspendLayout();
            // 
            // verticalFlowLayoutPanel1
            // 
            verticalFlowLayoutPanel1.BorderStyle = BorderStyle.FixedSingle;
            verticalFlowLayoutPanel1.Location = new Point(12, 12);
            verticalFlowLayoutPanel1.Name = "verticalFlowLayoutPanel1";
            verticalFlowLayoutPanel1.Size = new Size(200, 426);
            verticalFlowLayoutPanel1.TabIndex = 0;
            verticalFlowLayoutPanel1.Layout += TabPanel_Layout;
            verticalFlowLayoutPanel1.Resize += TabPanel_Resize;
            // 
            // verticalFlowLayoutPanel2
            // 
            verticalFlowLayoutPanel2.BorderStyle = BorderStyle.FixedSingle;
            verticalFlowLayoutPanel2.Location = new Point(251, 12);
            verticalFlowLayoutPanel2.Name = "verticalFlowLayoutPanel2";
            verticalFlowLayoutPanel2.Size = new Size(220, 426);
            verticalFlowLayoutPanel2.TabIndex = 1;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(verticalFlowLayoutPanel2);
            Controls.Add(verticalFlowLayoutPanel1);
            Name = "MainForm";
            Text = "MainForm";
            ResumeLayout(false);
        }

        #endregion

        private inpsNuGet.VerticalFlowLayoutPanel verticalFlowLayoutPanel1;
        private VerticalFlowLayoutPanel verticalFlowLayoutPanel2;
    }
}
