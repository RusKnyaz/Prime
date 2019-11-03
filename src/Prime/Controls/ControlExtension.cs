using System;
using System.Windows.Forms;

namespace Prime
{
	public static class ControlExtension
	{
		public static void SafeBeginInvoke(this Control obj, Action action)
		{
			if (obj.InvokeRequired)
			{
				var args = new object[0];
				obj.BeginInvoke(action, args);
			}
			else
			{
				action();
			}
		}

		public static void SafeInvoke(this Control obj, Action action)
		{
			if (obj.InvokeRequired)
			{
				var args = new object[0];
				obj.Invoke(action, args);
			}
			else
			{
				action();
			}
		}
		
		/// <summary>
		/// Creates form that filled by the given control.
		/// </summary>
		/// <param name="control"></param>
		public static Form PopupForm(this Control control)
		{
			control.Dock = DockStyle.Fill;
			return new Form {Controls = {control}};
		}
	}
}
