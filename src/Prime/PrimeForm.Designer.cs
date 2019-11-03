using Knyaz.Optimus.WinForms;

namespace Prime
{
	partial class PrimeForm
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
			this.panel1 = new System.Windows.Forms.Panel();
			this._textBoxUrl = new System.Windows.Forms.TextBox();
			this.buttonGo = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this._browserControl = new Prime.HtmlView.BrowserControl();
			this.panel1.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this._textBoxUrl);
			this.panel1.Controls.Add(this.buttonGo);
			this.panel1.Controls.Add(this.button1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(924, 29);
			this.panel1.TabIndex = 0;
			// 
			// _textBoxUrl
			// 
			this._textBoxUrl.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Prime.Properties.Settings.Default, "Setting", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this._textBoxUrl.Dock = System.Windows.Forms.DockStyle.Fill;
			this._textBoxUrl.Location = new System.Drawing.Point(0, 0);
			this._textBoxUrl.Name = "_textBoxUrl";
			this._textBoxUrl.Size = new System.Drawing.Size(799, 20);
			this._textBoxUrl.TabIndex = 0;
			this._textBoxUrl.Text = global::Prime.Properties.Settings.Default.Setting;
			// 
			// buttonGo
			// 
			this.buttonGo.Dock = System.Windows.Forms.DockStyle.Right;
			this.buttonGo.Location = new System.Drawing.Point(799, 0);
			this.buttonGo.Name = "buttonGo";
			this.buttonGo.Size = new System.Drawing.Size(64, 29);
			this.buttonGo.TabIndex = 1;
			this.buttonGo.Text = "Go";
			this.buttonGo.UseVisualStyleBackColor = true;
			this.buttonGo.Click += new System.EventHandler(this.buttonGo_Click);
			// 
			// button1
			// 
			this.button1.Dock = System.Windows.Forms.DockStyle.Right;
			this.button1.Location = new System.Drawing.Point(863, 0);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(61, 29);
			this.button1.TabIndex = 2;
			this.button1.Text = "inspect";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// statusStrip1
			// 
			this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
			this.statusStrip1.Location = new System.Drawing.Point(0, 435);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 10, 0);
			this.statusStrip1.Size = new System.Drawing.Size(924, 22);
			this.statusStrip1.TabIndex = 2;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// toolStripStatusLabel1
			// 
			this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
			// 
			// _browserControl
			// 
			this._browserControl.AutoScroll = true;
			this._browserControl.BackColor = System.Drawing.Color.White;
			this._browserControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this._browserControl.Location = new System.Drawing.Point(0, 29);
			this._browserControl.Margin = new System.Windows.Forms.Padding(4);
			this._browserControl.Name = "_browserControl";
			this._browserControl.Size = new System.Drawing.Size(924, 406);
			this._browserControl.TabIndex = 1;
			this._browserControl.NodeClick += new System.EventHandler<Prime.HtmlView.NodeEventArgs>(this._browserControl_NodeClick);
			this._browserControl.StateChanged += new System.EventHandler(this._browserControl_StateChanged);
			// 
			// PrimeForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(924, 457);
			this.Controls.Add(this._browserControl);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.statusStrip1);
			this.Name = "PrimeForm";
			this.Text = "Form1";
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PrimeForm_KeyDown);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		private Prime.HtmlView.BrowserControl _browserControl;
		private System.Windows.Forms.TextBox _textBoxUrl;
		private System.Windows.Forms.Button buttonGo;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
		private System.Windows.Forms.Button button1;
	}
}