using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Xml;
using HtmlAgilityPack;

namespace GetCarDataService.Tools
{
    public static class HttpRequestProxy
    {
        private static WebClient _webClient;
        static readonly string XmlPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ResourceFile\\" + "ProxyIp.xml");
        private static List<ProxyIp> _proxyIpLists;
        
        static HttpRequestProxy()
        {
            RequestProxyIp();
            GetProxyIpList();
        }
        
        /// <summary>
        /// 使用代理服务器，请求数据
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static HtmlDocument GetHtmlDocument(string uri)
        {
            var wc = GetWebClient();
            if (wc == null)
            {
                return null;
            }
            var strResult = wc.DownloadString(uri);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(strResult);
            return htmlDocument;
        }


        public static WebClient GetWebClient()
        {
            
            if (_webClient == null)
            {
                for (int i = 0; i < 10; i++)
                {
                    var wc = GetWc();

                    if (CheckWebClient(wc))
                    {
                        _webClient = wc;
                        break;
                    }
                }
            }
            return _webClient;
        }

        private static WebClient GetWc()
        {
            var index = new Random().Next(0, _proxyIpLists.Count);
            ProxyIp proxyIp = _proxyIpLists[index];
            using (var wc = new WebClient())
            {
                var ip = proxyIp.Ip;
                var port = proxyIp.Port;
                var wp = new WebProxy(ip + ":" + port, true)
                {
                    Credentials = CredentialCache.DefaultCredentials
                };
                wc.Proxy = wp; //指定代理
                wc.Encoding = Encoding.Default; //此项用来预防返回乱码
                return wc;
            }
        }

        private static bool CheckWebClient(WebClient wc)
        {
            const string uri = "http://www.baidu.com";
            try
            {
                wc.DownloadString(uri);
            }
            catch (Exception e)
            {
                Common.WriteErrorLog("代理地址错误：" + e.ToString());
                return false;
            }
            return true;
        }

        
        private static List<ProxyIp> GetProxyIpList()
        {
            if (_proxyIpLists == null)
            {
                _proxyIpLists = new List<ProxyIp>();

                var doc = new XmlDocument();
                doc.Load(XmlPath);
                XmlNodeList lists = doc.GetElementsByTagName("item");
                foreach (XmlElement item in lists)
                {
                    var proxy = new ProxyIp();
                    proxy.Ip = item.GetElementsByTagName("ip")[0].InnerText;
                    proxy.Port = item.GetElementsByTagName("port")[0].InnerText;
                    proxy.Type = item.GetElementsByTagName("type")[0].InnerText;
                    proxy.Country = item.GetElementsByTagName("country")[0].InnerText;
                    _proxyIpLists.Add(proxy);
                }
            }
            return _proxyIpLists;
        }

        /// <summary>
        /// 请求代理ip，保存在xml中。//http://www.proxy360.cn/default.aspx
        /// </summary>
        private static void RequestProxyIp()
        {
            var htmlWeb = new HtmlWeb { OverrideEncoding = Encoding.GetEncoding("UTF-8") };
            HtmlDocument htmlDocument = htmlWeb.Load("http://www.proxy360.cn/default.aspx");
            var div = htmlDocument.GetElementbyId("ctl00_ContentPlaceHolder1_upProjectList");
            var divLists = div.SelectNodes("//div[starts-with(@class,'proxylistitem')]");
            if (divLists != null && divLists.Count > 0)
            {
                var doc = new XmlDocument();
                XmlNode xmlIplists = doc.CreateElement("root");
                foreach (HtmlNode divList in divLists)
                {
                    var spans = divList.ChildNodes[1].SelectNodes("child::span[starts-with(@class,'tbBottomLine')]");
                    XmlNode item = doc.CreateElement("item");
                    XmlElement xmlip = doc.CreateElement("ip");
                    xmlip.InnerText = spans[0].InnerText.Trim();
                    item.AppendChild(xmlip);
                    XmlElement xmlport = doc.CreateElement("port");
                    xmlport.InnerText = spans[1].InnerText.Trim();
                    item.AppendChild(xmlport);
                    XmlElement xmltype = doc.CreateElement("type");
                    xmltype.InnerText = spans[2].InnerText.Trim();
                    item.AppendChild(xmltype);
                    XmlElement xmlcountry = doc.CreateElement("country");
                    xmlcountry.InnerText = spans[3].InnerText.Trim();
                    item.AppendChild(xmlcountry);
                    xmlIplists.AppendChild(item);
                }
                doc.AppendChild(xmlIplists);
                doc.Save(XmlPath);
                //using (var fileStream = File.Open(XmlPath, FileMode.Create, FileAccess.Write))
                //using (var streamWriter = new StreamWriter(fileStream))
                //{
                //    streamWriter.Write(doc.ToString());
                //    streamWriter.Close();
                //    fileStream.Close();
                //}
            }
        }
    }

    public class ProxyIp
    {
        public string Ip { get; set; }

        public string Port { get; set; }

        public string Type { get; set; }

        public string Country { get; set; }

    }
}
