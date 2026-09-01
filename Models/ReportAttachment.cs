using System;
using System.IO;

namespace MunicipalReporter.Models
{
    public class ReportAttachment
    {
        public ReportAttachment(string filePath)
        {
            FilePath = filePath;
            FileName = Path.GetFileName(filePath);
            AttachedAt = DateTime.Now;
        }

        public string FilePath { get; private set; }
        public string FileName { get; private set; }
        public DateTime AttachedAt { get; private set; }

        public override string ToString()
        {
            return FileName;
        }
    }
}
