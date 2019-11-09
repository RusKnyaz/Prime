using System;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using Prime.HtmlView;

namespace Prime.Controls
{
	/// <summary>
	/// Login/password win forms control
	/// </summary>
    public partial class LoginPasswordControl : HtmlUserControl
    {
        public LoginPasswordControl()
        {
            InitializeComponent();
            AddResource("Prime.Res.bootstrap.min.css", "text/css", () => typeof(LoginPasswordForm).Assembly.GetManifestResourceStream("Prime.Res.bootstrap.min.css"));
        }
        
        protected override void OnInitDocument(Document document)
        {
	        //add interactivity to the HTML.
	        
            var okButton = (HtmlButtonElement)document.GetElementById("ok");
            var cancelButton = (HtmlButtonElement)document.GetElementById("cancel");

            cancelButton.OnClick += arg =>
            {
	            BeginInvoke(new Action(() => OnCancel?.Invoke(this, EventArgs.Empty)));
	            return true;
            };
            okButton.OnClick += arg =>
            {
                Login = ((HtmlInputElement)document.GetElementById("username")).Value;
                Password = ((HtmlInputElement)document.GetElementById("password")).Value;
                BeginInvoke(new Action(() => OnOk?.Invoke(this, EventArgs.Empty)));
                return true;
            };
        }

        public string Login;
        public string Password;
        
        public event EventHandler OnOk;

        public event EventHandler OnCancel;

    }
}