using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Droploris.MIMProxy
{
	class Program
	{
		public static Server m;
		private static KeepAlive ka;

		public static void Main(string[] args)
		{
			NativeMethods.Handler = ConsoleCtrlCheck;
			NativeMethods.SetConsoleCtrlHandler(NativeMethods.Handler, true);
			ka = new KeepAlive();
			ka.Start();

			m = new Server();

		}


		private static bool ConsoleCtrlCheck(int eventType)
		{
			if (eventType != 2) return false;
			try
			{
				ka.Stop();
				m.Close();
			}
			catch (Exception e)
			{ Console.WriteLine(e.ToString()); }
			Environment.Exit(0);

			return false;
		}

		internal static class NativeMethods
		{
			// Keeps it from getting garbage collected
			internal static ConsoleEventDelegate Handler;

			[DllImport("kernel32.dll", SetLastError = true)]
			internal static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);

			// Pinvoke
			internal delegate bool ConsoleEventDelegate(int eventType);
		}
	}
}
