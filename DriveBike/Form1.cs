using RacerMotors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
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
        WebRequest webRequest = new WebRequest();
        CHPU chpu = new CHPU();
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
            for(int i = 0; categoriesUrls.Count > i; i++)
            {
                string categories = new Regex("(?<=<a href=\").*?(?=\">)").Match(categoriesUrls[i].ToString()).ToString();
                string razdelDB = new Regex("(?<=\">).*?(?=</a>)").Match(categoriesUrls[i].ToString()).ToString();
                otv = webRequest.getRequest(categories);
                MatchCollection availability = new Regex("(?<=<p class=\"availability).*?(?=</span></p>)").Matches(otv);
                MatchCollection urlTovars = new Regex("(?<=<li class=\"item)[\\w\\W]*?(?=\" title=\")").Matches(otv);
                if(availability.Count == urlTovars.Count)
                {
                    for(int n = 0; urlTovars.Count > n; n++)
                    {
                        string availabilityTovar = availability[n].ToString();
                        if (availabilityTovar.Contains("Есть в наличии"))
                        {
                            //Если товар в наличии
                            string url = new Regex("(?<=a href=\").*").Match(urlTovars[n].ToString()).ToString();
                            otv = webRequest.getRequest(url);

                            string nameTovar = new Regex("(?<=<h1><font style=\"color:#459B06; \">).*(?=</h1>)").Match(otv).ToString();
                            nameTovar = nameTovar.Replace("</font><br/>", " ");
                            string articl = new Regex("(?<=<div class=\"std\">Код товара:).*?(?=<br /> )").Match(otv).ToString();
                            string price = new Regex("(?<=<meta itemprop=\"price\" content=\").*?(?=\" />)").Match(otv).ToString();
                            string miniText = new Regex("(?<=<tbody>)[\\w\\W]*?(?=</tbody>)").Match(otv).ToString().Replace("\n", "").Replace("    ", " ").Replace("   ", " ").Replace("  ", " ").Replace("  ", " ");
                            string fullText = new Regex("(?<=<meta itemprop=\"description\" content=\").*?(?=\" />)").Match(otv).ToString();

                            bool b = false;
                            otv = webRequest.getRequest("http://bike18.ru/products/search/page/1?sort=0&balance=&categoryId=&min_cost=&max_cost=&text=" + nameTovar);
                            MatchCollection searchTovars = new Regex("(?<=title=\").*?(?=\")").Matches(otv);
                            if(searchTovars.Count > 0)
                            {
                                for(int m = 0; searchTovars.Count > m; m++)
                                {
                                    string searchTovarName = searchTovars[m].ToString();
                                    if(searchTovarName == nameTovar)
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

                                string razdel = "Запчасти и расходники => Расходники для японских, европейских, американских мотоциклов => " + razdelDB;
                                string miniTextTemplate = MinitextStr();
                                string titleText = null;
                                string descriptionText = null;
                                string keywordsText = null;
                                string fullTextTemplate = FulltextStr();
                                int priceActual = webRequest.price(Convert.ToInt32(price), discounts);

                                string dblProduct = "НАЗВАНИЕ также подходит для: аналогичных моделей.";
                                titleText = tbTitle.Lines[0].ToString();
                                descriptionText = tbDescription.Lines[0].ToString();
                                keywordsText = tbKeywords.Lines[0].ToString();


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
    }
}
