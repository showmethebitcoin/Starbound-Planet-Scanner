using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starbound_Planet_Tagger
{
    class SBSymbol
    {
        public string TextConversion;
        List<PixelRule> Rules;
        public Color RuleColor;
        public int Width;

        public SBSymbol(string SymbolPath, Color SymbolColor, string SymbolText)
        {
            var B = new Bitmap(SymbolPath);
            var skip = true;
            var offsetX = 0;
            var offsetY = 0;

          

            RuleColor = SymbolColor;

            if (SymbolText.Length == 2)
            {
                SymbolText = SymbolText.Substring(1);
            }

          
          

            switch (SymbolText)
            {
                case "dash": SymbolText = "-";
                    break;
                case "colon": SymbolText = ":";
                    break;
                case "period": SymbolText = ".";
                    break;
                case "semicolon": SymbolText = ";";
                    break;
                case "apostrophe": SymbolText = "'";
                    break;
                case "exclamation": SymbolText = "!";
                    break;
            }

            TextConversion = SymbolText;

          
            var Min = 0;
            var Max = 0;

            Rules = new List<PixelRule>();

            for (int y = 0; y < B.Height; y++)
            {
                for (int x = 0; x < B.Width; x++)
                {
                    var PixR = B.GetPixel(x, y);
                    var Pix = PixR.ToArgb();
                    if (skip && (PixR.A == 0))
                    {
                        // Skip any leading transparent pixels
                        continue;
                    }
                    else if (skip)
                    {
                        skip = false;
                        offsetX = x;
                        offsetY = y;
                    }

                    if (Pix.Equals(Color.Magenta.ToArgb()))
                    {
                        Rules.Add(new DoNotMatchColor( x - offsetX,  y - offsetY,RuleColor.ToArgb() ));

                    }
                    else if (Pix.Equals(Color.Black.ToArgb()))
                    {
                        Rules.Add(new MatchExactColor( x - offsetX, y - offsetY,  RuleColor.ToArgb()));
                    } else {
                        // Match the same pixel color as the source image if it's not nothing
                        if (PixR.A != 0)
                            Rules.Add(new MatchExactColor(x - offsetX, y - offsetY, Pix));
                    }

                    Min = Math.Min(x - offsetX, Min);
                    Max = Math.Max(x - offsetX, Max);

                }
            }

            // Normalize values so the character starts in the upper left of the box
            if (Min < 0)
            {
                for (int i = 0; i < Rules.Count; i++)
                {
                    Rules[i].offsetX += -1 * Min;
                }

                Max += -1 * Min;
            }

            Width = Max - 1;

        }

        public bool IsMatch(Viewport Test) {
          
            foreach (var Rule in Rules) {
                
                if (!Rule.Assert(Test))
                    return false;
              
            }

            return true;
        }



    }
}
