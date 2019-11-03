using Knyaz.Optimus;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.ResourceProviders;
using Knyaz.Optimus.WinForms;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Knyaz.Optimus.WinForms.Tools;

namespace Prime.Controls
{
	public partial class LoginPasswordForm : Form
	{
		private readonly LoginPasswordControl _view = new LoginPasswordControl();

		public LoginPasswordForm()
		{
			InitializeComponent();
			_view.OnOk += ViewOnOnOk;
			_view.OnCancel += ViewOnOnCancel;
			Controls.Add(_view);
			_view.Dock = DockStyle.Fill;
		}

		private void ViewOnOnCancel(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void ViewOnOnOk(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}

		public string Login => _view.Login;
		public string Password => _view.Password;
	}
}
