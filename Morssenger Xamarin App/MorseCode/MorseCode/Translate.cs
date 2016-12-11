using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MorseCode
{
    class Translate
    {
        private Dictionary<string, string> MorseKey;
        public Translate()
        {
            MorseKey = new Dictionary<string, string>();
            MorseKey.Add(" ", "|");
            MorseKey.Add("a", " ._ ");
            MorseKey.Add("b", " _... ");
            MorseKey.Add("c", " _._. ");
            MorseKey.Add("d", " _.. ");
            MorseKey.Add("e", " . ");
            MorseKey.Add("f", " .._. ");
            MorseKey.Add("g", " __. ");
            MorseKey.Add("h", " .... ");
            MorseKey.Add("i", " .. ");
            MorseKey.Add("j", " .___ ");
            MorseKey.Add("k", " _._ ");
            MorseKey.Add("l", " ._.. ");
            MorseKey.Add("m", " __ ");
            MorseKey.Add("n", " _. ");
            MorseKey.Add("o", " ___ ");
            MorseKey.Add("p", " .__. ");
            MorseKey.Add("q", " __._ ");
            MorseKey.Add("r", " ._. ");
            MorseKey.Add("s", " ... ");
            MorseKey.Add("t", " _ ");
            MorseKey.Add("u", " .._ ");
            MorseKey.Add("v", " ..._ ");
            MorseKey.Add("w", " .__ ");
            MorseKey.Add("x", " _.._ ");
            MorseKey.Add("y", " _.__ ");
            MorseKey.Add("z", " __.. ");
            MorseKey.Add("0", " __--- ");
            MorseKey.Add("1", " ._--- ");
            MorseKey.Add("2", " ..--- ");
            MorseKey.Add("3", " ...-- ");
            MorseKey.Add("4", " ....- ");
            MorseKey.Add("5", " ..... ");
            MorseKey.Add("6", " _.... ");
            MorseKey.Add("7", " __... ");
            MorseKey.Add("8", " __-.. ");
            MorseKey.Add("9", " __--. ");
            MorseKey.Add(".", " ._._._ ");
            MorseKey.Add(",", " __..__ ");
            MorseKey.Add("?", " ..__.. ");
            MorseKey.Add("'", " .____. ");
            MorseKey.Add("!", " _._.__ ");

        }
        public string toMorse(string text)
        {
            string result = "";
            for (int i = 0; i < text.Length; i++)
            {
                string a;
                MorseKey.TryGetValue(text[i].ToString().ToLower(), out a);
               
                result += a;
            }
            return result;
        }

        public string toWord(string text)
        {
            return MorseKey.FirstOrDefault(y => y.Value.Trim() == text).Key;
        }
    }
}
