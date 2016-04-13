//Author: Ritesh Mahato
//Program to stem words

using ikvm.extensions;

namespace RiteshMahato.Utilities
{
    public class Stemmer
    {
        private char[] b;
        private int i, i_end, j, k;
        private static int INC = 50;

        public Stemmer()
        {
            b = new char[INC];
            i = 0;
            i_end = 0;
        }

        public string GetWordStem(string word)
        {
            char[] characters = word.ToCharArray();
            foreach (var character in characters)
            {
                add(character);
            }

            Stem();
            return new string(b, 0, i_end);
        }

        public string GetSentenceStem(string sentence)
        {
            var words = sentence.Split();
            string sentenceStem = "";
            foreach (var word in words)
            {
                char[] characters = word.ToCharArray();
                foreach (var character in characters)
                {
                    add(character);
                }

                Stem();
                sentenceStem += new string(b, 0, i_end) + " ";
                b = new char[INC];
                i = 0;
                i_end = 0;
            }
            return sentenceStem.TrimEnd();
        }

        private void add(char ch)
        {
            if (i == b.Length)
            {
                var new_b = new char[i + INC];
                for (int c = 0; c < i; c++)
                {
                    new_b[c] = b[c];
                }
                b = new_b;
            }
            b[i++] = ch;
        }


        /* cons(i) is true <=> b[i] is a consonant. */

        private bool Cons(int i)
        {
            switch (b[i])
            {
                case 'a':
                case 'e':
                case 'i':
                case 'o':
                case 'u':
                    return false;
                case 'y':
                    return (i == 0) || !Cons(i - 1);
                default:
                    return true;
            }
        }

        /* m() measures the number of consonant sequences between 0 and j. if c is
         a consonant sequence and v a vowel sequence, and <..> indicates arbitrary
         presence,
              <c><v>       gives 0
              <c>vc<v>     gives 1
              <c>vcvc<v>   gives 2
              <c>vcvcvc<v> gives 3
          ....      */    
        private int M()
        {
            int n = 0;
            int i = 0;
            while (true)
            {
                if (i > j)
                {
                    return n;
                }
                if (!Cons(i))
                {
                    break;
                }
                i++;
            }
            i++;
            while (true)
            {
                while (true)
                {
                    if (i > j)
                    {
                        return n;
                    }
                    if (Cons(i))
                    {
                        break;
                    }
                    i++;
                }
                i++;
                n++;
                while (true)
                {
                    if (i > j)
                    {
                        return n;
                    }
                    if (!Cons(i))
                    {
                        break;
                    }
                    i++;
                }
                i++;
            }
        }

        /* vowelinstem() is true <=> 0,...j contains a vowel */
        private bool VowelInStem()
        {
            int i;
            for (i = 0; i <= j; i++)
            {
                if (!Cons(i))
                {
                    return true;
                }
            }
            return false;
        }

        /* doublec(j) is true <=> j,(j-1) contain a double consonant. */
        private bool doublec(int j)
        {
            if (j < 1)
            {
                return false;
            }
            if (b[j] != b[j - 1])
            {
                return false;
            }
            return Cons(j);
        }

        /* cvc(i) is true <=> i-2,i-1,i has the form consonant - vowel - consonant
        and also if the second c is not w,x or y. this is used when trying to
        restore an e at the end of a short word. e.g.
        cav(e), lov(e), hop(e), crim(e), but
        snow, box, tray.*/
        private bool cvc(int i)
        {
            if (i < 2 || !Cons(i) || Cons(i - 1) || !Cons(i - 2))
            {
                return false;
            }
            {
                int ch = b[i];
                if (ch == 'w' || ch == 'x' || ch == 'y')
                {
                    return false;
                }
            }
            return true;
        }

        private bool ends(string s)
        {
            int l = s.length();
            int o = k - l + 1;
            if (o < 0)
            {
                return false;
            }
            for (int i = 0; i < l; i++)
            {
                if (b[o + i] != s.charAt(i))
                {
                    return false;
                }
            }
            j = k - l;
            return true;
        }

        /* setto(s) sets (j+1),...k to the characters in the string s, readjusting  k. */
        private void SetTo(string s)
        {
            int l = s.length();
            int o = j + 1;
            for (int i = 0; i < l; i++)
            {
                b[o + i] = s.charAt(i);
            }
            k = j + l;
        }

        /* r(s) is used further down. */
        private void R(string s)
        {
            if (M() > 0)
            {
                SetTo(s);
            }
        }

        /* step1() gets rid of plurals and -ed or -ing. e.g.
         caresses  ->  caress
         ponies    ->  poni
         ties      ->  ti
         caress    ->  caress
         cats      ->  cat
         feed      ->  feed
         agreed    ->  agree
         disabled  ->  disable
         matting   ->  mat
         mating    ->  mate
         meeting   ->  meet
         milling   ->  mill
         messing   ->  mess
         meetings  ->  meet
  */
        private void Step1()
        {
            if (b[k] == 's')
            {
                if (ends("sses"))
                {
                    k -= 2;
                }
                else if (ends("ies"))
                {
                    SetTo("i");
                }
                else if (b[k - 1] != 's')
                {
                    k--;
                }
            }
            if (ends("eed"))
            {
                if (M() > 0)
                {
                    k--;
                }
            }
            else if ((ends("ed") || ends("ing")) && VowelInStem())
            {
                k = j;
                if (ends("at"))
                {
                    SetTo("ate");
                }
                else if (ends("bl"))
                {
                    SetTo("ble");
                }
                else if (ends("iz"))
                {
                    SetTo("ize");
                }
                else if (doublec(k))
                {
                    k--;
                    {
                        int ch = b[k];
                        if (ch == 'l' || ch == 's' || ch == 'z')
                        {
                            k++;
                        }
                    }
                }
                else if (M() == 1 && cvc(k))
                {
                    SetTo("e");
                }
            }
        }

        /* step2() turns terminal y to i when there is another vowel in the stem. */
        private void Step2()
        {
            if (ends("y") && VowelInStem())
            {
                b[k] = 'i';
            }
        }

        /* step3() maps double suffices to single ones. so -ization ( = -ize plus
           -ation) maps to -ize etc. note that the string before the suffix must give
            m() > 0. */
        private void Step3()
        {
            if (k == 0)
            {
                return; /* For Bug 1 */
            }
            switch (b[k - 1])
            {
                case 'a':
                    if (ends("ational"))
                    {
                        R("ate");
                        break;
                    }
                    if (ends("tional"))
                    {
                        R("tion");
                        break;
                    }
                    break;
                case 'c':
                    if (ends("enci"))
                    {
                        R("ence");
                        break;
                    }
                    if (ends("anci"))
                    {
                        R("ance");
                        break;
                    }
                    break;
                case 'e':
                    if (ends("izer"))
                    {
                        R("ize");
                        break;
                    }
                    break;
                case 'l':
                    if (ends("bli"))
                    {
                        R("ble");
                        break;
                    }
                    if (ends("alli"))
                    {
                        R("al");
                        break;
                    }
                    if (ends("entli"))
                    {
                        R("ent");
                        break;
                    }
                    if (ends("eli"))
                    {
                        R("e");
                        break;
                    }
                    if (ends("ousli"))
                    {
                        R("ous");
                        break;
                    }
                    break;
                case 'o':
                    if (ends("ization"))
                    {
                        R("ize");
                        break;
                    }
                    if (ends("ation"))
                    {
                        R("ate");
                        break;
                    }
                    if (ends("ator"))
                    {
                        R("ate");
                        break;
                    }
                    break;
                case 's':
                    if (ends("alism"))
                    {
                        R("al");
                        break;
                    }
                    if (ends("iveness"))
                    {
                        R("ive");
                        break;
                    }
                    if (ends("fulness"))
                    {
                        R("ful");
                        break;
                    }
                    if (ends("ousness"))
                    {
                        R("ous");
                        break;
                    }
                    break;
                case 't':
                    if (ends("aliti"))
                    {
                        R("al");
                        break;
                    }
                    if (ends("iviti"))
                    {
                        R("ive");
                        break;
                    }
                    if (ends("biliti"))
                    {
                        R("ble");
                        break;
                    }
                    break;
                case 'g':
                    if (ends("logi"))
                    {
                        R("log");
                        break;
                    }
                    break;
            }
        }

        /* step4() deals with -ic-, -full, -ness etc. similar strategy to step3. */
        private void Step4()
        {
            switch (b[k])
            {
                case 'e':
                    if (ends("icate"))
                    {
                        R("ic");
                        break;
                    }
                    if (ends("ative"))
                    {
                        R("");
                        break;
                    }
                    if (ends("alize"))
                    {
                        R("al");
                        break;
                    }
                    break;
                case 'i':
                    if (ends("iciti"))
                    {
                        R("ic");
                        break;
                    }
                    break;
                case 'l':
                    if (ends("ical"))
                    {
                        R("ic");
                        break;
                    }
                    if (ends("ful"))
                    {
                        R("");
                        break;
                    }
                    break;
                case 's':
                    if (ends("ness"))
                    {
                        R("");
                        break;
                    }
                    break;
            }
        }

        /* step5() takes off -ant, -ence etc., in context <c>vcvc<v>. */
        private void Step5()
        {
            if (k == 0)
            {
                return; /* for Bug 1 */
            }
            switch (b[k - 1])
            {
                case 'a':
                    if (ends("al"))
                    {
                        break;
                    }
                    return;
                case 'c':
                    if (ends("ance"))
                    {
                        break;
                    }
                    if (ends("ence"))
                    {
                        break;
                    }
                    return;
                case 'e':
                    if (ends("er"))
                    {
                        break;
                    }
                    return;
                case 'i':
                    if (ends("ic"))
                    {
                        break;
                    }
                    return;
                case 'l':
                    if (ends("able"))
                    {
                        break;
                    }
                    if (ends("ible"))
                    {
                        break;
                    }
                    return;
                case 'n':
                    if (ends("ant"))
                    {
                        break;
                    }
                    if (ends("ement"))
                    {
                        break;
                    }
                    if (ends("ment"))
                    {
                        break;
                    }
                    /* element etc. not stripped before the m */
                    if (ends("ent"))
                    {
                        break;
                    }
                    return;
                case 'o':
                    if (ends("ion") && j >= 0 && (b[j] == 's' || b[j] == 't'))
                    {
                        break;
                    }
                    /* j >= 0 fixes Bug 2 */
                    if (ends("ou"))
                    {
                        break;
                    }
                    return;
                    /* takes care of -ous */
                case 's':
                    if (ends("ism"))
                    {
                        break;
                    }
                    return;
                case 't':
                    if (ends("ate"))
                    {
                        break;
                    }
                    if (ends("iti"))
                    {
                        break;
                    }
                    return;
                case 'u':
                    if (ends("ous"))
                    {
                        break;
                    }
                    return;
                case 'v':
                    if (ends("ive"))
                    {
                        break;
                    }
                    return;
                case 'z':
                    if (ends("ize"))
                    {
                        break;
                    }
                    return;
                default:
                    return;
            }
            if (M() > 1)
            {
                k = j;
            }
        }

        /* step6() removes a final -e if m() > 1. */
        private void Step6()
        {
            j = k;
            if (b[k] == 'e')
            {
                int a = M();
                if (a > 1 || a == 1 && !cvc(k - 1))
                {
                    k--;
                }
            }
            if (b[k] == 'l' && doublec(k) && M() > 1)
            {
                k--;
            }
        }

        /**
         * Stem the word placed into the Stemmer buffer through calls to add().
         * Returns true if the stemming process resulted in a word different
         * from the input.  You can retrieve the result with
         * getResultLength()/getResultBuffer() or toString().
         */
        private void Stem()
        {
            k = i - 1;
            if (k > 1)
            {
                Step1();
                Step2();
                Step3();
                Step4();
                Step5();
                Step6();
            }
            i_end = k + 1;
            i = 0;
        }
    }
}
