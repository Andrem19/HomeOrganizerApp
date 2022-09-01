using System;
using System.Collections.Generic;
using System.Text;

namespace HomeOrganizerApp
{
    public static class AppSettings
    {
        public static bool DevMode = false;
        public static string ApiUrlDev = "http://192.168.0.29:45455/";
        public static string ApiUrlProd = "https://remart.herokuapp.com/";
        public static string ApiUrl = DevMode ? ApiUrlDev : ApiUrlProd;
    }
}
