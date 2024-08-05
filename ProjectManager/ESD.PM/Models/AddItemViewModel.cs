using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ESD.PM.Models
{
    public class AddItemViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<FileNameModel> _items;
        public ObservableCollection<FileNameModel> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                OnPropertyChanged();
            }
        }

        public string Path { get; set; }

        public AddItemViewModel()
        {
            Items = new ObservableCollection<FileNameModel>();
        }

        private void CreateSubfolders(string path)
        {
            var folders = new List<string>()
            {
                "0 - Scope",
                "1 - Incoming",
                "2 - Outcoming",
                "3 - Checking"
            };
            foreach (var folder in folders)
            {
                Directory.CreateDirectory(path + "\\" + folder);
            }
        }

        public void AddEmptyLine()
        {
            if (Items.Any() && Items.Last().Name != string.Empty)
            {
                Items.Add(new FileNameModel(string.Empty));
            }
            else if (!Items.Any())
            {
                Items.Add(new FileNameModel(string.Empty));
            }
            else if (Items[Items.Count() - 2].Name == string.Empty)
            {
                Items.RemoveAt(Items.Count() - 1);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public void CreateItems()
        {
            foreach (var item in Items)
            {
                if (item.Name != string.Empty)
                {
                    var fullPath = Path + "\\" + item.Name;
                    if (!Directory.Exists(fullPath))
                    {
                        if (item.Name.Contains("item", StringComparison.OrdinalIgnoreCase))
                        {
                            Directory.CreateDirectory(fullPath);
                            CreateSubfolders(fullPath);
                        }
                        else
                        {
                            MessageBoxResult result = System.Windows.MessageBox.Show(
                                $"Are you sure you want to create {item.Name} item?",
                                "Confirmation",
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Warning);
                            if (result == MessageBoxResult.Yes)
                            {
                                Directory.CreateDirectory(fullPath);
                                CreateSubfolders(fullPath);
                            }
                        }
                    }
                    else
                    {
                        System.Windows.MessageBox.Show($"File '{item.Name}' already exists");
                    }
                }
            }
        }
    }
}
