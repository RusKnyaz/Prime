using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Knyaz.Optimus.Dom.Css;
using Knyaz.Optimus.Dom.Events;
using Knyaz.Optimus.Environment;
using Knyaz.Optimus.WinForms;
using Prime.Model;

namespace Prime.HtmlView
{
	/// <summary>
	/// Shows the HTML document represented by <see cref="Document"/>.
	/// </summary>
	public partial class HtmlDocumentView : UserControl
	{
		private readonly ComboBox _comboBox;
		
		private OptimusGraphicsRenderer _renderer;
		private HtmlDocumentViewModel _model;
		private Document _document;
		private Exception _exception;

		public EventHandler<LinkClickedEventArgs> LinkClicked;

		public HtmlDocumentView()
		{
			InitializeComponent();
			panel1.KeyPress += BrowserControl_KeyPress;
			SetDoubleBuffered(panel1);
			_comboBox = new ComboBox() { Visible = false};
			Controls.Add(_comboBox);
			_comboBox.BringToFront();
			
			panel1.MouseMove+=Panel1OnMouseMove;
		}

		private void Panel1OnMouseMove(object sender, MouseEventArgs e)
		{
			_model?.Hover(e.Location);
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

				if (_model != null)
				{
					_model.PropertyChanged-= ModelOnPropertyChanged;
					_model = null;
				}

				_document = value;

				if(_document != null)
				{
					//timer1.Enabled = true;
					_renderer = new OptimusGraphicsRenderer(_document);
					_model = new HtmlDocumentViewModel(_renderer, _document);
					_model.PropertyChanged+= ModelOnPropertyChanged;
					_model.LinkClicked += s => LinkClicked?.Invoke(this, new LinkClickedEventArgs(s)); 
					_model.ShowComboBox+= ShowComboBox;
				}
			}
		}

		private void ShowComboBox(HitTestResult result)
		{
			//todo: do not use native combo box
			var selectElement = (HtmlSelectElement)result.Elt;
			_comboBox.DataSource = selectElement.Options.ToList();
			_comboBox.DisplayMember = nameof(HtmlOptionElement.Text);
			_comboBox.SelectedItem = selectElement.SelectedOptions.FirstOrDefault();
			_comboBox.SelectionChangeCommitted += (o, args) =>
			{
				_comboBox.Visible = false;
				_renderer.Invalidate();
				Invalidate();
				selectElement.SelectedOptions.Clear();
				selectElement.SelectedOptions.Add((HtmlOptionElement)_comboBox.SelectedItem);
			};
			_comboBox.KeyUp += (o, args) =>
			{
				if (args.KeyCode == Keys.Escape)
				{
					_comboBox.Visible = false;
					Invalidate();
				}
			};
			var rectangle = result.Rect;
			_comboBox.Left = rectangle.Left;
			_comboBox.Top = rectangle.Top;
			_comboBox.Width = rectangle.Width;
			_comboBox.Height = rectangle.Height;
			var style = selectElement.OwnerDocument.DefaultView.GetComputedStyle(selectElement);
			_comboBox.BackColor = ColorTranslator.FromHtml((string)style[Css.Background]);
			_comboBox.ForeColor = ColorTranslator.FromHtml((string)style[Css.Background]);
			var fontFamily = ((string) style[Css.FontFamily]).Split(',')[0].Trim();
			var fontSize = float.TryParse((string) style[Css.FontSize], out var sz) ? sz : 12;
			_comboBox.Font = new Font(fontFamily, fontSize);
			_comboBox.Visible = true;
			_comboBox.BringToFront();
			Invalidate();
		}

		private void ModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(HtmlDocumentViewModel.Cursor))
			{
				Cursor = _model.Cursor;
			}
		}

		public event EventHandler<NodeEventArgs> NodeClick;

		private void BrowserControl_KeyPress(object sender, KeyPressEventArgs e)
		{
			_model?.KeyPress(e.KeyChar);
		}

		private void On_MouseClick(object sender, MouseEventArgs e)
		{
			panel1.Focus();
			
			_model.Click(e.Location);
		}

		private async void _updateTimer_Tick(object sender, EventArgs e)
		{
			var doc = Document;
			if (_renderer != null && _renderer.IsDirty(ClientRectangle) && doc != null)
			{
				await Relayout();
			}
		}

		protected override async void OnResize(EventArgs e)
		{
			base.OnResize(e);
			
			if (_renderer != null)
				await Relayout();

			if (_document != null)
			{
				
				var window = (Window)_document.DefaultView;
				var resizeEvent = window.NewUiEvent("resize",
					new UiEventInitOptions {Bubbles = true, Cancelable = false}); 
				window.DispatchEvent(resizeEvent);
				var rect = ClientRectangle;
				window.CurrentMedia.Width = rect.Width;
				window.CurrentMedia.Landscape = rect.Width > rect.Height;
			}
		}

		private bool _rendering = false;
		private async Task Relayout()
		{
			if (_rendering)
				return;
			
			var cr = ClientRectangle;
			//hack to avoid scrollbars flickering. Todo: revise it.
			cr.Width-=10;
			cr.Height-=10;
			var oldCursor = Cursor;
			Cursor = Cursors.WaitCursor;
			try
			{
				_rendering = true;
				var sz = await Task.Run(() => _renderer.Relayout(cr));
				if (!sz.IsEmpty)
					panel1.Size = new Size(Math.Max(sz.Width, cr.Width), Math.Max(sz.Height, cr.Height));
			}
			finally
			{
				_rendering = false;
				Cursor = oldCursor;	
			}

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
				renderer.Render(e.Graphics);
				
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
				_exception = ex;
			}
			
			if(_comboBox.Visible)
				_comboBox.Invalidate();
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
	
	public class NodeEventArgs : EventArgs
	{
		public readonly Node Node;

		public NodeEventArgs(Node node)
		{
			Node = node;
		}
	}

	public class LinkClickedEventArgs : EventArgs
	{
		public readonly string Href;

		public LinkClickedEventArgs(string href) => Href = href;
	}
}
