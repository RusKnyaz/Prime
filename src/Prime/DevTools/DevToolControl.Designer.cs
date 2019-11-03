namespace Prime.DevTools
{
	partial class DevToolControl
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
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.domTreeControl1 = new Prime.DevTools.DomTreeControl();
			this.computedStyleControl1 = new Prime.DevTools.ComputedStyleControl();
			this.layoutInfoControl1 = new Prime.DevTools.LayoutInfoControl();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.domTreeControl1);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.computedStyleControl1);
			this.splitContainer1.Panel2.Controls.Add(this.layoutInfoControl1);
			this.splitContainer1.Size = new System.Drawing.Size(537, 864);
			this.splitContainer1.SplitterDistance = 521;
			this.splitContainer1.SplitterWidth = 5;
			this.splitContainer1.TabIndex = 1;
			// 
			// domTreeControl1
			// 
			this.domTreeControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.domTreeControl1.Engine = null;
			this.domTreeControl1.Location = new System.Drawing.Point(0, 0);
			this.domTreeControl1.Margin = new System.Windows.Forms.Padding(5);
			this.domTreeControl1.Name = "domTreeControl1";
			this.domTreeControl1.SelectedNode = null;
			this.domTreeControl1.ShowAttributes = true;
			this.domTreeControl1.Size = new System.Drawing.Size(537, 521);
			this.domTreeControl1.TabIndex = 0;
			// 
			// computedStyleControl1
			// 
			this.computedStyleControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.computedStyleControl1.Location = new System.Drawing.Point(0, 0);
			this.computedStyleControl1.Margin = new System.Windows.Forms.Padding(5);
			this.computedStyleControl1.Name = "computedStyleControl1";
			this.computedStyleControl1.Size = new System.Drawing.Size(537, 305);
			this.computedStyleControl1.Style = null;
			this.computedStyleControl1.TabIndex = 0;
			// 
			// layoutInfoControl1
			// 
			this.layoutInfoControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.layoutInfoControl1.Location = new System.Drawing.Point(0, 305);
			this.layoutInfoControl1.Name = "layoutInfoControl1";
			this.layoutInfoControl1.Size = new System.Drawing.Size(537, 33);
			this.layoutInfoControl1.TabIndex = 1;
			// 
			// DevToolControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitContainer1);
			this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.Name = "DevToolControl";
			this.Size = new System.Drawing.Size(537, 864);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private DomTreeControl domTreeControl1;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private ComputedStyleControl computedStyleControl1;
		private LayoutInfoControl layoutInfoControl1;
	}
}
