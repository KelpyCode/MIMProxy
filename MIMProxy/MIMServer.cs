using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.Models;
using CsQuery;
using System.Diagnostics;

namespace Droploris.MIMProxy
{
	public class Server
	{
		public ProxyServer ps;
		private Dictionary<string, Func<CQ, Titanium.Web.Proxy.EventArguments.SessionEventArgs, CQ>> websiteHooks;

		public Server()
		{
			ps = new ProxyServer();
			websiteHooks = new Dictionary<string, Func<CQ, Titanium.Web.Proxy.EventArguments.SessionEventArgs, CQ>>();

			ps.TrustRootCertificate = true;
			ps.BeforeRequest += OnRequest;
			ps.BeforeResponse += OnResponse;
			ps.ServerCertificateValidationCallback += CertificateValidation;
			ps.ClientCertificateSelectionCallback += CertificateSelection;


			var endPoint = new ExplicitProxyEndPoint(System.Net.IPAddress.Any, 8030, true)
			{

			};

			ps.AddEndPoint(endPoint);
			ps.Start();

			var transparentEndPoint = new TransparentProxyEndPoint(System.Net.IPAddress.Any, 8001, true)
			{
				GenericCertificateName = "google.com"
			};
			ps.AddEndPoint(transparentEndPoint);

			ps.SetAsSystemHttpProxy(endPoint);
			ps.SetAsSystemHttpsProxy(endPoint);
			foreach (var ep in ps.ProxyEndPoints)
				Console.WriteLine("Listening on '{0}' endpoint at Ip {1} and port: {2} ",
					ep.GetType().Name, ep.IpAddress, ep.Port);

			Console.WriteLine("Ready");

		}

		private Task CertificateSelection(object arg1, Titanium.Web.Proxy.EventArguments.CertificateSelectionEventArgs e)
		{
			return Task.FromResult(0);
		}

		private Task CertificateValidation(object arg1, Titanium.Web.Proxy.EventArguments.CertificateValidationEventArgs e)
		{
			if (e.SslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
				e.IsValid = true;

			return Task.FromResult(0);
		}

		public void Close()
		{
			Console.WriteLine("Disposing..");
			ps.DisableSystemHttpProxy();
			ps.DisableSystemHttpsProxy();
			ps.Dispose();
		}

		public void OnWebsite(string domain, Func<CQ, Titanium.Web.Proxy.EventArguments.SessionEventArgs, CQ> callback)
		{
			websiteHooks[domain] = callback;
			Console.WriteLine("Set " + domain + " handler");
		}

		private void WebsiteManipulation(Titanium.Web.Proxy.EventArguments.SessionEventArgs e, ref CQ dom)
		{
			foreach (KeyValuePair<string, Func<CQ, Titanium.Web.Proxy.EventArguments.SessionEventArgs, CQ>> v in websiteHooks)
			{
				if (e.WebSession.Request.RequestUri.AbsoluteUri.Contains(v.Key))
				{
					dom = v.Value(dom, e);
					return;
				}
			}
		}

		private async Task OnResponse(object arg1, Titanium.Web.Proxy.EventArguments.SessionEventArgs e)
		{
			//Console.WriteLine(e.WebSession.Request.Url + " RES");

			if (e.WebSession.Request.Method == "GET" || e.WebSession.Request.Method == "POST")
			{
				if (e.WebSession.Response.ResponseStatusCode == "200")
				{
					if (e.WebSession.Response.ContentType != null && e.WebSession.Response.ContentType.Trim().ToLower().Contains("text/html"))
					{
						string body = await e.GetResponseBodyAsString();

						CQ dom = CQ.Create(body);

						WebsiteManipulation(e, ref dom);

						await e.SetResponseBodyString(dom.Render());
					}

				}
			}

		}

		private async Task OnRequest(object arg1, Titanium.Web.Proxy.EventArguments.SessionEventArgs e)
		{
			//Console.WriteLine(e.WebSession.Request.Url + " REQ");


			var requestHeaders = e.WebSession.Request.RequestHeaders;

			var method = e.WebSession.Request.Method.ToUpper();
			if ((method == "POST" || method == "PUT" || method == "PATCH"))
			{
				//Get/Set request body bytes
				byte[] bodyBytes = await e.GetRequestBody();
				await e.SetRequestBody(bodyBytes);

				/*
				string bodyString = await e.GetRequestBodyAsString();
				await e.SetRequestBodyString(bodyString);*/

			}


			if (e.WebSession.Request.RequestUri.AbsoluteUri.Contains("tenrys.isgay"))
			{
				await e.Ok("<!DOCTYPE html>" +
					  "<html><body><h1>" +
					  "XAOTI CMAKES MEMES XDDD" +
					  "</h1>" +
					  "<p>FUCCC.</p>" +
					  "</body>" +
					  "</html>");
			}
			/*if (e.WebSession.Request.RequestUri.AbsoluteUri.Contains("wikipedia.org"))
			{
				await e.Redirect("https://www.paypal.com");
			}*/
		}


	}
}
