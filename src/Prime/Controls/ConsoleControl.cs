using System;
using System.Collections.Generic;
using System.Text;
using Knyaz.Optimus;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.Dom.Interfaces;
using Knyaz.Optimus.ResourceProviders;
using Prime.HtmlView;
using HtmlElement = Knyaz.Optimus.Dom.Elements.HtmlElement;

namespace Prime.Controls
{
    /// <summary>
    /// Shows the output of the Html browser control.
    /// </summary>
    public partial class ConsoleControl : HtmlUserControl, IConsole
    {
        public ConsoleControl()
        {
            InitializeComponent();
//            _engine.Scripting.ScriptExecutionError+=ScriptingOnScriptExecutionError;
            //_engine.PreHandleResponse+=EngineOnPreHandleResponse;
        }

        /*private void EngineOnPreHandleResponse(object sender, ResponseEventArgs e)
        {
            if (e.Response is HttpResponse http && http.StatusCode == HttpStatusCode.NotFound)
            {
                var node = (HtmlElement)Document.CreateElement("p");
                node.ClassName = "error";
                node.TextContent = "Error loading: " + http.Uri;
                Document.Body.AppendChild(node);    
            }
        }*/


        private void ScriptingOnScriptExecutionError(Script arg1, Exception ex)
        {
            var node = (HtmlElement)Document.CreateElement("p");
            node.ClassName = "error";
            node.TextContent = ex.Message;
            Document.Body.AppendChild(node);
        }

        enum LogMessageType
        {
	        Info,
	        Error, 
	        Warning
        }
        
        
        private void WriteMessage(LogMessageType type, object obj)
        {
            var node = Document.CreateElement("p");
            node.TextContent = obj?.ToString() ?? "<null>";
            Document.Body.AppendChild(node);
        }

        protected override string GetTemplate() =>
            "<html><head><style>.error{color:red}</style></head><body></body></html>";

        
        #region .    IConsole    .

        private static string FormatMessage(string format, object[] objs)
        {
	        if(string.IsNullOrEmpty(format))
		        return format;
			
	        var builder = new StringBuilder();
	        var parts = format.Split('%');
	        builder.Append(parts[0]);
	        for (var idx = 0; idx < parts.Length - 1; idx++)
	        {
		        if (idx < objs.Length)
		        {
			        builder.Append(objs[idx]);
		        }

		        builder.Append(parts[idx + 1]);
	        }

	        return builder.ToString();
        }
        
        public void Assert(bool assertion, params object[] objs)
        {
	        if(!assertion)
		        WriteMessage(LogMessageType.Error, objs);
        }

        public void Assert(bool assertion, string format, params object[] objs)
        {
	        if(!assertion)
		        WriteMessage(LogMessageType.Error, FormatMessage(format, objs));
        }

        public void Clear() => Document.Body.InnerHTML = "";

        public void Error(params object[] objs) =>
	        WriteMessage(LogMessageType.Error, objs);

        public void Error(string format, params object[] objs) => 
	        WriteMessage(LogMessageType.Error, FormatMessage(format, objs));

        public void Group()
        {
	        //todo: implement grouping
        }

        public void Group(string label) { }

        public void GroupCollapsed() { }

        public void GroupEnd() { }

        public void Info(params object[] objs) => WriteMessage(LogMessageType.Info, objs);

        public void Info(string format, params object[] objs) =>
	        WriteMessage(LogMessageType.Info, FormatMessage(format, objs));

        public void Log(string format, params object[] objs) => WriteMessage(LogMessageType.Info, objs);

        public void Log(params object[] objs) =>
	        WriteMessage(LogMessageType.Info, objs);

        readonly Dictionary<string, DateTime> _timers = new Dictionary<string, DateTime>();
        
        public void Time(string label) => _timers[label] = DateTime.Now;

        public void TimeEnd(string label)
        {
	        if (_timers.TryGetValue(label, out var time))
	        {
		        WriteMessage(LogMessageType.Info, label + ":" + (DateTime.Now - time).TotalMilliseconds);
		        _timers.Remove(label);
	        }
        }
        public void TimeLog(string label)
        {
	        if (_timers.TryGetValue(label, out var time))
		        WriteMessage(LogMessageType.Info, label + ":" + (DateTime.Now - time).TotalMilliseconds);
        }

        public void Warn(params object[] objs) => WriteMessage(LogMessageType.Warning, objs);

        public void Warn(string format, params object[] objs) =>
	        WriteMessage(LogMessageType.Warning, FormatMessage(format, objs));
        #endregion // IConsole
    }
}