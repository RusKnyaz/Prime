using System;
using System.IO;
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
        }
        
        string Bootstrap () 
        {
            using (var reader = new StreamReader(typeof(LoginPasswordForm).Assembly.GetManifestResourceStream("Prime.Res.bootstrap.min.css")))
                return reader.ReadToEnd();
        }

        protected override string GetTemplate() =>
	        "<!DOCTYPE html><html><head><style>" + Bootstrap() + "</style></head><body>" +
	        "<div class=container>" +
	        "<h2>Please enter your login and password</h2>" +
	        "<div class='form-group'>" +
	        "<label for=username>User name</label>" +
	        "<input type=text id=username class='form-control'/><br/>" +
	        "</div>" +
	        "<div class='form-group'>" +
	        "<label for=password>Password</label>" +
	        "<input type=password id=password class='form-control'/><br/>" +
	        "</div>" +
	        "<button id=ok class='btn btn-primary'>OK</button>" +
	        "<button id=cancel class='btn btn-secondary'>Cancel</button>" +
	        "</div>" +
	        "</body></html>";

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