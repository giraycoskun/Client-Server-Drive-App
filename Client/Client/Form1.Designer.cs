namespace Client
{
    partial class CLIENT
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.outputBox = new System.Windows.Forms.RichTextBox();
            this.ipBox = new System.Windows.Forms.TextBox();
            this.portBox = new System.Windows.Forms.TextBox();
            this.connectButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.usernameBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.uploadFileBox = new System.Windows.Forms.TextBox();
            this.browseButton = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.uploadButton = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.downloadFolderBox = new System.Windows.Forms.TextBox();
            this.browseFolderButton = new System.Windows.Forms.Button();
            this.downloadButton = new System.Windows.Forms.Button();
            this.deleteButton = new System.Windows.Forms.Button();
            this.copyButton = new System.Windows.Forms.Button();
            this.getFileButton = new System.Windows.Forms.Button();
            this.changeAccessButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(738, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "CLIENT";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(95, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "Server IP:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(97, 122);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(102, 20);
            this.label3.TabIndex = 2;
            this.label3.Text = "Port Number:";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // outputBox
            // 
            this.outputBox.Location = new System.Drawing.Point(70, 202);
            this.outputBox.Name = "outputBox";
            this.outputBox.Size = new System.Drawing.Size(676, 390);
            this.outputBox.TabIndex = 3;
            this.outputBox.Text = "";
            // 
            // ipBox
            // 
            this.ipBox.Location = new System.Drawing.Point(257, 68);
            this.ipBox.Name = "ipBox";
            this.ipBox.Size = new System.Drawing.Size(200, 26);
            this.ipBox.TabIndex = 4;
            this.ipBox.TextChanged += new System.EventHandler(this.ipBox_TextChanged);
            // 
            // portBox
            // 
            this.portBox.Location = new System.Drawing.Point(257, 116);
            this.portBox.Name = "portBox";
            this.portBox.Size = new System.Drawing.Size(200, 26);
            this.portBox.TabIndex = 5;
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point(526, 68);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(182, 45);
            this.connectButton.TabIndex = 6;
            this.connectButton.Text = "CONNECT";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.Location = new System.Drawing.Point(526, 136);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(182, 43);
            this.stopButton.TabIndex = 7;
            this.stopButton.Text = "STOP";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // usernameBox
            // 
            this.usernameBox.Location = new System.Drawing.Point(253, 158);
            this.usernameBox.Name = "usernameBox";
            this.usernameBox.Size = new System.Drawing.Size(205, 26);
            this.usernameBox.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(97, 167);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 20);
            this.label4.TabIndex = 9;
            this.label4.Text = "Username";
            // 
            // uploadFileBox
            // 
            this.uploadFileBox.Location = new System.Drawing.Point(946, 154);
            this.uploadFileBox.Name = "uploadFileBox";
            this.uploadFileBox.Size = new System.Drawing.Size(240, 26);
            this.uploadFileBox.TabIndex = 11;
            this.uploadFileBox.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // browseButton
            // 
            this.browseButton.Location = new System.Drawing.Point(946, 195);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(240, 29);
            this.browseButton.TabIndex = 12;
            this.browseButton.Text = "Choose File";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
            // 
            // uploadButton
            // 
            this.uploadButton.Location = new System.Drawing.Point(873, 266);
            this.uploadButton.Name = "uploadButton";
            this.uploadButton.Size = new System.Drawing.Size(162, 45);
            this.uploadButton.TabIndex = 13;
            this.uploadButton.Text = "UPLOAD";
            this.uploadButton.UseVisualStyleBackColor = true;
            this.uploadButton.Click += new System.EventHandler(this.uploadButton_Click);
            // 
            // downloadFolderBox
            // 
            this.downloadFolderBox.Location = new System.Drawing.Point(1083, 545);
            this.downloadFolderBox.Name = "downloadFolderBox";
            this.downloadFolderBox.Size = new System.Drawing.Size(251, 26);
            this.downloadFolderBox.TabIndex = 14;
            // 
            // browseFolderButton
            // 
            this.browseFolderButton.Location = new System.Drawing.Point(798, 545);
            this.browseFolderButton.Name = "browseFolderButton";
            this.browseFolderButton.Size = new System.Drawing.Size(240, 33);
            this.browseFolderButton.TabIndex = 15;
            this.browseFolderButton.Text = "Choose Download Folder";
            this.browseFolderButton.UseVisualStyleBackColor = true;
            this.browseFolderButton.Click += new System.EventHandler(this.browseFolderButton_Click);
            // 
            // downloadButton
            // 
            this.downloadButton.Location = new System.Drawing.Point(1120, 266);
            this.downloadButton.Name = "downloadButton";
            this.downloadButton.Size = new System.Drawing.Size(162, 45);
            this.downloadButton.TabIndex = 17;
            this.downloadButton.Text = "DOWNLOAD";
            this.downloadButton.UseVisualStyleBackColor = true;
            this.downloadButton.Click += new System.EventHandler(this.downloadButton_Click);
            // 
            // deleteButton
            // 
            this.deleteButton.Location = new System.Drawing.Point(873, 335);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(162, 44);
            this.deleteButton.TabIndex = 18;
            this.deleteButton.Text = "DELETE";
            this.deleteButton.UseVisualStyleBackColor = true;
            // 
            // copyButton
            // 
            this.copyButton.Location = new System.Drawing.Point(1120, 335);
            this.copyButton.Name = "copyButton";
            this.copyButton.Size = new System.Drawing.Size(162, 44);
            this.copyButton.TabIndex = 19;
            this.copyButton.Text = "COPY";
            this.copyButton.UseVisualStyleBackColor = true;
            this.copyButton.Click += new System.EventHandler(this.button4_Click);
            // 
            // getFileButton
            // 
            this.getFileButton.Location = new System.Drawing.Point(937, 69);
            this.getFileButton.Name = "getFileButton";
            this.getFileButton.Size = new System.Drawing.Size(249, 48);
            this.getFileButton.TabIndex = 20;
            this.getFileButton.Text = "GET FILES";
            this.getFileButton.UseVisualStyleBackColor = true;
            // 
            // changeAccessButton
            // 
            this.changeAccessButton.Location = new System.Drawing.Point(1004, 408);
            this.changeAccessButton.Name = "changeAccessButton";
            this.changeAccessButton.Size = new System.Drawing.Size(162, 44);
            this.changeAccessButton.TabIndex = 21;
            this.changeAccessButton.Text = "CHANGE ACCESS";
            this.changeAccessButton.UseVisualStyleBackColor = true;
            // 
            // CLIENT
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1373, 618);
            this.Controls.Add(this.changeAccessButton);
            this.Controls.Add(this.getFileButton);
            this.Controls.Add(this.copyButton);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.downloadButton);
            this.Controls.Add(this.browseFolderButton);
            this.Controls.Add(this.downloadFolderBox);
            this.Controls.Add(this.uploadButton);
            this.Controls.Add(this.browseButton);
            this.Controls.Add(this.uploadFileBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.usernameBox);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.connectButton);
            this.Controls.Add(this.portBox);
            this.Controls.Add(this.ipBox);
            this.Controls.Add(this.outputBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "CLIENT";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.CLIENT_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RichTextBox outputBox;
        private System.Windows.Forms.TextBox ipBox;
        private System.Windows.Forms.TextBox portBox;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.TextBox usernameBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox uploadFileBox;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button uploadButton;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.TextBox downloadFolderBox;
        private System.Windows.Forms.Button browseFolderButton;
        private System.Windows.Forms.Button downloadButton;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Button copyButton;
        private System.Windows.Forms.Button getFileButton;
        private System.Windows.Forms.Button changeAccessButton;
    }
}

