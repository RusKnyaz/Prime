using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using Prime.Model;

namespace Knyaz.Optimus.WinForms
{
    public partial class BrowserControl : UserControl
    {
        private BrowserStates _state;

        public readonly Browser Browser = new Browser();
        public Engine Engine => Browser.Engine;

        public BrowserControl()
        {
            InitializeComponent();
            _documentView.AutoScroll = true;
            Engine.DocumentChanged += Engine_DocumentChanged;

            Browser.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "State")
                    StateChanged?.Invoke(this, EventArgs.Empty);
            };
        }

        private void Engine_DocumentChanged()
        {
            _documentView.Document = Engine.Document;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            var rect = ClientRectangle;
            Engine.CurrentMedia.Width = rect.Width;
            Engine.CurrentMedia.Landscape = rect.Width > rect.Height;
        }

        public string Url
        {
            set
            {
                var rect = ClientRectangle;
                new Thread(async () =>
                {
                    Engine.CurrentMedia.Width = rect.Width;
                    Engine.CurrentMedia.Landscape = rect.Width > rect.Height;
                    await Browser.OpenUrl(value);
                    _documentView.Document = Engine.Document;
                }).Start();
            }
            get => Engine?.Uri.AbsoluteUri;
        }

        public Rectangle GetRect(Node arg) => _documentView.GetRect(arg);


        public BrowserStates State => Browser.State;

        public void SetHighlight(Rectangle rect)
        {
            //throw new NotImplementedException();
        }

        public event EventHandler<NodeEventArgs> NodeClick;
        public event EventHandler StateChanged;
    }

    public class NodeEventArgs : EventArgs
    {
        public readonly Node Node;

        public NodeEventArgs(Node node)
        {
            Node = node;
        }
    }

    public enum BrowserStates
    {
        /// <summary>
        /// No document available
        /// </summary>
        None,
        /// <summary>
        /// Document is ready
        /// </summary>
        Ready,
        /// <summary>
        /// There are resources to load.
        /// </summary>
        Loading,
        Error
    }
}
