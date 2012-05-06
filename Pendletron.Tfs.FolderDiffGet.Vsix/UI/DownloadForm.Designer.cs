namespace Pendletron.Pendletron_Tfs_FolderDiffGet_Vsix.UI {
	partial class DownloadForm {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DownloadForm));
			this.uxOutputDirectory = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.uxBrowse = new System.Windows.Forms.Button();
			this.uxDownload = new System.Windows.Forms.Button();
			this.uxCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// uxOutputDirectory
			// 
			this.uxOutputDirectory.Location = new System.Drawing.Point(108, 21);
			this.uxOutputDirectory.Name = "uxOutputDirectory";
			this.uxOutputDirectory.Size = new System.Drawing.Size(338, 20);
			this.uxOutputDirectory.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(17, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(85, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Output directory:";
			// 
			// uxBrowse
			// 
			this.uxBrowse.Location = new System.Drawing.Point(453, 20);
			this.uxBrowse.Name = "uxBrowse";
			this.uxBrowse.Size = new System.Drawing.Size(75, 23);
			this.uxBrowse.TabIndex = 2;
			this.uxBrowse.Text = "&Browse...";
			this.uxBrowse.UseVisualStyleBackColor = true;
			this.uxBrowse.Click += new System.EventHandler(this.uxBrowse_Click);
			// 
			// uxDownload
			// 
			this.uxDownload.Location = new System.Drawing.Point(201, 47);
			this.uxDownload.Name = "uxDownload";
			this.uxDownload.Size = new System.Drawing.Size(75, 23);
			this.uxDownload.TabIndex = 3;
			this.uxDownload.Text = "D&ownload";
			this.uxDownload.UseVisualStyleBackColor = true;
			this.uxDownload.Click += new System.EventHandler(this.uxDownload_Click);
			// 
			// uxCancel
			// 
			this.uxCancel.Location = new System.Drawing.Point(283, 46);
			this.uxCancel.Name = "uxCancel";
			this.uxCancel.Size = new System.Drawing.Size(75, 23);
			this.uxCancel.TabIndex = 4;
			this.uxCancel.Text = "&Cancel";
			this.uxCancel.UseVisualStyleBackColor = true;
			this.uxCancel.Click += new System.EventHandler(this.uxCancel_Click);
			// 
			// DownloadForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(558, 80);
			this.Controls.Add(this.uxCancel);
			this.Controls.Add(this.uxDownload);
			this.Controls.Add(this.uxBrowse);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.uxOutputDirectory);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "DownloadForm";
			this.Text = "FolderDiffGet";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox uxOutputDirectory;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button uxBrowse;
		private System.Windows.Forms.Button uxDownload;
		private System.Windows.Forms.Button uxCancel;
	}
}