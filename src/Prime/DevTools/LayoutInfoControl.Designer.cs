namespace Prime.DevTools
{
	partial class LayoutInfoControl
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
			this._layoutInfoLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// _layoutInfoLabel
			// 
			this._layoutInfoLabel.AutoSize = true;
			this._layoutInfoLabel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._layoutInfoLabel.Location = new System.Drawing.Point(0, 0);
			this._layoutInfoLabel.Name = "_layoutInfoLabel";
			this._layoutInfoLabel.Size = new System.Drawing.Size(41, 17);
			this._layoutInfoLabel.TabIndex = 0;
			this._layoutInfoLabel.Text = "Rect:";
			// 
			// LayoutInfoControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._layoutInfoLabel);
			this.Name = "LayoutInfoControl";
			this.Size = new System.Drawing.Size(302, 33);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label _layoutInfoLabel;
	}
}
