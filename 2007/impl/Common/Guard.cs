using System;
using System.IO;

namespace Common
{
    public static class Guard
    {
        public static void FileExists(string filename)
        {
            ArgumentNotNull(filename, "filename");

            if (!File.Exists(filename))
                throw new InvalidDataException(String.Format("File \"{0}\" does not exists. Absolute path: \"{1}\"",
                                                             filename,
                                                             new FileInfo(filename).FullName));
        }

        public static void ArgumentNotNull(object arg, string argName)
        {
            if (ReferenceEquals(arg, null))
                throw new ArgumentNullException(argName);
        }
    }
}