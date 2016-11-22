using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bike18
{
    class nethouse
    {
        int addCount = 0;

        httpRequest webRequest = new httpRequest();

        public CookieContainer CookieNethouse(string login, string password)
        {
            CookieContainer cookie = new CookieContainer();
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("https://nethouse.ru/signin");
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:44.0) Gecko/20100101 Firefox/44.0";
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.CookieContainer = cookie;
            byte[] ms = Encoding.ASCII.GetBytes("login=" + login + "&password=" + password + "&quick_expire=0&submit=%D0%92%D0%BE%D0%B9%D1%82%D0%B8");
            req.ContentLength = ms.Length;
            Stream stre = req.GetRequestStream();
            stre.Write(ms, 0, ms.Length);
            stre.Close();
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();
            return cookie;
        }

        public List<string> GetProductList(CookieContainer cookie, string urlTovar)
        {
            if (!urlTovar.Contains("nethouse"))
                urlTovar = urlTovar.Replace("http://bike18.ru/", "http://bike18.nethouse.ru/");

            List<string> listTovar = new List<string>();
            string otv = webRequest.PostRequest(cookie, urlTovar);
            if (otv != null)
            {
                string productId = new Regex("(?<=<section class=\"comment\" id=\").*?(?=\">)").Match(otv).ToString();
                String article = new Regex("(?<=Артикул:)[\\w\\W]*?(?=</div>)").Match(otv).Value.Trim();
                if (article.Length > 15)
                {
                    article = new Regex("(?<=Артикул:)[\\w\\W]*(?=</title>)").Match(otv).ToString().Trim();
                }
                String prodName = new Regex("(?<=<h1>).*(?=</h1>)").Match(otv).Value;
                String price = new Regex("(?<=<span class=\"product-price-data\" data-cost=\").*?(?=\">)").Match(otv).Value;
                String imgId = new Regex("(?<=<div id=\"avatar-).*(?=\")").Match(otv).Value;
                String desc = new Regex("(?<=<div class=\"user-inner\">).*?(?=</div>)").Match(otv).Value;
                desc = desc.Replace("&nbsp;", " ");
                String fulldesc = new Regex("(?<=<div id=\"product-full-desc\" data-ng-non-bindable class=\"user-inner\">).*?(?=</div>)").Match(otv).Value.Replace("&nbsp;&nbsp;", " ").Replace("&deg;", "°");
                fulldesc = fulldesc.Replace("&nbsp;", " ");
                String seometa = new Regex("(?<=<meta name=\"description\" content=\").*?(?=\" >)").Match(otv).Value;
                String keywords = new Regex("(?<=<meta name=\"keywords\" content=\").*?(?=\" >)").Match(otv).Value;
                String title = new Regex("(?<=<title>).*?(?=</title>)").Match(otv).Value;
                String visible = new Regex("(?<=,\"balance\":).*?(?=,\")").Match(otv).Value;
                string reklama = new Regex("(?<=<div class=\"marker-icon size-big type-4\"><div class=\"left\"></div><div class=\"center\"><div class=\"text\">).*?(?=</div></div>)").Match(otv).ToString();
                if (reklama == "акция")
                {
                    reklama = "&markers[3]=1";
                }
                if (reklama == "новинка")
                {
                    reklama = "&markers[1]=1";
                }

                otv = webRequest.PostRequest(cookie, "http://bike18.nethouse.ru/api/catalog/getproduct?id=" + productId);
                string slug = new Regex("(?<=\",\"slug\":\").*?(?=\")").Match(otv).ToString();
                string balance = new Regex("(?<=,\"balance\":\").*?(?=\",\")").Match(otv).ToString();
                string productCastomGroup = new Regex("(?<=productCustomGroup\":).*?(?=,\")").Match(otv).ToString();
                string discountCoast = new Regex("(?<=discountCost\":\").*?(?=\")").Match(otv).Value;
                string serial = new Regex("(?<=serial\":\").*?(?=\")").Match(otv).Value;
                string categoryId = new Regex("(?<=\",\"categoryId\":\").*?(?=\")").Match(otv).Value;
                string productGroup = new Regex("(?<=productGroup\":).*?(?=,\")").Match(otv).Value;
                string havenDetail = new Regex("(?<=haveDetail\".).*?(?=,\")").Match(otv).Value;
                string canMakeOrder = new Regex("(?<=canMakeOrder\".).*?(?=,\")").Match(otv).Value;
                canMakeOrder = canMakeOrder.Replace("false", "0");
                canMakeOrder = canMakeOrder.Replace("true", "1");
                string showOnMain = new Regex("(?<=showOnMain\".).*?(?=,\")").Match(otv).Value;
                string customDays = new Regex("(?<=,\"customDays\":\").*?(?=\")").Match(otv).Value;
                string isCustom = new Regex("(?<=\",\"isCustom\":).*?(?=,)").Match(otv).Value;
                string atribut = "";
                string atributes = new Regex("(?<=attributes\":{\").*?(?=,\"customDays)").Match(otv).Value;
                MatchCollection stringAtributes = new Regex("(?<=\":{\").*?(?=])").Matches(atributes);
                for (int i = 0; stringAtributes.Count > i; i++)
                {
                    string id = new Regex("(?<=primaryKey\":).*?(?=,\")").Match(stringAtributes[i].ToString()).Value;
                    string valueId = new Regex("(?<=\"valueId\":\").*?(?=\")").Match(stringAtributes[i].ToString()).Value;
                    string valueText = new Regex("(?<=valueText\":).*?(?=})").Match(stringAtributes[i].ToString()).Value;
                    string text = new Regex("(?<=\"text\":).*?(?=})").Match(stringAtributes[i].ToString()).Value;
                    string checkBox = new Regex("(?<=checkbox\":).*?(?=})").Match(stringAtributes[i].ToString()).Value;

                    if (valueId != "")
                    {
                        atribut = atribut + "&attributes[" + i + "][primaryKey]=" + id + "&attributes[" + i + "][attributeId]=" + id + "&attributes[" + i + "][values][0][empty]=0&attributes[" + i + "][values][0][valueId]=" + valueId;
                    }
                    else
                    {
                        if (text != "")
                            atribut = atribut + "&attributes[" + i + "][primaryKey]=" + id + "&attributes[" + i + "][attributeId]=" + id + "&attributes[" + i + "][values][0][empty]=0&attributes[" + i + "][values][0][text]=" + text;
                        if (checkBox != "")
                            atribut = atribut + "&attributes[" + i + "][primaryKey]=" + id + "&attributes[" + i + "][attributeId]=" + id + "&attributes[" + i + "][values][0][empty]=0&attributes[" + i + "][values][0][checkbox]=" + checkBox;
                    }
                }
                atribut = atribut.Replace("true", "1");
                string alsoBuy = new Regex("(?<=alsoBuy\":).*?(?=,\"markers)").Match(otv).ToString();
                alsoBuy = alsoBuy.Remove(alsoBuy.Length - 1, 1).Remove(0, 1);
                string[] alsoBuyArray = alsoBuy.Split(',');
                string alsoBuyStr = "";

                if (alsoBuyArray.Length > 0)
                {
                    for (int i = 0; alsoBuyArray.Length > i; i++)
                    {
                        alsoBuyStr += "&alsoBuy[" + i + "]=" + alsoBuyArray[i].ToString();
                    }
                }

                otv = webRequest.PostRequest(cookie, "http://bike18.nethouse.ru/api/catalog/productmedia?id=" + productId);
                string avatarId = new Regex("(?<=\"id\":\").*?(?=\")").Match(otv).Value;
                string objektId = new Regex("(?<=\"objectId\":\").*?(?=\")").Match(otv).Value;
                string timestamp = new Regex("(?<=\"timestamp\":\").*?(?=\")").Match(otv).Value;
                string type = new Regex("(?<=\"type\":\").*?(?=\")").Match(otv).Value;
                string name = new Regex("(?<=\",\"name\":\").*?(?=\")").Match(otv).Value;
                string descimg = new Regex("(?<=\"desc\":\").*?(?=\")").Match(otv).Value;
                string ext = new Regex("(?<=\"ext\":\").*?(?=\")").Match(otv).Value;
                string raw = new Regex("(?<=\"raw\":\").*?(?=\")").Match(otv).Value;
                string W215 = new Regex("(?<=\"W215\":\").*?(?=\")").Match(otv).Value;
                string srimg = new Regex("(?<=\"150x120\":\").*?(?=\")").Match(otv).Value;
                string minimg = new Regex("(?<=\"104x82\":\").*?(?=\")").Match(otv).Value;
                string filesize = new Regex("(?<=\"fileSize\":).*?(?=})").Match(otv).Value;
                string alt = new Regex("(?<=\"alt\":\").*?(?=\")").Match(otv).Value;
                string isvisibleonmain = new Regex("(?<=\"isVisibleOnMain\".).*?(?=,)").Match(otv).Value;
                string prioriti = new Regex("(?<=\"priority\":\").*?(?=\")").Match(otv).Value;
                string avatarurl = new Regex("(?<=\"url\":\").*?(?=\")").Match(otv).Value;
                string filtersleft = new Regex("(?<=\"left\":).*?(?=,)").Match(otv).Value;
                string filterstop = new Regex("(?<=\"top\":).*?(?=,)").Match(otv).Value;
                string filtersright = new Regex("(?<=\"right\":).*?(?=,)").Match(otv).Value;
                string filtersbottom = new Regex("(?<=\"bottom\":).*?(?=})").Match(otv).Value;

                listTovar.Add(productId);       //0
                listTovar.Add(slug);            //1
                listTovar.Add(categoryId);      //2
                listTovar.Add(productGroup);    //3
                listTovar.Add(prodName);        //4
                listTovar.Add(serial);          //5
                listTovar.Add(article);         //6
                listTovar.Add(desc);            //7
                listTovar.Add(fulldesc);        //8
                listTovar.Add(price);           //9
                listTovar.Add(discountCoast);   //10
                listTovar.Add(seometa);         //11
                listTovar.Add(keywords);        //12
                listTovar.Add(title);           //13
                listTovar.Add(havenDetail);     //14
                listTovar.Add(canMakeOrder);    //15 купить с сайта в 1 клик
                                                //listTovar.Add(balance);
                listTovar.Add(showOnMain);      //16
                listTovar.Add(avatarId);        //17
                listTovar.Add(objektId);        //18
                listTovar.Add(timestamp);       //19
                listTovar.Add(type);            //20
                listTovar.Add(name);            //21
                listTovar.Add(descimg);         //22
                listTovar.Add(ext);             //23
                listTovar.Add(raw);             //24
                listTovar.Add(W215);            //25
                listTovar.Add(srimg);           //26
                listTovar.Add(minimg);          //27
                listTovar.Add(filesize);        //28
                listTovar.Add(alt);             //29
                listTovar.Add(isvisibleonmain); //30
                listTovar.Add(prioriti);        //31
                listTovar.Add(avatarurl);       //32
                listTovar.Add(filtersleft);     //33
                listTovar.Add(filterstop);      //34
                listTovar.Add(filtersright);    //35
                listTovar.Add(filtersbottom);   //36
                listTovar.Add(customDays);      //37
                listTovar.Add(isCustom);        //38
                listTovar.Add(reklama);         //39
                listTovar.Add(atribut);         //40
                listTovar.Add(productCastomGroup); //41
                listTovar.Add(alsoBuyStr);      //42
                listTovar.Add(balance);         //43
            }
            return listTovar;
        }

        public void DeleteProduct(CookieContainer cookie, List<string> getProduct)
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("http://bike18.nethouse.ru/api/catalog/deleteproduct");
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:44.0) Gecko/20100101 Firefox/44.0";
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.CookieContainer = cookie;
            byte[] ms = System.Text.Encoding.GetEncoding("utf-8").GetBytes("id=" + getProduct[0] + "&slug=" + getProduct[1] + "&categoryId=" + getProduct[2] + "&productGroup=" + getProduct[3] + "&name=" + getProduct[4] + "&serial=" + getProduct[5] + "&serialByUser=" + getProduct[6] + "&desc=" + getProduct[7] + "&descFull=" + getProduct[8] + "&cost=" + getProduct[9] + "&discountCost=" + getProduct[10] + "&seoMetaDesc=" + getProduct[11] + "&seoMetaKeywords=" + getProduct[12] + "&seoTitle=" + getProduct[13] + "&haveDetail=" + getProduct[14] + "&canMakeOrder=" + getProduct[15] + "&balance=100&showOnMain=" + getProduct[16] + "&isVisible=1&hasSale=0&avatar[id]=" + getProduct[17] + "&avatar[objectId]=" + getProduct[18] + "&avatar[timestamp]=" + getProduct[19] + "&avatar[type]=" + getProduct[20] + "&avatar[name]=" + getProduct[21] + "&avatar[desc]=" + getProduct[22] + "&avatar[ext]=" + getProduct[23] + "&avatar[formats][raw]=" + getProduct[24] + "&avatar[formats][W215]=" + getProduct[25] + "&avatar[formats][150x120]=" + getProduct[26] + "&avatar[formats][104x82]=" + getProduct[27] + "&avatar[formatParams][raw][fileSize]=" + getProduct[28] + "&avatar[alt]=" + getProduct[29] + "&avatar[isVisibleOnMain]=" + getProduct[30] + "&avatar[priority]=" + getProduct[31] + "&avatar[url]=" + getProduct[32] + "&avatar[filters][crop][left]=" + getProduct[33] + "&avatar[filters][crop][top]=" + getProduct[34] + "&avatar[filters][crop][right]=" + getProduct[35] + "&avatar[filters][crop][bottom]=" + getProduct[36] + "&customDays=" + getProduct[37] + "&isCustom=" + getProduct[38]);
            req.ContentLength = ms.Length;
            Stream stre = req.GetRequestStream();
            stre.Write(ms, 0, ms.Length);
            stre.Close();
            HttpWebResponse res1 = (HttpWebResponse)req.GetResponse();
            StreamReader ressr1 = new StreamReader(res1.GetResponseStream());
        }

        public void DeleteProduct(CookieContainer cookie, string url)
        {
            List<string> getProduct = GetProductList(cookie, url);
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("http://bike18.nethouse.ru/api/catalog/deleteproduct");
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:44.0) Gecko/20100101 Firefox/44.0";
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.CookieContainer = cookie;
            byte[] ms = System.Text.Encoding.GetEncoding("utf-8").GetBytes("id=" + getProduct[0] + "&slug=" + getProduct[1] + "&categoryId=" + getProduct[2] + "&productGroup=" + getProduct[3] + "&name=" + getProduct[4] + "&serial=" + getProduct[5] + "&serialByUser=" + getProduct[6] + "&desc=" + getProduct[7] + "&descFull=" + getProduct[8] + "&cost=" + getProduct[9] + "&discountCost=" + getProduct[10] + "&seoMetaDesc=" + getProduct[11] + "&seoMetaKeywords=" + getProduct[12] + "&seoTitle=" + getProduct[13] + "&haveDetail=" + getProduct[14] + "&canMakeOrder=" + getProduct[15] + "&balance=100&showOnMain=" + getProduct[16] + "&isVisible=1&hasSale=0&avatar[id]=" + getProduct[17] + "&avatar[objectId]=" + getProduct[18] + "&avatar[timestamp]=" + getProduct[19] + "&avatar[type]=" + getProduct[20] + "&avatar[name]=" + getProduct[21] + "&avatar[desc]=" + getProduct[22] + "&avatar[ext]=" + getProduct[23] + "&avatar[formats][raw]=" + getProduct[24] + "&avatar[formats][W215]=" + getProduct[25] + "&avatar[formats][150x120]=" + getProduct[26] + "&avatar[formats][104x82]=" + getProduct[27] + "&avatar[formatParams][raw][fileSize]=" + getProduct[28] + "&avatar[alt]=" + getProduct[29] + "&avatar[isVisibleOnMain]=" + getProduct[30] + "&avatar[priority]=" + getProduct[31] + "&avatar[url]=" + getProduct[32] + "&avatar[filters][crop][left]=" + getProduct[33] + "&avatar[filters][crop][top]=" + getProduct[34] + "&avatar[filters][crop][right]=" + getProduct[35] + "&avatar[filters][crop][bottom]=" + getProduct[36] + "&customDays=" + getProduct[37] + "&isCustom=" + getProduct[38]);
            req.ContentLength = ms.Length;
            Stream stre = req.GetRequestStream();
            stre.Write(ms, 0, ms.Length);
            stre.Close();
            HttpWebResponse res1 = (HttpWebResponse)req.GetResponse();
            StreamReader ressr1 = new StreamReader(res1.GetResponseStream());
        }

        public void WriteFileInCSV(List<string> newProduct, string nameFile)
        {
            StreamWriter newProductcsv = new StreamWriter(nameFile + ".csv", true, Encoding.GetEncoding("windows-1251"));
            int count = newProduct.Count - 1;
            for (int i = 0; count > i; i++)
            {
                newProductcsv.Write(newProduct[i], Encoding.GetEncoding("windows-1251"));
                newProductcsv.Write(";");
            }
            newProductcsv.Write(newProduct[count], Encoding.GetEncoding("windows-1251"));
            newProductcsv.WriteLine();
            newProductcsv.Close();
        }

        public int ReturnPrice(double priceTovar, double discount)
        {
            priceTovar = priceTovar - (priceTovar * discount);
            priceTovar = Math.Round(priceTovar);
            int price = Convert.ToInt32(priceTovar);
            price = (price / 10) * 10;
            return price;
        }

        public string SaveTovar(CookieContainer cookie, List<string> getProduct)
        {
            string otv = "";
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("http://bike18.nethouse.ru/api/catalog/saveproduct");
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:44.0) Gecko/20100101 Firefox/44.0";
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.CookieContainer = cookie;
            string descFull = getProduct[8].ToString();
            descFull = descFull.Replace("&laquo;", "«").Replace("&raquo;", "»").Replace("&ndash;", "-");
            getProduct[8] = descFull;
            string request = "id=" + getProduct[0] + "&slug=" + getProduct[1] + "&categoryId=" + getProduct[2] + "&productCustomGroup=" + getProduct[41] + "&productGroup=" + getProduct[3] + "&name=" + getProduct[4] + "&serial=" + getProduct[5] + "&serialByUser=" + getProduct[6] + "&desc=" + getProduct[7] + "&descFull=" + getProduct[8] + "&cost=" + getProduct[9] + "&discountCost=" + getProduct[10] + "&seoMetaDesc=" + getProduct[11] + "&seoMetaKeywords=" + getProduct[12] + "&seoTitle=" + getProduct[13] + "&haveDetail=" + getProduct[14] + "&canMakeOrder=" + getProduct[15] + "&balance=" + getProduct[43] + "&showOnMain=" + getProduct[16] + "&isVisible=1&hasSale=0" + "&customDays=" + getProduct[37] + "&isCustom=" + getProduct[38] + getProduct[39] + getProduct[40] + getProduct[42] + "&alsoBuyLabel=%D0%9F%D0%BE%D1%85%D0%BE%D0%B6%D0%B8%D0%B5%20%D1%82%D0%BE%D0%B2%D0%B0%D1%80%D1%8B%20%D0%B2%20%D0%BD%D0%B0%D1%88%D0%B5%D0%BC%20%D0%BC%D0%B0%D0%B3%D0%B0%D0%B7%D0%B8%D0%BD%D0%B5";
            request = request.Replace("false", "0").Replace("true", "1").Replace("&mdash;", "-").Replace("&laquo;", "\"").Replace("&raquo;", "\"").Replace("&mdash;", "-");

            request = request.Replace("false", "0").Replace("true", "1");
            byte[] ms = System.Text.Encoding.GetEncoding("utf-8").GetBytes(request);
            req.ContentLength = ms.Length;
            Stream stre = req.GetRequestStream();
            stre.Write(ms, 0, ms.Length);
            stre.Close();
            try
            {
                HttpWebResponse res1 = (HttpWebResponse)req.GetResponse();
                StreamReader ressr1 = new StreamReader(res1.GetResponseStream());
                otv = ressr1.ReadToEnd();
            }
            catch
            {

            }
            return otv;
        }

        public int ReturnCountAdd()
        {
            if (addCount == 99)
                addCount = 0;
            addCount++;
            return addCount;
        }

        public void NewListUploadinBike18(string nameFile)
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
            WriteFileInCSV(newProduct, nameFile);
        }
        
        public string alsoBuyTovars(List<string> tovarList)
        {
            string name = tovarList[4].ToString();
            string otv = webRequest.getRequest("http://bike18.ru/products/search/page/1?sort=0&balance=&categoryId=&min_cost=&max_cost=&text=" + name);
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

        #region Загрузка картинки товара на сайта Bike18.ru
        public void UploadImage(CookieContainer cookieBike18, string urlProduct)
        {
            string otv = null;
            urlProduct = urlProduct.Replace("http://bike18.ru", "http://bike18.nethouse.ru");

            otv = webRequest.PostRequest(cookieBike18, urlProduct);
            string artProd = new Regex("(?<=Артикул:)[\\w\\W]*?(?=</title><)").Match(otv).ToString().Trim();

            MatchCollection prId = new Regex("(?<=data-id=\").*?(?=\")").Matches(otv);
            string productId = prId[0].ToString();

            Image newImg = Image.FromFile("Pic\\" + artProd + ".jpg");
            double widthImg = newImg.Width;
            double heigthImg = newImg.Height;

            if (widthImg > heigthImg)
            {
                double dblx = widthImg * 0.9;
                if (dblx < heigthImg)
                    heigthImg = heigthImg * 0.9;
                else
                    widthImg = widthImg * 0.9;
            }
            else
            {
                double dblx = heigthImg * 0.9;
                if (dblx < widthImg)
                    widthImg = widthImg * 0.9;
                else
                    heigthImg = heigthImg * 0.9;
            }

            string epoch = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds.ToString().Replace(",", "");
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("https://bike18.nethouse.ru/putimg?fileapi" + epoch);
            req.Accept = "*/*";
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.99 Safari/537.36";
            req.Method = "POST";
            req.ContentType = "multipart/form-data; boundary=----WebKitFormBoundaryDxXeyjY3R0nRHgrP";
            req.CookieContainer = cookieBike18;
            req.Headers.Add("X-Requested-With", "XMLHttpRequest");
            byte[] pic = File.ReadAllBytes("Pic\\" + artProd + ".jpg");
            byte[] end = Encoding.ASCII.GetBytes("\r\n------WebKitFormBoundaryDxXeyjY3R0nRHgrP\r\nContent-Disposition: form-data; name=\"_file\"\r\n\r\n" + artProd + ".jpg\r\n------WebKitFormBoundaryDxXeyjY3R0nRHgrP--\r\n");
            byte[] ms1 = Encoding.ASCII.GetBytes("------WebKitFormBoundaryDxXeyjY3R0nRHgrP\r\nContent-Disposition: form-data; name=\"file\"; filename=\"4680329013422.jpg\"\r\nContent-Type: image/jpeg\r\n\r\n");
            req.ContentLength = ms1.Length + pic.Length + end.Length;
            Stream stre1 = req.GetRequestStream();
            stre1.Write(ms1, 0, ms1.Length);
            stre1.Write(pic, 0, pic.Length);
            stre1.Write(end, 0, end.Length);
            stre1.Close();
            HttpWebResponse resimg = (HttpWebResponse)req.GetResponse();
            StreamReader ressrImg = new StreamReader(resimg.GetResponseStream());
            string otvimg = ressrImg.ReadToEnd();
            resimg.Close();
            string urlSaveImg = new Regex("(?<=url\":\").*?(?=\")").Match(otvimg).Value.Replace("\\/", "%2F");

            req = (HttpWebRequest)HttpWebRequest.Create("https://bike18.nethouse.ru/api/catalog/save-image");
            req.Accept = "application/json, text/plain, */*";
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.99 Safari/537.36";
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.CookieContainer = cookieBike18;
            byte[] saveImg = Encoding.ASCII.GetBytes("url=" + urlSaveImg + "&id=0&type=4&objectId=" + productId + "&imgCrop[x]=0&imgCrop[y]=0&imgCrop[width]=" + widthImg + "&imgCrop[height]=" + heigthImg + "&imageId=0&iObjectId=" + productId + "&iImageType=4&replacePhoto=0");
            req.ContentLength = saveImg.Length;
            Stream srSave = req.GetRequestStream();
            srSave.Write(saveImg, 0, saveImg.Length);
            srSave.Close();
            HttpWebResponse resSave = (HttpWebResponse)req.GetResponse();
            StreamReader ressrSave = new StreamReader(resSave.GetResponseStream());
            ressrImg.Close();

            string otvSave = ressrSave.ReadToEnd();
        }
        #endregion

        #region UploadCSVInNethouse

        public void UploadCSVNethouse(CookieContainer cookie, string nameFile)
        {
            string trueOtv = null;
            do
            {
                string otvimg = DownloadNaSite(cookie, nameFile);
                string check = "{\"success\":true,\"imports\":{\"state\":1,\"errorCode\":0,\"errorLine\":0}}";
                do
                {
                    System.Threading.Thread.Sleep(2000);
                    otvimg = ChekedLoading(cookie);
                }
                while (otvimg == check);

                trueOtv = new Regex("(?<=\":{\"state\":).*?(?=,\")").Match(otvimg).ToString();
                string error = new Regex("(?<=errorCode\":).*?(?=,\")").Match(otvimg).ToString();

                if (error == "13")
                    ErrUpload13(otvimg, nameFile);

                if (error == "37")
                    ErrUpload37(otvimg, nameFile);

                if (error == "27")
                    ErrUpload27(otvimg, nameFile);
            }
            while (trueOtv != "2");
        }

        private void ErrUpload10(string otv, string nameFile)
        {
            string errstr = new Regex("(?<=errorLine\":).*?(?=,\")").Match(otv).ToString();
            string[] naSite = File.ReadAllLines(nameFile, Encoding.GetEncoding(1251));
            string[] newList = new string[naSite.Length - 1];
            int i = 0;
            string delString = naSite[Convert.ToInt32(errstr) - 1].ToString();
            foreach (string str in naSite)
            {
                if (str != delString)
                    newList[i] = str;
                i++;
            }
            File.WriteAllLines(nameFile, newList, Encoding.GetEncoding(1251));
        }

        private void ErrUpload27(string otv, string nameFile)
        {
            string errstr = new Regex("(?<=errorLine\":).*?(?=,\")").Match(otv).ToString();
            string[] naSite = File.ReadAllLines(nameFile, Encoding.GetEncoding(1251));
            int u = Convert.ToInt32(errstr) - 1;
            string[] s = naSite[u].ToString().Split(';');
        }

        private void ErrUpload13(string otv, string nameFile)
        {
            string errstr = new Regex("(?<=errorLine\":).*?(?=,\")").Match(otv).ToString();
            string[] naSite = File.ReadAllLines(nameFile, Encoding.GetEncoding(1251));
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
            strslug2 = strslug2.Replace("”", "").Replace("~", "").Replace("#", "").Replace("?", "");
            if (strslug2.Contains("\""))
            {
                strslug2 = strslug2 + "\"";
                countDel = countDel - 2;
            }
            naSite[u] = naSite[u].Replace(strslug, strslug2);
            File.WriteAllLines(nameFile, naSite, Encoding.GetEncoding(1251));
        }

        private void ErrUpload37(string otv, string nameFile)
        {
            string errstr = new Regex("(?<=errorLine\":).*?(?=,\")").Match(otv).ToString();
            string[] naSite = File.ReadAllLines(nameFile, Encoding.GetEncoding(1251));
            int u = Convert.ToInt32(errstr) - 1;
            string[] strslug3 = naSite[u].Split(';');
            int slugint = strslug3.Length - 5;
            string strslug = strslug3[slugint].ToString();
            int slug = strslug.Length;
            int countAdd = ReturnCountAdd();
            int countDel = countAdd.ToString().Length;
            string strslug2 = strslug.Remove(slug - countDel);
            strslug2 += countAdd;
            strslug2 = strslug2.Replace(" -", "-").Replace("?", "");
            naSite[u] = naSite[u].Replace(strslug, strslug2);
            File.WriteAllLines(nameFile, naSite, Encoding.GetEncoding(1251));
        }

        private string DownloadNaSite(CookieContainer cookie, string nameFile)
        {
            string epoch = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds.ToString().Replace(",", "");
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("https://bike18.nethouse.ru/api/export-import/import-from-csv?fileapi" + epoch);
            req.Accept = "*/*";
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.71 Safari/537.36";
            req.Method = "POST";
            req.ContentType = "multipart/form-data; boundary=----WebKitFormBoundaryVB016NRhkhMFanjn";
            req.CookieContainer = cookie;
            req.Headers.Add("X-Requested-With", "XMLHttpRequest");
            byte[] csv = File.ReadAllBytes(nameFile);
            byte[] end = Encoding.ASCII.GetBytes("\r\n------WebKitFormBoundaryVB016NRhkhMFanjn\r\nContent-Disposition: form-data; name=\"_catalog_file\"\r\n\r\n" + nameFile + "\r\n------WebKitFormBoundaryVB016NRhkhMFanjn--\r\n");
            byte[] ms1 = Encoding.ASCII.GetBytes("------WebKitFormBoundaryVB016NRhkhMFanjn\r\nContent-Disposition: form-data; name=\"catalog_file\"; filename=\"" + nameFile + "\"\r\nContent-Type: application/vnd.ms-excel\r\n\r\n");
            req.ContentLength = ms1.Length + csv.Length + end.Length;
            Stream stre1 = req.GetRequestStream();
            stre1.Write(ms1, 0, ms1.Length);
            stre1.Write(csv, 0, csv.Length);
            stre1.Write(end, 0, end.Length);
            stre1.Close();
            HttpWebResponse resimg = (HttpWebResponse)req.GetResponse();
            StreamReader ressrImg = new StreamReader(resimg.GetResponseStream());
            string otvimg = ressrImg.ReadToEnd();
            resimg.Close();
            return otvimg;
        }

        public string ChekedLoading(CookieContainer cookie)
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("https://bike18.nethouse.ru/api/export-import/check-import");
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
            resimg.Close();
            return otvimg;
        }

        #endregion
    }
}
