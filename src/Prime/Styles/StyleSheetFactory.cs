using System;
using System.IO;
using Knyaz.Optimus.Dom.Css;

namespace Prime.Styles
{
	internal class StyleSheetFactory
	{
		public static StyleSheetFactory Instance = new StyleSheetFactory();
		
		private Lazy<CssStyleSheet> _browserDefault = new Lazy<CssStyleSheet>(
			() => StyleSheetBuilder.CreateStyleSheet(new StreamReader(R.DefaultCss)));//todo: make the style sheet readonly somehow

		public CssStyleSheet GetBrowserDefault() => _browserDefault.Value;
	}
}