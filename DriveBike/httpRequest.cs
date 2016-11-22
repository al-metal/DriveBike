using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Bike18
{
    class httpRequest
    {
        public string getRequestEncod(string url)
        {
            HttpWebResponse res = null;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:44.0) Gecko/20100101 Firefox/44.0";
            res = (HttpWebResponse)req.GetResponse();
            StreamReader ressr = new StreamReader(res.GetResponseStream(), Encoding.GetEncoding(1251));
            string otv = ressr.ReadToEnd();
            return otv;
        }

        public string getRequest(string url)
        {
            HttpWebResponse res = null;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:44.0) Gecko/20100101 Firefox/44.0";
            res = (HttpWebResponse)req.GetResponse();
            StreamReader ressr = new StreamReader(res.GetResponseStream());
            string otv = ressr.ReadToEnd();
            res.GetResponseStream().Close();
            req.GetResponse().Close();

            return otv;
        }

        public CookieContainer webCookie(string url)
        {
            CookieContainer cooc = new CookieContainer();
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:44.0) Gecko/20100101 Firefox/44.0";
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.CookieContainer = cooc;
            Stream stre = req.GetRequestStream();
            stre.Close();
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();
            return cooc;
        }

        public string PostRequest(CookieContainer cookie, string nethouseTovar)
        {
            string otv = null;
            do
            {
                HttpWebResponse res = null;
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(nethouseTovar);
                req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:44.0) Gecko/20100101 Firefox/44.0";
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                req.CookieContainer = cookie;
                try
                {
                    res = (HttpWebResponse)req.GetResponse();
                    StreamReader ressr = new StreamReader(res.GetResponseStream());
                    otv = ressr.ReadToEnd();
                }
                catch (WebException e)
                {
                    break;
                }
            } while (otv == null);
            return otv;
        }
    }
}
