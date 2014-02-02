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

        public SBSymbolTable(Color SymbolColor)
        {
            Symbols = new List<SBSymbol>();
            MatchColor = SymbolColor;


        }

        public void AddDir(string Dir) {
            foreach (var F in Directory.GetFiles(Dir.Replace("~", Environment.CurrentDirectory))) {
                var Name = F.Substring(F.LastIndexOf("\\")+1, F.LastIndexOf(".") - F.LastIndexOf("\\") - 1);
                Symbols.Add(new SBSymbol(F, MatchColor, Name));
            }
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
