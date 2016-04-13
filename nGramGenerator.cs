//Author: Ritesh Mahato
//Program to generate n-grams of strings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiteshMahato.Utilities
{
	//<definition> function to generate nGrams for given list of strings</definition>
	//<param = "tokens">List of strings for which ngrams are to be generated</param>
	//<param = "ngramLength"> bigram = 2, trigram = 3, etc</param>
	//<return>List of nGrams (strings) </return>
    public class NGramGenerator{
        public static List<string> GetNgrams(List<string> tokens, int nGramLength)
        {
            var ngramList = new List<string>();
            for (int index = 0; index < (questionTokens.Count - nGramLength + 1); index++)
            {
                string s = "";
                int start = index;
                int end = index + nGramLength;
                for (int j = start; j < end; j++)
                {
                    s = s + " " + questionTokens[j];
                }
                s = s.TrimStart();
                ngramList.Add(s);
            }
            return ngramList;
        }
    }
}
