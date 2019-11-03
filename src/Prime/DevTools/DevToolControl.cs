using System;
using System.Drawing;
using System.Windows.Forms;
using Knyaz.Optimus;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.TestingTools;
using HtmlElement = Knyaz.Optimus.Dom.Elements.HtmlElement;

namespace Prime.DevTools
{
	public partial class DevToolControl : UserControl
	{
		private readonly Func<Node, Rectangle> _getRect;
		private Engine _engine;

		public DevToolControl(Func<Node, Rectangle> getRect)
		{
			_getRect = getRect;
			InitializeComponent();
			domTreeControl1.NodeSelected += DomTreeControl1_NodeSelected;
		}

		private void DomTreeControl1_NodeSelected(TreeNode obj)
		{
			if (obj == null)
				return;

			var node = obj.Tag as Node;
			if(node != null)
				layoutInfoControl1.Rect = _getRect(node);

			var elt = obj.Tag as HtmlElement;
			if (elt != null)
				computedStyleControl1.Style = elt.GetComputedStyle();
		}

		public Engine Engine
		{
			get { return _engine; }
			set
			{
				_engine = value;
				domTreeControl1.Engine = value;
			}
		}
		
		

		public DomTreeControl DomTreeControl { get { return domTreeControl1; } }
	}
}
