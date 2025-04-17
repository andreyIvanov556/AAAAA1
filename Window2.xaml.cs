using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace AAAAA1
{
    /// <summary>
    /// Логика взаимодействия для Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        public Window2()
        {
            InitializeComponent();
            УП_01_ИвановEntities BD = new УП_01_ИвановEntities();
            Analaz.ItemsSource = BD.HistoryResearch_.ToList().Where(a=> a.Zakaz_.statusOrder==2);// Заполнение DAtaGrid только не выполнеными услугамим 
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
        }
        public static TimeSpan seconds = TimeSpan.FromSeconds(0);
        public static bool obl = false;
        public static TimeSpan time = TimeSpan.FromSeconds(9000);
        private void timer_Tick(object sender, EventArgs e)
        {
            if (obl = false && seconds >= TimeSpan.FromSeconds(8100)) { obl = true; MessageBox.Show("Осталось 15 минут"); }
            if (seconds < TimeSpan.FromSeconds(9000)) seconds = seconds.Add(TimeSpan.FromSeconds(1));
            else Application.Current.Shutdown();
            timer.Text = time.Subtract(seconds).ToString();
        }

        private void otpravit_Click(object sender, RoutedEventArgs e)
        {
            

        }
        public Zakaz_ zakazik = new Zakaz_();
        private void Analaz_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Services services1 = new Services();
            services1.serviceCode = 619;
            Services services2 = new Services();
            services2.serviceCode = 543;
            List<Services> services = new List<Services>();
            services.Add(services1);
            services.Add(services2);
            string patient = zakazik.UserId.ToString();
            //    name - название выбранного анализатора
            var httpWebRequest = (HttpWebRequest)WebRequest.Create($"http://localhost:5000/api/analyzer/(name1"); // строка поклонения к API
            httpWebRequest.ContentType = "application/json"; // форма работы с данными.
            httpWebRequest.Method = "POST"; // Метод для отправки данных.

            // формируем из данных формата JSON. ВСЕ НАИМЕНОВАНИЯ ПЕРЕМЕННЫХ ДОЛЖНЫ БЫТЬ КАК В СТРУКТУРЕ!!
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream())) ;
            {
                string json = new JavaScriptSerializer().Serialize(new
                {
                    patient, // Пациент.
                    services // СПИСОК услуг (список элементов класса Services)
                });
                streamWriter.Write(json);
            }

            HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse(); // Отправка запроса на API

            // Спрашиваем хорошо ли отправлен запрос.
            if (httpResponse.StatusCode == HttpStatusCode.OR) //
                MessageBox.Show("Услуги успешно отправлены!");
            else
                MessageBox.Show("Ошибка отправки!");
        }
    }
}

