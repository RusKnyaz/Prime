using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.Graphics;
using Microsoft.FSharp.Core;


namespace Knyaz.Optimus.WinForms
{
	public class OptimusGraphicsRenderer : IDisposable
	{
		private static System.Drawing.Graphics _measureGraphics = System.Drawing.Graphics.FromImage(new Bitmap(1, 1));
		
		private Document _document = null;
        private bool _isDocumentDirty = true;

        public OptimusGraphicsRenderer(Document document)
		{
			_document = document;
			document.NodeInserted += OnNodeInserted;
			document.NodeRemoved += OnNodeRemoved;
		}

		public void Dispose()
		{
			_document.NodeInserted -= OnNodeInserted;
			_document.NodeRemoved -= OnNodeRemoved;
			_document = null;
		}

		public bool IsDirty { get { return _isDocumentDirty; } }

		private void OnNodeRemoved(Node arg1, Node arg2) => _isDocumentDirty = true;

		void OnNodeInserted(Node obj) => _isDocumentDirty = true;

		private RectangleF _lastBounds;
        private FSharpFunc<int, IEnumerable<Tuple<Rectangle, Layout.RenderItem>>> _lfun;
        private IEnumerable<Tuple<Rectangle, Layout.RenderItem>> _layout;

		public Size GetSize()
		{
			if(_layout == null)
				return Size.Empty;

			var width = 0;
			var height = 0;
			foreach (var area in _layout.Select(x => x.Item1))
			{
				width = Math.Max(width, area.Left + area.Width);
				height = Math.Max(height, area.Top + area.Height);
			}
			return new Size(width, height);
		}

		static Size MeasureString(Layout.FontInfo fontInfo, string text)
		{
			var key = new Tuple<Layout.FontInfo, string>(fontInfo, text);
			if (_measureResults.TryGetValue(key, out var result))
				return result;
			
			var font = GetFont(fontInfo);
			var siz = _measureGraphics.MeasureString(text, font);
			result = new Size((int)Math.Ceiling(siz.Width), (int)Math.Ceiling(siz.Height));
			_measureResults.Add(key, result);
			return result;
		}
		
		static Dictionary<Tuple<Layout.FontInfo,string>, Size> _measureResults = 
			new Dictionary<Tuple<Layout.FontInfo, string>, Size>(new TupleComparer<Layout.FontInfo, string>());

		class TupleComparer<T1, T2> : IEqualityComparer<Tuple<T1, T2>>
		{
			public bool Equals(Tuple<T1, T2> x, Tuple<T1, T2> y) => Equals(x.Item1, y.Item1) && Equals(x.Item2, y.Item2);
			public int GetHashCode(Tuple<T1, T2> obj) => obj.Item1.GetHashCode() ^ obj.Item2.GetHashCode();
		}
		
		//todo: dispose fonts
		static Dictionary<Layout.FontInfo, Font> _fonts = new Dictionary<Layout.FontInfo, Font>();

		static private Font GetFont(Layout.FontInfo fontInfo)
		{
			if (_fonts.TryGetValue(fontInfo, out var font))
				return font;

			font = new Font(fontInfo.Name, fontInfo.Size, fontInfo.Style);
			_fonts.Add(fontInfo, font);
			return font;
		}

		private FSharpFunc<Layout.FontInfo, FSharpFunc<string, Size>> FSharpMeasureString =
			FSharpFunc<Layout.FontInfo, FSharpFunc<string, Size>>.FromConverter(input =>
				FSharpFunc<string, Size>.FromConverter(s => MeasureString(input, s))
			);
			

		public Size Relayout(RectangleF bounds)
		{
			if (_document != null && _document.Body != null)
			{
				if (_document.ReadyState != "complete")
					return Size.Empty;

                if (_isDocumentDirty || _layout == null)
                {
                    _lfun = OptimusLayout.Layout(new Layout.LayoutSettings(true, FSharpMeasureString), _document.Body);
                }

                if(_isDocumentDirty || _lastBounds != bounds || _layout == null)
                { 
                    _layout = _lfun.Invoke((int)bounds.Right).ToList();
					_lastBounds = bounds;
					_isDocumentDirty = false;
				}
			}

			return GetSize();
		}

		public void Render(System.Drawing.Graphics graphics, RectangleF bounds)
		{
			var sz = Relayout(bounds);
			if (!sz.IsEmpty)
			{
				graphics.PageUnit = GraphicsUnit.Pixel;
				OptimusRenderer.Render(_layout, graphics);
			}
		}

		public void HitTest(int x, int y, Func<Rectangle, Node, bool> handler)
		{
			if (_layout == null)
				return;
			
			OptimusRenderer.HitTest(_layout, x, y, FSharpFunc<Tuple<Rectangle, Layout.RenderItem>, bool>.FromConverter(
				t =>
				{
					HtmlElement elt = null;

					var qwe = t.Item2 as Layout.RenderItem.Element;
					

					if (qwe != null)
					{
						elt = qwe.Item.Node as HtmlElement;
					}
					else
					{
						var text = t.Item2 as Layout.RenderItem.Text;
						if(text != null)
							elt = text.Item.Node.ParentNode as HtmlElement;
					}

					if (elt != null)
					{
						if (!handler(new Rectangle(t.Item1.Left, t.Item1.Top, t.Item1.Width, t.Item1.Height), elt))
						{
							elt.OwnerDocument.ActiveElement = elt;
							elt.Click();
						}
					}

					return true;
				}));
		}

		public Tuple<Rectangle, Layout.RenderItem> FindArea(Node node)
		{
			return _layout? .FirstOrDefault(x => x?.Item2 is Layout.RenderItem.Element renderItem && renderItem.Item.Node == node);
		}
	}
}
