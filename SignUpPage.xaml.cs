using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Husnutdinov_Autoservice
{
    /// <summary>
    /// Логика взаимодействия для SignUpPage.xaml
    /// </summary>
    public partial class SignUpPage : Page
    {
        //добавим новое поле, которое будет хранить в себе экземпляр добавляемого сервиса
        private Service _currentService = new Service();
        public SignUpPage(Service SelectedService)
        {
            InitializeComponent();
            if (SelectedService != null)
                this._currentService = SelectedService;

            //При инициализации установим DataContext страницы - этот созданный объект
            //чтобы на форму подгрузить выбранные наименование услуги и длительность
            DataContext = _currentService;

            //вытащим из БД таблицу Клиент
            var _currentClient = Husnutdinov_autoserviceEntities.GetContext().Client.ToList();
            //свяжем ее с комбобоксом
            ComboClient.ItemsSource = _currentClient;
        }

        private ClientService _currentClientService = new ClientService();

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (ComboClient.SelectedItem == null)
                errors.AppendLine("Укажите ФИО клиента");
            if (StartDate.Text == "")
                errors.AppendLine("Укажите дату услуги");

            string s = TBStart.Text;
            if (s.Length <= 3 || !s.Contains(":"))
                TBEnd.Text = "";
            else
            {
                string[] start = s.Split(new char[] { ':' });
                int startHour = Convert.ToInt32(start[0].ToString()) * 60;
                int startMin = Convert.ToInt32(start[1].ToString());
                if (startHour / 60 >= 24 || startMin >= 60)
                    errors.AppendLine("Укажите верное время начала услуги");
            }

            if (TBStart.Text == "")
                errors.AppendLine("Укажите время начала услуги");


            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            //добавить текущие значения новой записи
            _currentClientService.ClientID = ComboClient.SelectedIndex + 1;//т.к. нумерация с 0
            _currentClientService.ServiceID = _currentService.ID;
            _currentClientService.StartTime = Convert.ToDateTime(StartDate.Text + " " + TBStart.Text);
            if (_currentClientService.ID == 0)
                Husnutdinov_autoserviceEntities.GetContext().ClientService.Add(_currentClientService);
            //сохранить изменения, если никаких ошибок не получилось при этом
            try
            {
                Husnutdinov_autoserviceEntities.GetContext().SaveChanges();
                MessageBox.Show("информация сохранена");
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void TBStart_TextChanged(object sender, TextChangedEventArgs e)
        {
            string s = TBStart.Text;

            if (s.Length <= 3 || !s.Contains(':'))
                TBEnd.Text = "";
            else
            {
                string[] start = s.Split(new char[] { ':' });
                int startHour = Convert.ToInt32(start[0].ToString()) * 60;
                int startMin = Convert.ToInt32(start[1].ToString());

                int sum = startHour + startMin + _currentService.Duration;

                int EndHour = sum / 60;
                EndHour = EndHour % 24;
                int EndMin = sum % 60;
                if (EndMin > 10)
                    s = EndHour.ToString() + ":" + EndMin.ToString();
                else
                    s = EndHour.ToString() + ":" + "0" + EndMin.ToString();
                TBEnd.Text = s;
            }
        }
    }
}