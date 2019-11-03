using System;
using System.Windows.Forms;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.WinForms.Tools;

namespace Knyaz.Optimus.WinForms
{
    /// <summary>
    /// Html base user control
    /// </summary>
    public partial class HtmlUserControl : UserControl
    {
        private HtmlDocumentView _documentView;

        private Engine _engine;
        
        public HtmlUserControl()
        {
            InitializeComponent();
            _documentView = new HtmlDocumentView {Dock = DockStyle.Fill};
            InitView();
            Controls.Add(_documentView);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (_engine != null)
            {
                var rect = ClientRectangle;
                _engine.CurrentMedia.Width = rect.Width;
                _engine.CurrentMedia.Landscape = rect.Width > rect.Height;
            }
        }

        private async void InitView()
        {
            Cursor = Cursors.WaitCursor;

            var html = GetTemplate();
            
            _engine = new Engine(new StaticResourceProvider(html))
                { ComputedStylesEnabled = true };

            var rect = ClientRectangle;
            _engine.CurrentMedia.Width = rect.Width;
            _engine.CurrentMedia.Landscape = rect.Width > rect.Height;
			
            var result = await _engine.OpenUrl("http://nourl");

            OnInitDocument(_engine.Document);

            _documentView.Document = _engine.Document;
            
            Cursor = Cursors.Default;
        }

        protected virtual void OnInitDocument(Document document){}

        protected virtual string GetTemplate() => "<html><body></body></html>";

        protected Document Document => _engine.Document;
    }
}