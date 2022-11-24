using System;

namespace FileLib
{
    [Serializable]
    public class FileSystemObject
    {
        public string Name { get; set; }
        public long Size { get; set; }
        public string Path { get; set; }
        public ObjectType Type { get; set; }
        public string MimeType { get; set; }
    }
}