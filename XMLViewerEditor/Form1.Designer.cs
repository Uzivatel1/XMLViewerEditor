namespace XMLViewerEditor
{
    partial class Form1
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
        private ToolStrip toolStrip;
        private ToolStripButton openButton;
        private ToolStripButton saveButton;
        private ToolStripButton closeButton;
        private TreeView treeView;
        private Label fileInfoLabel;
        private Label elementInfoLabel;

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            toolStrip = new ToolStrip();
            openButton = new ToolStripButton();
            saveButton = new ToolStripButton();
            closeButton = new ToolStripButton();
            treeView = new TreeView();
            imageList = new ImageList(components);
            fileInfoLabel = new Label();
            elementInfoLabel = new Label();
            toolStrip.SuspendLayout();
            SuspendLayout();
            // 
            // toolStrip
            // 
            toolStrip.ImageScalingSize = new Size(20, 20);
            toolStrip.Items.AddRange(new ToolStripItem[] { openButton, saveButton, closeButton });
            toolStrip.Location = new Point(0, 0);
            toolStrip.Name = "toolStrip";
            toolStrip.Size = new Size(782, 27);
            toolStrip.TabIndex = 0;
            // 
            // openButton
            // 
            openButton.Name = "openButton";
            openButton.Size = new Size(58, 24);
            openButton.Text = "Otevřít";
            // 
            // saveButton
            // 
            saveButton.Name = "saveButton";
            saveButton.Size = new Size(52, 24);
            saveButton.Text = "Uložit";
            // 
            // closeButton
            // 
            closeButton.Name = "closeButton";
            closeButton.Size = new Size(51, 24);
            closeButton.Text = "Zavřít";
            // 
            // treeView
            // 
            treeView.ImageIndex = 0;
            treeView.ImageList = imageList;
            treeView.Location = new Point(10, 50);
            treeView.Name = "treeView";
            treeView.SelectedImageIndex = 0;
            treeView.Size = new Size(400, 400);
            treeView.TabIndex = 1;
            // 
            // imageList
            // 
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            imageList.ImageStream = (ImageListStreamer)resources.GetObject("imageList.ImageStream");
            imageList.TransparentColor = Color.Transparent;
            imageList.Images.SetKeyName(0, "file.png");
            imageList.Images.SetKeyName(1, "folder.png");
            // 
            // fileInfoLabel
            // 
            fileInfoLabel.Location = new Point(420, 50);
            fileInfoLabel.Name = "fileInfoLabel";
            fileInfoLabel.Size = new Size(350, 100);
            fileInfoLabel.TabIndex = 2;
            // 
            // elementInfoLabel
            // 
            elementInfoLabel.Location = new Point(420, 160);
            elementInfoLabel.Name = "elementInfoLabel";
            elementInfoLabel.Size = new Size(350, 300);
            elementInfoLabel.TabIndex = 3;
            // 
            // Form1
            // 
            ClientSize = new Size(782, 453);
            Controls.Add(toolStrip);
            Controls.Add(treeView);
            Controls.Add(fileInfoLabel);
            Controls.Add(elementInfoLabel);
            Name = "Form1";
            Text = "XML Viewer and Editor";
            toolStrip.ResumeLayout(false);
            toolStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ImageList imageList;
    }
}
