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
using Windows.System;
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
        MENU_MAIN,
        MENU_PARTY_INVENTORY,
        MENU_APPLY_ITEM_TO_PARTY_MEMBER
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page {
        GAME_STATE CurrentGameState = GAME_STATE.GAME;

        Map map;

        MenuMain menuMain;
        MenuPartyInventory menuPartyInventory;
        MenuApplyItemToPartyMember menuApplyItemToPartyMember;

        List<IDrawableUpdatable> drawList = new List<IDrawableUpdatable>();
        object drawListLock = new object();
        Party party;
        Character character1;
        Character character2;
        Character character3;
        Character character4;
        Character character5;

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
            args.Handled = true;
            var virtualKey = args.VirtualKey;
            var action = canvasMain.RunOnGameLoopThreadAsync(() => KeyDown_GameLoopThread(virtualKey));
        }

        private void KeyDown_GameLoopThread(VirtualKey virtualKey) {
            switch(CurrentGameState) {
                case GAME_STATE.GAME:
                    switch(virtualKey) {
                        case VirtualKey.Space:
                            bCreateNewMapOnNextUpdate = true;
                            break;
                        case VirtualKey.Escape:
                            Application.Current.Exit();
                            break;
                        case VirtualKey.M:
                            CurrentGameState = GAME_STATE.MENU_MAIN;
                            drawList.Add(menuMain);
                            break;
                    }
                    break;
                case GAME_STATE.MENU_MAIN:
                    switch(virtualKey) {
                        case VirtualKey.M:
                        case VirtualKey.Escape:
                            CurrentGameState = GAME_STATE.GAME;
                            drawList.Remove(menuMain);
                            break;
                        default:
                            menuMain.KeyDown(virtualKey);
                            break;
                    }
                    break;
                case GAME_STATE.MENU_PARTY_INVENTORY:
                    switch(virtualKey) {
                        case VirtualKey.Escape:
                            CurrentGameState = GAME_STATE.MENU_MAIN;
                            drawList.Remove(menuPartyInventory);
                            break;
                        default:
                            menuPartyInventory.KeyDown(virtualKey);
                            break;
                    }
                    break;
                case GAME_STATE.MENU_APPLY_ITEM_TO_PARTY_MEMBER:
                    //switch(virtualKey) {
                    //    case VirtualKey.Escape:
                    //        CurrentGameState = GAME_STATE.MENU_ITEM;
                    //        drawList.Remove(menuParty);
                    //        break;
                    //    default:
                    //        menuParty.KeyDown(virtualKey);
                    //        break;
                    //}
                    break;

            }
        }

        private void canvasMain_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args) {
            Stopwatch s = Stopwatch.StartNew();
            lock(drawListLock) {
                foreach(IDrawableUpdatable idu in drawList) {
                    idu.Draw(args);
                }
            }
            s.Stop();
            DebugDrawTimeMilliseconds = s.ElapsedMilliseconds;

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
            drawList.Add(map);

            float fMenuWidth = 1200.0f;
            float fMenuHeight = 800.0f;
            Vector2 menuPosition = new Vector2((1920 - fMenuWidth) * 0.5f, (1080 - fMenuHeight) * 0.5f);
            menuMain = new MenuMain(menuPosition, fMenuWidth, fMenuHeight);
            menuMain.Items.Add(new MenuItem("Test 1"));
            menuMain.Items.Add(new MenuItem("Test 2"));

            MenuItem m3 = new MenuItem("Items");
            m3.Event += M3_Event;
            menuMain.Items.Add(m3);

            // create mock party and inventory
            party = new Party();
            character1 = new Character("Jerb");
            character2 = new Character("Cecilia");
            character3 = new Character("Branzolo");
            character4 = new Character("Joffin");
            character5 = new Character("Segbag");

            PartyInventory partyInventory = new PartyInventory();
            // TODO: maintain master list of item references
            // TODO: potions restore some amount of health
            Item item1 = new Item();
            item1.Name = "Potion";
            partyInventory.Add(item1, 5);
            partyInventory.Add(item1, 2);
            Item item2 = new Item();
            item2.Name = "Potion";
            partyInventory.Add(item2, 3);
            party.Inventory = partyInventory;

            fMenuWidth = 1100.0f;
            fMenuHeight = 700.0f;
            menuPosition = new Vector2((1920 - fMenuWidth) * 0.5f, (1080 - fMenuHeight) * 0.5f);
            menuPartyInventory = new MenuPartyInventory(party.Inventory, menuPosition, fMenuWidth, fMenuHeight, Colors.Green);

            //MenuItem m4 = new MenuItem("Potion");
            //m4.Event += M4_Event;
            //menuItem.Items.Add(m4);
            //menuItem.Items.Add(new MenuItem("Test 5"));

            //fMenuWidth = 1000.0f;
            //fMenuHeight = 600.0f;
            //menuPosition = new Vector2((1920 - fMenuWidth) * 0.5f, (1080 - fMenuHeight) * 0.5f);
            //menuParty = new Menu(menuPosition, fMenuWidth, fMenuHeight, Colors.Purple);
            //menuParty.Items.Add(new MenuItem("Test 6"));
            //menuParty.Items.Add(new MenuItem("Test 7"));
        }

        //private void M4_Event() {
        //    CurrentGameState = GAME_STATE.MENU_PARTY;
        //    lock(drawListLock) {
        //        drawList.Add(menuParty);
        //    }
        //}

        private void M3_Event() {
            CurrentGameState = GAME_STATE.MENU_PARTY_INVENTORY;
            lock(drawListLock) {
                drawList.Add(menuPartyInventory);
            }            
            //menu.Items.Add(new MenuItem("Event!"));
        }

        private void canvasMain_PointerMoved(object sender, PointerRoutedEventArgs e) {
            PointerPoint p = e.GetCurrentPoint(canvasMain);
            mouseX = p.Position.X;
            mouseY = p.Position.Y;
        }
    }
}
