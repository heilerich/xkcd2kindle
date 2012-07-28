using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace xkcd2kindle
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        [Serializable]
        private class comic
        {
            public string year;
            public string month;
            public string day;
            public string title;
            public string safe_title;
            public string img;
            public string alt;
            public string link;
            public string news;
            public string transcript;
            public int num;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private comic getComic(int num)
        {
            string url = "http://xkcd.com/" + num.ToString() + "/info.0.json";

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            WebResponse wb = req.GetResponse();
            Stream str = wb.GetResponseStream();
            StreamReader rdr = new StreamReader(str);
            string resstr = rdr.ReadToEnd();
            rdr.Close();

            JavaScriptSerializer sr = new JavaScriptSerializer();
            comic cm = sr.Deserialize<comic>(resstr);
            return cm;
        }

        private comic getComic()
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://xkcd.com/info.0.json");
            WebResponse wb = req.GetResponse();
            Stream str = wb.GetResponseStream();
            StreamReader rdr = new StreamReader(str);
            string resstr = rdr.ReadToEnd();
            rdr.Close();

            JavaScriptSerializer sr = new JavaScriptSerializer();
            comic cm = sr.Deserialize<comic>(resstr);
            return cm;
        }

        private void addtext(ref Bitmap inp, string text)
        {
            Font fnt = new Font("Comic Sans MS",10);
            formatText(ref text, ref fnt);
            TextRenderer.DrawText(Graphics.FromImage(inp), text,fnt, new Rectangle(0, 700, 600, 100), Color.Black);
        }

        private void formatText(ref string text, ref Font fnt)
        {
            for (int i = 0; i <= 10; i++)
            {
                Size sz = TextRenderer.MeasureText(text, fnt);
                if (sz.Width > 600)
                {
                    double br = (600.0 / sz.Width);
                    br = br * text.Length;
                    double brx = Math.Round(br);
                    int bri = Convert.ToInt32(brx);
                    string str = "";
                    int x = 0;
                    while (str != " ")
                    {
                        if (x < bri)
                        {
                            str = text.ToCharArray()[bri - ++x].ToString();
                        }
                        else
                        {
                            break;
                        }
                    }
                    bri = bri - x;
                    text = text.Substring(0, bri) + "\n" + text.Substring(bri);
                }
                else
                {
                    break;
                }
            }
        }

        comic com;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (com == null)
            {
                com = getComic();
            }
            else
            {
                com = getComic(com.num-1);
            }
            pictureBox1.Load(com.img);
            Bitmap bmp = new Bitmap(600,800);
            Graphics.FromImage(bmp).Clear(Color.White);
            pictureBox1.DrawToBitmap(bmp, new Rectangle(0, 0, 600, 700));
            addtext(ref bmp, com.safe_title + ": " + com.alt);
            pictureBox2.Image = bmp;
            bmp.Save(com.num + ".png", System.Drawing.Imaging.ImageFormat.Png);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Stop();
        }
    }
}
