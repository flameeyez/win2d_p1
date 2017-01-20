using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;

namespace win2d_p1 {
    class Menu {
        public List<string> Strings = new List<string>();

        public void Draw(CanvasAnimatedDrawEventArgs args) {
            DrawBackground(args);
            DrawBorder(args);
        }

        private void DrawBackground(CanvasAnimatedDrawEventArgs args) {
            args.DrawingSession.FillRectangle(new Rect(200, 200, 200, 200), Colors.Blue);
        }

        private void DrawBorder(CanvasAnimatedDrawEventArgs args) {

        }
    }
}
