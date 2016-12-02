using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsQuery;
using Titanium.Web.Proxy.EventArguments;

namespace Droploris.MIMProxy
{
	public static class ModifiedWebsites
	{
		public static void AddPages(Server s)
		{
			#region Google
			s.OnWebsite("google.de", new Func<CQ, SessionEventArgs, CQ>((CQ dom, SessionEventArgs e) =>
			{
				//dom["head"].Append("<script>alert(420)</script>");
				return dom;
			}));
			#endregion


			#region Steamcommunity
			s.OnWebsite("steamcommunity.com", new Func<CQ, SessionEventArgs, CQ>((CQ dom, SessionEventArgs e) =>
			{
				
				dom["head"].Append("<style>"
								+ ".admint span{color:red !important;}"
								+ "</style>"
								+ "<script>"
								+ "function runEv(){try{eval(document.querySelectorAll(\"#evalBox\")[0].value)}catch(e){alert(e);} }"
								+ "</script>");

				dom[".profile_header"].Prepend(
					"<a class='btn_profile_action btn_medium' style='position:fixed;left:0px;top:0px;' href='javascript: alert(\"Control Panel error\"); '>"
					+ "<span>Control Panel</span></a>");

				dom[".profile_header_content"].Css("margin-bottom", "30px");

				dom[".profile_header_centered_persona"].Append("<div><script>document.write(g_rgProfileData['steamid']);</script></div>");

				dom[".profile_header_actions"].Append(
					"<br><a class='btn_profile_action btn_medium' href='javascript: alert(\"ban\"); '><span>View Account Information</span></a>").Append(
					"<a class='btn_profile_action btn_medium' href='javascript: alert(\"ban\"); '><span>Admin Settings</span></a>");

				dom[".popup_body"].Append(
					"<a class='popup_menu_item' href='#' onclick='"
					+ "ShowDialog(\"Eval\", \"<input type=\\\"textbox\\\" id=\\\"evalBox\\\" /> <a class=\\\"btn_profile_action btn_medium\\\" href=\\\"javascript: runEv()\\\"><span>Run</span></a>\")"
					+ "'><img style='margin: 0 1px;' src='http://steamcommunity-a.akamaihd.net/public/images/skin_1/notification_icon_flag.png'>"
					+ "&nbsp; Evaluate</a>");

				dom[".profile_item_links"].Append(
					"<div class='profile_count_link ellipsis admint'><a href=''><span class='count_link_label'>Reports</span>&nbsp;</a></div>");

				//dom["span,a,p"].Text("meme");

				return dom;
			}));

			
			#endregion
		}
	}
}
