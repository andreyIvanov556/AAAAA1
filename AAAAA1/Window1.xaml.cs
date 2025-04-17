using Aspose.BarCode.Generation;// Для генерации Штрих-кода 
using iTextSharp.text;// Для работы с PDF
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using Path = System.IO.Path;
using AAA1;// Использование библиотеки классов

namespace AAAAA1
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
       public УП_01_ИвановEntities BD = new УП_01_ИвановEntities();
        public Window1()
        {
            InitializeComponent();
           
            ServiceCB.ItemsSource = BD.Services_.ToList();//Заполнение комбобокса намиенованиеями услуг
            ServiceCB.DisplayMemberPath = "Service";//Показывает
            ServiceCB.SelectedValuePath = "Service";// хранит
            UserCB.ItemsSource = BD.Zakaz_.ToList();//Заполнение комбобокса пользователя
            UserCB.DisplayMemberPath = "Users";
            UserCB.SelectedValuePath = "Users";
            NazvCB.ItemsSource = BD.insuranceCompany_.ToList();//Заполнение комбобокса названиями страховых компаний
            NazvCB.DisplayMemberPath = "insuranceCompany";
            NazvCB.SelectedValuePath = "insuranceCompany";
            TipStrCB.ItemsSource = BD.Type_.ToList();
            TipStrCB.DisplayMemberPath = "name";
            TipStrCB.SelectedValuePath = "name";
            BioMAt.ItemsSource = BD.Zakaz_.ToList();// Заполнение DAtaGrida
            comboBox.ItemsSource = BD.Zakaz_.ToList();//Заполнение комбобокса
            comboBox.DisplayMemberPath = "Users";
            comboBox.SelectedValuePath = "Users";
            DispatcherTimer timer = new DispatcherTimer();//Создаём таймер
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 1);// указываем интервал
            timer.Start();// запускаем таймер
        }
        public static TimeSpan seconds = TimeSpan.FromSeconds(0);// счётчик прошедших секунд
        public static bool onl = false;//для отслеживания уведомления
        public static TimeSpan time = TimeSpan.FromSeconds(9000);// указание общего времени отсчёта в секундах
        /// <summary>
        /// тик таймера что он тделает каждую секунду
        /// </summary>
        /// <param name="timer_Tick"></param>
        /// <param name="тaймер"></param>
        private void timer_Tick(object sender, EventArgs e)
        {
            if (onl == false && seconds >= TimeSpan.FromSeconds(8100))
            {
                onl = true; MessageBox.Show("Осталось 15 минут"); // вывод уведомления
            }
            if (seconds < TimeSpan.FromSeconds(9000)) seconds = seconds.Add(TimeSpan.FromSeconds(1));
            else Application.Current.Shutdown();//Закрытие приложение по тому как пройдёт 2,5 часа
            timer.Text = time.Subtract(seconds).ToString();
        }
        /// <summary>
        /// Предназначен для фильтрации списка заказов  в DataGrid на основе пользователей ComboBox, происходит очистка текущих данных в DataGrid, далее происходит  получение Заказов из БД и сама фильтрация.
        /// </summary>
        /// <param name="comboBox_SelectionChanged"></param>
        /// <param name="Фильтрация списка заказов"></param>
        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Фильтрация закакзов по выбранному пользователю
            BioMAt.ItemsSource = null;
            
            List<Zakaz_> zakaz = BD.Zakaz_.ToList();
            BioMAt.ItemsSource = zakaz.Where(a => a.Users == comboBox.SelectedValue.ToString()).ToList();//Сортировка
        }
        /// <summary>
        /// При закрытии окна  записывается выход пользователя в Историю входов . Создаётся новая запись , добавляется и  сохраняется в бд
        /// </summary>
        /// <param name="Window_Closing"></param>
        /// <param name="При закрытии окна происходит запись Истории входа"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Запись в историю входа
            
            HistoryEnter_ enter = new HistoryEnter_()
            {
                date = DateTime.Now,
                userid = MainWindow.usermain.id,
                ip = MainWindow.GetIp(),
                idEror = 3
            };
            BD.HistoryEnter_.Add(enter);
            BD.SaveChanges();
        }
        static int ch = 0;
        /// <summary>
        /// При нажатии Enter в текстовом поле выполняется валидая ввода , генерация штрих-кода, и создаётся PDF документ со щтрих кодом
        /// </summary>
        /// <param name="textBox_KeyDown"></param>
        /// <param name="При нажатии Enter в текстовом поле"></param>
        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            
            if (e.Key == Key.Enter)// при нажатии Enter
            {
                // текст бокс в нужные значения(Валидация ввода)
                if (!int.TryParse(textBox.Text, out int number)) textBox.Text = null;
                if (textBox.Text == null || textBox.Text == "") textBox.Text = "0";
                if (Convert.ToInt32(textBox.Text) <= BD.Zakaz_.ToList().Last().id)
                {
                    textBox.Text = (BD.Zakaz_.ToList().Last().id + 1).ToString();
                }
                //Генерация штрих кода
                var imageType = "Jpeg";// тип картинки в переменной вар
                var imageFormat = (BarCodeImageFormat)Enum.Parse(typeof(BarCodeImageFormat), imageType);// сначала штрихкод потом конвертируем формат
                var encodeType = EncodeTypes.Code128;
                string imagePath = "Code128_" + ch.ToString() + "." + imageType;// название пути 
                string imagePath1 = "Code128" + ch.ToString() + "." + imageType;// название пути 
                string TextBarcode = textBox.Text;
                if (textBox.Text.Count() == 1)
                {
                    TextBarcode = "00" + textBox.Text;
                }
                else
                {
                    if (textBox.Text.Count() == 2) TextBarcode = "0" + textBox.Text;
                }
                TextBarcode += GenerateBarcodeData();// ИСпользование метода для получения даты и 6 цифр
                BarcodeGenerator generator = new BarcodeGenerator(encodeType, TextBarcode);// генерируем штрих код
                generator.Save(imagePath, imageFormat);
                generator.Save(imagePath1, imageFormat);
                image.Source = new BitmapImage(new Uri(Path.GetFullPath(imagePath)));
                generator.Dispose();
                ch++;//инкримент счётчика
                // генерация PDF и вставка туда штрих кода
                var document = new iTextSharp.text.Document();
                using (var writer = PdfWriter.GetInstance(document, new FileStream("Жизннь без почки.pdf", FileMode.Create)))
                {
                    document.Open();
                    //вставка изображения штрихкода
                    var logo = iTextSharp.text.Image.GetInstance(new FileStream(Environment.CurrentDirectory.ToString() + @"\Code128" + (ch - 1).ToString() + ".Jpeg", FileMode.Open));//готовим переменую для хранения в ней фотки
                    logo.SetAbsolutePosition(0, 680);//кординаты для картинки
                    writer.DirectContent.AddImage(logo);// добавляем картинку
                    BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\arial.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);//берем ариал так как нормально воспринимет русский текст
                    iTextSharp.text.Font helvetica1 = new iTextSharp.text.Font(baseFont, 78, iTextSharp.text.Font.NORMAL);
                    document.Add(new iTextSharp.text.Paragraph(" ", helvetica1));
                    document.Close();
                    writer.Close();
                }
            }
        }
        /// <summary>
        /// Если текстбокс пустой то  выводится последний айди из таблицы заказ увеличенный на 1 если текстбокс не пустой  текстблок очищается
        /// </summary>
        /// <param name="textBox_TextChanged"></param>
        /// <param name="Обработка события изменения текста в textBox"></param>
        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            
            if (textBox.Text == null || textBox.Text == "")// если текстовое поле пустое 
            {
                textBlock.Text = (BD.Zakaz_.ToList().Last().id + 1).ToString();// показываем последний айди +1

            }
            else textBlock.Text = null;// очищаем текст блок


        }
        private string GenerateBarcodeData()//генерация дополнительных данных для штрих-кода
        {
            // Текущая дата в формате yyyyMMdd
            string datePart = DateTime.Now.ToString("ddMMyyyy");

            // 6 случайных чисел
            Random random = new Random();
            string randomPart = random.Next(100000, 999999).ToString();

            return datePart + randomPart;
        }
        /// <summary>
        /// Обрабатывает событие нажатия на кнопку и создаёт новую запись в таблице заказ и сохраняет в БД
        /// </summary>
        /// <param name="DOBAVIZAKAZ_Click"></param>
        /// <param name="Обрабатывает событие нажатия на кнопку и создаёт новую запись в таблице заказ"></param>
        private void DOBAVIZAKAZ_Click(object sender, RoutedEventArgs e)
        {
            
            if (textBox.Text != null)// проверка на пустоту 
            {
                Zakaz_ zakaz = new Zakaz_()// создание  нового объекта (запись в бд)
                {
                    Date = DateTime.Today,
                    id = Convert.ToInt32(textBox.Text),
                    statusOrder = 1,
                    UserId = BD.Zakaz_.FirstOrDefault(a => a.Users_ == comboBox.SelectedValue).id
                };
                BD.Zakaz_.Add(zakaz);// добавление в бд
                BD.SaveChanges();// сохранение

            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            
            if (Class1.login_check(LogTT.Text))// проверка из библиотеки классов логина 
            {
                if (Class1.password_check(passTT.Text))// проверка из библиотеки классов пароля
                {
                    if (Class1.mail_check(EMailTT.Text))// проверка из бибилиотеки классов 
                    {
                        Users_ user = new Users_// создание новго пользователя
                        {
                            firstName = NateTT.Text,
                            secondName = FamTT.Text,
                            thirdName = OnhTT.Text,
                            birthDate = Date.SelectedDate,
                            E_mail = EMailTT.Text,
                            Role = 3,
                            login = LogTT.Text,
                            passport = PasTT.Text,
                            password = passTT.Text,
                            phoneNumber = NomTT.Text,
                            type = TipStrCB.SelectedIndex + 1,
                            insuranceNumber = Convert.ToDouble(NomStrBlockTT.Text),
                            insuranceCompany = NazvCB.SelectedIndex + 1
                        };
                        BD.Users_.Add(user);
                        BD.SaveChanges();
                        Zakaz_ zakaz = new Zakaz_// создание связанного заказа тюк при заполнение комбобокса  с кликнтами я буру имено пациентов а не пользователей
                        {
                            UserId = user.id,
                            Date = DateTime.Today,
                            statusOrder = 3
                        };
                        BD.Zakaz_.Add(zakaz);
                        BD.SaveChanges();
                        MessageBox.Show("Пользователь создан");
                        BioMAt.ItemsSource = null;
                        comboBox.ItemsSource = null;
                        BioMAt.ItemsSource = BD.Zakaz_.ToList();
                        comboBox.ItemsSource = BD.Zakaz_.ToList();
                        comboBox.DisplayMemberPath = "Users";
                        comboBox.SelectedValuePath = "Users";
                    }
                    else MessageBox.Show("Почта введена неверно");
                }
                else MessageBox.Show("Пароль введен неверно");
            }
            else MessageBox.Show("Логин введен не верно");
        }

        private void EMailTT_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        /// <summary>
        /// Выполняет переход между вкладками TabControl и синхранизует выбранное значение
        /// </summary>
        /// <param name="DOBAVI_Click"></param>
        /// <param name="переход между вкладками TabControl"></param>
        private void DOBAVI_Click(object sender, RoutedEventArgs e)
        {

            wew.Focus();// переход между TabItem
            UserCB.SelectedValue = comboBox.SelectedValue;
        }
        /// <summary>
        /// Обрабатывает добавление услуг к заказу и обновление статуса заказа"
        /// </summary>
        /// <param name="Uslugi_Click"></param>
        /// <param name="Обрабатывает добавление услуг к заказу и обновление статуса заказа"></param>
        private void Uslugi_Click(object sender, RoutedEventArgs e)
        {
            //Сохранение услуг в заказе
            
            foreach (HistoryResearch_ histories in ServiceDG.Items)
            {
                zakazik.statusOrder = 2;// изменениее статуса заказа
                BD.HistoryResearch_.Add(histories);
                BD.SaveChanges();
            }
            ServiceDG.Items.Clear();
            if (textBox.Text != null && ServiceCB.SelectedIndex != -1)
            {
                Zakaz_ Usersd = (Zakaz_)UserCB.SelectedItem;
                Zakaz_ zakaziks = new Zakaz_();
                zakaziks.Date = DateTime.Now;
                zakaziks.statusOrder = 2;
                zakaziks.UserId = Usersd.id;
                BD.Zakaz_.Add(zakaziks);
                BD.SaveChanges();
                MessageBox.Show("Заказ создан");
            }
            MessageBox.Show("Услуги добавлены к заказу");
        }
        /// <summary>
        ///  Выполняется Добавление выбранной услуги в DataGrid и подсчёт итговой суммы заказа
        /// </summary>
        /// <param name="ServiceCB_SelectionChanged"></param>
        /// <param name="Добавление выбранной услуги в DataGrid и подсчёт итговой суммы заказа"></param>
        private void ServiceCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // добавление выбранной услуги в dateGrid
            
            HistoryResearch_ hist = new HistoryResearch_();

            hist.services = BD.Services_.FirstOrDefault(x => x.Service == ServiceCB.SelectedValue.ToString()).Code;
            hist.zakazid = zakazik.id;
            hist.Services_ = BD.Services_.ToList()[ServiceCB.SelectedIndex];
            ServiceDG.Items.Add(hist);
            Decimal? dec = 0;// подсчёт суммы заказа
            foreach (HistoryResearch_ histo in ServiceDG.Items)
            {
                dec += histo.Services_.Price;
            }
            Sum.Text = dec.ToString();

        }

        private void BioMAt_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
           
            // проверяем статус выбранного заказа
            if (zakazik.statusOrder == 1)
            {
                // очистка DataGrida с услугами
                ServiceDG.ItemsSource = null;
                ServiceDG.Items.Clear();
                // устанавливаем выбранного пользователя в CB
                UserCB.SelectedValue = zakazik.Users_;
                // Активация элементов управления для работы с услугами 
                Uslugi.IsEnabled = true;
                ServiceCB.IsEnabled = true;
            }
            else
            {
                // очисктка DAtaGrid
                ServiceDG.ItemsSource = null;
                ServiceDG.Items.Clear();
                // Загрузка списка иследований для выбранного заказа
                ServiceDG.ItemsSource = BD.HistoryResearch_.Where(x => x.zakazid == zakazik.id).ToList();
                // только просмотр
                Uslugi.IsEnabled = false;
                ServiceCB.IsEnabled = false;
                UserCB.SelectedValue = zakazik.Users_;// установка выбранного пользователя
            }
        }
        public int index;
        public Zakaz_ zakazik = new Zakaz_();// поле для хранения выбранного заказа
        private void BioMAt_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            index = BioMAt.SelectedIndex;// получение индекса  выбранного элемента
            if (index >= 0)// если  выбран существующий элемент 
            {
                zakazik = (Zakaz_)BioMAt.Items[index];// сохраняем выбранный элемент 
            }
        }
        private void UserCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {


        }
    }
}