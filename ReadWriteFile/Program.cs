using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace ReadWriteFile
{
    class Program
    {
        static void Main(string[] args)
        {
            const string SOURCE = "daftar-nama.txt";
            const string FILENAME = "4210191003_AryazakyImanFauzy.csv";
            const string PATTERN = @"\d{10}\t(\w+[.]?[^\t]?)+\t\w+[-]?\w+";
            const string HEADER = "Daftar Mahasiswa Pens\n#,NRP,Nama,Jenis Kelamin";

            string raw_text = File.ReadAllText(SOURCE);
            Console.WriteLine(raw_text);
            List<Mahasiswa> tabel_mahasiswa = ExtractStringToList(raw_text, PATTERN);
            CreateCsvFile(FILENAME);
            int index = 1;
            WriteStringToFile(HEADER, FILENAME);
            WriteListToFile(FILENAME, tabel_mahasiswa, ref index);
            if (tabel_mahasiswa.Count > 0)
            {
                tabel_mahasiswa.Reverse();
                tabel_mahasiswa.RemoveAt(0);
            }
            WriteListToFile(FILENAME, tabel_mahasiswa, ref index);
            Console.WriteLine("\nEnd of program. Press any key.");
            Console.ReadLine();
        }

        struct Mahasiswa
        {
            public string NRP;
            public string nama;
            public bool isLaki;
        }

        private static void WriteStringToFile(string text, string filename)
        {
            try
            {
                StreamWriter sw = new StreamWriter(filename);
                sw.Write(text + "\n");
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }

        private static void WriteListToFile(string filename, List<Mahasiswa> tabel_mahasiswa, ref int i)
        {
            try
            {
                StreamWriter sw = new StreamWriter(filename, true);
                int idx = 0;
                foreach (Mahasiswa mahasiswa in tabel_mahasiswa)
                {
                    if (idx > 0 && !mahasiswa.Equals(tabel_mahasiswa[idx - 1]))
                    {
                        sw.Write($"{i},{mahasiswa.NRP},{mahasiswa.nama},{(mahasiswa.isLaki ? "Laki-laki" : "Perempuan")}\n");
                        Console.Write($"{i},{mahasiswa.NRP},{mahasiswa.nama},{(mahasiswa.isLaki ? "Laki-laki" : "Perempuan")}\n");
                        i++;
                    }
                    idx++;
                }
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }

        private static void CreateCsvFile(string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }

                FileStream f = File.Create(fileName);
                f.Close();
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }
        }

        private static List<Mahasiswa> ExtractStringToList(string raw_text, string pattern)
        {
            if (raw_text == string.Empty)
            {
                throw new ArgumentException("String kosong!");
            }

            List<Mahasiswa> list_mahasiswa = new List<Mahasiswa>();
            Regex regex = new Regex(pattern);
            while (regex.IsMatch(raw_text))
            {
                Mahasiswa mahasiswa = new Mahasiswa();
                Match match = regex.Match(raw_text);
                Console.WriteLine("<Captured>" + match);
                ExtractTextToMahasiswa(match.ToString(), ref mahasiswa);
                list_mahasiswa.Add(mahasiswa);
                //Console.WriteLine("Size raw=" + raw_text.Length + ", Size substring=" + match.Length);
                raw_text = raw_text.Substring(match.Length, raw_text.Length - match.Length);
            }
            
            return list_mahasiswa;
        }

        private static void ExtractTextToMahasiswa(string text, ref Mahasiswa mahasiswa)
        {
            if (text == string.Empty)
            {
                Console.WriteLine("String kosong! " + text);
                return;
            }

            string[] nrp_nama_jk = { @"\d{10}", @"\w[.]?[^\d\t](\w+[.]?\s?)+\t", @"(Laki-laki|Perempuan)" };
            Match[] matches = new Match[nrp_nama_jk.Length];

            for (int i = 0; i < nrp_nama_jk.Length; i++)
            {
                //Console.WriteLine(nrp_nama_jk[i]);
                Regex regex = new Regex(nrp_nama_jk[i]);
                matches[i] = regex.Match(text);
                Console.WriteLine("Matches i = " + matches[i].ToString() + "-end-");
            }

            mahasiswa.NRP = matches[0].ToString();
            mahasiswa.nama = matches[1].ToString().Remove(matches[1].Length - 1);
            if (matches[2].ToString().Contains("Perempuan"))
            {
                mahasiswa.isLaki = false;
            }
            else if (matches[2].ToString().Contains("Laki-laki"))
            {
                mahasiswa.isLaki = true;
            }
            Console.WriteLine("NRP:" + matches[0].ToString() + ", Nama:" + matches[1].ToString() + ", " + matches[2].ToString() + "\n");
        }
    }
}
