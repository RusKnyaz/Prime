using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using System;
using System.Drawing;
using System.Windows.Forms;
using Knyaz.Optimus.WinForms;

namespace Prime.HtmlView
{
	/// <summary>
	/// Shows the HTML document represented by <see cref="Document"/>.
	/// </summary>
	public partial class HtmlDocumentView : UserControl
	{
		private OptimusGraphicsRenderer _renderer;
		private Document _document;
		private Exception _exception;

		public HtmlDocumentView()
		{
			InitializeComponent();
			panel1.KeyPress += BrowserControl_KeyPress;
			SetDoubleBuffered(panel1);
		}
		
		public static void SetDoubleBuffered(System.Windows.Forms.Control c)
		{
			//Taxes: Remote Desktop Connection and painting
			//http://blogs.msdn.com/oldnewthing/archive/2006/01/03/508694.aspx
			if (System.Windows.Forms.SystemInformation.TerminalServerSession)
				return;

			System.Reflection.PropertyInfo aProp = 
				typeof(System.Windows.Forms.Control).GetProperty(
					"DoubleBuffered", 
					System.Reflection.BindingFlags.NonPublic | 
					System.Reflection.BindingFlags.Instance);

			aProp.SetValue(c, true, null); 
		}

		public Document Document
		{
			get => _document;
			set
			{
				if (_document == value)
					return;

				if(_document != null)
				{
					_renderer?.Dispose();
					_renderer = null;
					//timer1.Enabled = false;
				}

				_document = value;

				if(_document != null)
				{
					//timer1.Enabled = true;
					_renderer = new OptimusGraphicsRenderer(_document);
				}
			}
		}

		public event EventHandler<NodeEventArgs> NodeClick;

		private void BrowserControl_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (Document != null && Document.ActiveElement is HtmlInputElement elt)
			{
				elt.Value += e.KeyChar;
				var evt = elt.OwnerDocument.CreateEvent("Event");
				evt.InitEvent("change", false, false);
				elt.DispatchEvent(evt);
				panel1.Invalidate();
			}
		}

		private void On_MouseClick(object sender, MouseEventArgs e)
		{
			panel1.Focus();
			
			_renderer?.HitTest(e.X, e.Y, (rectangle, node) =>
			{
				NodeClick?.Invoke(this, new NodeEventArgs(node));
				return false;
			});
		}

		private void _updateTimer_Tick(object sender, EventArgs e)
		{
			var doc = Document;
			if (_renderer != null && _renderer.IsDirty && doc != null)
			{
				Relayout();
			}
		}

		protected override void OnResize(EventArgs e)
		{
			if (_renderer != null)
				Relayout();
		}

		private void Relayout()
		{
			var cr = ClientRectangle;
			var sz = _renderer.Relayout(cr);
			if (!sz.IsEmpty)
				panel1.Size = new Size(Math.Max(sz.Width, cr.Width), Math.Max(sz.Height, cr.Height));
			panel1.Invalidate();
		}

		public Rectangle GetRect(Node node)
		{
			var area = _renderer.FindArea(node);
			return area != null ? new Rectangle(area.Item1.Left, area.Item1.Top, area.Item1.Width, area.Item1.Height) : Rectangle.Empty;
		}

		private void panel1_Paint(object sender, PaintEventArgs e)
		{
			var renderer = _renderer;
			if (renderer == null)
				return;

			if (_exception != null)
			{
				using (var font = new Font("Arial", 12))
				using (var brush = new SolidBrush(Color.Black))
				{
					e.Graphics.DrawString(_exception.ToString(), font, brush, 0, 0);
				}
				return;
			}

			try
			{
				if (renderer.IsDirty)
				{
					Relayout();
					return;
				}
				
				renderer.Render(e.Graphics, ClientRectangle);
				
				if (!_highlight.IsEmpty)
				{
					using (var brush = new SolidBrush(Color.FromArgb(30, Color.CornflowerBlue)))
						e.Graphics.FillRectangle(brush, _highlight);

					using (var pen = new Pen(Color.CornflowerBlue))
						e.Graphics.DrawRectangle(pen, _highlight);
				}
			}
			catch (Exception ex)
			{

			}
		}

		private Rectangle _highlight;
		public void SetHighlight(Rectangle rect)
		{
			var old = _highlight;
			_highlight = rect;
			if (!old.IsEmpty)
				panel1.Invalidate(old);
			if (!_highlight.IsEmpty)
				panel1.Invalidate(_highlight);
		}
	}
}
