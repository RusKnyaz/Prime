using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Knyaz.Optimus;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.ResourceProviders;

namespace Prime.HtmlView
{
    /// <summary>
    /// The class used to create UI based on HTML markup. 
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

            var html = GetTemplate(); // Get the user control html markup.
            
            //Create engine with custom resource provider to load the template provided by HtmlUserControl inheritor.
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
        
        /// <summary>
        /// The custom resource provider that always returns static content.
        /// </summary>
        internal class StaticResourceProvider : IResourceProvider
        {
	        public StaticResourceProvider(string data) =>
		        Data = data;

	        public string Data { get; }

	        public Task<IResource> SendRequestAsync(Request request) =>
		        Task.Run(() => (IResource)new Response("text/html", new MemoryStream(Encoding.UTF8.GetBytes(Data))));
        }
    }
}