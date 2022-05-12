using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaRaIndex
{
    public class ViewModel
    {
        private Model model = new Model();
        public ObservableCollection<Folder> Folders { get => model.Folders; }

        public void GetFoldersList() => model.GetFoldersList();
    }
}
