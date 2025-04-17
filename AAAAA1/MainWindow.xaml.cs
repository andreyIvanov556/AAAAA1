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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace AAAAA1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool showPassword = false;//Видисоть пароля
        private string Password//Свойство для получения пароля
        {
            get
            {
                return (showPassword) ? passwordVisible.Text : passwordHide.Password.ToString();
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            //Генерация капчи
            string pwd = string.Empty;

            System.Random r = new System.Random();
            var charactersAvailable = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();

            string rst = string.Empty;
            for (int i = 0; i < 4; i++)
            {
                rst = charactersAvailable[r.Next(0, charactersAvailable.Length - 1)].ToString();
                pwd += rst;

            }
            capthText.Text = pwd;

            var rotateTransform = new RotateTransform { Angle = r.Next(-10, 10) };
            capthText.LayoutTransform = rotateTransform;
        }

        public static Users_ usermain;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (enterin)// Если вход не заблокирован
            {
                if (capth.Text == capthText.Text || capth.Visibility == Visibility.Collapsed)//капча
                {
                    if (passwordHide.Password != string.Empty && loginBox.Text != string.Empty)//Проверка полей логина и пароля
                    {
                        УП_01_ИвановEntities BD = new УП_01_ИвановEntities();// подключение  к бд
                        Users_ user = BD.Users_.FirstOrDefault(x => x.login == loginBox.Text);
                         usermain = user;
                        if (user != null)// если пользователь найден
                        {
                            // проверка времени после неудачной попытки
                            TimeSpan? schet;
                            var Historys = BD.HistoryEnter_.Where(x => x.userid == user.id && x.idEror == 2).ToList();
                            if (Historys.Count > 0)
                            {
                                schet = DateTime.Now - Historys.Last().date;
                            }
                            else
                            {
                                schet = DateTime.Now - DateTime.MinValue;
                            }

                            if (schet > TimeSpan.FromHours(3))// Если прошло более 3х часов
                            {
                                if (user.password == passwordHide.Password)// проврека пароля
                                {
                                    HistoryEnter_ enter = new HistoryEnter_()//Запись успешного входа
                                    {
                                        date = DateTime.Now,
                                        idEror = null,
                                        ip = GetIp(),
                                        userid = user.id
                                    };
                                    BD.HistoryEnter_.Add(enter);
                                    BD.SaveChanges();
                                    switch (user.Role)// открытие окон в зависимости от роли
                                    {
                                        case 1: // Админ
                                            Window4 window4 = new Window4();
                                            window4.Name.Text = $"Здравстуйте {user.firstName} {user.secondName}  {user.thirdName} Администаратор";
                                            this.Hide();// прячем окно
                                            window4.ShowDialog();
                                            Show();// Показываем окно
                                            break;
                                        case 2: // Бухгалтер
                                            Window3 window3 = new Window3();
                                            window3.Name.Text = $"Здравстуйте {user.firstName} {user.secondName}  {user.thirdName} Бухгалтер";
                                            this.Hide();// прячем окно
                                            window3.ShowDialog();
                                            Show();// Показываем окно
                                            break;
                                        case 3: //Пациент
                                            break;
                                        case 4: // Лаборант
                                            Window1 window1 = new Window1();
                                            window1.Name.Text = $"Здравстуйте {user.firstName} {user.secondName}  {user.thirdName} Лаборант";
                                            this.Hide();// прячем окно
                                            window1.ShowDialog();
                                            Show();// Показываем окно
                                            break;
                                        case 5: // Лаборант-исследователь
                                            Window2 window2 = new Window2();
                                            window2.Name.Text = $"Здравстуйте {user.firstName} {user.secondName}  {user.thirdName} Лаборант-иследователь";
                                            this.Hide();// прячем окно
                                            window2.ShowDialog();
                                            Show();// Показываем окно
                                            break;
                                    }
                                    MessageBox.Show("Успешный вход");
                                }
                                else
                                {
                                    HistoryEnter_ enter = new HistoryEnter_//Запись неудачной попытки в историю входа
                                    {
                                        date = DateTime.Now,
                                        idEror = 2,
                                        ip = GetIp()
                                    };
                                    BD.HistoryEnter_.Add(enter);
                                    BD.SaveChanges();

                                }
                            }
                            else
                            {
                                MessageBox.Show("Подождите, идёт кварцевание");
                                HistoryEnter_ enter = new HistoryEnter_
                                {
                                    date = DateTime.Now,
                                    idEror = 1,
                                    ip = GetIp(),
                                    userid = user.id
                                };
                            }
                        }
                        else
                        {
                            MessageBox.Show("Неверный логин или пароль");
                            capth.Visibility = Visibility.Visible;// Показ капчи 
                            capthText.Visibility = Visibility.Visible;
                            obn.Visibility = Visibility.Visible;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Неверная капча");// блокировка входа на 10 секунд если капча неверна
                    timer.Tick += new EventHandler(timer_Tick);
                    timer.Interval = new TimeSpan(0, 0, 1);
                    enterin = false;
                    timer.Start();
                }
                // Кнопка входа
            }
            else MessageBox.Show("В соответствии с ТЗ тебе нужно подпждать 10 сек");
        }
        DispatcherTimer timer = new DispatcherTimer();// таймер для блокировки входа
        public static int block = 10;
        public static bool enterin = true;// разрешение входа
        private void timer_Tick(object sender, EventArgs e)// сам таймер
        {
            if (block > 0)
            {
                block--;
            }
            else
            {
                timer.Stop();
                MessageBox.Show("Вход разблокирован");
                enterin = true;
                block = 10;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // кнопка открытия пароля или скрытия
            showPassword = !showPassword;

            if (showPassword)
            {
                passwordVisible.Text = passwordHide.Password.ToString();
                passwordVisible.Visibility = Visibility.Visible;
                passwordHide.Visibility = Visibility.Hidden;
            }
            else
            {
                passwordHide.Password = passwordVisible.Text;
                passwordVisible.Visibility = Visibility.Hidden;
                passwordHide.Visibility = Visibility.Visible;
            }
        }

        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            // Кнопка обновления капчи
            string pwd = string.Empty;

            System.Random r = new System.Random();
            var charactersAvailable = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();

            string rst = string.Empty;
            for (int i = 0; i < 4; i++)
            {
                rst = charactersAvailable[r.Next(0, charactersAvailable.Length - 1)].ToString();
                pwd += rst;

            }
            capthText.Text = pwd;

            var rotateTransform = new RotateTransform { Angle = r.Next(-10, 10) };
            capthText.LayoutTransform = rotateTransform;
        }
        public static string GetIp()//Метод для получения IP
        {
            string ipAddress = string.Empty;
            if (Dns.GetHostAddresses(Dns.GetHostName()).Length > 0)
            {
                ipAddress = Dns.GetHostAddresses(Dns.GetHostName())[0].ToString();
            }
            return ipAddress;
        }
    }
}

