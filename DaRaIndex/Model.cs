using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IOExtension;

namespace DaRaIndex
{
    public class Model
    {
        private readonly string currentDirectoryPath = ".\\";
        private readonly string indexFileName = "index.ind";
        private readonly string appProperty = "Application";
        private readonly string appValue = "DaRaIndex";
        private readonly string dateProperty = "Date";
        private readonly string rateProperty = "Rate";
        private readonly int rateMinValue = 1;
        private readonly int rateMaxValue = 5;
        private readonly string noRate = string.Empty;
        private readonly string noDate = string.Empty;
        public List<string> Rates { get; private set; } = new List<string>();

        public ObservableCollection<Folder> Folders { get; private set; } = new ObservableCollection<Folder>();
        public string ErrorMessage { get; private set; } = string.Empty;

        public Model()
        {
            if (rateMinValue > rateMaxValue)
                throw new IndexOutOfRangeException(nameof(rateMinValue) + " bigger than " + nameof(rateMaxValue));

            Rates.Add(noRate);

            for (int i = rateMinValue; i <= rateMaxValue; i++)
                Rates.Add(i.ToString());
        }

        public void GetFoldersList()
        {
            try
            {
                string[] foldersPaths = Directory.GetDirectories(currentDirectoryPath, "*", SearchOption.AllDirectories);
                Array.Sort(foldersPaths);
                Folders.Clear();

                foreach (var folderPath in foldersPaths)
                {
                    string filePath = GetIndexFilePath(folderPath);

                    if (File.Exists(filePath))
                    {
                        SettingsFile.SettingsFilePath = filePath;

                        string inputDate = SettingsFile.GetPropertyValue(dateProperty);
                        string validDate = GetValidDate(inputDate);

                        if (validDate != inputDate)
                            SettingsFile.SetPropertyValue(dateProperty, validDate);

                        string inputRate = SettingsFile.GetPropertyValue(rateProperty);
                        string validRate = GetValidRate(inputRate);

                        if (validRate != inputRate)
                            SettingsFile.SetPropertyValue(rateProperty, validRate);

                        SettingsFile.SetPropertyValue(appProperty, appValue);
                        File.SetAttributes(filePath, File.GetAttributes(filePath) | FileAttributes.Hidden);

                        Folders.Add(IndexedFolderEntity(folderPath, validDate, validRate));

                        
                    }
                    else
                    {
                        Folders.Add(NotIndexedFolderEntity(folderPath));
                    }
                }
            }
            catch(IOException e)
            {
                ErrorMessage = e.Message;
            }
        }

        public void IndexSelected(int[] selectedIndexes)
        {
            try
            {
                foreach (int index in selectedIndexes)
                {
                    if (!Folders[index].IsIndexed)
                    {
                        string folderPath = Folders[index].Path;
                        string filePath = GetIndexFilePath(folderPath);

                        File.Create(filePath).Dispose();
                        
                        SettingsFile.SettingsFilePath = filePath;
                        SettingsFile.SetPropertyValue(dateProperty, noDate);
                        SettingsFile.SetPropertyValue(rateProperty, noRate);
                        SettingsFile.SetPropertyValue(appProperty, appValue);

                        File.SetAttributes(filePath, File.GetAttributes(filePath) | FileAttributes.Hidden);

                        Folders.RemoveAt(index);
                        Folders.Insert(index, IndexedFolderEntity(folderPath));
                    }
                }
            }
            catch(IOException e)
            {
                ErrorMessage = e.Message;
            }
        }

        public void UnindexSelected(int[] selectedIndexes)
        {
            try
            {
                foreach (int index in selectedIndexes)
                {
                    string folderPath = Folders[index].Path;
                    string filePath = GetIndexFilePath(folderPath);

                    if (File.Exists(filePath))
                        File.Delete(filePath);

                    Folders.RemoveAt(index);
                    Folders.Insert(index, NotIndexedFolderEntity(folderPath));
                }
            }
            catch (IOException e)
            {
                ErrorMessage = e.Message;
            }
        }

        public void SetDateForSelected(int[] selectedIndexes, DateTime dateTime)
        {
            try
            {
                string date = dateTime.ToString("d");

                foreach (int index in selectedIndexes)
                {
                    if (Folders[index].IsIndexed)
                    {
                        Folder folder = Folders[index];
                        string filePath = GetIndexFilePath(folder.Path);

                        SettingsFile.SettingsFilePath = filePath;
                        SettingsFile.SetPropertyValue(dateProperty, date);

                        File.SetAttributes(filePath, File.GetAttributes(filePath) | FileAttributes.Hidden);

                        Folders.RemoveAt(index);
                        Folders.Insert(index, IndexedFolderEntity(folder.Path, date, folder.Rate));
                    }
                }
            }
            catch (IOException e)
            {
                ErrorMessage = e.Message;
            }
        }

        public void SetRateForSelected(int[] selectedIndexes, int rateIndex)
        {
            try
            {
                string rate = Rates[rateIndex];

                foreach (int index in selectedIndexes)
                {
                    if (Folders[index].IsIndexed)
                    {
                        Folder folder = Folders[index];
                        string filePath = GetIndexFilePath(folder.Path);

                        SettingsFile.SettingsFilePath = filePath;
                        SettingsFile.SetPropertyValue(rateProperty, rate);

                        File.SetAttributes(filePath, File.GetAttributes(filePath) | FileAttributes.Hidden);

                        Folders.RemoveAt(index);
                        Folders.Insert(index, IndexedFolderEntity(folder.Path, folder.Date, rate));
                    }
                }
            }
            catch (IOException e)
            {
                ErrorMessage = e.Message;
            }
        }

        private string GetIndexFilePath(string folderPath) => folderPath + "\\" + indexFileName;

        private Folder NotIndexedFolderEntity(string folderPath)
            => new Folder(GetShortPath(folderPath), string.Empty, string.Empty, false);

        private Folder IndexedFolderEntity(string folderPath)
            => new Folder(GetShortPath(folderPath), noDate, noRate, true);

        private Folder IndexedFolderEntity(string folderPath, string date, string rate)
            => new Folder(GetShortPath(folderPath), date, rate, true);

        private string GetValidDate(string date)
        {
            if (string.IsNullOrEmpty(date))
            {
                return noDate;
            }
            else
            {
                return date;
            }
        }

        private string GetValidRate(string rate)
        {
            if (rate is null)
                return noRate;

            bool isFound = false;

            foreach (string validRate in Rates)
            {
                if (validRate == rate)
                {
                    isFound = true;
                    break;
                }
            }

            if (isFound)
            {
                return rate;
            }
            else
            {
                return noRate;
            }
        }

        private string GetShortPath(string path)
        {
            if (path.StartsWith(currentDirectoryPath))
            {
                return path[currentDirectoryPath.Length..];
            }
            else
            {
                return path;
            }
        }

        public void ClearErrorMessage() => ErrorMessage = string.Empty;
    }
}
