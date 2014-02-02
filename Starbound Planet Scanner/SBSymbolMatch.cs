using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starbound_Planet_Tagger
{
    class SBSymbolMatch
    {
        public int x;
        public int y;

        public SBSymbol MatchObject;

        public SBSymbolMatch(int px, int py, SBSymbol Found)
        {
            x = px;
            y = py;
            MatchObject = Found;

          
        }

    }
}
