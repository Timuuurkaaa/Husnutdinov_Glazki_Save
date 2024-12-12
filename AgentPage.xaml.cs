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

namespace Husnutdinov_Glazki_Save
{
    public partial class AgentPage : Page
    {

        int CountRecords;
        int CountPage;
        int CurrentPage = 0;

        List<Agent> CurrentPageList = new List<Agent>();
        List<Agent> TableList;

        public AgentPage()
        {
            InitializeComponent();
            var currentAgents = Husnutdinov_Glazki_SaveEntities.GetContext().Agent.ToList();
            AgentsListView.ItemsSource = currentAgents;

            Filter.SelectedIndex = 0;
            Sort.SelectedIndex = 0;
        }
        private void UpdateAgents()
        {
            var currentAgents = Husnutdinov_Glazki_SaveEntities.GetContext().Agent.ToList();

            if (Filter.SelectedIndex == 0)
            {
                AgentsListView.ItemsSource = currentAgents;
            }
            if (Filter.SelectedIndex == 1)
            {
                currentAgents = currentAgents.Where(p => (p.AgentTypeString == "МФО")).ToList();
            }
            if (Filter.SelectedIndex == 2)
            {
                currentAgents = currentAgents.Where(p => (p.AgentTypeString == "ООО")).ToList();
            }
            if (Filter.SelectedIndex == 3)
            {
                currentAgents = currentAgents.Where(p => (p.AgentTypeString == "ЗАО")).ToList();
            }
            if (Filter.SelectedIndex == 4)
            {
                currentAgents = currentAgents.Where(p => (p.AgentTypeString == "МКК")).ToList();
            }
            if (Filter.SelectedIndex == 5)
            {
                currentAgents = currentAgents.Where(p => (p.AgentTypeString == "ОАО")).ToList();
            }
            if (Filter.SelectedIndex == 6)
            {
                currentAgents = currentAgents.Where(p => (p.AgentTypeString == "ПАО")).ToList();
            }

            if (Sort.SelectedIndex == 1)
            {
                currentAgents = currentAgents.OrderBy(p => p.Title).ToList();
            }
            if (Sort.SelectedIndex == 2)
            {
                currentAgents = currentAgents.OrderByDescending(p => p.Title).ToList();
            }
            if (Sort.SelectedIndex == 3)
            {
                currentAgents = currentAgents.OrderBy(p => p.Title).ToList();
            }
            if (Sort.SelectedIndex == 4)
            {
                currentAgents = currentAgents.OrderBy(p => p.Title).ToList();
            }
            if (Sort.SelectedIndex == 5)
            {
                currentAgents = currentAgents.OrderBy(p => p.Priority).ToList();
            }
            if (Sort.SelectedIndex == 6)
            {
                currentAgents = currentAgents.OrderByDescending(p => p.Priority).ToList();
            }

            string SearchText = Search.Text;
            if (SearchText != null)
            {
                currentAgents = currentAgents.Where(p => (p.Title.ToLower().Contains(SearchText.ToLower())) || p.Phone.Replace("+", "").Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "").Contains(SearchText) || p.Email.ToLower().Contains(SearchText.ToLower())).ToList();
            }

            AgentsListView.ItemsSource = currentAgents;
            TableList = currentAgents;
            ChangePage(0, 0);
        }

        private void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAgents();
        }

        private void Sort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAgents();
        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateAgents();
        }

        private void ChangePage(int direction, int? selectedPage)
        {
            CurrentPageList.Clear();
            CountRecords = TableList.Count;
            if (CountRecords % 10 > 0)
            {
                CountPage = CountRecords / 10 + 1;
            }
            else
            {
                CountPage = CountRecords / 10;
            }
            Boolean Ifupdate = true;
            int min;
            if (selectedPage.HasValue)
            {
                if (selectedPage >= 0 && selectedPage <= CountPage)
                {
                    CurrentPage = (int) selectedPage;
                    min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                    for(int i = CurrentPage*10;i<min;i++)
                    {
                        CurrentPageList.Add(TableList[i]);
                    }
                }
            }
            else//если нажата стрелка
            {
                switch (direction)
                {
                    
                    case 1://пред стр
                        if (CurrentPage > 0)//если назад можно
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
                            //если выйдем из диап страниц, то ничего не произойдёт
                        }
                        break;
                    case 2://след стр
                        if(CurrentPage < CountPage - 1)//если вперёд можно
                        {
                            CurrentPage++;
                            min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                            for(int i = CurrentPage * 10;i<min;i++)
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
            if (Ifupdate)//если currentPage не вышел из  диап, то
            {
                PageListBox.Items.Clear();
                //удаление старых значений их листбокса номеров страниц, нужно чтобы при изменении
                //кол-ва записей, колич страниц динамич изм
                for (int i = 1; i <= CountPage; i++)
                {
                    PageListBox.Items.Add(i);
                }
                PageListBox.SelectedIndex = CurrentPage;
                AgentsListView.ItemsSource = CurrentPageList;
                //обн отобрад списка услуг
                AgentsListView.Items.Refresh();
            }
            //вывод количества записей на странице и общего количества
            min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
            TBCount.Text = min.ToString();
            TBAllRecords.Text = " из " + CountRecords.ToString();
        }

        private void LeftDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(1,null);
        }

        private void RightDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(2,null);
        }

        private void PageListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ChangePage(0, Convert.ToInt32(PageListBox.SelectedItem.ToString()) - 1);
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage((sender as Button).DataContext as Agent));
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage(null));
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Husnutdinov_Glazki_SaveEntities.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                AgentsListView.ItemsSource = Husnutdinov_Glazki_SaveEntities.GetContext().Agent.ToList();
                UpdateAgents();
            }
        }
    }
}