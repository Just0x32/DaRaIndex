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
        public List<string> Rates { get => model.Rates; }
        public ObservableCollection<Folder> Folders { get => model.Folders; }
        public string ErrorMessage { get => model.ErrorMessage; }

        public void GetFoldersList() => model.GetFoldersList();

        public void IndexSelected(int[] selectedIndexes) => model.IndexSelected(selectedIndexes);

        public void UnindexSelected(int[] selectedIndexes) => model.UnindexSelected(selectedIndexes);

        public void SetDateForSelected(int[] selectedIndexes, DateTime dateTime) => model.SetDateForSelected(selectedIndexes, dateTime);

        public void SetRateForSelected(int[] selectedIndexes, int rateIndex) => model.SetRateForSelected(selectedIndexes, rateIndex);

        public void ClearErrorMessage() => model.ClearErrorMessage();
    }
}
