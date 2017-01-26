using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;

namespace win2d_p1 {
    static class Images {
        public static CanvasBitmap Ocean { get; internal set; }
        public static CanvasBitmap Mountains { get; internal set; }
        public static CanvasBitmap Desert { get; internal set; }
        public static CanvasBitmap Grass { get; internal set; }

        public static async Task Load(CanvasDevice device) {
            Ocean = await CanvasBitmap.LoadAsync(device, "images\\ocean.png");
            Mountains = await CanvasBitmap.LoadAsync(device, "images\\mountains.png");
            Desert = await CanvasBitmap.LoadAsync(device, "images\\desert.png");
            Grass = await CanvasBitmap.LoadAsync(device, "images\\grass.png");
        }
    }
}
