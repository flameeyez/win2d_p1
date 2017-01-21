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
    enum GAME_STATE {
        GAME,
        MENU
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page {
        GAME_STATE CurrentGameState = GAME_STATE.GAME;

        Map map;
        Menu menu;
        int mapRows = 1 + 1080 / Map.TileSizeInPixels;
        int mapColumns = 1 + 1920 / Map.TileSizeInPixels;

        long DebugDrawTimeMilliseconds;

        double mouseX;
        double mouseY;

        bool bCreateNewMapOnNextUpdate = false;

        public MainPage() {
            this.InitializeComponent();

            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
        }

        private void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args) {
            switch(CurrentGameState) {
                case GAME_STATE.GAME:
                    switch(args.VirtualKey) {
                        case Windows.System.VirtualKey.Space:
                            bCreateNewMapOnNextUpdate = true;
                            break;
                        case Windows.System.VirtualKey.Escape:
                            Application.Current.Exit();
                            break;
                        case Windows.System.VirtualKey.M:
                            CurrentGameState = GAME_STATE.MENU;
                            break;
                    }
                    break;
                case GAME_STATE.MENU:
                    switch(args.VirtualKey) {
                        case Windows.System.VirtualKey.M:
                        case Windows.System.VirtualKey.Escape:
                            CurrentGameState = GAME_STATE.GAME;
                            break;
                        default:
                            menu.KeyDown(args.VirtualKey);
                            break;
                    }
                    break;
            }
        }

        private void canvasMain_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args) {
            Stopwatch s = Stopwatch.StartNew();
            map.Draw(args);
            s.Stop();
            DebugDrawTimeMilliseconds = s.ElapsedMilliseconds;

            switch(CurrentGameState) {
                case GAME_STATE.MENU:
                    menu.Draw(args);
                    break;
            }

            DrawDebug(args);
        }

        private void DrawDebug(CanvasAnimatedDrawEventArgs args) {
            List<string> DebugStrings = new List<string>();

            DebugStrings.Add("Mouse: " + ((int)mouseX).ToString() + ", " + ((int)mouseY).ToString());
            DebugStrings.Add("Creation time: " + map.DebugCreationTimeMilliseconds.ToString() + "ms");
            DebugStrings.Add("Draw time: " + DebugDrawTimeMilliseconds.ToString() + "ms");
            DebugStrings.Add("Map dimensions: " + map.Tiles.GetLength(1) + ", " + map.Tiles.GetLength(0));

            int mapTileX = (int)(mouseX / Map.TileSizeInPixels);
            int mapTileY = (int)(mouseY / Map.TileSizeInPixels);
            if(mapTileX >= 0 && mapTileX < map.Tiles.GetLength(1) && mapTileY >= 0 && mapTileY < map.Tiles.GetLength(0)) {
                DebugStrings.Add("Map coordinates: " + mapTileX.ToString() + ", " + mapTileY.ToString());
                DebugStrings.Add("Elevation: " + map.Tiles[mapTileY, mapTileX].Elevation.ToString());
                args.DrawingSession.DrawRectangle(new Rect(mapTileX * Map.TileSizeInPixels, mapTileY * Map.TileSizeInPixels, Map.TileSizeInPixels, Map.TileSizeInPixels), Colors.Red);
            }

            float y = 10.0f;
            foreach(string str in DebugStrings) {
                args.DrawingSession.DrawText(str, new Vector2(1500, y), Colors.White);
                y += 20.0f;
            }

        }

        private void canvasMain_Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args) {
            if(bCreateNewMapOnNextUpdate) {
                map = new Map(device: canvasMain.Device, rows: mapRows, columns: mapColumns);
                bCreateNewMapOnNextUpdate = false;
            }
        }

        private void canvasMain_CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args) {
            args.TrackAsyncAction(CreateResourcesAsync(sender).AsAsyncAction());
        }

        private async Task CreateResourcesAsync(CanvasAnimatedControl sender) {
            Images.Ocean = await CanvasBitmap.LoadAsync(sender, "images\\ocean.png");
            Images.Mountains = await CanvasBitmap.LoadAsync(sender, "images\\mountains.png");
            Images.Desert = await CanvasBitmap.LoadAsync(sender, "images\\desert.png");
            Images.Grass = await CanvasBitmap.LoadAsync(sender, "images\\grass.png");
            map = new Map(device: sender.Device, rows: mapRows, columns: mapColumns);

            float fMenuWidth = 1200.0f;
            float fMenuHeight = 800.0f;
            Vector2 menuPosition = new Vector2((1920 - fMenuWidth) * 0.5f, (1080 - fMenuHeight) * 0.5f);
            menu = new Menu(menuPosition, fMenuWidth, fMenuHeight);
            menu.Items.Add(new MenuItem("Test 1"));
            menu.Items.Add(new MenuItem("Test 2"));

            MenuItem m3 = new MenuItem("Test 3");
            m3.Event += M3_Event;
            menu.Items.Add(m3);
        }

        private void M3_Event() {
            menu.Items.Add(new MenuItem("Event!"));
        }

        private void canvasMain_PointerMoved(object sender, PointerRoutedEventArgs e) {
            PointerPoint p = e.GetCurrentPoint(canvasMain);
            mouseX = p.Position.X;
            mouseY = p.Position.Y;
        }
    }
}
