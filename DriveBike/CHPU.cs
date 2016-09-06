using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Формирование_ЧПУ
{
    class CHPU
    {
       

       Dictionary<string, string> words = new Dictionary<string, string>();

        public void Form1()
        {
            words.Add(" ", "-");
            words.Add("а", "a");
            words.Add("б", "b");
            words.Add("в", "v");
            words.Add("г", "g");
            words.Add("д", "d");
            words.Add("е", "e");
            words.Add("ё", "yo");
            words.Add("ж", "zh");
            words.Add("з", "z");
            words.Add("и", "i");
            words.Add("й", "j");
            words.Add("к", "k");
            words.Add("л", "l");
            words.Add("м", "m");
            words.Add("н", "n");
            words.Add("о", "o");
            words.Add("п", "p");
            words.Add("р", "r");
            words.Add("с", "s");
            words.Add("т", "t");
            words.Add("у", "u");
            words.Add("ф", "f");
            words.Add("х", "h");
            words.Add("ц", "c");
            words.Add("ч", "ch");
            words.Add("ш", "sh");
            words.Add("щ", "sch");
            words.Add("ъ", "j");
            words.Add("ы", "i");
            words.Add("ь", "j");
            words.Add("э", "e");
            words.Add("ю", "yu");
            words.Add("я", "ya");
            words.Add("А", "a");
            words.Add("Б", "b");
            words.Add("В", "v");
            words.Add("Г", "g");
            words.Add("Д", "d");
            words.Add("Е", "e");
            words.Add("Ё", "yo");
            words.Add("Ж", "zh");
            words.Add("З", "z");
            words.Add("И", "i");
            words.Add("Й", "j");
            words.Add("К", "k");
            words.Add("Л", "l");
            words.Add("М", "m");
            words.Add("Н", "n");
            words.Add("О", "o");
            words.Add("П", "p");
            words.Add("Р", "r");
            words.Add("С", "s");
            words.Add("Т", "t");
            words.Add("У", "u");
            words.Add("Ф", "f");
            words.Add("Х", "h");
            words.Add("Ц", "c");
            words.Add("Ч", "ch");
            words.Add("Ш", "sh");
            words.Add("Щ", "sch");
            words.Add("Ъ", "j");
            words.Add("Ы", "i");
            words.Add("Ь", "j");
            words.Add("Э", "e");
            words.Add("Ю", "yu");
            words.Add("Я", "ya");
        }

        public string vozvr(string vhod)
        {
            Form1();
            string source = vhod;
            foreach (KeyValuePair<string, string> pair in words)
            {
                source = source.Replace(pair.Key, pair.Value);
            }
             
            string vyhod = source;
            if (vyhod.Length > 64)
            {
                vyhod = vyhod.Substring(0, vyhod.Length - (vyhod.Length - 64));
            }
            MatchCollection sovpad = new Regex("\\S").Matches(vyhod);

            int res;
            bool isInt = Int32.TryParse(sovpad[0].Value, out res);
            if (isInt == true)
            {
                vyhod = "a" + vyhod;
            }

            vyhod = vyhod.ToLower();
            vyhod = new Regex("[-.?!)(,:]").Replace(vyhod, "-").Replace("--", "-").Replace("---", "-").Replace("-----", "-").Replace("&", "").Replace("\"", "").Replace("=", "").Replace("*", "").Replace("+", "").Replace("(", "").Replace(")", "").Replace("<", "").Replace(">", "").Replace("^", "").Replace("\\", "").Replace("/", "").Replace("'", "").Replace("`", "").Replace("$", "").Replace("°", "").Replace(";", "").Replace("«", "").Replace("»", "").Replace("№", "").Replace("_", "");

            words.Clear();

            return vyhod;
        }
}
}
