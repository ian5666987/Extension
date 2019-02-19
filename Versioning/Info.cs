using System.Reflection;
using System.Diagnostics;

namespace Extension.Versioning
{
  public class Info
  {
		private static Assembly assembly = Assembly.GetExecutingAssembly();
		private static FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);

		public static string ProductVersion { get { return fvi.ProductVersion; } }
		
		public static string FileVersion { get { return fvi.FileVersion; } }

    public static string GetProductVersionFor(Assembly assembly) {
      FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
      return fvi.ProductVersion;
    }

    public static string GetFileVersionFor(Assembly assembly) {
      FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
      return fvi.FileVersion;
    }
  }
}
