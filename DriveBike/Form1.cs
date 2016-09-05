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

namespace DriveBike
{
    public partial class Form1 : Form
    {
        WebRequest webRequest = new WebRequest();

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
            //System.Net.CookieContainer cookie = webRequest.webCookie("http://www.drivebike.ru/");

            otv = webRequest.getRequest("http://www.drivebike.ru/rashodniki-dlya-motocikla-i-kvadrocikla?limit=60");
            MatchCollection categoriesUrls = new Regex("(?<=<li class=\"amshopby-cat amshopby-cat-level-1\">)[\\w\\W]*?(?=</li>)").Matches(otv);
            for(int i = 0; categoriesUrls.Count > i; i++)
            {
                string categories = new Regex("(?<=<a href=\").*?(?=\">)").Match(categoriesUrls[i].ToString()).ToString();
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

                            string nameTovar = new Regex("(?<=<br/>).*(?=</h1>)").Match(otv).ToString();
                            string articl = new Regex("(?<=<div class=\"std\">Код товара:).*?(?=<br /> )").Match(otv).ToString();
                            string price = new Regex("(?<=<meta itemprop=\"price\" content=\").*?(?=\" />)").Match(otv).ToString();
                            string miniText = new Regex("(?<=<tbody>)[\\w\\W]*?(?=</tbody>)").Match(otv).ToString().Replace("\n", "").Replace("    ", " ").Replace("   ", " ").Replace("  ", " ").Replace("  ", " ");
                            string fullText = new Regex("(?<=<meta itemprop=\"description\" content=\").*?(?=\" />)").Match(otv).ToString();
                        }
                        else
                        {
                            //Если товара нет в наличии
                        }
                    }
                }
                else
                {
                    //Если разное кол-во ссылок на товар и наличия товара
                }
            }
        }
    }
}
