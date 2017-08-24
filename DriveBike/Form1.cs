using RacerMotors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Формирование_ЧПУ;
using NehouseLibrary;
using xNet.Net;

namespace DriveBike
{
    public partial class Form1 : Form
    {
        web.WebRequest webRequest = new web.WebRequest();
        CHPU chpu = new CHPU();
        WebClient webClient = new WebClient();
        nethouse nethouse = new nethouse();

        int addCount = 0;
        int editsProduct = 0;
        string boldOpen = "<span style=\"\"font-weight: bold; font-weight: bold; \"\">";
        string boldClose = "</span>";
        double discounts = 0.02;
        FileEdit files = new FileEdit();
        string otv = null;
        string miniTextTemplate = "";
        string fullTextTemplate = "";
        string titleTextTemplate = "";
        string descriptionTextTemplate = "";
        string keywordsTextTemplate = "";
        List<string> newProduct = new List<string>();

        bool chekedSEO;

        public Form1()
        {
            InitializeComponent();

            tbLogin.Text = Properties.Settings.Default.login;
            tbPassword.Text = Properties.Settings.Default.password;

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
            #region Подготовка к работе
            Properties.Settings.Default.login = tbLogin.Text;
            Properties.Settings.Default.password = tbPassword.Text;
            Properties.Settings.Default.Save();

            chekedSEO = cbUpdateSEO.Checked;

            CookieDictionary cookie = nethouse.CookieNethouse(tbLogin.Text, tbPassword.Text);

            if (cookie.Count == 1)
            {
                MessageBox.Show("Логин или пароль введены не верно!", "Ошибка");
                return;
            }

            File.Delete("naSite.csv");
            nethouse.NewListUploadinBike18("naSite");

            #endregion

            miniTextTemplate = MinitextStr();
            fullTextTemplate = FulltextStr();
            titleTextTemplate = tbTitle.Lines[0].ToString();
            descriptionTextTemplate = tbDescription.Lines[0].ToString();
            keywordsTextTemplate = tbKeywords.Lines[0].ToString();

            editsProduct = 0;

            otv = nethouse.getRequest("http://www.drivebike.ru/rashodniki-dlya-motocikla-i-kvadrocikla");
            MatchCollection categoriesUrls = new Regex("(?<=<li class=\"amshopby-cat amshopby-cat-level-1\">)[\\w\\W]*?(?=</li>)").Matches(otv);

            for (int i = 3; categoriesUrls.Count > i; i++)
            {
                string categories = new Regex("(?<=<a href=\").*?(?=\">)").Match(categoriesUrls[i].ToString()).ToString();
                if (categories == "http://www.drivebike.ru/rashodniki-dlya-motocikla-i-kvadrocikla/motornoye-maslo-i-smazki")
                    continue;

                string section1 = "Расходники для японских, европейских, американских мотоциклов";
                string section2 = new Regex("(?<=\">).*?(?=</a>)").Match(categoriesUrls[i].ToString()).ToString();

                otv = nethouse.getRequest(categories + "?limit=60");

                string allPagesText = new Regex("(?<=<ol>)[\\w\\W]*?(?=</ol>)").Match(otv).ToString();
                MatchCollection pagesUrl = new Regex("(?<=<a href=\").*?(?=\">)").Matches(allPagesText);
                int countPages = pagesUrl.Count;
                int pages = 1;

                do
                {
                    if (pages != 1)
                    {
                        otv = nethouse.getRequest(pagesUrl[pages].ToString());
                    }

                    MatchCollection cartTovars = new Regex("(?<=<div class=\"item col-lg-3 col-md-4 col-sm-4 respl-item\">)[\\w\\W]*?(?=<!-- QUICKVIEW -->)").Matches(otv);
                    foreach (Match str in cartTovars)
                    {
                        string cartTovar = str.ToString();
                        string urlTovar = new Regex("(?<=<div class=\"item-title\">)[\\w\\W]*?(?=\" title=\")").Match(cartTovar).ToString().Trim();
                        urlTovar = urlTovar.Replace("<a href=\"", "");
                        bool buyTovar = cartTovar.Contains("<span>Купить</span>");

                        if (!buyTovar)
                            continue;

                        string otvTovar = nethouse.getRequest(urlTovar);

                        List<string[]> tovarDB = getTovarDB(otvTovar, section1, section2);

                        string[] resultSearch = SearchTovar(tovarDB);

                        for (int y = 0; resultSearch.Length > y; y++)
                        {
                            string urlProduct = resultSearch[y];
                            string[] product = tovarDB[y];

                            if (urlProduct == "")
                            {
                                WriteTovarInCSV(product);
                            }
                            else
                            {
                                UpdatePrice(cookie, urlProduct, product);
                            }
                        }
                    }
                    pages++;
                } while (pages < countPages);

                //загружаем на сайт
                #region
                System.Threading.Thread.Sleep(20000);
                string[] naSite1 = File.ReadAllLines("naSite.csv", Encoding.GetEncoding(1251));
                if (naSite1.Length > 1)
                    nethouse.UploadCSVNethouse(cookie, "naSite.csv", tbLogin.Text, tbPassword.Text);
                File.Delete("naSite.csv");
                nethouse.NewListUploadinBike18("naSite");
                #endregion

            }

            //запчасти
            File.Delete("naSite.csv");
            File.Delete("noAvailability");
            nethouse.NewListUploadinBike18("naSite.csv");

            otv = webRequest.getRequest("http://www.drivebike.ru/zapchasti");
            categoriesUrls = new Regex("(?<=<li class=\"amshopby-cat amshopby-cat-level-1\">)[\\w\\W]*?(?=</li>)").Matches(otv);

            for (int i = 0; categoriesUrls.Count > i; i++)
            {
                string categories = new Regex("(?<=<a href=\").*?(?=\">)").Match(categoriesUrls[i].ToString()).ToString();

                string section1 = "Расходники для японских, европейских, американских мотоциклов";
                string section2 = new Regex("(?<=\">).*?(?=</a>)").Match(categoriesUrls[i].ToString()).ToString();

                otv = nethouse.getRequest(categories + "?limit=60");

                string allPagesText = new Regex("(?<=<ol>)[\\w\\W]*?(?=</ol>)").Match(otv).ToString();
                MatchCollection pagesUrl = new Regex("(?<=<a href=\").*?(?=\">)").Matches(allPagesText);
                int countPages = pagesUrl.Count;
                int pages = 0;

                do
                {
                    pages++;
                    if (pages != 1)
                    {
                        otv = nethouse.getRequest(pagesUrl[pages].ToString());
                    }

                    MatchCollection cartTovars = new Regex("(?<=<div class=\"item col-lg-3 col-md-4 col-sm-4 respl-item\">)[\\w\\W]*?(?=<!-- QUICKVIEW -->)").Matches(otv);
                    foreach (Match str in cartTovars)
                    {
                        string cartTovar = str.ToString();
                        string urlTovar = new Regex("(?<=<div class=\"item-title\">)[\\w\\W]*?(?=\" title=\")").Match(cartTovar).ToString().Trim();
                        urlTovar = urlTovar.Replace("<a href=\"", "");
                        bool buyTovar = cartTovar.Contains("<span>Купить</span>");

                        if (!buyTovar)
                            continue;

                        string otvTovar = nethouse.getRequest(urlTovar);

                        List<string[]> tovarDB = getTovarDB(otvTovar, section1, section2);

                        string[] resultSearch = SearchTovar(tovarDB);

                        for (int y = 0; resultSearch.Length > y; y++)
                        {
                            string urlProduct = resultSearch[y];
                            string[] product = tovarDB[y];

                            if (urlProduct == "")
                            {
                                WriteTovarInCSV(product);
                            }
                            else
                            {
                                UpdatePrice(cookie, urlProduct, product);
                            }
                        }
                    }
                } while (pages < countPages);

                //загружаем на сайт
                #region
                System.Threading.Thread.Sleep(20000);
                string[] naSite1 = File.ReadAllLines("naSite.csv", Encoding.GetEncoding(1251));
                if (naSite1.Length > 1)
                    nethouse.UploadCSVNethouse(cookie, "naSite.csv", tbLogin.Text, tbPassword.Text);
                File.Delete("naSite.csv");
                nethouse.NewListUploadinBike18("naSite");
                #endregion

                //обновляем наличие товара
                #region
                /*
                System.Threading.Thread.Sleep(70000);
                if (File.Exists("noAvailability"))
                {
                    string[] noAvailabilityArr = File.ReadAllLines("noAvailability");
                    if (noAvailabilityArr.Length > 0)
                    {
                        for (int z = 0; noAvailabilityArr.Length > z; z++)
                        {
                            string[] str = noAvailabilityArr[z].Split(';');
                            string articl = str[0];
                            string name = str[1];

                            otv = webRequest.getRequest("http://bike18.ru/products/search/page/1?sort=0&balance=&categoryId=&min_cost=&max_cost=&text=" + name);
                            MatchCollection searchTovars = new Regex("(?<=\" >).*?(?=</a>)").Matches(otv);
                            bool b = ReturnBoolB(searchTovars, name);

                            if (b)
                            {
                                //товар найден и надо обновить цену
                                string urlTovar = null;
                                MatchCollection searchTovarsBike = new Regex("(?<=<div class=\"product-link -text-center\"><a href=\").*?(?=\" >)").Matches(otv);
                                for (int m = 0; searchTovarsBike.Count > m; m++)
                                {
                                    string searchNameTovar = new Regex("(?<=" + searchTovarsBike[m].ToString() + "\" >).*?(?=</a>)").Match(otv).ToString();
                                    if (searchNameTovar == name)
                                    {
                                        urlTovar = searchTovarsBike[m].ToString();
                                        List<string> listProd = webRequest.arraySaveimage(urlTovar);
                                        listProd[43] = "0";

                                        webRequest.saveTovar(listProd);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                */
                #endregion
                File.Delete("noAvailability");
            }
            MessageBox.Show("Обновлено товаров на сайте");
        }

        private void UpdatePrice(CookieDictionary cookie, string urlSearch, string[] tovarDB)
        {
            bool edits = false;

            string article = tovarDB[0];
            string name = tovarDB[1];
            string price = tovarDB[2];
            string miniText = tovarDB[3];
            string fullText = tovarDB[4];

            List<string> productB18 = new List<string>();
            productB18 = nethouse.GetProductList(cookie, urlSearch);
            if (productB18 == null)
                return;

            int priceDB = Convert.ToInt32(price);
            int priceB18 = Convert.ToInt32(productB18[9]);

            if (priceB18 != priceDB)
            {
                productB18[9] = priceDB.ToString();
                edits = true;
            }

            if (chekedSEO)
            {
                string titleText = ReplaceSEO(titleTextTemplate, name, article);
                string descriptionText = ReplaceSEO(descriptionTextTemplate, name, article);
                string keywordsText = ReplaceSEO(keywordsTextTemplate, name, article);

                productB18[11] = Remove(descriptionText, 200);
                productB18[12] = Remove(keywordsText, 100);
                productB18[13] = Remove(titleText, 255);
                edits = true;
            }

            if (edits)
            {
                nethouse.SaveTovar(cookie, productB18);
                editsProduct++;
            }
        }

        private void WriteTovarInCSV(string[] tovarDB)
        {
            string article = tovarDB[0];
            string name = tovarDB[1];
            string price = tovarDB[2];
            string miniText = tovarDB[3];
            string fullText = tovarDB[4];
            string razdel = tovarDB[5];

            string slug = chpu.vozvr(name);
            slug = Remove(slug, 64);

            miniText = Replace(miniTextTemplate, name, article, miniText, "");
            fullText = Replace(fullTextTemplate, name, article, "", fullText);

            string titleText = ReplaceSEO(titleTextTemplate, name, article);
            string descriptionText = ReplaceSEO(descriptionTextTemplate, name, article);
            string keywordsText = ReplaceSEO(keywordsTextTemplate, name, article);

            titleText = Remove(titleText, 255);
            descriptionText = Remove(descriptionText, 200);
            keywordsText = Remove(keywordsText, 100);

            newProduct = new List<string>();
            newProduct.Add(""); //id
            newProduct.Add("\"" + article + "\""); //артикул
            newProduct.Add("\"" + name + "\"");  //название
            newProduct.Add("\"" + price + "\""); //стоимость
            newProduct.Add("\"" + "" + "\""); //со скидкой
            newProduct.Add("\"" + razdel + "\""); //раздел товара
            newProduct.Add("\"" + "100" + "\""); //в наличии
            newProduct.Add("\"" + "0" + "\"");//поставка
            newProduct.Add("\"" + "1" + "\"");//срок поставки
            newProduct.Add("\"" + miniText + "\"");//краткий текст
            newProduct.Add("\"" + fullText + "\"");//полностью текст
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

        private string[] SearchTovar(List<string[]> tovarDB)
        {
            int countTovars = tovarDB.Count;
            string[] search = new string[countTovars];

            for (int i = 0; countTovars > i; i++)
            {
                string[] product = tovarDB[i];

                string articl = product[0];
                string name = product[1];

                otv = webRequest.getRequest("https://bike18.ru/products/search/page/1?sort=0&balance=&categoryId=&min_cost=&max_cost=&text=" + articl);
                MatchCollection cartSearchProducts = new Regex("(?<=<div class=\"product-item__link\">).*?(?=</div>)").Matches(otv);
                for (int y = 0; cartSearchProducts.Count > y; y++)
                {
                    string cartSearch = cartSearchProducts[y].ToString();
                    string nameSearch = new Regex("(?<=\">).*?(?=</a>)").Match(cartSearch).ToString();
                    string urlSearch = new Regex("(?<=<a href=\").*?(?=\">)").Match(cartSearch).ToString();

                    if (name == nameSearch)
                    {
                        search[i] = urlSearch;
                    }
                    else
                    {
                        search[i] = "";
                    }
                }
                if (cartSearchProducts.Count == 0)
                    search[i] = "";
            }
            return search;
        }

        private List<string[]> getTovarDB(string otvTovar, string section1, string section2)
        {
            List<string[]> tovarsList = new List<string[]>();
            string[] product = new string[6];

            string articl = new Regex("(?<=Код товара:).*?(?=<br />)").Match(otvTovar).ToString().Trim();
            articl = "DB_" + articl.Replace("-", "_");

            string urlImageProduct = new Regex("(?<=id=\"image\" src=\").*?(?=\")").Match(otvTovar).ToString();
            DownloadImages(urlImageProduct, articl);

            string nameTovar = new Regex("(?<=<li class=\"product\")[\\w\\W]*?</strong>").Match(otvTovar).ToString();
            nameTovar = new Regex("(?<=<strong>).*?(?=</strong>)").Match(nameTovar).ToString();
            nameTovar = nameTovar.Replace("[", "").Replace("]", "");

            string miniDescription = new Regex("(?<=ИНФОРМАЦИЯ:</h2>)[\\w\\W]*?(?=</div>)").Match(otvTovar).ToString().Trim();
            string numberCatalog = new Regex("Номер по каталогу:.*?<br />").Match(miniDescription).ToString();
            string codeCatalog = new Regex("(?<=Код товара: )[\\w\\W]*?(?=<br />)").Match(miniDescription).ToString();
            miniDescription = miniDescription.Replace(numberCatalog, "").Replace(codeCatalog, articl).Replace("'", "\"");

            MatchCollection ahref = new Regex("<a.*?</a>").Matches(miniDescription);
            if (ahref.Count != 0)
                miniDescription = ReplaceUrl(ahref, miniDescription);

            string fullDescriptionTable = new Regex("(?<=<div class=\"attribute-specs\">)[\\w\\W]*?</table>").Match(otvTovar).ToString().Trim().Replace("\r\n", "");
            string fullDescription = new Regex("(?<=<div class=\"std\" itemprop=\"description\">)[\\w\\W]*?(?=</div>)").Match(otvTovar).ToString().Trim().Replace("\r\n", "");

            ahref = new Regex("<a.*?</a>").Matches(fullDescriptionTable);
            if (ahref.Count != 0)
                fullDescriptionTable = ReplaceUrl(ahref, fullDescriptionTable);

            ahref = new Regex("<a.*?</a>").Matches(fullDescription);
            if (ahref.Count != 0)
                fullDescription = ReplaceUrl(ahref, fullDescription);


            MatchCollection atributes = new Regex("(?<=<table).*?(?=>)").Matches(fullDescriptionTable);
            if (atributes.Count != 0)
                fullDescriptionTable = ReplaceUrl(atributes, fullDescriptionTable);

            atributes = new Regex("(?<=<td).*?(?=>)").Matches(fullDescriptionTable);
            if (atributes.Count != 0)
                fullDescriptionTable = ReplaceUrl(atributes, fullDescriptionTable);

            string razdel = "Запчасти и расходники => " + section1 + " => " + section2;

            string price = new Regex("(?<=<meta itemprop=\"price\" content=\").*?(?=\" />)").Match(otvTovar).ToString();
            if (price == "")
            {
                MatchCollection cartProduct = new Regex("(?<=<tr>)[\\w\\W]*?(?=</tr>)").Matches(otvTovar);
                for (int i = 0; cartProduct.Count > i; i++)
                {
                    string name = new Regex("(?<=name=\"cur_pro_name\" value=\").*?(?=\")").Match(cartProduct[i].ToString()).ToString();
                    name = name.Replace("[", "").Replace("]", "");

                    if (name == "")
                        continue;

                    string priceStr = new Regex("(?<=<span class=\"price\">).*?(?=руб.)").Match(cartProduct[i].ToString()).ToString();
                    priceStr = priceStr.Replace("1 ", "1").Replace("2 ", "2").Replace("3 ", "3").Replace("4 ", "4").Replace("5 ", "5").Replace("6 ", "6").Replace("7 ", "7").Replace("8 ", "8").Replace("9 ", "9").Trim();
                    int priceProduct = Convert.ToInt32(priceStr);
                    int actualPriceProduct = nethouse.ReturnPrice(priceProduct, discounts);

                    product[0] = articl;
                    product[1] = name;
                    product[2] = actualPriceProduct.ToString();
                    product[3] = miniDescription;
                    product[4] = "<p>" + fullDescriptionTable + "</p><p>" + fullDescription + "</p>";
                    product[5] = razdel;

                    tovarsList.Add(product);
                }
            }
            else
            {
                int priceProduct = Convert.ToInt32(price);
                int actualPriceProduct = nethouse.ReturnPrice(priceProduct, discounts);
                price = actualPriceProduct.ToString();

                product[0] = articl;
                product[1] = nameTovar;
                product[2] = price;
                product[3] = miniDescription;
                product[4] = "<p>" + fullDescriptionTable + "</p><p>" + fullDescription + "</p>";
                product[5] = razdel;

                tovarsList.Add(product);
            }

            return tovarsList;
        }

        private string ReplaceUrl(MatchCollection urls, string text)
        {
            foreach (Match str in urls)
            {
                string url = str.ToString();
                if (url != "")
                    text = text.Replace(url, "");
            }
            return text;
        }

        private void DownloadImages(string urlImageProduct, string articl)
        {
            try
            {
                webClient.DownloadFile(urlImageProduct, "Pic\\" + articl + ".jpg");
            }
            catch
            {

            }
        }

        private MatchCollection countPagesTovars(string otv)
        {
            MatchCollection pagesUrl = null;
            string pagesString = new Regex("(?<=<li class=\"current\">)[\\w\\W]*?(?=<a class=\"next i-next\" )").Match(otv).ToString();
            if (pagesString != "")
                pagesUrl = new Regex("(?<=><a href=\")[\\w\\W]*?(?=\">)").Matches(pagesString);

            return pagesUrl;
        }

        private void SaveProductInCSV(List<string> newProduct, string articl, string nameTovar, int priceActual, string razdel, string miniTextTemplate, string fullTextTemplate, string titleText, string descriptionText, string keywordsText, string slug)
        {
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

        private string ReturnNameTovar(string otv)
        {
            string nameTovar = new Regex("(?<=<h1><font style=\"color:#459B06; \">).*(?=</h1>)").Match(otv).ToString();
            nameTovar = nameTovar.Replace("</font><br/>", " ");
            return nameTovar;
        }

        private void dowloadImagesTovar(string urlImageProduct, string articl)
        {
            try
            {
                webClient.DownloadFile(urlImageProduct, "pic\\" + articl + ".jpg");
            }
            catch
            {

            }
        }

        private void ErrorDownloadInSite37(string otvimg)
        {
            string errstr = new Regex("(?<=errorLine\":).*?(?=,\")").Match(otvimg).ToString();
            string[] naSite = File.ReadAllLines("naSite.csv", Encoding.GetEncoding(1251));
            int u = Convert.ToInt32(errstr) - 1;
            string[] strslug3 = naSite[u].ToString().Split(';');
            string strslug = strslug3[strslug3.Length - 5];
            int slug = strslug.Length;
            int countAdd = ReturnCountAdd();
            int countDel = countAdd.ToString().Length;
            if (strslug.Contains("\""))
            {
                countDel = countDel + 2;
            }
            string strslug2 = strslug.Remove(slug - countDel);
            strslug2 += countAdd;
            strslug2 = strslug2.Replace("”", "").Replace("~", "").Replace("#", "");
            if (strslug2.Contains("\""))
            {
                strslug2 = strslug2 + "\"";
                countDel = countDel - 2;
            }
            naSite[u] = naSite[u].Replace(strslug, strslug2);
            File.WriteAllLines("naSite.csv", naSite, Encoding.GetEncoding(1251));
        }

        private void ErrorDownloadInSite13(string otvimg)
        {
            string errstr = new Regex("(?<=errorLine\":).*?(?=,\")").Match(otvimg).ToString();
            string[] naSite = File.ReadAllLines("naSite.csv", Encoding.GetEncoding(1251));
            int u = Convert.ToInt32(errstr) - 1;
            string[] strslug3 = naSite[u].ToString().Split(';');
            string strslug = strslug3[strslug3.Length - 5];
            int slug = strslug.Length;
            int countAdd = ReturnCountAdd();
            int countDel = countAdd.ToString().Length;
            if (strslug.Contains("\""))
                countDel = countDel + 1;
            string strslug2 = strslug.Remove(slug - countDel);
            if (strslug.Contains("\""))
                strslug2 += countAdd + "\"";
            else
                strslug2 += countAdd;
            naSite[u] = naSite[u].Replace(strslug, strslug2);
            File.WriteAllLines("naSite.csv", naSite, Encoding.GetEncoding(1251));
        }

        private bool ReturnBoolB(MatchCollection searchTovars, string nameTovar)
        {
            bool b = false;
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
            return b;
        }

        private string ReturnTable(string otv)
        {
            string table = new Regex("<table[\\w\\W]*?</table>").Match(otv).ToString().Replace("\n        ", "").Replace("            ", " ").Replace("  ", " ").Replace("    ", " ").Replace("        ", " ").Replace("  ", "").Replace("\n", "").Replace(" class=\"data\"", "").Replace(" class=\"label\"", "").Replace(" class=\"data-table\" id=\"product-attribute-specs-table\"><col width=\"25%\" /><col /", "");
            return table;
        }

        private string ReturnPrice(string tv)
        {

            string pricePodtovar = new Regex("(?<=span class=\"price\">).*(?= р.</span)").Match(tv).ToString();
            if (pricePodtovar == "")
                pricePodtovar = new Regex("(?<=span class=\"price-label\"></span>)[\\w\\W]*?(?=</span)").Match(tv).ToString();
            if (pricePodtovar.Contains("\n"))
                pricePodtovar = new Regex("(?<=\">)[\\w\\W]*?(?= р.)").Match(pricePodtovar).ToString();
            pricePodtovar = pricePodtovar.Replace("р.", "").Trim();
            pricePodtovar = pricePodtovar.Replace("1 ", "1").Replace("2 ", "2").Replace("3 ", "3").Replace("4 ", "4").Replace("5 ", "5").Replace("6 ", "6").Replace("7 ", "7").Replace("8 ", "8").Replace("9 ", "9").Replace("0 ", "0").Trim();
            if (pricePodtovar == "")
            {

            }
            return pricePodtovar;
        }

        private void btnUpdateImages_Click(object sender, EventArgs e)
        {
            //CookieContainer cookie = webRequest.webCookieBike18();
            //otv = webRequest.getRequest("http://bike18.nethouse.ru/products/category/2426429");
            //MatchCollection razdels = new Regex("(?<=<div class=\"category-capt-txt -text-center\"><a href=\").*?(?=\" class=\"blue\">)").Matches(otv);
            //for (int i = 0; razdels.Count > i; i++)
            //{
            //    otv = webRequest.getRequest(razdels[i].ToString() + "/page/all");
            //    MatchCollection tovars = new Regex("(?<=<div class=\"product-link -text-center\"><a href=\").*?(?=\" >)").Matches(otv);
            //    for (int n = 0; tovars.Count > n; n++)
            //    {
            //        string urlTovar = tovars[n].ToString().Replace("http://bike18.ru/", "http://bike18.nethouse.ru/");
            //        otv = webRequest.PostRequest(cookie, urlTovar);
            //        string articl = new Regex("(?<=Артикул:)[\\w\\W]*?(?=</div><div>)").Match(otv).ToString().Replace("\n", "").Trim();
            //        if (articl.Length > 11)
            //        {
            //            articl = new Regex("(?<=Артикул:)[\\w\\W]*(?=</title>)").Match(otv).ToString().Trim();
            //        }
            //        if (File.Exists("pic\\" + articl + ".jpg"))
            //        {
            //            MatchCollection prId = new Regex("(?<=data-id=\").*?(?=\")").Matches(otv);
            //            int prodId = Convert.ToInt32(prId[0].ToString());
            //            bool b = true;
            //            double widthImg = 0;
            //            double heigthImg = 0;

            //            try
            //            {
            //                Image newImg = Image.FromFile("pic\\" + articl + ".jpg");
            //                widthImg = newImg.Width;
            //                heigthImg = newImg.Height;
            //            }
            //            catch
            //            {
            //                b = false;
            //            }

            //            if (b)
            //            {
            //                if (widthImg > heigthImg)
            //                {
            //                    double dblx = widthImg * 0.9;
            //                    if (dblx < heigthImg)
            //                    {
            //                        heigthImg = heigthImg * 0.9;
            //                    }
            //                    else
            //                        widthImg = widthImg * 0.9;
            //                }
            //                else
            //                {
            //                    double dblx = heigthImg * 0.9;
            //                    if (dblx < widthImg)
            //                    {
            //                        widthImg = widthImg * 0.9;
            //                    }
            //                    else
            //                        heigthImg = heigthImg * 0.9;
            //                }

            //                string otvimg = DownloadImages(articl);
            //                string urlSaveImg = new Regex("(?<=url\":\").*?(?=\")").Match(otvimg).Value.Replace("\\/", "%2F");
            //                string otvSave = SaveImages(urlSaveImg, prodId, widthImg, heigthImg);
            //                List<string> listProd = webRequest.arraySaveimage(urlTovar);
            //                listProd[3] = "10833347";
            //                listProd[42] = alsoBuyTovars(listProd);
            //                otv = webRequest.saveTovar(listProd);
            //                if (otv.Contains("errors"))
            //                {
            //                    int g = 1;
            //                    if (otv.Contains("slug"))
            //                    {
            //                        do
            //                        {
            //                            string s = listProd[1].ToString();
            //                            s = s.Remove(s.Length - 1, 1);
            //                            s = s + g;
            //                            g++;
            //                            listProd[1] = s;
            //                            otv = webRequest.saveTovar(listProd);
            //                        }
            //                        while (otv.Contains("errors"));

            //                    }

            //                }
            //            }
            //        }
            //    }
            //}

            CookieContainer cookie = webRequest.webCookieBike18();
            otv = webRequest.getRequest("http://bike18.nethouse.ru/products/category/2426430");
            MatchCollection razdels = new Regex("(?<=<div class=\"category-capt-txt -text-center\"><a href=\").*?(?=\" class=\"blue\">)").Matches(otv);
            for (int i = 0; razdels.Count > i; i++)
            {
                otv = webRequest.getRequest(razdels[i].ToString() + "/page/all");
                MatchCollection tovars = new Regex("(?<=<div class=\"product-link -text-center\"><a href=\").*?(?=\" >)").Matches(otv);
                for (int n = 0; tovars.Count > n; n++)
                {
                    string urlTovar = tovars[n].ToString().Replace("http://bike18.ru/", "http://bike18.nethouse.ru/");
                    otv = webRequest.PostRequest(cookie, urlTovar);
                    string articl = new Regex("(?<=Артикул:)[\\w\\W]*?(?=</div><div>)").Match(otv).ToString().Replace("\n", "").Trim();
                    if (articl.Length > 11)
                    {
                        articl = new Regex("(?<=Артикул:)[\\w\\W]*(?=</title>)").Match(otv).ToString().Trim();
                    }
                    if (File.Exists("pic\\" + articl + ".jpg"))
                    {
                        MatchCollection prId = new Regex("(?<=data-id=\").*?(?=\")").Matches(otv);
                        int prodId = Convert.ToInt32(prId[0].ToString());
                        bool b = true;
                        double widthImg = 0;
                        double heigthImg = 0;

                        try
                        {
                            Image newImg = Image.FromFile("pic\\" + articl + ".jpg");
                            widthImg = newImg.Width;
                            heigthImg = newImg.Height;
                        }
                        catch
                        {
                            b = false;
                        }

                        if (b)
                        {
                            if (widthImg > heigthImg)
                            {
                                double dblx = widthImg * 0.9;
                                if (dblx < heigthImg)
                                {
                                    heigthImg = heigthImg * 0.9;
                                }
                                else
                                    widthImg = widthImg * 0.9;
                            }
                            else
                            {
                                double dblx = heigthImg * 0.9;
                                if (dblx < widthImg)
                                {
                                    widthImg = widthImg * 0.9;
                                }
                                else
                                    heigthImg = heigthImg * 0.9;
                            }

                            string otvimg = DownloadImages(articl);
                            string urlSaveImg = new Regex("(?<=url\":\").*?(?=\")").Match(otvimg).Value.Replace("\\/", "%2F");
                            string otvSave = SaveImages(urlSaveImg, prodId, widthImg, heigthImg);
                            List<string> listProd = webRequest.arraySaveimage(urlTovar);
                            listProd[3] = "10833347";
                            listProd[42] = alsoBuyTovars(listProd);
                            otv = webRequest.saveTovar(listProd);
                            if (otv.Contains("errors"))
                            {
                                int g = 1;
                                if (otv.Contains("slug"))
                                {
                                    do
                                    {
                                        string s = listProd[1].ToString();
                                        s = s.Remove(s.Length - 1, 1);
                                        s = s + g;
                                        g++;
                                        listProd[1] = s;
                                        otv = webRequest.saveTovar(listProd);
                                    }
                                    while (otv.Contains("errors"));
                                }
                            }
                        }
                    }
                }
            }
            MessageBox.Show("Обновление завершено");
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

        private string Replace(string text, string nameTovar, string article, string miniText, string fullText)
        {
            string discount = Discount();
            string nameText = boldOpen + nameTovar + boldClose;
            text = text.Replace("СКИДКА", discount).Replace("НАЗВАНИЕ", nameText).Replace("АРТИКУЛ", article).Replace("МИНИТЕКСТ", miniText).Replace("ТЕКСТ", fullText).Replace("<p><br /></p><p><br /></p><p><br /></p><p>", "<p><br /></p>");
            return text;
        }

        private string ReplaceSEO(string text, string nameTovarRacerMotors, string article)
        {
            string discount = Discount();
            text = text.Replace("СКИДКА", discount).Replace("НАЗВАНИЕ", nameTovarRacerMotors).Replace("АРТИКУЛ", article);
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
                try
                {
                    text = text.Remove(text.LastIndexOf(" "));
                }
                catch
                {

                }
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

        private string alsoBuyTovars(List<string> tovarList)
        {
            string name = tovarList[4].ToString();
            otv = webRequest.getRequest("http://bike18.ru/products/search/page/1?sort=0&balance=&categoryId=&min_cost=&max_cost=&text=" + name);
            MatchCollection searchTovars = new Regex("(?<=<div class=\"product-item preview-size-156\" id=\"item).*?(?=\"><div class=\"background\">)").Matches(otv);
            string alsoBuy = "";
            int count = 0;
            if (searchTovars.Count > 1)
            {
                for (int i = 1; 5 > i; i++)
                {

                    alsoBuy += "&alsoBuy[" + count + "]=" + searchTovars[i].ToString();
                    count++;
                }
            }
            return alsoBuy;
        }

        public string DownloadImages(string artProd)
        {
            CookieContainer cookie = webRequest.webCookieBike18();
            string epoch = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds.ToString().Replace(",", "");
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("http://bike18.nethouse.ru/putimg?fileapi" + epoch);
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:44.0) Gecko/20100101 Firefox/44.0";
            req.Method = "POST";
            req.ContentType = "multipart/form-data; boundary=---------------------------12709277337355";
            req.CookieContainer = cookie;
            req.Headers.Add("X-Requested-With", "XMLHttpRequest");
            byte[] pic = File.ReadAllBytes("Pic\\" + artProd + ".jpg");
            byte[] end = Encoding.ASCII.GetBytes("\r\n-----------------------------12709277337355\r\nContent-Disposition: form-data; name=\"_file\"\r\n\r\n" + artProd + ".jpg\r\n-----------------------------12709277337355--\r\n");
            byte[] ms1 = Encoding.ASCII.GetBytes("-----------------------------12709277337355\r\nContent-Disposition: form-data; name=\"file\"; filename=\"" + artProd + ".jpg\"\r\nContent-Type: image/jpeg\r\n\r\n");
            req.ContentLength = ms1.Length + pic.Length + end.Length;
            Stream stre1 = req.GetRequestStream();
            stre1.Write(ms1, 0, ms1.Length);
            stre1.Write(pic, 0, pic.Length);
            stre1.Write(end, 0, end.Length);
            stre1.Close();
            HttpWebResponse resimg = (HttpWebResponse)req.GetResponse();
            StreamReader ressrImg = new StreamReader(resimg.GetResponseStream());
            string otvimg = ressrImg.ReadToEnd();
            return otvimg;
        }

        public string SaveImages(string urlSaveImg, int prodId, double widthImg, double heigthImg)
        {
            CookieContainer cookie = webRequest.webCookieBike18();
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("http://bike18.nethouse.ru/api/catalog/save-image");
            req.Accept = "application/json, text/plain, */*";
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:44.0) Gecko/20100101 Firefox/44.0";
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.CookieContainer = cookie;
            byte[] saveImg = Encoding.ASCII.GetBytes("url=" + urlSaveImg + "&id=0&type=4&objectId=" + prodId + "&imgCrop[x]=0&imgCrop[y]=0&imgCrop[width]=" + widthImg + "&imgCrop[height]=" + heigthImg + "&imageId=0&iObjectId=" + prodId + "&iImageType=4&replacePhoto=0");
            req.ContentLength = saveImg.Length;
            Stream srSave = req.GetRequestStream();
            srSave.Write(saveImg, 0, saveImg.Length);
            srSave.Close();
            HttpWebResponse resSave = (HttpWebResponse)req.GetResponse();
            StreamReader ressrSave = new StreamReader(resSave.GetResponseStream());
            string otvSave = ressrSave.ReadToEnd();
            return otvSave;
        }
    }
} // 2230 строк старая
  //1570 новая