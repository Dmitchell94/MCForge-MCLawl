/*
Copyright (C) 2010-2013 David Mitchell

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
namespace MCForge
{
    public static class Extensions
    {
        public static string Truncate(this string source, int maxLength)
        {
            if (source.Length > maxLength)
            {
                source = source.Substring(0, maxLength);
            }
            return source;
        }
        public static byte[] GZip(this byte[] bytes)
        {
			using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
			{
				GZipStream gs = new GZipStream(ms, CompressionMode.Compress, true);
				gs.Write(bytes, 0, bytes.Length);
				gs.Close();
				ms.Position = 0;
				bytes = new byte[ms.Length];
				ms.Read(bytes, 0, (int)ms.Length);
				ms.Close();
				ms.Dispose();
			}
            return bytes;
        }
        public static byte[] Decompress(this byte[] gzip)
        {
            // Create a GZIP stream with decompression mode.
            // ... Then create a buffer and write into while reading from the GZIP stream.
            using (GZipStream stream = new GZipStream(new MemoryStream(gzip), CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    return memory.ToArray();
                }
            }
        }
        public static string[] Slice(this string[] str, int offset)
        {
            return str.Slice(offset, 0);
        }
        public static string[] Slice(this string[] str, int offset, int length)
        {
            IEnumerable<string> tmp = str.ToList();
            if (offset > 0)
            {
                tmp = str.Skip(offset);
            }
            else throw new NotImplementedException("This function only supports positive integers for offset");

            if(length > 0)
            {
                tmp = tmp.Take(length);
            }
            else if (length == 0)
            {
                // Do nothing
            }
            else throw new NotImplementedException("This function only supports non-negative integers for length");
            
            return tmp.ToArray();
        }
        public static string Capitalize(this string str)
        {
            if (String.IsNullOrEmpty(str))
                return String.Empty;
            char[] a = str.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }
        public static string Concatenate<T>(this List<T> list)
        {
            return list.Concatenate(String.Empty);
        }
        public static string Concatenate<T>(this List<T> list, string separator)
        {
            if (list.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (T obj in list)
                    sb.Append(separator + obj.ToString());
                sb.Remove(0, separator.Length);
                return sb.ToString();
            }
            return String.Empty;
        }
        public static string MCCharFilter(this string str)
        {
            // Allowed chars are any ASCII char between 20h/32 and 7Dh/125 inclusive, except for 26h/38 (&) and 60h/96 (`)
            str = Regex.Replace(str, @"[^\u0000-\u007F]", "");
             
            if (String.IsNullOrEmpty(str.Trim())) 
                return str;

            StringBuilder sb = new StringBuilder();

            foreach (char b in Encoding.ASCII.GetBytes(str))
            {
                if (b != 38 && b != 96 && b >= 32 && b <= 125)
                    sb.Append(b);
                else
                    sb.Append("*");
            }

            return sb.ToString();
        }
        public static string GetMimeType(this FileInfo file)
        {
            Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(file.Extension.ToLower());
            if (rk != null && rk.GetValue("Content Type") != null)
                return rk.GetValue("Content Type").ToString();
            return "application/octet-stream";
        }

        public static void DeleteLine(string file, string line) {
            var complete = from selectLine in File.ReadAllLines(file) where selectLine != line select selectLine;
            File.WriteAllLines(file, complete.ToArray());
        }

        public static void DeleteLineWord(string file, string word) {
                var complete = from selectLine in File.ReadAllLines(file) where !selectLine.Contains(word) select selectLine;
                File.WriteAllLines(file, complete.ToArray());
        }

        public static void DeleteExactLineWord(string file, string word) {
            var complete = from selectLine in File.ReadAllLines(file) where !selectLine.Equals(word) select selectLine;
            File.WriteAllLines(file, complete.ToArray());
        }

        public static void UncapitalizeAll(string file) {
            string[] complete = File.ReadAllLines(file);
            for (int i = 0; i < complete.Length; i++)
                complete[i] = complete[i].ToLower();
            File.WriteAllLines(file, complete);
        }
    }
}
