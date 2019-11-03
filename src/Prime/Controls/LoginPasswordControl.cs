using System;
using System.IO;
using System.Windows.Forms;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.Dom.Events;
using Knyaz.Optimus.WinForms;

namespace Prime.Controls
{
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

        protected override string GetTemplate()
        {
            return "<!DOCTYPE html><html><head><style>" + Bootstrap() + "</style></head><body>" +
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
        }

        protected override void OnInitDocument(Document document)
        {
            var okButton = (HtmlButtonElement)document.GetElementById("ok");
            var cancelButton = (HtmlButtonElement)document.GetElementById("cancel");

            cancelButton.OnClick += CancelButton_OnClick;
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
        
        private bool? CancelButton_OnClick(Knyaz.Optimus.Dom.Events.Event arg)
        {
            BeginInvoke(new Action(() => OnCancel?.Invoke(this, EventArgs.Empty)));
            return true;
        }

        public event EventHandler OnOk;

        public event EventHandler OnCancel;

    }
}