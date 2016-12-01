using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.Models;

namespace Droploris.MIMProxy
{
	public class MIMServer
	{
		public ProxyServer ps;

		public MIMServer()
		{
			ps = new ProxyServer();

			ps.TrustRootCertificate	=	true;
			ps.BeforeRequest +=			OnRequest;
			//ps.BeforeResponse +=		OnResponse;
		

			var endPoint = new ExplicitProxyEndPoint(System.Net.IPAddress.Any, 8000, true)
			{
				
			};

			ps.AddEndPoint(endPoint);
			foreach (var ep in ps.ProxyEndPoints)
				Console.WriteLine("Listening on '{0}' endpoint at Ip {1} and port: {2} ",
					ep.GetType().Name, ep.IpAddress, ep.Port);

			ps.Start();
			ps.SetAsSystemHttpProxy(endPoint);
			ps.SetAsSystemHttpsProxy(endPoint);
			Console.WriteLine("Ready");
			
		}

		public void Close()
		{
			Console.WriteLine("Disposing..");
			ps.DisableAllSystemProxies();
			ps.Dispose();
		}

		private async Task OnResponse(object arg1, Titanium.Web.Proxy.EventArguments.SessionEventArgs e)
		{
			Console.WriteLine(e.WebSession.Request.Url+" RES");

			byte[] bodyBytes = await e.GetResponseBody();
			await e.SetResponseBody(bodyBytes);
		}

		private async Task OnRequest(object arg1, Titanium.Web.Proxy.EventArguments.SessionEventArgs e)
		{
			Console.WriteLine(e.WebSession.Request.Url+" REQ");
			
			//var a = await e.GetRequestBodyAsString();
			//await e.SetRequestBodyString(a);
			
			//await e.SetResponseBodyString("<!--ass-->");
			//await e.Ok("<script>alert(1337)</script>");
		}
	}
}
