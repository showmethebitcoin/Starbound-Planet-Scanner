/*
Copyright (c) 2014 Nicholas Tantillo

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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starbound_Planet_Tagger
{
    class SBSymbolTable
    {
        List<SBSymbol> Symbols;

        Color MatchColor;

        public string Tag;

        public SBSymbolTable(Color SymbolColor, string pTag="Default")
        {
            Symbols = new List<SBSymbol>();
            MatchColor = SymbolColor;

            Tag = pTag;
        }

        public void SetTolerance(int Tolerance)
        {
            for (int i = 0; i < Symbols.Count; i++)
            {


                Symbols[i].SetTolerance(Tolerance);
                }
        }

        public void ScaleRules(int Factor)
        {// Does not work
            for (int i = 0; i < Symbols.Count; i++)
            {


                Symbols[i].ScaleRules(Factor);
            }
        }

        public void AddDir(string Dir, int Tolerance=0) {
            foreach (var F in Directory.GetFiles(Dir.Replace("~", Environment.CurrentDirectory))) {
                var Name = F.Substring(F.LastIndexOf("\\")+1, F.LastIndexOf(".") - F.LastIndexOf("\\") - 1);
                Symbols.Add(new SBSymbol(F, MatchColor, Name, Tolerance));
            }

            Tag = Dir;
        }

        public SBSymbol GetMatch(Viewport VP)
        {
            for (int i = 0; i < Symbols.Count; i++)
            {


                if (Symbols[i].IsMatch(VP))
                {
                    return Symbols[i];
                }
            }

            return null;
        }

    }
}
