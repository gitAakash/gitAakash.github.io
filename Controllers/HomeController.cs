using System;
using System.Configuration;
using System.IO;
using System.Net;
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
        private string Client_ID = "";
        public string Return_url = "";
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


        public ActionResult ClickMe()
        {
            var ddd =
                "https://accounts.google.com/o/oauth2/auth?response_type=token&" +
                "redirect_uri=" + Return_url + "&scope=" +
                "https://www.googleapis.com/auth/userinfo.email%20" +
                "https://www.googleapis.com/auth/userinfo.profile&client_id=" + Client_ID;

            return Redirect(ddd);
        }



        public ActionResult GoogleResultCallBack()
        {
            Client_ID = ConfigurationSettings.AppSettings["google_clientId"];
            Return_url = ConfigurationSettings.AppSettings["google_RedirectUrl"];
            Demo demo = new Demo();

            if (Request.QueryString["access_token"] != null)
            {
                var URI = "https://www.googleapis.com/oauth2/v1/userinfo?access_token=" +
                          Request.QueryString["access_token"];

                var webClient = new WebClient();
                var stream = webClient.OpenRead(URI);
                string b;

                /*I have not used any JSON parser because I do not want to use any extra dll/3rd party dll*/
                using (var br = new StreamReader(stream))
                {
                    b = br.ReadToEnd();
                }

                b = b.Replace("id", "").Replace("email", "");
                b = b.Replace("given_name", "");
                b = b.Replace("family_name", "").Replace("link", "").Replace("picture", "");
                b = b.Replace("gender", "").Replace("locale", "").Replace(":", "");
                b = b.Replace("\"", "").Replace("name", "").Replace("{", "").Replace("}", "");

                /*
                 
                "id": "109124950535374******"
                  "email": "usernamil@gmail.com"
                  "verified_email": true
                  "name": "firstname lastname"
                  "given_name": "firstname"
                  "family_name": "lastname"
                  "link": "https://plus.google.com/10912495053537********"
                  "picture": "https://lh3.googleusercontent.com/......./photo.jpg"
                  "gender": "male"
                  "locale": "en" } 
               */

                Array ar = b.Split(",".ToCharArray());
                for (var p = 0; p < ar.Length; p++)
                {
                    ar.SetValue(ar.GetValue(p).ToString().Trim(), p);
                }

                demo.Email_address = ar.GetValue(1).ToString();
                demo.Google_ID = ar.GetValue(0).ToString();
                demo.firstName = ar.GetValue(4).ToString();
                demo.LastName = ar.GetValue(5).ToString();
            }
            else
            {
                demo.Email_address ="Eeeoe";
                demo.Google_ID = "Eeeoe";
                demo.firstName = "Eeeoe";
                demo.LastName = "Eeeoe";
               
            }
            return View(demo);
        }

    }

    public class Demo
    {
        public string Email_address { get; set; }
        public string Google_ID { get; set; }
        public string LastName { get; set; }
        public string firstName { get; set; }
        public string Token { get; set; }
    }
}