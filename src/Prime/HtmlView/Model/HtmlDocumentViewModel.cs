using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.TestingTools;
using Knyaz.Optimus.WinForms;
using Prime.Annotations;
using Prime.Tools;
using HtmlElement = Knyaz.Optimus.Dom.Elements.HtmlElement;

namespace Prime.Model
{
	internal class HtmlDocumentViewModel : INotifyPropertyChanged
	{
		private OptimusGraphicsRenderer _renderer;

		public Cursor Cursor
		{
			get => _cursor;
			private set
			{
				if (_cursor == value)
					return;
				
				_cursor = value;
				
				OnPropertyChanged();
			} 
		}

		public Action<HitTestResult> ShowComboBox;
		public Action<string> LinkClicked;
		
		private Cursor _cursor;

		public HtmlDocumentViewModel(OptimusGraphicsRenderer renderer)
		{
			_renderer = renderer;
		}

		public void Click(PointF location)
		{
			if (_renderer == null)
				return;
			
			var result = _renderer.HitTest((int)location.X, (int)location.Y);

			var node = result.Elt;
			//todo: do not use native combo box
			if (node is Knyaz.Optimus.Dom.Elements.HtmlSelectElement selectElement && selectElement.Options.Length > 0)
			{
				ShowComboBox?.Invoke(result);
			}
			else if(GetAnchor(node) is HtmlAnchorElement anchorElement
				&& !string.IsNullOrEmpty(anchorElement.Href))
			{
				LinkClicked?.Invoke(anchorElement.Href);
			}
			else if(node is HtmlElement elt)
			{
				elt.OwnerDocument.ActiveElement = elt;
				elt.Click();
			}
		}

		private HtmlAnchorElement GetAnchor(HtmlElement elt) =>
			elt.GetRecursive(x => x.ParentNode as HtmlElement).OfType<HtmlAnchorElement>().FirstOrDefault();
		
		public void Hover(Point location)
		{
			var cursor = Cursors.Default;

			var result = _renderer?.HitTest(location.X, location.Y);
			if (result.IsNone)
				return;

			var node = result.Elt;
				
			if (GetAnchor(node) != null) //todo: search in parents.
			{
				cursor = Cursors.Hand;
			}
			else if (node is Knyaz.Optimus.Dom.Elements.HtmlElement htmlElement)
			{
				var style = htmlElement.GetComputedStyle();
				var cursorValue = style.GetPropertyValue("cursor"); 
				switch (cursorValue)
				{
					case "default":
						cursor = Cursors.Default;
						break;
					case "crosshair":
						cursor = Cursors.Cross;
						break;
					case "help":
						cursor = Cursors.Help;
						break;
					case "move":
						cursor = Cursors.SizeAll;
						break;
					case "pointer":
						cursor = Cursors.Hand;
						break;
					case "wait":
					case "progress":
						cursor = Cursors.WaitCursor;
						break;
					case "text":
						cursor = Cursors.IBeam;
						break;
					case "n-resize":
						cursor = Cursors.UpArrow;
						break;
					case "ne-resize":
						cursor = Cursors.PanNE;
						break;
					case "e-resize": cursor = Cursors.PanEast;
						break;
					case "se-resize": cursor = Cursors.PanSE;
						break;
					case "s-resize": cursor = Cursors.PanSouth;
						break;
					case "sw-resize": cursor = Cursors.PanSW;
						break;
					case "w-resize": cursor = Cursors.PanWest;
						break;
					case "nw-resize": cursor = Cursors.PanNW;
						break;
				}
			}
			
			Cursor = cursor;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}