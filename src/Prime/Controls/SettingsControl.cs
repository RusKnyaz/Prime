using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using Prime.HtmlView;
using Prime.Properties;

namespace Prime.Controls
{
	public class SettingsControl : HtmlUserControl
	{
		protected override void OnInitDocument(Document document)
		{
			var jsEngineSelector = (HtmlSelectElement)document.GetElementById("jsEngine");
			jsEngineSelector.Value = Settings.Default.JsEngine;


			var btnSave = (HtmlButtonElement)document.GetElementById("btnSave");
			btnSave.OnClick += evt =>
			{
				Settings.Default.JsEngine = jsEngineSelector.Value;
				return true;
			};
		}
	}
}