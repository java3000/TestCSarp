using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MimeMapping;

namespace FileLib
{
    public class FileSystemHelper
    {
        private List<FileSystemObject> _nodes;

        public FileSystemHelper()
        {
            _nodes = new List<FileSystemObject>();
        }

        public List<FileSystemObject> Scan(string directory)
        {
            directory ??= Directory.GetCurrentDirectory();

            try
            {
                Walk(directory);
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return _nodes;
        }

        private void Walk(string directory, int depth = 0)
        {
            foreach (string f in Directory.EnumerateFileSystemEntries(directory))
            {
                if ((File.GetAttributes(f) & FileAttributes.Directory) != FileAttributes.Directory)
                {
                    var file = new FileInfo(f);

                    var fileSystemObject = new FileSystemObject()
                    {
                        Path = f,
                        Type = ObjectType.FILE,
                        MimeType = MimeUtility.GetMimeMapping(file.Name),
                        Name = file.Name,
                        Size = file.Length
                    };
                    
                    _nodes.Add(fileSystemObject);

                    for (int i = 0; i < depth; i++) Console.Write("  ");
                    Console.WriteLine(
                        $"|__ {fileSystemObject.Name} [size: {fileSystemObject.Size / 1024} KB, mime: {fileSystemObject.MimeType}]");
                }
                else
                {
                    foreach (string d in Directory.GetDirectories(directory))
                    {
                        var dir = new DirectoryInfo(d);
                        
                        var fileSystemObject = new FileSystemObject()
                        {
                            Path = d,
                            Type = ObjectType.DIRECTORY,
                            MimeType = "",
                            Name = dir.FullName,
                            Size = dir.EnumerateFiles("*", SearchOption.AllDirectories).Sum(file => file.Length) / 1024
                        };
                    
                        _nodes.Add(fileSystemObject);
                        
                        for (int i = 0; i < depth + 1; i++) Console.Write("  ");
                        Console.WriteLine(
                            $@"|__ {fileSystemObject.Name.ToUpper()} [size: {fileSystemObject.Size} KB]");

                        Walk(d, ++depth);
                    }
                }
            }
        }
        //
    }
}