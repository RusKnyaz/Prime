namespace Prime.DevTools
{
	partial class DomTreeControl
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
			this.components = new System.ComponentModel.Container();
			this.treeView1 = new System.Windows.Forms.TreeView();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.clickToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.setAttributeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.panel1 = new System.Windows.Forms.Panel();
			this._ctrlSelect = new System.Windows.Forms.CheckBox();
			this.contextMenuStrip1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// treeView1
			// 
			this.treeView1.ContextMenuStrip = this.contextMenuStrip1;
			this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeView1.HideSelection = false;
			this.treeView1.Location = new System.Drawing.Point(0, 34);
			this.treeView1.Name = "treeView1";
			this.treeView1.Size = new System.Drawing.Size(165, 203);
			this.treeView1.TabIndex = 0;
			this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.OnTreeViewAfterSelect);
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clickToolStripMenuItem,
            this.setAttributeToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(138, 48);
			// 
			// clickToolStripMenuItem
			// 
			this.clickToolStripMenuItem.Name = "clickToolStripMenuItem";
			this.clickToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
			this.clickToolStripMenuItem.Text = "click";
			this.clickToolStripMenuItem.Click += new System.EventHandler(this.clickToolStripMenuItem_Click);
			// 
			// setAttributeToolStripMenuItem
			// 
			this.setAttributeToolStripMenuItem.Name = "setAttributeToolStripMenuItem";
			this.setAttributeToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
			this.setAttributeToolStripMenuItem.Text = "set attribute";
			this.setAttributeToolStripMenuItem.Click += new System.EventHandler(this.setAttributeToolStripMenuItem_Click);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this._ctrlSelect);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(165, 34);
			this.panel1.TabIndex = 1;
			// 
			// _ctrlSelect
			// 
			this._ctrlSelect.Appearance = System.Windows.Forms.Appearance.Button;
			this._ctrlSelect.AutoSize = true;
			this._ctrlSelect.Location = new System.Drawing.Point(3, 5);
			this._ctrlSelect.Name = "_ctrlSelect";
			this._ctrlSelect.Size = new System.Drawing.Size(32, 23);
			this._ctrlSelect.TabIndex = 1;
			this._ctrlSelect.Text = "Sel";
			this._ctrlSelect.UseVisualStyleBackColor = true;
			this._ctrlSelect.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
			// 
			// DomTreeControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.treeView1);
			this.Controls.Add(this.panel1);
			this.Name = "DomTreeControl";
			this.Size = new System.Drawing.Size(165, 237);
			this.contextMenuStrip1.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TreeView treeView1;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem clickToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem setAttributeToolStripMenuItem;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.CheckBox _ctrlSelect;
	}
}
