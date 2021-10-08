using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.Graphics;
using Knyaz.Optimus.Graphics.Layout;
using Microsoft.FSharp.Core;
using HtmlElement = Knyaz.Optimus.Dom.Elements.HtmlElement;


namespace Knyaz.Optimus.WinForms
{
	public class OptimusGraphicsRenderer : IDisposable
	{
		class LayoutService : ILayoutService
		{
			private readonly OptimusGraphicsRenderer _renderer;

			public LayoutService(OptimusGraphicsRenderer renderer) => _renderer = renderer;
			public RectangleF[] GetElementBounds(Element element)
			{
				var layout = _renderer._layout;
				if (layout == null)
					return new RectangleF[0];

				return layout.Where(x => x?.Item2 is Types.RenderItem.Element renderItem && renderItem.Item.Node == element)
					.Select(x => new RectangleF(x.Item1.Left, x.Item1.Top, x.Item1.Width, x.Item1.Height)).ToArray();
			}
		}
		
		private static System.Drawing.Graphics _measureGraphics = System.Drawing.Graphics.FromImage(new Bitmap(1, 1));
		
		private Document _document = null;
        private bool _isDocumentDirty = true;

        public OptimusGraphicsRenderer(Document document)
		{
			_document = document;
			((HtmlDocument)_document).AttachLayoutService(new LayoutService(this));
			document.NodeInserted += OnNodeInserted;
			document.NodeRemoved += OnNodeRemoved;
		}

		public void Dispose()
		{
			_document.NodeInserted -= OnNodeInserted;
			_document.NodeRemoved -= OnNodeRemoved;
			_document = null;
		}

		public bool IsDirty(RectangleF rect) => _isDocumentDirty || !_lastBounds.Equals(rect);

		private void OnNodeRemoved(Node arg1, Node arg2) => _isDocumentDirty = true;

		void OnNodeInserted(Node obj) => _isDocumentDirty = true;

		private RectangleF _lastBounds;
        private IEnumerable<Tuple<Rectangle, Types.RenderItem>> _layout;

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

		static Size MeasureString(Types.FontInfo fontInfo, string text)
		{
			var key = new Tuple<Types.FontInfo, string>(fontInfo, text);
			if (_measureResults.TryGetValue(key, out var result))
				return result;
			
			var font = GetFont(fontInfo);
			var siz = _measureGraphics.MeasureString(text, font);
			result = new Size((int)Math.Ceiling(siz.Width), (int)Math.Ceiling(siz.Height));
			_measureResults.Add(key, result);
			return result;
		}
		
		static Dictionary<Tuple<Types.FontInfo,string>, Size> _measureResults = 
			new Dictionary<Tuple<Types.FontInfo, string>, Size>(new TupleComparer<Types.FontInfo, string>());

		class TupleComparer<T1, T2> : IEqualityComparer<Tuple<T1, T2>>
		{
			public bool Equals(Tuple<T1, T2> x, Tuple<T1, T2> y) => Equals(x.Item1, y.Item1) && Equals(x.Item2, y.Item2);
			public int GetHashCode(Tuple<T1, T2> obj) => obj.Item1.GetHashCode() ^ obj.Item2.GetHashCode();
		}
		
		//todo: dispose fonts
		
		static ConcurrentDictionary<Types.FontInfo, Font> _fonts = new ConcurrentDictionary<Types.FontInfo, Font>();

		private static Font GetFont(Types.FontInfo fontInfo)
		{
			if (_fonts.TryGetValue(fontInfo, out var font))
				return font;

			font = new Font(fontInfo.Name, fontInfo.Size, fontInfo.Style);
			_fonts.TryAdd(fontInfo, font);
			return font;
		}

		private FSharpFunc<Types.FontInfo, FSharpFunc<string, Size>> FSharpMeasureString =
			FSharpFunc<Types.FontInfo, FSharpFunc<string, Size>>.FromConverter(input =>
				FSharpFunc<string, Size>.FromConverter(s => MeasureString(input, s))
			);


		private bool _layoutInProgress = false;
		
		public Size Relayout(RectangleF bounds)
		{
			if (_document?.Body != null)
			{
				if (_document.ReadyState != "complete")
					return Size.Empty;

                if (_isDocumentDirty || _lastBounds != bounds || _layout == null)
                {
	                _isDocumentDirty = false;
	                _layoutInProgress = true;
	                try
	                {
		                var lfun = OptimusLayout.Layout(new Types.LayoutSettings(true, FSharpMeasureString),
			                _document.Body);
		                _layout = lfun.Invoke(bounds.Size.ToSize()).ToList();
	                }
	                finally
	                {
		                _layoutInProgress = false;    
	                }
					_lastBounds = bounds;
				}
			}

			return GetSize();
		}

		public void Render(System.Drawing.Graphics graphics)
		{
			if (_layoutInProgress)
			{
				using(var font = new Font("Arial", 16, FontStyle.Bold))
				using(var brush = new SolidBrush(Color.Blue))
					graphics.DrawString("Processing...", font , brush, 10,10);
				return;
			}
			
			var sz = GetSize();
			if (!sz.IsEmpty)
			{
				var clipBounds = new Rectangle(
					(int)graphics.VisibleClipBounds.X,
					(int)graphics.VisibleClipBounds.Y,
					(int)graphics.VisibleClipBounds.Width,
					(int)graphics.VisibleClipBounds.Height); 
				graphics.PageUnit = GraphicsUnit.Pixel;
				OptimusRenderer.Render(_layout.Where(x => x.Item1.Bottom > clipBounds.Top && x.Item1.Top < clipBounds.Bottom)
					, graphics);
			}
		}

		public void HitTest(int x, int y, Func<Rectangle, Node, bool> handler)
		{
			if (_layout == null)
				return;
			
			var point = new Point(x, y);

			var hitArea = _layout.LastOrDefault(area => area.Item1.Contains(point));

			if (hitArea == null)
				return;
			
			HtmlElement elt = null;

			if (hitArea.Item2 is Types.RenderItem.Element qwe)
			{
				elt = qwe.Item.Node;
			}
			else if(hitArea.Item2 is Types.RenderItem.Text text)
			{
				elt = text.Item.Node.ParentNode as HtmlElement;
			}

			if (elt != null)
			{
				if (!handler(hitArea.Item1, elt))
				{
					elt.OwnerDocument.ActiveElement = elt;
					elt.Click();
				}
			}
		}

		public Tuple<Rectangle, Types.RenderItem> FindArea(Node node)
		{
			return _layout? .FirstOrDefault(x => x?.Item2 is Types.RenderItem.Element renderItem && renderItem.Item.Node == node);
		}

		public void Invalidate() => _isDocumentDirty = true;
	}
}
