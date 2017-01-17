using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace win2d_p1 {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page {
        Map map;
        int mapRows = 1 + 1080 / Map.TileSize;// 200;
        int mapColumns = 1 + 1920 / Map.TileSize; //400;

        long DebugDrawTimeMilliseconds;

        double mouseX;
        double mouseY;

        public MainPage() {
            this.InitializeComponent();

            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
        }

        private void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args) {
            switch(args.VirtualKey) {
                case Windows.System.VirtualKey.Space:
                    map = new Map(rows: mapRows, columns: mapColumns);
                    break;
            }
        }

        private void canvasMain_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args) {
            Stopwatch s = Stopwatch.StartNew();
            map.Draw(args);
            s.Stop();
            DebugDrawTimeMilliseconds = s.ElapsedMilliseconds;

            DrawDebug(args);
        }

        private void DrawDebug(CanvasAnimatedDrawEventArgs args) {
            args.DrawingSession.DrawText("Mouse: " + ((int)mouseX).ToString() + ", " + ((int)mouseY).ToString(), new Vector2(1500, 10), Colors.White);
            args.DrawingSession.DrawText("Creation time: " + map.DebugCreationTimeMilliseconds.ToString() + "ms", new Vector2(1500, 30), Colors.White);
            args.DrawingSession.DrawText("Draw time: " + DebugDrawTimeMilliseconds.ToString() + "ms", new Vector2(1500, 50), Colors.White);
            int mapTileX = (int)(mouseX / Map.TileSize);
            int mapTileY = (int)(mouseY / Map.TileSize);
            if(mapTileX > 0 && mapTileX < map.Tiles.GetLength(1) && mapTileY > 0 && mapTileY < map.Tiles.GetLength(0)) {
                args.DrawingSession.DrawText("Elevation: " + map.Tiles[mapTileY, mapTileX].Elevation.ToString(), new System.Numerics.Vector2(1500, 70), Colors.White);
            }
        }

        private void canvasMain_Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args) {

        }

        private void canvasMain_CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args) {
            map = new Map(rows: mapRows, columns: mapColumns);
            args.TrackAsyncAction(CreateResourcesAsync(sender).AsAsyncAction());
        }

        private async Task CreateResourcesAsync(CanvasAnimatedControl sender) {
            Images.Ocean = await CanvasBitmap.LoadAsync(sender, "images\\ocean.png");
            Images.Mountains = await CanvasBitmap.LoadAsync(sender, "images\\mountains.png");
            Images.Desert = await CanvasBitmap.LoadAsync(sender, "images\\desert.png");
            Images.Grass = await CanvasBitmap.LoadAsync(sender, "images\\grass.png");
        }

        private void canvasMain_PointerMoved(object sender, PointerRoutedEventArgs e) {
            PointerPoint p = e.GetCurrentPoint(canvasMain);
            mouseX = p.Position.X;
            mouseY = p.Position.Y;
        }
    }
}
