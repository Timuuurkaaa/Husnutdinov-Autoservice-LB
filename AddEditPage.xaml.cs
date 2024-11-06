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
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>
    public partial class AddEditPage : Page
    {
        //добавим новое поле, которое будет хранить в себе экземпляр добавляемого сервиса
        private Service _currentService = new Service();
        public AddEditPage(Service SelectedService)
        {
            InitializeComponent();

            if (SelectedService != null)
            {
                _currentService = SelectedService;
            }

            //при инициализации установим DataContext страницы - этот созданный объект
            DataContext = _currentService;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_currentService.Title))
                errors.AppendLine("Укажите название услуги");

            if (_currentService.Cost == 0)
                errors.AppendLine("Укажите стоимость услуги");

            //дискаунт у студентов числом может быть, тогда защита у всех cost
            if (_currentService.Discount < 0 || _currentService.Discount > 1 || !_currentService.Discount.HasValue)
                errors.AppendLine("Укажите скидку");

            if (_currentService.Duration == 0)
                errors.AppendLine("Укажите длительность услуги");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }
            //добавить в контекст текущие значения новой услуги
            if (_currentService.ID == 0)
                Husnutdinov_autoserviceEntities.GetContext().Service.Add(_currentService);

            //сохр изм, если никаких ошибок не получилось 
            try
            {
                Husnutdinov_autoserviceEntities.GetContext().SaveChanges();
                MessageBox.Show("информация сохранена");
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
