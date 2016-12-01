using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Droploris.MIMProxy
{
	class Program
	{
		public static MIMServer m;

		static void Main(string[] args)
		{
			KeepAlive ka = new KeepAlive();
			ka.Start();

			m = new MIMServer();

			AppDomain.CurrentDomain.ProcessExit += ProgramExit;
		}

		private static void ProgramExit(object sender, EventArgs e)
		{
			if (m != null)
				m.Close();
		}
	}
}
