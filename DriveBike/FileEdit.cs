using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace RacerMotors
{
    internal class FileEdit
    {
        internal void fileWriter(string v1, string v2)
        {
            
            StreamWriter writers = new StreamWriter(v1 + ".txt", true, Encoding.GetEncoding("windows-1251"));
            writers.WriteLine(v2 + "\n");
            writers.Close();
        }

        internal void fileWriterCSV(List<string> newProduct, string nameFile)
        {
            StreamWriter newProductcsv = new StreamWriter(nameFile + ".csv", true, Encoding.GetEncoding("windows-1251"));
            int count = newProduct.Count - 1;
            for (int i = 0; count >i; i++)
            {
                newProductcsv.Write(newProduct[i], Encoding.GetEncoding("windows-1251"));
                newProductcsv.Write(";");
            }
            newProductcsv.Write(newProduct[count], Encoding.GetEncoding("windows-1251"));
            newProductcsv.WriteLine();
            newProductcsv.Close();
        }

        internal void writerCSV(string v, string section1, string section2)
        {
            StreamWriter newProductcsv = new StreamWriter("fullProducts.csv", true, Encoding.GetEncoding("windows-1251"));
            newProductcsv.Write(v, Encoding.GetEncoding("windows-1251"));
            newProductcsv.Write(";");
            newProductcsv.Write(section1, Encoding.GetEncoding("windows-1251"));
            newProductcsv.Write(";");
            newProductcsv.Write(section2, Encoding.GetEncoding("windows-1251"));
            newProductcsv.Write(";");
            newProductcsv.WriteLine();
            newProductcsv.Close();
        }

        internal void writerCSV(string v1, string v2, string section1, string section2)
        {
            StreamWriter newProductcsv = new StreamWriter("fullProducts.csv", true, Encoding.GetEncoding("windows-1251"));
            newProductcsv.Write(v1, Encoding.GetEncoding("windows-1251"));
            newProductcsv.Write(";");
            newProductcsv.Write(v2, Encoding.GetEncoding("windows-1251"));
            newProductcsv.Write(";");
            newProductcsv.Write(section1, Encoding.GetEncoding("windows-1251"));
            newProductcsv.Write(";");
            newProductcsv.Write(section2, Encoding.GetEncoding("windows-1251"));
            newProductcsv.Write(";");
            newProductcsv.WriteLine();
            newProductcsv.Close();
        }

        internal void doubleTovar(string v, string dblProduct, int i, int section)
        {
            
        }
    }
}