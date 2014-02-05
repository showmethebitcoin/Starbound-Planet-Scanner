
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

        public SBSymbol(string SymbolPath, Color SymbolColor, string SymbolText, int Tolerance=0)
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


                    var update = false;
                    if (Pix.Equals(Color.Magenta.ToArgb()))
                    {
                        Rules.Add(new DoNotMatchColorWithinTolerance( x - offsetX,  y - offsetY,RuleColor.ToArgb(), Tolerance ));
                        update = true;
                    } else
                        if (Pix.Equals(Color.Lime.ToArgb()))
                        {
                            Rules.Add(new DoNotMatchColorWithinTolerance(x - offsetX, y - offsetY, RuleColor.ToArgb(), Tolerance));
                            update = true;
                        }
                    else if (Pix.Equals(Color.Black.ToArgb()))
                    {
                        Rules.Add(new MatchColorWithinTolerance(x - offsetX, y - offsetY, RuleColor.ToArgb(), Tolerance));
                        update = true;
                    } else {
                        // Match the same pixel color as the source image if it's not nothing
                        if (PixR.A != 0) { 
                            Rules.Add(new MatchColorWithinTolerance(x - offsetX, y - offsetY, Pix, Tolerance));
                            update = true;
                        }
                    }

                    if (update) { 
                        Min = Math.Min(x - offsetX, Min);
                    Max = Math.Max(x - offsetX, Max);
                    }
                    

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
            int r = 0;
            foreach (var Rule in Rules) {
                
                if (!Rule.Assert(Test))
                {

                    return false;
                }
                r++;
            }

            return true;
        }

        public void SetTolerance(int Tolerance)
        {
            foreach (var Rule in Rules)
            {

                Rule.GlobalTolerance = Tolerance;

            }
        }

        public void ScaleRules(int Factor)
        {
            foreach (var Rule in Rules)
            {

                Rule.offsetX *= Factor;
                Rule.offsetY *= Factor;

            }
        }



    }
}
