using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Droploris.MIMProxy
{
	public class KeepAlive
	{
		private Thread t;

		public void Start()
		{
			t = new Thread(() => Loop());
			t.Start();
		}

		public void Stop()
		{
			if (t != null)
				t.Abort();
		}

		private static void Loop()
		{
			while (true)
				Thread.Sleep(5000);
		}

	}
}
