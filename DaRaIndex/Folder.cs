using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaRaIndex
{
    public class Folder
    {
        public string Path { get; private set; }
        public string Date { get; private set; }
        public int Rate { get; private set; }
        public bool IsIndexed { get; private set; }

        public Folder(string path, string date, int rate, bool isIndexed)
        {
            if (path is null)
                throw new NullReferenceException(nameof(path));

            if (date is null)
                throw new NullReferenceException(nameof(date));

            Path = path;
            Date = date;
            Rate = rate;
            IsIndexed = isIndexed;
        }
    }
}
