using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.WinForms;
using Prime.Controls;
using Prime.DevTools;
using Prime.Model;
using Prime.Properties;
using Knyaz.Optimus.TestingTools;

namespace Prime
{
	public partial class PrimeForm : Form
	{
		Lazy<Form> _domInspectorForm;
		Lazy<Form> _consoleForm;
		private DevToolControl _devToolControl;
		private DomTreeControl _domTreeControl { get { return _devToolControl.DomTreeControl; } }

		public PrimeForm()
		{
			_domInspectorForm = new Lazy<Form>(() =>
			{
				_devToolControl = new DevToolControl(_browserControl.GetRect) {Engine = _browserControl.Engine};

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
				var consoleControl = new ConsoleControl(_browserControl.Engine);

				var frm = consoleControl.PopupForm(); 
				frm.FormClosing += (sender, args) =>
				{
					args.Cancel = true;
					frm.Hide();
				};
				return frm;
			});

			InitializeComponent();

			_browserControl.Browser.OnAuthorize += Browser_OnAuthorize;
			_browserControl.KeyDown += PrimeForm_KeyDown;
			_textBoxUrl.KeyDown += PrimeForm_KeyDown;
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
					: _browserControl.State == BrowserStates.Error ? "Error" : "Complete";
			}
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
				if (_browserControl.Engine == null)
					return;
				
				if (_consoleForm.Value.Visible)
					_consoleForm.Value.Hide();
				else
					_consoleForm.Value.Show();
			}
			else
			if (e.KeyCode == Keys.F12)
			{
				if (_browserControl.Engine == null)
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
