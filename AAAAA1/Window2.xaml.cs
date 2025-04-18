using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Globalization;


namespace AAAAA1
{
    /// <summary>
    /// Логика взаимодействия для Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        string name;// название выбранного анализатора
        int IDZAKAZ;// Id текущего закакза
        public Window2()
        {
            InitializeComponent();
            УП_01_ИвановEntities BD = new УП_01_ИвановEntities();
            Analaz.ItemsSource = BD.HistoryResearch_.ToList().Where(a=> a.Zakaz_.statusOrder==2);// Заполнение DAtaGrid только не выполнеными услугамим 
            Analiz.ItemsSource = BD.Analizators_.ToList();//Заполнение комбобокса
            Analiz.DisplayMemberPath = "name";
            Analiz.SelectedValuePath = "name";
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
        }
        public static TimeSpan seconds = TimeSpan.FromSeconds(0);
        public static bool obl = false;
        public static TimeSpan time = TimeSpan.FromSeconds(9000);// для отслеживания сеанса
        private void timer_Tick(object sender, EventArgs e)
        {
            if (obl = false && seconds >= TimeSpan.FromSeconds(8100)) { obl = true; MessageBox.Show("Осталось 15 минут"); }
            if (seconds < TimeSpan.FromSeconds(9000)) seconds = seconds.Add(TimeSpan.FromSeconds(1));
            else Application.Current.Shutdown();
            timer.Text = time.Subtract(seconds).ToString();
        }
        // таймер иметации процесса иследования
        public DispatcherTimer researchTimer = new DispatcherTimer();
        public int times = 0;
        public bool GoNext = false;
        private void Research_Tick(object sender, EventArgs e)
        {
            // Процес иследования  Заполнение прогресс бара
            if (times < 10)
            {
                times++;
                Bar.Value += 10;
                textBlock.Text = (times * 100 / 10).ToString()+"%";
            }
            else
            {
                Services serices1 = new Services();// завершение процесса иследования
               // serices1.serviceCode = ThisResearch.services.Value;
                researchTimer.Stop();
                times = 0;
                Bar.Value = times;
                Bar.Visibility = Visibility.Collapsed;
                MessageBox.Show("Загрузка окончена");

            }
        }
        public HistoryResearch_ ThisResearch = new HistoryResearch_();

        public Zakaz_ zakazik = new Zakaz_();
        private void Analaz_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string vixod = Environment.CurrentDirectory.ToString() + "\\Analyzer\\Analyzer\\LIMSAnalyzers.exe";// запуск программы анализатора
            var s= Process.Start(vixod);
            УП_01_ИвановEntities BD = new УП_01_ИвановEntities();
            // Настройка и запуск прогресс бара
            Bar.Visibility = Visibility.Visible;
            researchTimer.Tick += new EventHandler(Research_Tick);
            researchTimer.Interval = new TimeSpan(0, 0, 1);
            researchTimer.Start();

            // получение выбранного анализатора
            name= (Analiz.SelectedValue).ToString();

            Services services1 = new Services();
            // получекние выбранного иследования
            HistoryResearch_ выбранный = (HistoryResearch_)Analaz.SelectedItem;
            aboba.Add(выбранный);
            List<HistoryResearch_> заказ_s = new List<HistoryResearch_>() { выбранный };

            IDZAKAZ = (int)заказ_s[0].zakazid;//получение айди закакза
            // получение данных о заказе
            var datazakaz1 = BD.Zakaz_.FirstOrDefault(z => z.id == IDZAKAZ);

            string patient = (datazakaz1.UserId).ToString();
            // получение кода услуги
            services1.serviceCode = (int)заказ_s[0].services;

            List<Services> services = new List<Services>();
            services.Add(services1);

            // name – название выбранного анализатора
            //    name - название выбранного анализатора
            // отправка данных на Api анализатора
            var httpWebRequest = (HttpWebRequest)WebRequest.Create($"http://localhost:5000/api/analyzer/{name}"); // строка подключения к API
            httpWebRequest.ContentType = "application/json"; // формат работы с данными
            httpWebRequest.Method = "POST"; // Метод для отправки данных.
            // формируем из данных формата JSON. ВСЕ НАИМЕНОВАНИЯ ПЕРЕМЕННЫХ ДОЛЖНЫ БЫТЬ КАК В СТРУКТУРЕ!!
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = new JavaScriptSerializer().Serialize(new
                {
                    patient,  // Пациент.
                    services  // СПИСОК услуг (список элементов класса Services)
                });

                streamWriter.Write(json);
            }

            HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse(); // Отправка запроса на API

            // Спрашиваем хорошо ли отправлен запрос.
            if (httpResponse.StatusCode == HttpStatusCode.OK)
                MessageBox.Show("Услуги успешно отправлены!");
            else
                MessageBox.Show("Ошибка отправки!");
        }

        private void Analaz_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        string alltext = null;// параметр для хранения тексат результата
        private void Poluchi_Click(object sender, RoutedEventArgs e)
        {
            УП_01_ИвановEntities BD = new УП_01_ИвановEntities();
            // ОБРАТЕЛЯЕМ ЭЛЕМЕНТ КЛАССА, ДЛЯ ПОЛУЧЕНИЯ ДАННЫХ.
            name = (Analiz.SelectedValue).ToString();
            GetAnalzer getAnalizators = new GetAnalzer();

            //    name - название выбранного анализатора.
            var httpWebRequest = (HttpWebRequest)WebRequest.Create($"http://localhost:5000/api/analyzer/{name}");// строка поключения к API.
            httpWebRequest.ContentType = "application/json";// Формат работы с данными.
            httpWebRequest.Method = "GET"; // Метод для отправки данных.

            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse(); // отправляем запрос на анализатор.
                if (httpResponse.StatusCode == HttpStatusCode.OK) // если запрос успешный (вернулся код 200) - расшифровываем результат.
                {
                    using (Stream stream = httpResponse.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream); // Спрашиваем полученные данные.
                        string json = reader.ReadToEnd(); // Записываем прочитанные данные в переменную.
                        JavaScriptSerializer serializer = new JavaScriptSerializer(); // переменная для перевода из JSON.
                        getAnalizators = serializer.Deserialize<GetAnalzer>(json); // Записываем результат.

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            // отобрапжение результатов текст блок
            ContentOfList.Text = (getAnalizators.patient + " " + getAnalizators.services[0].serviceCode + " " + getAnalizators.services[0].result + " " + getAnalizators.progress);
            ContentOfList123.Text = (getAnalizators.services[0].result.ToString());
            // проверка результатов на 
            foreach (Services serv in getAnalizators.services)
            {
                if (double.TryParse(serv.result, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
                {
                    var serviceInfo = BD.Services_.First(x => x.Code == serv.serviceCode);
                    if (result > BD.Services_.First(x => x.Code == serv.serviceCode).UpperLimitOfNormal * 5 || result < BD.Services_.First(x => x.Code == serv.serviceCode).LowerLimitOfNormal / 5)// проверка результатов на аномальные отклонения
                    {
                        MessageBoxResult resultM = MessageBox.Show(BD.Services_.First(x => x.Code == serv.serviceCode).Service + " отклоняется от нормы в 5 раз" + serv.result, "Отклонение от нормы", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                        if (resultM == MessageBoxResult.OK)
                        {
                            Work_analizators_ workanaliz = new Work_analizators_()// запись в бд
                            {
                                id_analisator = Analiz.SelectedIndex + 1,
                                date = DateTime.Now,
                                id_sotrudnilck = MainWindow.usermain.id,
                                Result = serv.result,
                                id_historyResearch = aboba[0].id,
                            };
                            BD.Work_analizators_.Add(workanaliz);
                            BD.SaveChanges();
                            alltext += BD.Services_.First(x => x.Code == serv.serviceCode).Service + " " + serv.result + "\n";
                            MessageBox.Show("Принято");
                        }
                        else
                        {
                            MessageBox.Show("Отменён");
                            Analaz.ItemsSource = BD.HistoryResearch_.ToList().Where(a => a.Zakaz_.statusOrder == 2);// Заполнение DAtaGrid только не выполнеными услугамим 
                            ContentOfList.Text = "";
                            ContentOfList123.Text = "";
                        }
                    }
                    else
                    {

                    }


                }
                else
                {
                    Work_analizators_ work = new Work_analizators_
                    {
                        date = DateTime.Today,
                        id_historyResearch = aboba[0].id,
                        id_analisator = Analiz.SelectedIndex + 1,
                        id_sotrudnilck = 101,
                        Result = ContentOfList123.Text
                    };
                    BD.Work_analizators_.Add(work);
                    BD.Zakaz_.First(Z => Z.id == IDZAKAZ).statusOrder = 3;
                    BD.SaveChanges();
                    MessageBox.Show("Успешно добавленны в бд");
                    Analaz.ItemsSource = BD.HistoryResearch_.ToList().Where(a => a.Zakaz_.statusOrder == 2);// Заполнение DAtaGrid только не выполнеными услугамим 
                    ContentOfList.Text = "";
                    ContentOfList123.Text = "";
                }
            }
        }
        
        private void otpravit_Click(object sender, RoutedEventArgs e)
        {// При подтверждении  запись в бд
            УП_01_ИвановEntities BD = new УП_01_ИвановEntities();
            Work_analizators_ work = new Work_analizators_
            {
                date = DateTime.Today,
                id_historyResearch = aboba[0].id,
                id_analisator = Analiz.SelectedIndex+1,
                id_sotrudnilck = 101,
                Result = ContentOfList123.Text
            };
            BD.Work_analizators_.Add(work);
            BD.Zakaz_.First(Z => Z.id == IDZAKAZ).statusOrder = 3;
            BD.SaveChanges();
            MessageBox.Show("Успешно добавленны в бд");
            Analaz.ItemsSource = BD.HistoryResearch_.ToList().Where(a => a.Zakaz_.statusOrder == 2);// Заполнение DAtaGrid только не выполнеными услугамим 
            ContentOfList.Text = "";
            ContentOfList123.Text = "";
        }
        private void oprov_Click(object sender, RoutedEventArgs e)
        {
            // при отказе очистка и запись в бд с изменённым статусом
            УП_01_ИвановEntities BD = new УП_01_ИвановEntities();
            ContentOfList.Text = "";
            ContentOfList123.Text = "";
            MessageBox.Show("Повторите действия");
            BD.Zakaz_.First(Z => Z.id == IDZAKAZ).statusOrder = 2;
            BD.SaveChanges();
            Analaz.ItemsSource = BD.HistoryResearch_.ToList().Where(a => a.Zakaz_.statusOrder == 2);// Заполнение DAtaGrid только не выполнеными услугамим с 
            ContentOfList.Text = "";
            ContentOfList123.Text = "";
        }
        public List<HistoryResearch_> aboba = new List<HistoryResearch_>();
    }
}

