using Microsoft.Win32;
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
    public partial class AddEditPage : Page
    {
        private Agent currentAgent = new Agent();
        private ProductSale currentProductProductSale = new ProductSale();
        public AddEditPage(Agent SelectedAgent)
        {
            InitializeComponent();

            ComboType.SelectedIndex = 0;

            if (SelectedAgent != null)
            {
                currentAgent = SelectedAgent;
                ComboType.SelectedIndex = currentAgent.AgentTypeID - 1;
            }

            var currentProductSales = Husnutdinov_Glazki_SaveEntities.GetContext().ProductSale.ToList();
            var TitleAll = Husnutdinov_Glazki_SaveEntities.GetContext().Product.ToList();
            currentProductSales = currentProductSales.Where(x => x.AgentID == currentAgent.ID).ToList();
            HistoryOfRealisationListView.ItemsSource = currentProductSales;
            ProductTitleCombo.ItemsSource = TitleAll;

            DataContext = currentProductProductSale;
            DataContext = currentAgent;
        }

        private void ChangePictureBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            if (myOpenFileDialog.ShowDialog() == true)
            {
                //абсолютный путь берётся
                currentAgent.Logo = myOpenFileDialog.FileName;
                //загружаем в элемент картинку
                LogoImage.Source = new BitmapImage(new Uri(myOpenFileDialog.FileName));
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(currentAgent.Title))
                errors.AppendLine("Укажите наименование агента");
            if (string.IsNullOrWhiteSpace(currentAgent.Address))
                errors.AppendLine("Укажите адрес агента");
            if (string.IsNullOrWhiteSpace(currentAgent.DirectorName))
                errors.AppendLine("Укажите ФИО директора");
            if (ComboType.SelectedItem == null)
                errors.AppendLine("Укажите тип агента");
            if (string.IsNullOrWhiteSpace(currentAgent.Priority.ToString()))
                errors.AppendLine("Укажите приоритет агента");
            if (currentAgent.Priority <= 0)
                errors.AppendLine("Укажите положительный приоритет агента");
            if (string.IsNullOrWhiteSpace(currentAgent.INN))
                errors.AppendLine("Укажите ИНН агента");
            if (string.IsNullOrWhiteSpace(currentAgent.KPP))
                errors.AppendLine("Укажите КПП агента");
            if (string.IsNullOrWhiteSpace(currentAgent.Phone))
                errors.AppendLine("Укажите телефон агента");
            else
            {
                string ph = currentAgent.Phone.Replace("+", "").Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "");
                if (((ph[1] == '9' || ph[1] == '4' || ph[1] == '8') && ph.Length != 11)
                    || (ph[1] == '3' && ph.Length != 12))
                    errors.AppendLine("Укажите правильно телефон агента");
            }
            if (string.IsNullOrWhiteSpace(currentAgent.Email))
                errors.AppendLine("Укажите почту агента");
            if(errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            currentAgent.AgentTypeID = ComboType.SelectedIndex + 1;

            //добавить в контекст текущие значения новой услуги
            if (currentAgent.ID==0)
                Husnutdinov_Glazki_SaveEntities.GetContext().Agent.Add(currentAgent);
            //сохр изм, если нет ошибок
            try
            {
                Husnutdinov_Glazki_SaveEntities.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена");
                Manager.MainFrame.GoBack();
            }
            catch(Exception ex) 
            { 
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            //забираем агента, для которого нажата кнопка "Удалить"
            var currentAgent = (sender as Button).DataContext as Agent;
            //проверка на возможность удаления
            var currentProductSale = Husnutdinov_Glazki_SaveEntities.GetContext().ProductSale.ToList();
            var currentAgentPriorityHistory = Husnutdinov_Glazki_SaveEntities.GetContext().AgentPriorityHistory.ToList();
            var currentShop = Husnutdinov_Glazki_SaveEntities.GetContext().Shop.ToList();
            currentProductSale = currentProductSale.Where(p => p.AgentID == currentAgent.ID).ToList();
            currentAgentPriorityHistory = currentAgentPriorityHistory.Where(p => p.AgentID == currentAgent.ID).ToList();
            currentShop = currentShop.Where(p => p.AgentID == currentAgent.ID).ToList();
            if (currentProductSale.Count != 0)
                MessageBox.Show("Невозможно выполнить удаление, так как существует история реализации продуктов");
            else
            {
                if (MessageBox.Show("Вы точно хотите выполнить удаление?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        Husnutdinov_Glazki_SaveEntities.GetContext().Agent.Remove(currentAgent);
                        if (currentAgentPriorityHistory.Count != 0)
                        {
                            for (int i = 0; currentAgentPriorityHistory.Count == i; i++)
                                Husnutdinov_Glazki_SaveEntities.GetContext().AgentPriorityHistory.Remove(currentAgentPriorityHistory[i]);
                        }
                        if (currentShop.Count != 0)
                        {
                            for (int i = 0; currentShop.Count == i; i++)
                                Husnutdinov_Glazki_SaveEntities.GetContext().Shop.Remove(currentShop[i]);
                        }
                        Husnutdinov_Glazki_SaveEntities.GetContext().SaveChanges();
                        MessageBox.Show("Информация удалена!");
                        Manager.MainFrame.GoBack();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
            }
        }

        private void HistoryAddBtn_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (ProductTitleCombo.SelectedItem == null)
                errors.AppendLine("Укажите наименование продукта");
            bool flag = false;
            for (int i = 0; i < TextBoxProductCount.Text.Length; i++)
            {
                if (TextBoxProductCount.Text[i] < '0' || TextBoxProductCount.Text[i] > '9')
                {
                    flag = true;
                }
            }
            if (flag)
            {
                errors.AppendLine("Укажите количество продукта");
            }
            if(Convert.ToInt32(TextBoxProductCount.Text)<=0)
            {
              errors.AppendLine("Укажите положительное количество продукта");
            }
            if (string.IsNullOrWhiteSpace(DatePickerProductSale.Text))
                errors.AppendLine("Укажите дату продажи");
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            if (currentAgent.ID == 0)
                Husnutdinov_Glazki_SaveEntities.GetContext().Agent.Add(currentAgent);
            

            currentProductProductSale.AgentID = currentAgent.ID;
            currentProductProductSale.ProductID = ProductTitleCombo.SelectedIndex + 1;
            currentProductProductSale.SaleDate = Convert.ToDateTime(DatePickerProductSale.Text);
            currentProductProductSale.ProductCount = Convert.ToInt32(TextBoxProductCount.Text);

            if(currentProductProductSale != null)
                Husnutdinov_Glazki_SaveEntities.GetContext().ProductSale.Add(currentProductProductSale);

            //сохр изм, если нет ошибок
            try
            {
                Husnutdinov_Glazki_SaveEntities.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена");
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void HistoryDeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы точно хотите выполнить удаление?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    foreach(ProductSale ItemsProductSale in HistoryOfRealisationListView.SelectedItems)
                    {
                        Husnutdinov_Glazki_SaveEntities.GetContext().ProductSale.Remove(ItemsProductSale);
                    }
                    Husnutdinov_Glazki_SaveEntities.GetContext().SaveChanges();
                    MessageBox.Show("Информация удалена!");
                    Manager.MainFrame.GoBack();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }
    }
}