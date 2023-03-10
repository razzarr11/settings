using System;
using System.Runtime.InteropServices;

namespace SharpDX.XInput
{
	internal partial class XInput910 : IXInput
	{
		/// <summary>Reloads settings from INI file.</summary>
		public int Reset()
		{
			var result = (Native.Reset());
			return result;
		}

		public Exception LoadLibrary(string fileName)
		{
			Exception loadException;
			if (libHandle != IntPtr.Zero)
			{
				JocysCom.ClassLibrary.Win32.NativeMethods.FreeLibrary(libHandle, out loadException);
			}
			libHandle = JocysCom.ClassLibrary.Win32.NativeMethods.LoadLibrary(fileName, out loadException);
			return loadException;
		}

		static IntPtr libHandle;

		private static partial class Native
		{
			static Native()
			{
				Exception loadException;
				libHandle = JocysCom.ClassLibrary.Win32.NativeMethods.LoadLibrary("xinput9_1_0.dll", out loadException);
			}

			[DllImport("xinput9_1_0.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "Reset")]
			public static extern int Reset();

		}
	}
}
