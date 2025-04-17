using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AAA1;// использование библиотеки классов

namespace AAAAA1
{
    /// <summary>
    /// Логика взаимодействия для Window4.xaml
    /// </summary>
    public partial class Window4 : Window
    {
        public Window4()
        {
           InitializeComponent();

           УП_01_ИвановEntities BD = new УП_01_ИвановEntities();
           HistoriList.ItemsSource = BD.HistoryEnter_.ToList();
           DolCB.ItemsSource = BD.Role_.ToList();//Заполнение комбобокса
           DolCB.DisplayMemberPath = "Name_Rols";
           DolCB.SelectedValuePath = "Name_Rols";
            OblostPHTT.ItemsSource= BD.Users_.ToList();//Заполнение комбобокса
           OblostPHTT.DisplayMemberPath = "login";
           OblostPHTT.SelectedValuePath = "login";
           Nach.SelectedDateChanged += DatePicker_SelectedDateChanged;
           Con.SelectedDateChanged += DatePicker_SelectedDateChanged;
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateHistoryList();
        }

        private void UpdateHistoryList()
        {
            УП_01_ИвановEntities BD = new УП_01_ИвановEntities();
            var query = BD.HistoryEnter_.AsQueryable();

            // Фильтрация по датам, если они выбраны
            if (Nach.SelectedDate.HasValue)
            {
                query = query.Where(x => x.date >= Nach.SelectedDate.Value);
            }

            if (Con.SelectedDate.HasValue)
            {
                // Добавляем 1 день, чтобы включить выбранную дату окончания
                var endDate = Con.SelectedDate.Value.AddDays(1);
                query = query.Where(x => x.date < endDate);
            }

            HistoriList.ItemsSource = query.ToList();
        }
        /// <summary>
        /// Сортировка для Админа
        /// </summary>
        /// <param name="OblostPHTT"></param>
        /// <param name="Сортировка для Админа"></param>
        private void OblostPHTT_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            УП_01_ИвановEntities BD = new УП_01_ИвановEntities();
            string login = OblostPHTT.SelectedValue.ToString();
            var filtr = BD.HistoryEnter_.Where(a => a.Users_.login == login).ToList();//Сортировка
            HistoriList.ItemsSource = filtr;
            var query = BD.HistoryEnter_.AsQueryable();
            UpdateHistoryList();


        }

        private void DOBAV_Click(object sender, RoutedEventArgs e)
        {
            УП_01_ИвановEntities BD = new УП_01_ИвановEntities();
            if (Class1.login_check(LogTT.Text))// проверка из библиотеки классов  логина 
            {
                if (Class1.password_check(PassTT.Text))// проверка из библиотеки классов пароля 
                {
                    if (Class1.mail_check(EMailTT.Text))// а так же почты 
                    {
                        Users_ user = new Users_// создание новго пользователя
                        {
                            firstName = NameTT.Text,
                            secondName = FamTT.Text,
                            thirdName = OthTT.Text,
                            E_mail = EMailTT.Text,
                            Role = DolCB.SelectedIndex + 1,
                            login = LogTT.Text,
                            password = PassTT.Text,
                        };
                        BD.Users_.Add(user);
                        BD.SaveChanges();
                        MessageBox.Show("Пользователь создан");
                    }
                    else MessageBox.Show("Почта введена неверно");
                }
                else MessageBox.Show("Пароль введен неверно");
            }
            else MessageBox.Show("Логин введен не верно");
        }
    }
}