﻿using RacerMotors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using web;
using Формирование_ЧПУ;

namespace DriveBike
{
    public partial class Form1 : Form
    {
        web.WebRequest webRequest = new web.WebRequest();
        CHPU chpu = new CHPU();
        WebClient webClient = new WebClient();
        int addCount = 0;
        string boldOpen = "<span style=\"\"font-weight: bold; font-weight: bold; \"\">";
        string boldClose = "</span>";
        double discounts = 0.02;
        FileEdit files = new FileEdit();

        public Form1()
        {
            InitializeComponent();

            if (!Directory.Exists("files"))
            {
                Directory.CreateDirectory("files");
            }
            if (!Directory.Exists("pic"))
            {
                Directory.CreateDirectory("pic");
            }

            if (!File.Exists("files\\miniText.txt"))
            {
                File.Create("files\\miniText.txt");
            }

            if (!File.Exists("files\\fullText.txt"))
            {
                File.Create("files\\fullText.txt");
            }

            if (!File.Exists("files\\title.txt"))
            {
                File.Create("files\\title.txt");
            }

            if (!File.Exists("files\\description.txt"))
            {
                File.Create("files\\description.txt");
            }

            if (!File.Exists("files\\keywords.txt"))
            {
                File.Create("files\\keywords.txt");
            }
            StreamReader altText = new StreamReader("files\\miniText.txt", Encoding.GetEncoding("windows-1251"));
            while (!altText.EndOfStream)
            {
                string str = altText.ReadLine();
                rtbMiniText.AppendText(str + "\n");
            }
            altText.Close();

            altText = new StreamReader("files\\fullText.txt", Encoding.GetEncoding("windows-1251"));
            while (!altText.EndOfStream)
            {
                string str = altText.ReadLine();
                rtbFullText.AppendText(str + "\n");
            }
            altText.Close();

            altText = new StreamReader("files\\title.txt", Encoding.GetEncoding("windows-1251"));
            while (!altText.EndOfStream)
            {
                string str = altText.ReadLine();
                tbTitle.AppendText(str + "\n");
            }
            altText.Close();

            altText = new StreamReader("files\\description.txt", Encoding.GetEncoding("windows-1251"));
            while (!altText.EndOfStream)
            {
                string str = altText.ReadLine();
                tbDescription.AppendText(str + "\n");
            }
            altText.Close();

            altText = new StreamReader("files\\keywords.txt", Encoding.GetEncoding("windows-1251"));
            while (!altText.EndOfStream)
            {
                string str = altText.ReadLine();
                tbKeywords.AppendText(str + "\n");
            }
            altText.Close();
        }

        private void btnSaveTemplate_Click(object sender, EventArgs e)
        {
            int count = 0;
            StreamWriter writers = new StreamWriter("files\\miniText.txt", false, Encoding.GetEncoding(1251));
            count = rtbMiniText.Lines.Length;
            for (int i = 0; rtbMiniText.Lines.Length > i; i++)
            {
                if (count - 1 == i)
                {
                    if (rtbFullText.Lines[i] == "")
                        break;
                }
                writers.WriteLine(rtbMiniText.Lines[i].ToString());
            }
            writers.Close();

            writers = new StreamWriter("files\\fullText.txt", false, Encoding.GetEncoding(1251));
            count = rtbFullText.Lines.Length;
            for (int i = 0; count > i; i++)
            {
                if (count - 1 == i)
                {
                    if (rtbFullText.Lines[i] == "")
                        break;
                }
                writers.WriteLine(rtbFullText.Lines[i].ToString());
            }
            writers.Close();

            writers = new StreamWriter("files\\title.txt", false, Encoding.GetEncoding(1251));
            writers.WriteLine(tbTitle.Lines[0]);
            writers.Close();

            writers = new StreamWriter("files\\description.txt", false, Encoding.GetEncoding(1251));
            writers.WriteLine(tbDescription.Lines[0]);
            writers.Close();

            writers = new StreamWriter("files\\keywords.txt", false, Encoding.GetEncoding(1251));
            writers.WriteLine(tbKeywords.Lines[0]);
            writers.Close();

            MessageBox.Show("Сохранено");
        }

        private void btnActualCategory_Click(object sender, EventArgs e)
        {
            string otv = null;
            File.Delete("naSite.csv");
            File.Delete("allProducts.csv");
            List<string> newProduct = newList();

            otv = webRequest.getRequest("http://www.drivebike.ru/rashodniki-dlya-motocikla-i-kvadrocikla?limit=60");
            MatchCollection categoriesUrls = new Regex("(?<=<li class=\"amshopby-cat amshopby-cat-level-1\">)[\\w\\W]*?(?=</li>)").Matches(otv);
            for (int i = 0; categoriesUrls.Count > i; i++)
            {
                string categories = new Regex("(?<=<a href=\").*?(?=\">)").Match(categoriesUrls[i].ToString()).ToString();
                string section1 = "Расходники для японских, европейских, американских мотоциклов";
                string section2 = new Regex("(?<=\">).*?(?=</a>)").Match(categoriesUrls[i].ToString()).ToString();
                otv = webRequest.getRequest(categories);
                MatchCollection availability = new Regex("(?<=<p class=\"availability).*?(?=</span></p>)").Matches(otv);
                MatchCollection urlTovars = new Regex("(?<=<li class=\"item)[\\w\\W]*?(?=\" title=\")").Matches(otv);
                if (availability.Count == urlTovars.Count)
                {
                    for (int n = 0; urlTovars.Count > n; n++)
                    {
                        string availabilityTovar = availability[n].ToString();
                        if (availabilityTovar.Contains("Есть в наличии"))
                        {
                            //Если товар в наличии
                            string url = new Regex("(?<=a href=\").*").Match(urlTovars[n].ToString()).ToString();
                            otv = webRequest.getRequest(url);
                            string urlImageProduct = new Regex("(?<=<img src=\")http://www.drivebike.ru/media/catalog/product/.*?(?=\" />)").Match(otv).ToString();
                            string articl = new Regex("(?<=<div class=\"std\">Код товара:).*?(?=<br /> )").Match(otv).ToString();
                            try
                            {
                                webClient.DownloadFile(urlImageProduct, "pic\\" + articl + ".jpg");
                            }
                            catch
                            {

                            }
                            

                            string nameTovar = new Regex("(?<=<h1><font style=\"color:#459B06; \">).*(?=</h1>)").Match(otv).ToString();
                            nameTovar = nameTovar.Replace("</font><br/>", " ");
                            
                            string number = new Regex("(?<=<br /> Номер по каталогу: ).*?(?=<br />)").Match(otv).ToString();
                            string price = new Regex("(?<=<meta itemprop=\"price\" content=\").*?(?=\" />)").Match(otv).ToString();
                            MatchCollection Text = new Regex("(?<=<div class=\"std\">)[\\w\\W]*?(?=</div>)").Matches(otv);
                            string table = new Regex("<table[\\w\\W]*?</table>").Match(otv).ToString().Replace("\n        ", "").Replace("            ", " ").Replace("  ", " ").Replace("    ", " ").Replace("        ", " ").Replace("  ", "").Replace("\n", "").Replace(" class=\"data\"", "").Replace(" class=\"label\"", "").Replace(" class=\"data-table\" id=\"product-attribute-specs-table\"><col width=\"25%\" /><col /", "");
                            string miniText = Text[0].ToString();
                            string fullText = Text[1].ToString().Replace("\n", "") + "<br /> " + table;

                            bool b = false;
                            otv = webRequest.getRequest("http://bike18.ru/products/search/page/1?sort=0&balance=&categoryId=&min_cost=&max_cost=&text=" + nameTovar);
                            MatchCollection searchTovars = new Regex("(?<=\" >).*?(?=</a>)").Matches(otv);
                            if (searchTovars.Count > 0)
                            {
                                for (int m = 0; searchTovars.Count > m; m++)
                                {
                                    string searchTovarName = searchTovars[m].ToString();
                                    if (searchTovarName == nameTovar)
                                    {
                                        //товар найден
                                        b = true;
                                        break;
                                    }
                                }
                            }
                            if (b)
                            {
                                //товар найден и надо обновить цену
                            }
                            else
                            {
                                //товара нету и следует его добавить
                                string slug = chpu.vozvr(nameTovar);

                                string razdel = "Запчасти и расходники => Расходники для японских, европейских, американских мотоциклов => " + section2;
                                string miniTextTemplate = MinitextStr();
                                string fullTextTemplate = FulltextStr();
                                string titleText = tbTitle.Lines[0].ToString();
                                string descriptionText = tbDescription.Lines[0].ToString();
                                string keywordsText = tbKeywords.Lines[0].ToString();
                                int priceActual = webRequest.price(Convert.ToInt32(price), discounts);

                                string dblProduct = "НАЗВАНИЕ также подходит для: аналогичных моделей.";

                                miniTextTemplate = Replace(miniTextTemplate, section2, section1, dblProduct, nameTovar, articl, miniText, fullText);
                                miniTextTemplate = miniTextTemplate.Replace(" class=\"label\"", "").Replace(" class=\"data\"", "");
                                miniTextTemplate = miniTextTemplate.Remove(miniTextTemplate.LastIndexOf("<p>"));

                                fullTextTemplate = Replace(fullTextTemplate, section2, section1, dblProduct, nameTovar, articl, miniText, fullText);
                                fullTextTemplate = fullTextTemplate.Remove(fullTextTemplate.LastIndexOf("<p>"));
                                fullTextTemplate = fullTextTemplate.Remove(fullTextTemplate.LastIndexOf("<p>"));

                                titleText = ReplaceSEO(titleText, nameTovar, section1, section2, articl, dblProduct, number);
                                descriptionText = ReplaceSEO(descriptionText, nameTovar, section1, section2, articl, dblProduct, number);
                                keywordsText = ReplaceSEO(keywordsText, nameTovar, section1, section2, articl, dblProduct, number);

                                titleText = Remove(titleText, 255);
                                descriptionText = Remove(descriptionText, 200);
                                keywordsText = Remove(keywordsText, 100);
                                slug = Remove(slug, 64);

                                newProduct = new List<string>();
                                newProduct.Add(""); //id
                                newProduct.Add("\"" + articl + "\""); //артикул
                                newProduct.Add("\"" + nameTovar + "\"");  //название
                                newProduct.Add("\"" + priceActual + "\""); //стоимость
                                newProduct.Add("\"" + "" + "\""); //со скидкой
                                newProduct.Add("\"" + razdel + "\""); //раздел товара
                                newProduct.Add("\"" + "100" + "\""); //в наличии
                                newProduct.Add("\"" + "0" + "\"");//поставка
                                newProduct.Add("\"" + "1" + "\"");//срок поставки
                                newProduct.Add("\"" + miniTextTemplate + "\"");//краткий текст
                                newProduct.Add("\"" + fullTextTemplate + "\"");//полностью текст
                                newProduct.Add("\"" + titleText + "\""); //заголовок страницы
                                newProduct.Add("\"" + descriptionText + "\""); //описание
                                newProduct.Add("\"" + keywordsText + "\"");//ключевые слова
                                newProduct.Add("\"" + slug + "\""); //ЧПУ
                                newProduct.Add(""); //с этим товаром покупают
                                newProduct.Add("");   //рекламные метки
                                newProduct.Add("\"" + "1" + "\"");  //показывать
                                newProduct.Add("\"" + "0" + "\""); //удалить

                                files.fileWriterCSV(newProduct, "naSite");
                            }
                        }
                        else
                        {
                            //Если товара нет в наличии добавить и пометить ссылкой нет в наличии
                        }
                    }
                }
                else
                {
                    //Если разное кол-во ссылок на товар и наличия товара
                }
            }
            #region
            System.Threading.Thread.Sleep(20000);
            string trueOtv = null;
            string[] naSite1 = File.ReadAllLines("naSite.csv", Encoding.GetEncoding(1251));
            if (naSite1.Length > 1)
            {
                do
                {
                    string otvimg = DownloadNaSite();
                    string check = "{\"success\":true,\"imports\":{\"state\":1,\"errorCode\":0,\"errorLine\":0}}";
                    do
                    {
                        System.Threading.Thread.Sleep(2000);
                        otvimg = ChekedLoading();
                    }
                    while (otvimg == check);

                    trueOtv = new Regex("(?<=\":{\"state\":).*?(?=,\")").Match(otvimg).ToString();
                    string error = new Regex("(?<=errorCode\":).*?(?=,\")").Match(otvimg).ToString();
                    if (error == "13")
                    {
                        string errstr = new Regex("(?<=errorLine\":).*?(?=,\")").Match(otvimg).ToString();
                        string[] naSite = File.ReadAllLines("naSite.csv", Encoding.GetEncoding(1251));
                        int u = Convert.ToInt32(errstr) - 1;
                        string[] strslug3 = naSite[u].ToString().Split(';');
                        string strslug = strslug3[strslug3.Length - 5];
                        int slug = strslug.Length;
                        int countAdd = ReturnCountAdd();
                        int countDel = countAdd.ToString().Length;
                        string strslug2 = strslug.Remove(slug - countDel);
                        strslug2 += countAdd;
                        naSite[u] = naSite[u].Replace(strslug, strslug2);
                        File.WriteAllLines("naSite.csv", naSite, Encoding.GetEncoding(1251));
                    }
                    if (error == "37")
                    {
                        string errstr = new Regex("(?<=errorLine\":).*?(?=,\")").Match(otvimg).ToString();
                        string[] naSite = File.ReadAllLines("naSite.csv", Encoding.GetEncoding(1251));
                        int u = Convert.ToInt32(errstr) - 1;
                        string[] strslug3 = naSite[u].ToString().Split(';');
                        string strslug = strslug3[strslug3.Length - 5];
                        int slug = strslug.Length;
                        int countAdd = ReturnCountAdd();
                        int countDel = countAdd.ToString().Length;
                        string strslug2 = strslug.Remove(slug - countDel);
                        strslug2 += countAdd;
                        naSite[u] = naSite[u].Replace(strslug, strslug2);
                        File.WriteAllLines("naSite.csv", naSite, Encoding.GetEncoding(1251));
                    }
                    if (error == "10")
                    {
                    }
                }
                while (trueOtv != "2");
            }
            #endregion
            MessageBox.Show("Обновлено товаров на сайте");
        }

        private List<string> newList()
        {
            List<string> newProduct = new List<string>();
            newProduct.Add("id");                                                                               //id
            newProduct.Add("Артикул *");                                                 //артикул
            newProduct.Add("Название товара *");                                          //название
            newProduct.Add("Стоимость товара *");                                    //стоимость
            newProduct.Add("Стоимость со скидкой");                                       //со скидкой
            newProduct.Add("Раздел товара *");                                         //раздел товара
            newProduct.Add("Товар в наличии *");                                                    //в наличии
            newProduct.Add("Поставка под заказ *");                                                 //поставка
            newProduct.Add("Срок поставки (дни) *");                                           //срок поставки
            newProduct.Add("Краткий текст");                                 //краткий текст
            newProduct.Add("Текст полностью");                                          //полностью текст
            newProduct.Add("Заголовок страницы (title)");                               //заголовок страницы
            newProduct.Add("Описание страницы (description)");                                 //описание
            newProduct.Add("Ключевые слова страницы (keywords)");                                 //ключевые слова
            newProduct.Add("ЧПУ страницы (slug)");                                   //ЧПУ
            newProduct.Add("С этим товаром покупают");                              //с этим товаром покупают
            newProduct.Add("Рекламные метки");
            newProduct.Add("Показывать на сайте *");                                           //показывать
            newProduct.Add("Удалить *");                                    //удалить
            files.fileWriterCSV(newProduct, "naSite");
            return newProduct;
        }

        private string MinitextStr()
        {
            string minitext = "";
            for (int z = 0; rtbMiniText.Lines.Length > z; z++)
            {
                if (rtbMiniText.Lines[z].ToString() == "")
                {
                    minitext += "<p><br /></p>";
                }
                else
                {
                    minitext += "<p>" + rtbMiniText.Lines[z].ToString() + "</p>";
                }
            }
            return minitext;
        }

        private string FulltextStr()
        {
            string fullText = "";
            for (int z = 0; rtbFullText.Lines.Length > z; z++)
            {
                if (rtbFullText.Lines[z].ToString() == "")
                {
                    fullText += "<p><br /></p>";
                }
                else
                {
                    fullText += "<p>" + rtbFullText.Lines[z].ToString() + "</p>";
                }
            }
            return fullText;
        }

        private string Replace(string text, string section2, string section1, string dblProduct, string nameTovar, string article, string miniText, string fullText)
        {
            string discount = Discount();
            string nameText = boldOpen + nameTovar + boldClose;
            string nameRazdel = boldOpen + section1 + boldClose;
            string namePodrazdel = boldOpen + section2 + boldClose;
            text = text.Replace("СКИДКА", discount).Replace("ПОДРАЗДЕЛ", namePodrazdel).Replace("РАЗДЕЛ", nameRazdel).Replace("ДУБЛЬ", dblProduct).Replace("НАЗВАНИЕ", nameText).Replace("АРТИКУЛ", article).Replace("МИНИТЕКСТ", miniText).Replace("ТЕКСТ", fullText).Replace("<p><br /></p><p><br /></p><p><br /></p><p>", "<p><br /></p>");
            return text;
        }

        private string ReplaceSEO(string text, string nameTovarRacerMotors, string section1, string section2, string article, string dblProduct, string number)
        {
            string discount = Discount();
            text = text.Replace("СКИДКА", discount).Replace("ПОДРАЗДЕЛ", section2).Replace("РАЗДЕЛ", section1).Replace("ДУБЛЬ", dblProduct).Replace("НАЗВАНИЕ", nameTovarRacerMotors).Replace("АРТИКУЛ", article).Replace("НОМЕР", number);
            return text;
        }

        private string Discount()
        {
            string discount = "<p style=\"\"text-align: right;\"\"><span style=\"\"font -weight: bold; font-weight: bold;\"\"> Сделай ТРОЙНОЙ удар по нашим ценам! </span></p><p style=\"\"text-align: right;\"\"><span style=\"\"font -weight: bold; font-weight: bold;\"\"> 1. <a target=\"\"_blank\"\" href =\"\"http://bike18.ru/stock\"\"> Скидки за отзывы о товарах!</a> </span></p><p style=\"\"text-align: right;\"\"><span style=\"\"font -weight: bold; font-weight: bold;\"\"> 2. <a target=\"\"_blank\"\" href =\"\"http://bike18.ru/stock\"\"> Друзьям скидки и подарки!</a> </span></p><p style=\"\"text-align: right;\"\"><span style=\"\"font -weight: bold; font-weight: bold;\"\"> 3. <a target=\"\"_blank\"\" href =\"\"http://bike18.ru/stock\"\"> Нашли дешевле!? 110% разницы Ваши!</a></span></p>";
            return discount;
        }

        private string Remove(string text, int v)
        {
            if (text.Length > v)
            {
                text = text.Remove(v);
                text = text.Remove(text.LastIndexOf(" "));
            }
            return text;
        }

        private int ReturnCountAdd()
        {
            if (addCount == 99)
                addCount = 0;
            addCount++;
            return addCount;
        }

        public string DownloadNaSite()
        {
            CookieContainer cookie = webRequest.webCookieBike18();
            string epoch = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds.ToString().Replace(",", "");
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("http://bike18.nethouse.ru/api/export-import/import-from-csv?fileapi" + epoch);
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:44.0) Gecko/20100101 Firefox/44.0";
            req.Method = "POST";
            req.ContentType = "multipart/form-data; boundary=---------------------------12709277337355";
            req.CookieContainer = cookie;
            req.Headers.Add("X-Requested-With", "XMLHttpRequest");
            byte[] csv = File.ReadAllBytes("naSite.csv");
            byte[] end = Encoding.ASCII.GetBytes("\r\n-----------------------------12709277337355\r\nContent-Disposition: form-data; name=\"_catalog_file\"\r\n\r\nnaSite.csv\r\n-----------------------------12709277337355--\r\n");
            byte[] ms1 = Encoding.ASCII.GetBytes("-----------------------------12709277337355\r\nContent-Disposition: form-data; name=\"catalog_file\"; filename=\"naSite.csv\"\r\nContent-Type: text/csv\r\n\r\n");
            req.ContentLength = ms1.Length + csv.Length + end.Length;
            Stream stre1 = req.GetRequestStream();
            stre1.Write(ms1, 0, ms1.Length);
            stre1.Write(csv, 0, csv.Length);
            stre1.Write(end, 0, end.Length);
            stre1.Close();
            HttpWebResponse resimg = (HttpWebResponse)req.GetResponse();
            StreamReader ressrImg = new StreamReader(resimg.GetResponseStream());
            string otvimg = ressrImg.ReadToEnd();
            return otvimg;
        }

        public string ChekedLoading()
        {
            CookieContainer cookie = webRequest.webCookieBike18();
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("http://bike18.nethouse.ru/api/export-import/check-import");
            req.Accept = "application/json, text/plain, */*";
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:44.0) Gecko/20100101 Firefox/44.0";
            req.Method = "POST";
            req.ContentLength = 0;
            req.ContentType = "application/x-www-form-urlencoded";
            req.CookieContainer = cookie;
            Stream stre1 = req.GetRequestStream();
            stre1.Close();
            HttpWebResponse resimg = (HttpWebResponse)req.GetResponse();
            StreamReader ressrImg = new StreamReader(resimg.GetResponseStream());
            string otvimg = ressrImg.ReadToEnd();
            return otvimg;
        }

        private void btnUpdateImages_Click(object sender, EventArgs e)
        {
            string otv = webRequest.getRequest("http://bike18.nethouse.ru/products/category/2426429");
            MatchCollection razdels = new Regex("(?<=<div class=\"category-capt-txt -text-center\"><a href=\").*?(?=\" class=\"blue\">)").Matches(otv);
            for(int i = 0; razdels.Count > i; i++)
            {
                otv = webRequest.getRequest(razdels[i].ToString() + "/page/all");
                MatchCollection tovars = new Regex("(?<=<div class=\"product-link -text-center\"><a href=\").*?(?=\" >)").Matches(otv);
                for(int n = 0; tovars.Count > n; n++)
                {

                }
            }

        }
    }
}
