using ESD.PM.Commands;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace ESD.PM.Views
{
    public partial class AddItemDialog : Window
    {
        public ObservableCollection<ItemModel> ItemNames { get; set; } = new ObservableCollection<ItemModel>();
        public ObservableCollection<string> ExistingItemNames { get; set; }

        public ICommand AddItemCommand { get; }
        public ICommand OKCommand { get; }

        public AddItemDialog(ObservableCollection<string> existingItemNames)
        {
            InitializeComponent();
            DataContext = this;
            ExistingItemNames = existingItemNames;
            AddItemCommand = new DelegateCommand(AddItem);
            OKCommand = new DelegateCommand(OK);
            ItemNames.Add(new ItemModel());
        }

        private void AddItem(object obj)
        {
            ItemNames.Add(new ItemModel());
        }

        private void OK(object obj)
        {
            var duplicateItem = ItemNames.GroupBy(i => i.Name)
                                         .Where(g => g.Count() > 1)
                                         .Select(g => g.Key)
                                         .FirstOrDefault();

            if (duplicateItem != null)
            {
                System.Windows.MessageBox.Show($"Item name '{duplicateItem}' is duplicated. Please enter unique names.");
                return;
            }

            foreach (var item in ItemNames)
            {
                if (ExistingItemNames.Contains(item.Name))
                {
                    System.Windows.MessageBox.Show($"Item name '{item.Name}' already exists. Please enter a different name.");
                    return;
                }
            }

            DialogResult = true;
            Close();
        }
    }

    public class ItemModel
    {
        public string Name { get; set; }
    }
}