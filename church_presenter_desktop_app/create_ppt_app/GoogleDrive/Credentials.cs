using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace create_ppt_app.GoogleDrive
{
    internal class Credentials
    {
        public static readonly string CLIENT_ID = "";
        public static readonly string CLIENT_SECRET = "";
        public static readonly string API_KEY = "";
        public static readonly string[] SCOPES = new String[]
        {
            "https://www.googleapis.com/auth/drive",
            "https://www.googleapis.com/auth/userinfo.email"
        };
        public static readonly string REDIRECT_URI = "http://localhost:8080/oauth2callback";
        public static readonly string AUTH_ENDPOINT = "https://accounts.google.com/o/oauth2/v2/auth";
        public static readonly string TOKEN_ENDPOINT = "https://oauth2.googleapis.com/token";
        public static readonly string SCOPE = "https://www.googleapis.com/auth/drive";
    }
}
