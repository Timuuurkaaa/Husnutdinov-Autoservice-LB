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
    /// Логика взаимодействия для ServicePage.xaml
    /// </summary>
    public partial class ServicePage : Page
    {
        int CountRecords;//Количество записей в таблице
        int CountPage;//Общее количество страниц
        int CurrentPage = 0;//Текущая страница
        List<Service> CurrentPageList = new List<Service>();
        List<Service> TableList;
        public ServicePage()
        {
            InitializeComponent();
            //добавляем строки
            // загрузить в список из бд
            var currentService = Husnutdinov_autoserviceEntities.GetContext().Service.ToList();
            //связать с нашим листвью
            ServiceListView.ItemsSource = currentService;
            //добавили строки

            ComboType.SelectedIndex = 0;

            //вызываем UpdateSerices()
            UpdateSerices();
        }

        private void UpdateSerices()
        {
            //из бд берём таблицу Сервис
            var currentServices = Husnutdinov_autoserviceEntities.GetContext().Service.ToList();
            //фильтрация по условию задания
            if (ComboType.SelectedIndex == 0)
            {
                currentServices = currentServices.Where(p => ((p.DiscountInt) >= 0 && (p.DiscountInt) <= 100)).ToList();
            }
            if (ComboType.SelectedIndex == 1)
            {
                currentServices = currentServices.Where(p => ((p.DiscountInt) >= 0 && (p.DiscountInt) < 5)).ToList();
            }
            if (ComboType.SelectedIndex == 2)
            {
                currentServices = currentServices.Where(p => ((p.DiscountInt) >= 5 && (p.DiscountInt) < 15)).ToList();
            }
            if (ComboType.SelectedIndex == 3)
            {
                currentServices = currentServices.Where(p => ((p.DiscountInt) >= 15 && (p.DiscountInt) < 30)).ToList();
            }
            if (ComboType.SelectedIndex == 4)
            {
                currentServices = currentServices.Where(p => ((p.DiscountInt) >= 30 && (p.DiscountInt) < 70)).ToList();
            }
            if (ComboType.SelectedIndex == 5)
            {
                currentServices = currentServices.Where(p => ((p.DiscountInt) >= 70 && (p.DiscountInt) < 100)).ToList();
            }
            //реализуем поиск данных в листвью при вводе текста в окно поиска
            currentServices = currentServices.Where(p => p.Title.ToLower().Contains(TBoxSearch.Text.ToLower())).ToList();

            //для отображения итогов фильтра и поиска в листвью
            ServiceListView.ItemsSource = currentServices.ToList();

            if (RButtonDown.IsChecked.Value)
            {
                //для отображения итогов фильтра и поиска листвью по убыванию
                currentServices = currentServices.OrderByDescending(p => p.Cost).ToList();
            }

            if (RButtonUp.IsChecked.Value)
            {
                //для отображения итогов фильтра и поиска листвью по возрастанию
                currentServices = currentServices.OrderBy(p => p.Cost).ToList();
            }
            //для отображения итогов фильтра и поиска в листвью
            ServiceListView.ItemsSource = currentServices;
            //заполнение таблицы для постраничного вывода
            TableList = currentServices;
            //вызов функции отображения кол-ва страниц с параметрами:
            //направление 0 - начальная загрузка
            //0 - выбранная страница
            ChangePage(0, 0);
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateSerices();
        }

        private void ComboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSerices();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            UpdateSerices();
        }

        private void RButtonUp_Checked(object sender, RoutedEventArgs e)
        {
            UpdateSerices();
        }

        private void RButtonDown_Checked(object sender, RoutedEventArgs e)
        {
            UpdateSerices();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            //открыть окно ред/доб услуг
            Manager.MainFrame.Navigate(new AddEditPage(null));
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Husnutdinov_autoserviceEntities.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                ServiceListView.ItemsSource = Husnutdinov_autoserviceEntities.GetContext().Service.ToList();
            }
            UpdateSerices();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            //открыть окно ред/доб услуг
            Manager.MainFrame.Navigate(new AddEditPage((sender as Button).DataContext as Service));
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            //забираем Сервис, для которого нажата кнопка "Удалить"
            var currentService = (sender as Button).DataContext as Service;

            //проверка на возможность удаления
            var currentClientServices = Husnutdinov_autoserviceEntities.GetContext().ClientService.ToList();
            currentClientServices = currentClientServices.Where(p => p.ServiceID == currentService.ID).ToList();
            if (currentClientServices.Count != 0) //если есть записи на этот сервис
            {
                MessageBox.Show("Невозможно выполнить удаление, так как существуют записи на эту услугу");
            }
            else
            {
                if (MessageBox.Show("Вы точно хотите выполнить удаление?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        Husnutdinov_autoserviceEntities.GetContext().Service.Remove(currentService);
                        Husnutdinov_autoserviceEntities.GetContext().SaveChanges();
                        //выводим в листвью измененную таблицу Сервис
                        ServiceListView.ItemsSource = Husnutdinov_autoserviceEntities.GetContext().Service.ToList();
                        // чтобы применились фильтры и поиск, если они были на форме изначально
                        UpdateSerices();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
            }
        }

        private void LeftDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(1, null);
        }

        private void RightDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(2, null);
        }
        private void ChangePage(int direction, int? selectedPage)//Функция отвечающая за разделение list'a
        {
            //direction - направление. 0 - начало, 1 - предыдущая страница, 2 - следующая страница
            //selectedPage - при нажатии на стрелочки передается null,
            //при выборе определенной страницы в этой переменной находится номер страницы
            CurrentPageList.Clear();// начальная очистка листа
            CountRecords = TableList.Count;//определение количества записей во всем списке
            //определение кол-ва страниц
            if (CountRecords % 10 > 0)
            {
                CountPage = CountRecords / 10 + 1;
            }
            else
            {
                CountPage = CountRecords / 10;
            }
            Boolean Ifupdate = true;
            //Проверка на правильность - если
            //CurrentPage(номер текущей страница) "правильный"

            int min;
            if (selectedPage.HasValue)//Проверка на значение не null (тк может быть null)
            {
                if (selectedPage >= 0 && selectedPage <= CountPage)
                {
                    CurrentPage = (int)selectedPage;
                    min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                    for (int i = CurrentPage * 10; i < min; i++)
                    {
                        CurrentPageList.Add(TableList[i]);
                    }
                }
            }
            else //если нажата стрелка
            {
                switch (direction)
                {
                    case 1://нажата кнопка "Предыдущая страница"
                        if (CurrentPage > 0)
                //то есть кнопка нажата правильно и "назад" можно идти
                        {
                            CurrentPage--;
                            min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                            for (int i = CurrentPage * 10; i < min; i++)
                            {
                                CurrentPageList.Add(TableList[i]);
                            }
                        }
                        else
                        {
                            Ifupdate = false;
                            //в случае если currentPage попытается выйти из диапазона внесение данных не произойдет
                        }
                        break;
                    case 2://нажата кнопка "следующая страница"
                        if (CurrentPage < CountPage - 1)
                        //если вперед идти можно
                        {
                            CurrentPage++;
                            min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                            for (int i = CurrentPage * 10; i < min; i++)
                            {
                                CurrentPageList.Add(TableList[i]);
                            }
                        }
                        else
                        {
                            Ifupdate = false;
                        }
                        break;
                }
            }
            if (Ifupdate)//если currentPage не вышел из диапазона, то
            {
                PageListBox.Items.Clear();
                //Удаление старых значений из listBox'а номеров страниц, нужно чтобы при изменении
                //кол-ва записей кол-во страниц динамически изменялось
                for (int i = 1; i <= CountPage; i++)
                {
                    PageListBox.Items.Add(i);
                }
                PageListBox.SelectedIndex = CurrentPage;

                //вывод количества записей на странице и общего количества
                min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                TBCount.Text = min.ToString();
                TBAllRecords.Text = " из " + CountRecords.ToString();

                ServiceListView.ItemsSource = CurrentPageList;
                //обновить отображение списка услуг
                ServiceListView.Items.Refresh();
            }
        }
        private void PageListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ChangePage(0, Convert.ToInt32(PageListBox.SelectedItem.ToString()) - 1);
        }

        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            //открыть окно записи клиента на выбранную услугу
            Manager.MainFrame.Navigate(new SignUpPage((sender as Button).DataContext as Service));
        }
    }
}