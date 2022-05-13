using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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

namespace DaRaIndex
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ViewModel viewModel = new ViewModel();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = viewModel;
            DatePicker.SelectedDate = DateTime.Today;
        }

        private void GetFoldersList_Click(object sender, RoutedEventArgs e)
        {
            viewModel.GetFoldersList();
            CheckModelError();
        }

        private void IndexSelected_Click(object sender, RoutedEventArgs e)
        {
            viewModel.IndexSelected(GetSelectedIndexes(FoldersList.SelectedItems));
            CheckModelError();
        }

        private void UnindexSelected_Click(object sender, RoutedEventArgs e)
        {
            viewModel.UnindexSelected(GetSelectedIndexes(FoldersList.SelectedItems));
            CheckModelError();
        }

        private void SetDateForSelected_Click(object sender, RoutedEventArgs e)
        {
            if (DatePicker.SelectedDate != null)
                viewModel.SetDateForSelected(GetSelectedIndexes(FoldersList.SelectedItems), (DateTime)DatePicker.SelectedDate);

            CheckModelError();
        }

        private void SetRateForSelected_Click(object sender, RoutedEventArgs e)
        {
            if (RateComboBox.SelectedIndex >= 0)
                viewModel.SetRateForSelected(GetSelectedIndexes(FoldersList.SelectedItems), RateComboBox.SelectedIndex);

            CheckModelError();
        }

        private int[] GetSelectedIndexes(System.Collections.IList selectedItems)
        {
            int[] selectedIndexes = new int[selectedItems.Count];

            for (int i = 0; i < selectedItems.Count; i++)
                selectedIndexes[i] = FoldersList.Items.IndexOf(selectedItems[i]);

            return selectedIndexes;
        }

        private void CheckModelError()
        {
            if (!string.IsNullOrEmpty(viewModel.ErrorMessage))
                MessageBox.Show(viewModel.ErrorMessage);

            viewModel.ClearErrorMessage();
        }
    }
}
