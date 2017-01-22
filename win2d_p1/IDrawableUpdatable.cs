using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace win2d_p1 {
    interface IDrawableUpdatable {
        void Draw(CanvasAnimatedDrawEventArgs args);
        void Update(CanvasAnimatedUpdateEventArgs args);
    }
}
