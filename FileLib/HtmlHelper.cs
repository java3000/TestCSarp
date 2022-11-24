using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FileLib
{
    public class HtmlHelper
    {
        //private const string TABLE = "<!DOCTYPE html><html lang=\"en\"><head><meta charset=\"UTF-8\"><title>Report</title><style>table {{border: 1px solid black; border-collapse: collapse; width: 100%;}}td, th {{padding: 3px; border: 1px solid black;}}</style></head><body><div id=\"table\"><table>{0}</table></div></body></html>";
        private const string TABLE = @"<!DOCTYPE html>
<html lang=""en"">
        <head>
        <meta charset=""UTF-8"">
            <title>Отчёт</title>
        <style>
        table {{border: 1px solid black; border-collapse: collapse; width: 100%;}}
        td, th {{padding: 1px; border: 1px solid black;}}
        </style>
        </head>
        <body>
        <div id=""table"">
            <table>
        <tr><th>Имя</th><th>Размер</th><th>Тип mime</th></tr>
        {0}
        </table>
        </div>
        <div id=""statistics"">
        <table>
        <th colspan=""3"">Статистика (средний размер файла по типу)<th>
        {1}
        </table>
            </div>
        <div id=""statistics2"">
        <table>
        <th colspan=""3"">Статистика (частота появлений типов) среди {2}<th>
        {3}
        </table>
            </div>
        </body>
        </html>";


        private const string TableDirRow = "<tr><td colspan=\"3\">{0}</td></tr>";
        private const string TableFileRow = "<tr><td>{0}</td><td>{1} KB</td><td>{2}</td></tr>";
        private const string TableMimeStatisticsRow = "<tr><td>{0}</td><td>{1} KB</td></tr>";
        private const string TableFrequencyStatisticsRow = "<tr><td>{0}</td><td>{1}</td><td>{2:P1}</td></tr>";


        public static void ProduceHtml(string path, List<FileSystemObject> fileSystemObjects)
        {
            if (fileSystemObjects == null) throw new ArgumentException("list of files is null");

            if (string.IsNullOrEmpty(path))
                path = Directory.GetCurrentDirectory();

            string result = String.Empty;
            StringBuilder table = new StringBuilder();

            foreach (var file in fileSystemObjects)
            {
                switch (file.Type)
                {
                    case ObjectType.DIRECTORY:
                        table.AppendFormat(TableDirRow + Environment.NewLine, file.Path);
                        break;
                    default:
                        table.AppendFormat(TableFileRow + Environment.NewLine, file.Name, file.Size, file.MimeType);
                        break;
                }
            }

            result = String.Format(TABLE, table.ToString(), CalcMimeStats(fileSystemObjects),
                fileSystemObjects.Where(x => x.Type == ObjectType.FILE).ToList().Count,
                CalcFreqStats(fileSystemObjects));

            using (FileStream fs = new FileStream(path + "/Report.html", FileMode.Create))
            {
                try
                {
                    byte[] buffer = Encoding.Default.GetBytes(result);
                    fs.Write(buffer, 0, buffer.Length);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public static string CalcMimeStats(List<FileSystemObject> fileSystemObjects)
        {
            StringBuilder sb = new StringBuilder();

            var grouped =
                from f in fileSystemObjects
                where f.Type != ObjectType.DIRECTORY
                group f.Size by f.MimeType
                into g
                select new { FileName = g.Key, Files = g.ToList() };

            foreach (var element in grouped)
            {
                if (!string.IsNullOrEmpty(element.FileName))
                    sb.AppendFormat(TableMimeStatisticsRow, element.FileName,
                        ((element.Files.Sum(file => file) / element.Files.Count) / 1024));
            }

            return sb.ToString();
        }

        public static string CalcFreqStats(List<FileSystemObject> fileSystemObjects)
        {
            StringBuilder sb = new StringBuilder();

            var grouped =
                from f in fileSystemObjects
                where f.Type != ObjectType.DIRECTORY
                group f.Size by f.MimeType
                into g
                select new { FileName = g.Key, Files = g.ToList() };

            foreach (var element in grouped)
            {
                //if (!string.IsNullOrEmpty(element.FileName))
                sb.AppendFormat(TableFrequencyStatisticsRow, element.FileName, element.Files.Count,
                    (double)element.Files.Count /
                    fileSystemObjects.Where(x => x.Type == ObjectType.FILE).ToList().Count);
            }

            return sb.ToString();
        }
    }
}