using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Knyaz.Optimus;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.ResourceProviders;
using Prime.Styles;
using System.Linq;

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

        private async void InitView()
        {
            Cursor = Cursors.WaitCursor;

            //Create engine with custom resource provider to load the template provided by HtmlUserControl inheritor.

            _engine = EngineBuilder.New()
	            .EnableCss(css => css.UserAgentStyleSheet = StyleSheetFactory.Instance.GetBrowserDefault())
	            .SetResourceProvider(_resourceProvider)
	            .Build();
            
            var result = await _engine.OpenUrl("file://" + GetType().FullName);
            
            OnInitDocument(result.Document);

            _documentView.ResourceLocator = url =>
            {
                //todo: expose some helper methods from Optimus;
                var lp = new LinkProvider { Root = GetRoot(_engine.Uri) };
                return _engine.ResourceProvider.SendRequestAsync(new Request(lp.MakeUri(url))).Result.Stream;
            };
            _documentView.Document = result.Document;
            

            Cursor = Cursors.Default;
        }

        internal static string GetRoot(Uri uri)
        {
            var root = uri.GetLeftPart(UriPartial.Path);
            var ur = new Uri(root);
            if (ur.PathAndQuery != null && !ur.PathAndQuery.Contains('.') && ur.PathAndQuery.Last() != '/')
                return root + "/";

            return root;
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