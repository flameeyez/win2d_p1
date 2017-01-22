using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI;

namespace win2d_p1 {
    class MenuMain : Menu {
        // ff-style; display party on left; options on right
        private Vector2 _stringsPosition;
        public List<MenuItem> Items = new List<MenuItem>();
        private int nSelectedItem;

        public MenuMain(Vector2 position, double width, double height, Color? backgroundColor = default(Color?)) : base(position, width, height, backgroundColor) {
            _stringsPosition = new Vector2(_position.X + _defaultPadding, _position.Y + _defaultPadding);
        }

        public override void Draw(CanvasAnimatedDrawEventArgs args) {
            base.Draw(args);
            DrawStrings(args);
        }

        private void DrawStrings(CanvasAnimatedDrawEventArgs args) {
            float y = _stringsPosition.Y;
            for(int i = 0; i < Items.Count; i++) {
                args.DrawingSession.DrawText(Items[i].Text, new Vector2(_stringsPosition.X, y), i == nSelectedItem ? _selectedItemColor : _unselectedItemColor);
                y += 20.0f + _defaultPadding;
            }
        }

        public override void KeyDown(VirtualKey vk) {
            switch(vk) {
                case VirtualKey.Down:
                    if(Items.Count > 0) {
                        nSelectedItem = (nSelectedItem + 1) % Items.Count;
                    }
                    break;
                case VirtualKey.Up:
                    nSelectedItem--;
                    if(nSelectedItem < 0) { nSelectedItem += Items.Count; }
                    break;
                case VirtualKey.Enter:
                    // invoke menu item
                    Items[nSelectedItem].InvokeEvent();
                    break;
            }
        }
    }
}
