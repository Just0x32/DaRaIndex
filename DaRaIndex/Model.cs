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
        public static readonly string IndexFileName = "index.ind";
        public static readonly string AppProperty = "Application";
        public static readonly string AppValue = "DaRaIndex";
        public static readonly string DateProperty = "Date";
        public static readonly string RateProperty = "Rate";
        public static readonly int RateMinValue = 1;
        public static readonly int RateMaxValue = 10;
        private static readonly int noRate = 0;
        private static readonly string noDate = "00.00.0000";
        public ObservableCollection<Folder> Folders { get; private set; } = new ObservableCollection<Folder>();
        public string Error { get; private set; } = string.Empty;

        public void GetFoldersList()
        {
            try
            {
                string[] foldersPaths = Directory.GetDirectories(".", "*", SearchOption.AllDirectories);
                Array.Sort(foldersPaths);
                Folders.Clear();

                foreach (var folderPath in foldersPaths)
                {
                    string indexFilePath = folderPath + "\\" + IndexFileName;

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

                        Folders.Add(new Folder(folderPath[2..], date, rate, true));

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
                        Folders.Add(new Folder(folderPath[2..], string.Empty, default, false));
                    }
                }
            }
            catch(IOException e)
            {
                Error = e.Message;
            }
        }
    }
}
