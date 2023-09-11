using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tgbot
{
    internal class WordStorage
    {
        private const string _path = "wordstorage.txt";
        public Dictionary<string, string> GetAllWords()
        {
            try
            {
                var dic = new Dictionary<string, string>();
                if (File.Exists(_path))
                {
                    foreach (var line in File.ReadAllLines(_path))
                    {
                        var words = line.Split('|');
                        if (words.Length == 2)
                        {
                            dic.Add(words[0], words[1]);
                        }
                    }
                }
                return dic;
            }
            catch (Exception ex)
            {
                Console.WriteLine("не удалось прочесть файл");
                return new Dictionary<string, string>();
            }
        }
        public void AddWord(string eng, string rus)
        {
            try
            {
                using (var writer = new StreamWriter(_path, true))
                {
                    writer.WriteLine($"{eng}|{rus}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"не удалось добавить слово {eng} в словарь");
            }
        }
    }
}
