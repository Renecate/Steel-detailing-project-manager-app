using ESD.PM.ViewModels;
using System.Windows;

namespace ESD.PM.Views
{
    public partial class AddItemDialog : Window
    {
        private AddItemViewModel _addItemViewModel;

        public AddItemDialog(Window owner, string pathToItemsFolder)
        {
            InitializeComponent();
            _addItemViewModel = new AddItemViewModel();
            DataContext = _addItemViewModel;
            Owner = owner;
            _addItemViewModel.Path = pathToItemsFolder;
            _addItemViewModel.AddEmptyLine();
        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (sender is System.Windows.Controls.TextBox textBox)
            {
                var currentText = textBox.Text;
                _addItemViewModel.AddEmptyLine();
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            _addItemViewModel.CreateItems();
        }
    }
}