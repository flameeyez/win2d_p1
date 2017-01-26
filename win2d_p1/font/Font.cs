using Microsoft.Graphics.Canvas.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace win2d_p1 {
    static class Font {
        public static CanvasTextFormat Calibri12;
        public static CanvasTextFormat Calibri14;

        static Font() {
            Calibri12 = new CanvasTextFormat();
            Calibri12.FontFamily = "Calibri";
            Calibri12.FontSize = 12;
            Calibri12.WordWrapping = CanvasWordWrapping.NoWrap;

            Calibri14 = new CanvasTextFormat();
            Calibri14.FontFamily = "Calibri";
            Calibri14.FontSize = 12;
            Calibri14.WordWrapping = CanvasWordWrapping.NoWrap;
        }
    }
}
