using System;
using System.Net;
using Knyaz.Optimus;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.ResourceProviders;
using Prime.HtmlView;

namespace Prime.Controls
{
    /// <summary>
    /// Shows the output of the Html browser control.
    /// </summary>
    public partial class ConsoleControl : HtmlUserControl
    {
        private Engine _engine;
        
        public ConsoleControl(Engine engine)
        {
            InitializeComponent();
            _engine = engine;
            _engine.Scripting.ScriptExecutionError+=ScriptingOnScriptExecutionError;
            _engine.Console.OnLog += ConsoleOnOnLog;
            _engine.PreHandleResponse+=EngineOnPreHandleResponse;
        }

        private void EngineOnPreHandleResponse(object sender, ResponseEventArgs e)
        {
            if (e.Response is HttpResponse http && http.StatusCode == HttpStatusCode.NotFound)
            {
                var node = (HtmlElement)Document.CreateElement("p");
                node.ClassName = "error";
                node.TextContent = "Error loading: " + http.Uri;
                Document.Body.AppendChild(node);    
            }
        }


        private void ScriptingOnScriptExecutionError(Script arg1, Exception ex)
        {
            var node = (HtmlElement)Document.CreateElement("p");
            node.ClassName = "error";
            node.TextContent = ex.Message;
            Document.Body.AppendChild(node);
        }

        private void ConsoleOnOnLog(object obj)
        {
            var node = Document.CreateElement("p");
            node.TextContent = obj?.ToString() ?? "<null>";
            Document.Body.AppendChild(node);
        }

        protected override string GetTemplate() =>
            "<html><head><style>.error{color:red}</style></head><body></body></html>";
  }
}