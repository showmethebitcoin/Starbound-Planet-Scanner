using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starbound_Planet_Tagger
{
    class PixelRule 
    {
        public int offsetX;
        public int offsetY;
        protected int ArgColor;

        public PixelRule(int oX, int oY, int pColor)
        {
            offsetX = oX;
            offsetY = oY;

      
            ArgColor = pColor;
        }

        public virtual bool Assert(Viewport SourceImage) {
            return false;
        }
    }

   
    class MatchExactColor : PixelRule
    {
        public MatchExactColor(int oX, int oY, int pColor):base(oX, oY, pColor)
        {

        }

        public override bool Assert(Viewport SourceImage)
        {
          
            return SourceImage.GetPixel(offsetX, offsetY).ToArgb().Equals(ArgColor);
        }
    }

    class DoNotMatchColor : PixelRule
    {

        public DoNotMatchColor(int oX, int oY, int pColor)
            : base(oX, oY, pColor)
        {

        }

        public override bool Assert(Viewport SourceImage)
        {
            
            return !SourceImage.GetPixel(offsetX, offsetY).ToArgb().Equals(ArgColor);
        }
    }
}
