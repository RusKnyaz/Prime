using System.Drawing;
using System.Windows.Forms;

namespace Prime.DevTools
{
	public partial class LayoutInfoControl : UserControl
	{
		private Rectangle _rect;

		public LayoutInfoControl()
		{
			InitializeComponent();
		}

		public Rectangle Rect
		{
			get { return _rect; }
			set
			{
				_rect = value;
				_layoutInfoLabel.Text = string.Format("Rect: {0}; {1}; {2}; {3};", value.X, value.Y, value.Width, value.Height);
			}
		}
	}
}
