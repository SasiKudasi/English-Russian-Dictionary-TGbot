using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tgbot
{
    internal class Tutor
    {

        private WordStorage _storage = new WordStorage();
        private Dictionary<string, string> _dic;
        private Random _random = new Random();


        public Tutor()
        {
            _dic = _storage.GetAllWords();
        }

        public void AdWord(string eng, string rus)
        {
            if (!_dic.ContainsKey(eng))
            {

                _storage.AddWord(eng, rus);
                _dic.Add(eng, rus);
            }
        }

        public bool CheckWord(string eng, string rus)
        {
            var answer = _dic[eng];
            return answer == rus;
        }

        public string Translate(string eng)
        {
            if (_dic.ContainsKey(eng))
            {


                return _dic[eng];

            }
            else
            {
                return null;
            }
        }

        public string GetRandomWord()
        {
            var r = _random.Next(0, _dic.Count);

            var keys = new List<string>(_dic.Keys);

            Console.WriteLine(keys[r]);
            return keys[r];
        }
    }
}
