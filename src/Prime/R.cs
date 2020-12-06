using System.IO;
using System.Reflection;

namespace Prime
{
	/// <summary> Embedded resources </summary>
	internal class R
	{
		public static Stream DefaultCss =>
			Assembly.GetExecutingAssembly().GetManifestResourceStream("Prime.Res.moz_default.css");

		public static Stream BootstrapMinCss =>
			Assembly.GetExecutingAssembly().GetManifestResourceStream("Prime.Res.bootstrap.min.css");
	}
}