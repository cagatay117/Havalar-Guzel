using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Xml.Serialization;
using WrapperClasses;

namespace Havalar_Güzel // TODO: 1 eğer gün batımı saati geçildiyse, hava durumu resimleri _night olanlardan seçilecek, gündüz saatlerinde _day olanlardan seçilecek.
{
    public partial class MainForm : Form
    {
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        public query xmlData;
        public string now = DateTime.Now.ToString("h:mm tt", CultureInfo.InvariantCulture);

        public DateTime dtNowParsed;
        public DateTime sunsetParsed, sunriseParsed;
        public MainForm()
        {
            InitializeComponent();
            //DateTime sdf = DateTime. // Parse edilmiş string datetime olduktan sonra, messagebox içinde göstermek için tostring yaptığımızda, formatı özellikle
            // değiştiriyor olabilir. Tostring yapıp mesaj içinde göstermek yerine, yapmak istediğimiz compare işlemin gerçekleştirip çalışıp çalışmadığına bakalım.
            DateTime.TryParseExact(now, "h:mm tt", CultureInfo.InvariantCulture, 0, out dtNowParsed);
            //MessageBox.Show("now: " + now + "\r\ndtNow: " + dtNowParsed.ToString("hh:mm tt", CultureInfo.InvariantCulture));
            XmlSerializer xmlDeserialize = new XmlSerializer(typeof(query));
            try
            {
                var request = (HttpWebRequest)WebRequest.Create("https://query.yahooapis.com/v1/public/yql");
                var parameter1 = "q=select * from weather.forecast where woeid in (select woeid from geo.places(1) where text=%22Pendik, istanbul%22) and u=%22c%22&env=store://datatables.org/alltableswithkeys";
                var data = Encoding.UTF8.GetBytes(parameter1);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
                var response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string responseString = reader.ToString();
                object obj = xmlDeserialize.Deserialize(reader);
                xmlData = (query)obj;
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void MainForm_Shown(object sender, EventArgs e)        
            // TODO: 2 xmlData içindeki sunset ve sunrise attributelerini datetime.now ile compare ederek hava durumu simgelerini seçtireceğiz.
            // Bunu yaparken sunrise < now < sunset durumu var ise bir değişkene herhangi bir değer atayacağız, now aralıkta değilse null atayacağız.
            // x = y : z olayını kullanıp null check yapacağız (null değil ise y, null ise z yi ata şeklinde.)
        {
            sunriseParsed = DateTime.ParseExact(xmlData.results.channel.astronomy.sunrise, "h:mm tt", CultureInfo.InvariantCulture);
            sunsetParsed = DateTime.ParseExact(xmlData.results.channel.astronomy.sunset, "h:m tt", CultureInfo.InvariantCulture);
            int compareResult = DateTime.Compare(dtNowParsed, sunsetParsed);
            switch (xmlData.results.channel.item.condition.text)
            {
                case "Clear":
                    pictureBox1.Image = (compareResult < 0) ? Havalar_Güzel.Properties.Resources.clear_day : Havalar_Güzel.Properties.Resources.clear_night;
                    label3.Text = "\"Güneşli\"";
                    break;
                case "Sunny":
                    pictureBox1.Image = (compareResult < 0) ? Havalar_Güzel.Properties.Resources.clear_day : Havalar_Güzel.Properties.Resources.clear_night;
                    label3.Text = "\"Güneşli\"";
                    break;
                case "Mostly Sunny":
                    pictureBox1.Image = (compareResult < 0) ? Havalar_Güzel.Properties.Resources.clear_day : Havalar_Güzel.Properties.Resources.clear_night;
                    label3.Text = "\"Güneşli\"";
                    break;
                case "Cloudy":
                    pictureBox1.Image = (compareResult < 0) ? Havalar_Güzel.Properties.Resources.cloudy_day : Havalar_Güzel.Properties.Resources.cloudy_night;
                    label3.Text = "\"Bulutlu\"";
                    break;
                case "Partly Cloudy":
                    pictureBox1.Image = (compareResult < 0) ? Havalar_Güzel.Properties.Resources.partly_cloudy_day : Havalar_Güzel.Properties.Resources.partly_cloudy_night;
                    label3.Text = "\"Parçalı Bulutlu\"";
                    break;
                case "Mostly Cloudy":
                    pictureBox1.Image = (compareResult < 0) ? Havalar_Güzel.Properties.Resources.mostly_cloudy_day : Havalar_Güzel.Properties.Resources.mostly_cloudy_night;
                    label3.Text = "\"Çok Bulutlu\"";
                    break;
                case "Fair":
                    pictureBox1.Image = (compareResult < 0) ? Havalar_Güzel.Properties.Resources.fair_day : Havalar_Güzel.Properties.Resources.fair_night;
                    label3.Text = "\"Az Bulutlu\"";
                    break;
                case "Rain":
                    pictureBox1.Image = (compareResult < 0) ? Havalar_Güzel.Properties.Resources.rain_day : Havalar_Güzel.Properties.Resources.rain_night;
                    label3.Text = "\"Yağmurlu\"";
                    break;
                case "Showers":
                    pictureBox1.Image = (compareResult < 0) ? Havalar_Güzel.Properties.Resources.rain_day : Havalar_Güzel.Properties.Resources.rain_night;
                    label3.Text = "\"Sağanak Yağışlı\"";
                    break;
                case "Scattered Showers":
                    pictureBox1.Image = (compareResult < 0) ? Havalar_Güzel.Properties.Resources.scattered_showers_day : Havalar_Güzel.Properties.Resources.scattered_showers_night;
                    label3.Text = "\"Yer Yer Sağanak Yağışlı\"";
                    break;
                case "Thundershowers":
                    pictureBox1.Image = (compareResult < 0) ? Havalar_Güzel.Properties.Resources.thundershowers_day : Havalar_Güzel.Properties.Resources.thundershowers_night;
                    label3.Text = "\"Gök Gürültülü Sağanak Yağışlı\"";
                    break;
                case "Thunderstorms":
                    pictureBox1.Image = (compareResult < 0) ? Havalar_Güzel.Properties.Resources.thundershowers_day : Havalar_Güzel.Properties.Resources.thundershowers_night;
                    label3.Text = "\"Gök Gürültülü Fırtına\"";
                    break;
                case "Windy":
                    pictureBox1.Image = (compareResult < 0) ? Havalar_Güzel.Properties.Resources.windy_day : Havalar_Güzel.Properties.Resources.windy_night;
                    label3.Text = "\"Rüzgarlı\"";
                    break;
                case "Blustery":
                    pictureBox1.Image = (compareResult < 0) ? Havalar_Güzel.Properties.Resources.windy_day : Havalar_Güzel.Properties.Resources.windy_night;
                    label3.Text = "\"Kuvvetli Rüzgar\"";
                    break;
                case "Isolated Thundershowers":
                    pictureBox1.Image = (compareResult < 0) ? Havalar_Güzel.Properties.Resources.scattered_showers_day : Havalar_Güzel.Properties.Resources.scattered_showers_night;
                    label3.Text = "\"Gök Gürültülü Sağanak Yağışlı\"";
                    break;
                case "Scattered Thunderstorms":
                    pictureBox1.Image = (compareResult < 0) ? Havalar_Güzel.Properties.Resources.scattered_showers_day : Havalar_Güzel.Properties.Resources.scattered_showers_night;
                    label3.Text = "\"Yer Yer Gök Gürültülü Fırtına\"";
                    break;
                case "Mixed Rain and Snow":
                    pictureBox1.Image = (compareResult < 0) ? Havalar_Güzel.Properties.Resources.snow_rain_mix_day : Havalar_Güzel.Properties.Resources.snow_rain_mix_night;
                    label3.Text = "\"Karla Karışık Yağmurlu\"";
                    break;
                case "Snow":
                    pictureBox1.Image = (compareResult < 0) ? Havalar_Güzel.Properties.Resources.snow_day : Havalar_Güzel.Properties.Resources.snow_night;
                    label3.Text = "\"Kar Yağışlı\"";
                    break;
                default:
                    pictureBox1.Image = (compareResult < 0) ? Havalar_Güzel.Properties.Resources.clear_day : Havalar_Güzel.Properties.Resources.clear_night;
                    label3.Text = "Hava Durumu Bilgisi Alınamadı.";
                    break;
            }
            pictureBox1.Visible = true;
            label4.Text = xmlData.results.channel.location.city.ToString() + ", " + xmlData.results.channel.location.region.ToString();
            label4.Visible = true;
            label1.Text = xmlData.results.channel.item.condition.temp.ToString() + "°";
            label5.Text = xmlData.results.channel.item.forecast[1].high.ToString();
            label6.Text = xmlData.results.channel.item.forecast[1].low.ToString();
        }	// TODO: 3 geolocation ile lkasyon tespiti yapabilir miyiz?
    }
}
