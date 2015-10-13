using System;
using System.Web.Mvc;
using System.Xml;
using aakashPawar.Models;

namespace aakashPawar.Controllers
{
    public class HomeController : Controller
    {
        public string followersCount = "";
        public string name = "";
        public string noOfTweets = "";
        public string profileImage = "";
        public string recentTweet = "";
        private string url = "";
        public string username = "";
        private string xml = "";

        public ActionResult Index()
        {
            GetUserDetailsFromTwitter();
            return View();
        }


        private void GetUserDetailsFromTwitter()
        {
            if (Request["oauth_token"] != null & Request["oauth_verifier"] != null)
            {
                var oAuth = new oAuthTwitter();
                //Get the access token and secret.
                oAuth.AccessTokenGet(Request["oauth_token"], Request["oauth_verifier"]);
                if (oAuth.TokenSecret.Length > 0)
                {
                    //We now have the credentials, so make a call to the Twitter API.
                    url = "http://twitter.com/account/verify_credentials.xml";
                    xml = oAuth.oAuthWebRequest(oAuthTwitter.Method.GET, url, String.Empty);
                    var xmldoc = new XmlDocument();
                    xmldoc.LoadXml(xml);
                    XmlNodeList xmlList = xmldoc.SelectNodes("/user");
                    foreach (XmlNode node in xmlList)
                    {
                        name = node["name"].InnerText;
                        username = node["screen_name"].InnerText;
                        profileImage = node["profile_image_url"].InnerText;
                        followersCount = node["followers_count"].InnerText;
                        noOfTweets = node["statuses_count"].InnerText;
                        recentTweet = node["status"]["text"].InnerText;
                    }
                }
            }
        }


        public ActionResult About()
        {
            var oAuth = new oAuthTwitter();
            if (Request["oauth_token"] == null)
            {
                //Redirect the user to Twitter for authorization.
                //Using oauth_callback for local testing.
                oAuth.CallBackUrl = "http://aakashpawar.com/Home/Index";
                Response.Redirect(oAuth.AuthorizationLinkGet());
            }
            else
            {
                GetUserDetailsFromTwitter();
            }

            ViewBag.Message = "Your application description page.";

            return View();
        }

        [HttpGet]
        public ActionResult TwitterAuthentication()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}