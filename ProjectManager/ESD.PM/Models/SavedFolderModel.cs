using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESD.PM.Models
{
    public class SavedFolderModel
    {
        [JsonProperty]
        public string FullName { get; set; }

        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty]
        public ObservableCollection<TagsModel> Tags { get; set; }

        [JsonProperty]
        public bool HideFolderIsTrue { get; set; }

        [JsonProperty]
        public bool ViewIsToggled { get; set; }

        [JsonProperty]
        public bool DateSortIsTrue { get; set; }

        [JsonProperty]
        public bool HideNumbersIsTrue { get; set; }

        public SavedFolderModel(string fullName)
        {
            FullName = fullName;
            Name = new DirectoryInfo(fullName).Name;
        }
    }
}
