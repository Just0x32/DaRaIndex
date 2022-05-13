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
        public static readonly string currentDirectoryPath = ".\\";
        public static readonly string IndexFileName = "index.ind";
        public static readonly string AppProperty = "Application";
        public static readonly string AppValue = "DaRaIndex";
        public static readonly string DateProperty = "Date";
        public static readonly string RateProperty = "Rate";
        public static readonly int RateMinValue = 1;
        public static readonly int RateMaxValue = 5;
        private static readonly int noRate = 0;
        private static readonly string noDate = "";
        public ObservableCollection<Folder> Folders { get; private set; } = new ObservableCollection<Folder>();
        public string ErrorMessage { get; private set; } = string.Empty;

        public void GetFoldersList()
        {
            try
            {
                string[] foldersPaths = Directory.GetDirectories(currentDirectoryPath, "*", SearchOption.AllDirectories);
                Array.Sort(foldersPaths);
                Folders.Clear();

                foreach (var folderPath in foldersPaths)
                {
                    string indexFilePath = GetIndexFilePath(folderPath);

                    if (File.Exists(indexFilePath))
                    {
                        SettingsFile.SettingsFilePath = indexFilePath;

                        string date = GetValidDate(SettingsFile.GetPropertyValue(DateProperty));
                        if (date == noDate)
                            SettingsFile.SetPropertyValue(DateProperty, noDate);

                        int rate = GetValidRate(SettingsFile.GetPropertyValue(RateProperty));
                        if (rate == noRate)
                            SettingsFile.SetPropertyValue(RateProperty, noRate.ToString());

                        SettingsFile.SetPropertyValue(AppProperty, AppValue);

                        Folders.Add(IndexedFolderEntity(folderPath, date, rate));

                        string GetValidDate(string date)
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

                        int GetValidRate(string rate)
                        {
                            if (int.TryParse(rate, out int validRate) && validRate >= RateMinValue && validRate <= RateMaxValue)
                            {
                                return validRate;
                            }
                            else
                            {
                                return noRate;
                            }
                        }
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
                        File.Create(GetIndexFilePath(folderPath)).Close();

                        SettingsFile.SettingsFilePath = GetIndexFilePath(folderPath);
                        SettingsFile.SetPropertyValue(DateProperty, noDate);
                        SettingsFile.SetPropertyValue(RateProperty, noRate.ToString());
                        SettingsFile.SetPropertyValue(AppProperty, AppValue);

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

        private string GetIndexFilePath(string folderPath) => folderPath + "\\" + IndexFileName;

        private Folder NotIndexedFolderEntity(string folderPath)
            => new Folder(GetShortPath(folderPath), string.Empty, default, false);

        private Folder IndexedFolderEntity(string folderPath)
            => new Folder(GetShortPath(folderPath), noDate, noRate, true);

        private Folder IndexedFolderEntity(string folderPath, string date, int rate)
            => new Folder(GetShortPath(folderPath), date, rate, true);

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
