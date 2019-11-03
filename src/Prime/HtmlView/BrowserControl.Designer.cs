namespace Knyaz.Optimus.WinForms
{
	partial class BrowserControl
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._documentView = new Knyaz.Optimus.WinForms.HtmlDocumentView();
			this.SuspendLayout();
			// 
			// _documentView
			// 
			this._documentView.Dock = System.Windows.Forms.DockStyle.Fill;
			this._documentView.Document = null;
			this._documentView.Location = new System.Drawing.Point(0, 0);
			this._documentView.Name = "_documentView";
			this._documentView.Size = new System.Drawing.Size(605, 364);
			this._documentView.TabIndex = 0;
			// 
			// BrowserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this._documentView);
			this.Name = "BrowserControl";
			this.Size = new System.Drawing.Size(605, 364);
			this.ResumeLayout(false);

		}

		#endregion

		private HtmlDocumentView _documentView;
	}
}
