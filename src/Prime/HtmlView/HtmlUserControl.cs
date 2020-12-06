using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Knyaz.Optimus;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.ResourceProviders;
using Prime.Styles;

namespace Prime.HtmlView
{
    /// <summary>
    /// The class used to create UI based on HTML markup. 
    /// </summary>
    public partial class HtmlUserControl : UserControl
    {
        private readonly HtmlDocumentView _documentView;
        private readonly EmbeddedResourceProvider _resourceProvider;

        private Engine _engine;
        
        public HtmlUserControl()
        {
	        InitializeComponent();
	        
	        _resourceProvider = new EmbeddedResourceProvider();
	        AddResource(GetType().FullName, "text/html", () => GetType().Assembly.GetManifestResourceStream(GetType().FullName));
	        
            _documentView = new HtmlDocumentView {Dock = DockStyle.Fill};
            InitView();
            Controls.Add(_documentView);
        }

        
        public void AddResource( string path, string mimeType, Func<Stream> getStream) => 
	        _resourceProvider.AddResource(path,  mimeType, getStream);

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

            //Create engine with custom resource provider to load the template provided by HtmlUserControl inheritor.

            _engine = EngineBuilder.New()
	            .EnableCss(css => css.UserAgentStyleSheet = StyleSheetFactory.Instance.GetBrowserDefault())
	            .SetResourceProvider(_resourceProvider)
	            .Build();
            
            var rect = ClientRectangle;
            _engine.CurrentMedia.Width = rect.Width;
            _engine.CurrentMedia.Landscape = rect.Width > rect.Height;
			
            var result = await _engine.OpenUrl("file://" + GetType().FullName);
            
            OnInitDocument(result.Document);

            _documentView.Document = result.Document;
            
            Cursor = Cursors.Default;
        }

        protected virtual void OnInitDocument(Document document){}

       
        protected Document Document => _engine.Document;
        
        /// <summary> The custom resource provider that always returns static content. </summary>
        private class EmbeddedResourceProvider : IResourceProvider
        {
	        public Task<IResource> SendRequestAsync(Request request) =>
		        Task.Run(() =>
		        {
			        if(_resources.TryGetValue(request.Url, out var resource))
						return (IResource) new Response(resource.MimeType, resource.Stream());
			        return null;
		        });


	        private readonly IDictionary<Uri, Resource> _resources = new Dictionary<Uri, Resource>();
	        
	        public void AddResource(string path, string mimeType, Func<Stream> streamFn)
	        {
		        _resources.Add(new Uri("file://"+path), new Resource(mimeType, streamFn));
	        }
	        
	        class Resource
	        {
		        public readonly string MimeType;
		        public readonly Func<Stream> Stream;

		        public Resource(string mimeType, Func<Stream> streamFn)
		        {
			        MimeType = mimeType;
			        Stream = streamFn;
		        }
	        }
        }
    }
}