using System;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using Prime.HtmlView;
using Prime.Properties;

namespace Prime.Controls
{
	public class SettingsControl : HtmlUserControl
	{
		public SettingsControl()
		{
			AddResource("Prime.Res.bootstrap.min.css", "text/css", () => R.BootstrapMinCss);
		}
		
		protected override void OnInitDocument(Document document)
		{
			var jsEngineSelector = (HtmlSelectElement)document.GetElementById("jsEngine");
			jsEngineSelector.Value = Settings.Default.JsEngine;


			var btnSave = (HtmlButtonElement)document.GetElementById("btnSave");
			btnSave.OnClick += evt =>
			{
				Settings.Default.JsEngine = jsEngineSelector.Value;
				BeginInvoke(new Action(() => OnSave?.Invoke(this, EventArgs.Empty)));
				return true;
			};
		}

		public event EventHandler OnSave;
	}
}