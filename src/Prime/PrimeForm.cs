using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using Knyaz.Optimus.Dom.Elements;
using Prime.Controls;
using Prime.DevTools;
using Prime.Model;
using Prime.Properties;
using Prime.HtmlView;

namespace Prime
{
	public partial class PrimeForm : Form
	{
		private Prime.HtmlView.BrowserControl _browserControl;
		Lazy<Form> _domInspectorForm;
		Lazy<Form> _consoleForm;
		private DevToolControl _devToolControl;
		private DomTreeControl _domTreeControl { get { return _devToolControl.DomTreeControl; } }

		private readonly Browser Browser;

		public PrimeForm()
		{
			var consoleControl = new ConsoleControl();
			
			Browser = new Browser(consoleControl);
			
			_domInspectorForm = new Lazy<Form>(() =>
			{
				_devToolControl = new DevToolControl(_browserControl.GetRect) {Engine = Browser.Engine};

				_domTreeControl.NodeSelected += _domTreeControl_NodeSelected;

				var frm = _devToolControl.PopupForm(); 
				frm.FormClosing += (sender, args) =>
				{
					args.Cancel = true;
					frm.Hide();
				};
				return frm;
			});

			
			
			_consoleForm = new Lazy<Form>(() =>
			{
				var frm = consoleControl.PopupForm(); 
				frm.FormClosing += (sender, args) =>
				{
					args.Cancel = true;
					frm.Hide();
				};
				return frm;
			});

			InitializeComponent();
			
			// 
			// _browserControl
			// 
			
			_browserControl = new BrowserControl(Browser);
			_browserControl.AutoScroll = true;
			_browserControl.BackColor = System.Drawing.Color.White;
			_browserControl.Dock = System.Windows.Forms.DockStyle.Fill;
			_browserControl.Location = new System.Drawing.Point(0, 29);
			_browserControl.Margin = new System.Windows.Forms.Padding(4);
			_browserControl.Name = "_browserControl";
			_browserControl.Size = new System.Drawing.Size(924, 406);
			_browserControl.TabIndex = 1;
			_browserControl.NodeClick += new System.EventHandler<Prime.HtmlView.NodeEventArgs>(this._browserControl_NodeClick);
			_browserControl.StateChanged += new System.EventHandler(this._browserControl_StateChanged);
			_browserControl.Browser.OnAuthorize += Browser_OnAuthorize;
			_browserControl.KeyDown += PrimeForm_KeyDown;
			Controls.Add(this._browserControl);

			
			
			_textBoxUrl.KeyDown += PrimeForm_KeyDown;
			toolStripStatusLabel1.Click += (sender, args) =>
			{
				if (_browserControl.State == BrowserStates.Error)
					MessageBox.Show(_browserControl.Exception.ToString());
			};
		}


		private Tuple<string, string> Browser_OnAuthorize()
		{
			if (InvokeRequired)
			{
				var result = new Tuple<string, string>[1];
				var asy = BeginInvoke(new Action(() =>
				{
					var form = new LoginPasswordForm();
					if (form.ShowDialog(this) == DialogResult.OK)
						result[0] = new Tuple<string, string>(form.Login, form.Password); 
				}));

				asy.AsyncWaitHandle.WaitOne();
				return result[0];
			}
			else
			{
				if (RequestCredentials(out var tuple)) 
					return tuple;
			}

			return null;
		}

		private bool RequestCredentials(out Tuple<string, string> tuple)
		{
			var form = new LoginPasswordForm();
			if (form.ShowDialog(this) == DialogResult.OK)
			{
				tuple = new Tuple<string, string>(form.Login, form.Password);
				return true;
			}

			tuple = null;
			return false;
		}

		void _domTreeControl_NodeSelected(TreeNode obj)
		{
			var node = obj.Tag as Node;
			if (node != null)
			{
				var rect = _browserControl.GetRect(node);
				_browserControl.SetHighlight(rect);
			}
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			Settings.Default.Save();
			base.OnClosing(e);
		}

		private void buttonGo_Click(object sender, EventArgs e)
		{
			_browserControl.Url = _textBoxUrl.Text;
		}


		private void _browserControl_StateChanged(object sender, EventArgs e)
		{
			if (InvokeRequired)
				BeginInvoke((Action)(() => _browserControl_StateChanged(sender, e)));
			else
			{
				toolStripStatusLabel1.Text = _browserControl.State == BrowserStates.None
					? ""
					: _browserControl.State == BrowserStates.Loading ? "Loading..."
					: _browserControl.State == BrowserStates.Error ? $"Error: {GetErrorMessage()}" : "Complete";
			}
		}

		private string GetErrorMessage()
		{
			var msg = _browserControl.Exception.Message;
			var idx = msg.IndexOfAny(new []{'\r', '\n'});
			if (idx > 0)
			{
				msg = msg.Substring(0, idx);
			}

			return msg;
		}

		private void PrimeForm_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter && _textBoxUrl.Focused)
			{
				_browserControl.Url = _textBoxUrl.Text;
				e.Handled = true;
				return;
			}

			if (e.KeyCode == Keys.F12 && e.Shift)
			{
				if (Browser.Engine == null)
					return;
				
				if (_consoleForm.Value.Visible)
					_consoleForm.Value.Hide();
				else
					_consoleForm.Value.Show();
			}
			else
			if (e.KeyCode == Keys.F12)
			{
				if (Browser.Engine == null)
					return;
				
				if (_domInspectorForm.Value.Visible)
					_domInspectorForm.Value.Hide();
				else
					_domInspectorForm.Value.Show();
			}
			
			else if (e.KeyCode == Keys.S && e.Control)
			{
				var saveDialog = new SaveFileDialog();
				if (saveDialog.ShowDialog() == DialogResult.OK)
				{
					var fileName = saveDialog.FileName;
					using(var stream = System.IO.File.OpenWrite(fileName))
					using (var writer = new StreamWriter(stream))
					{
						//_browserControl.Engine.Document.Save(writer);
					}
				}
			}
			else
			{
				e.Handled = false;
			}
		}


		private DomInspector DomInspector = new DomInspector();

		private void _browserControl_NodeClick(object sender, NodeEventArgs e)
		{
			if (DomInspector.SelectorActive && _domTreeControl != null)
			{
				DomInspector.SelectedNode = e.Node;
				_domTreeControl.SelectedNode = e.Node;
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (_domInspectorForm.Value.Visible)
				_domInspectorForm.Value.Hide();
			else
				_domInspectorForm.Value.Show();
		}
	}
}
