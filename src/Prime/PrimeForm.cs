using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Knyaz.Optimus.Dom;
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
		private HtmlDocumentView _documentView;
		Lazy<Form> _domInspectorForm;
		Lazy<Form> _consoleForm;
		private DevToolControl _devToolControl;
		private DomTreeControl _domTreeControl => _devToolControl.DomTreeControl;

		private readonly Browser Browser;

		public PrimeForm()
		{
			var consoleControl = new ConsoleControl();
			
			Browser = new Browser(consoleControl);
			Browser.OnAuthorize += Browser_OnAuthorize;
			Browser.PropertyChanged += (sender, args) =>
			{
				if (args.PropertyName == "State")
					_browserControl_StateChanged(this, EventArgs.Empty);
			};

			
			_domInspectorForm = new Lazy<Form>(() =>
			{
				_devToolControl = new DevToolControl(_documentView.GetRect) {Engine = Browser.Engine};

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
			
			InitDocumentView();


			_textBoxUrl.KeyDown += PrimeForm_KeyDown;
			toolStripStatusLabel1.Click += (sender, args) =>
			{
				if (Browser.State == BrowserStates.Error)
					MessageBox.Show(Browser.Exception.ToString());
			};
		}

		private void InitDocumentView()
		{
			_documentView = new HtmlDocumentView();
			_documentView.AutoScroll = true;
			_documentView.BackColor = System.Drawing.Color.White;
			_documentView.Dock = System.Windows.Forms.DockStyle.Fill;
			_documentView.Location = new System.Drawing.Point(0, 29);
			_documentView.Margin = new System.Windows.Forms.Padding(4);
			_documentView.Name = "_browserControl";
			_documentView.Size = new System.Drawing.Size(924, 406);
			_documentView.TabIndex = 1;
			_documentView.KeyDown += PrimeForm_KeyDown;
			Controls.Add(this._documentView);
			_documentView.BringToFront();
			_documentView.SizeChanged += (sender, args) => {
				var rect = _documentView.ClientRectangle;
				Browser.Engine.CurrentMedia.Width = rect.Width;
				Browser.Engine.CurrentMedia.Landscape = rect.Width > rect.Height;
			};
			
			_documentView.NodeClick += _browserControl_NodeClick;
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
				var rect = _documentView.GetRect(node);
				//_browserControl.SetHighlight(rect);
			}
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			Settings.Default.Save();
			base.OnClosing(e);
		}

		private void buttonGo_Click(object sender, EventArgs e) => GoTo(_textBoxUrl.Text);

		private void GoTo(string url)
		{
			Task.Run(async () => {
				await Browser.OpenUrl(url);
				_documentView.Document = Browser.Engine.Document;
			});
		}
		
		private void ButtonSettingsClick(object sender, EventArgs e)
		{
			//todo: open settings form;
			var form = new Form();
			var settingsControl = new SettingsControl();
			settingsControl.Dock = DockStyle.Fill;
			form.Controls.Add(settingsControl);
			form.ShowDialog(this);
		}


		private void _browserControl_StateChanged(object sender, EventArgs e)
		{
			if (InvokeRequired)
				BeginInvoke((Action)(() => _browserControl_StateChanged(sender, e)));
			else
			{
				toolStripStatusLabel1.Text = Browser.State == BrowserStates.None
					? ""
					: Browser.State == BrowserStates.Loading ? "Loading..."
					: Browser.State == BrowserStates.Error ? $"Error: {GetErrorMessage()}" : "Complete";
			}
		}

		private string GetErrorMessage()
		{
			var msg = Browser.Exception.Message;
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
				GoTo(_textBoxUrl.Text);
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
