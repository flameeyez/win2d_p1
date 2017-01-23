using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Windows.System;
using Windows.UI;
using System.Diagnostics;
using Microsoft.Graphics.Canvas.Text;

namespace win2d_p1 {
    class MenuPartyInventory : Menu {
        private int nSelectedItem;
        private static int nItemsPerRow = 4;
        private Vector2 _stringsPosition;
        public PartyInventory PartyInventory { get; set; }

        public MenuPartyInventory(PartyInventory partyInventory, Vector2 position, double width, double height, Color? backgroundColor = default(Color?)) : base(position, width, height, backgroundColor) {
            _stringsPosition = new Vector2(_position.X + _defaultPadding, _position.Y + _defaultPadding);
            PartyInventory = partyInventory;
        }

        public override void Draw(CanvasAnimatedDrawEventArgs args) {
            base.Draw(args);
            DrawPartyInventory(args);
        }

        private void DrawPartyInventory(CanvasAnimatedDrawEventArgs args) {
            Stopwatch s1 = Stopwatch.StartNew();
            float x;
            float y = _stringsPosition.Y;
            for(int i = 0; i < PartyInventory.Slots.Count; i++) {
                x = _stringsPosition.X + (i % nItemsPerRow) * (float)_width / nItemsPerRow;

                if(i == nSelectedItem) {
                    PartyInventory.Slots[i].DrawSelected(args, new Vector2(x, y));
                }
                else {
                    PartyInventory.Slots[i].Draw(args, new Vector2(x, y));
                }

                if((i + 1) % nItemsPerRow == 0) {
                    y += 20.0f + _defaultPadding;
                }
            }
            s1.Stop();

            // draw debug timing
            CanvasTextLayout text = new CanvasTextLayout(args.DrawingSession.Device, s1.ElapsedMilliseconds.ToString() + "ms", Font.Calibri14, 0, 0);
            args.DrawingSession.DrawTextLayout(text, new Vector2(_position.X + (float)_width - (float)text.LayoutBounds.Width - _defaultPadding, _position.Y + _defaultPadding), Colors.White);

            // deduce number of items that can fit into a column
            // measure sample string
            // drawable height / sample string height + padding
            // add arbitrarily large x
        }

        public override void KeyDown(VirtualKey vk) {
            switch(vk) {
                case VirtualKey.Down:
                    nSelectedItem += nItemsPerRow;
                    if(nSelectedItem >= PartyInventory.Slots.Count) { nSelectedItem -= PartyInventory.Slots.Count; }
                    break;
                case VirtualKey.Up:
                    nSelectedItem -= nItemsPerRow;
                    if(nSelectedItem < 0) { nSelectedItem += PartyInventory.Slots.Count; }
                    break;
                case VirtualKey.Right:
                    nSelectedItem = (nSelectedItem + 1) % PartyInventory.Slots.Count;
                    break;
                case VirtualKey.Left:
                    nSelectedItem--;
                    if(nSelectedItem < 0) { nSelectedItem += PartyInventory.Slots.Count; }
                    break;
                case VirtualKey.Enter:
                    // invoke menu item
                    //Items[nSelectedItem].InvokeEvent();
                    break;
            }
        }
    }
}
