using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.System;
using Windows.UI;

namespace win2d_p1 {
    class MenuMain : Menu {
        // ff-style; display party on left; options on right
        private Vector2 _stringsPosition;
        public List<MenuItem> Items = new List<MenuItem>();
        private int nSelectedItem;

        public Party Party { get; set; }

        private Rect _leftPanelRect;
        private Rect _rightPanelRect;

        public MenuMain(Vector2 position, double width, double height, Color? backgroundColor = default(Color?)) : base(position, width, height, backgroundColor) {
            // left panel at 80%
            _leftPanelRect = new Rect(position.X, position.Y, width * 0.8, height);
            // right panel at 20%
            _rightPanelRect = new Rect(position.X + width * 0.8, position.Y, width * 0.2, height);
            // TODO: better centering of strings in right panel
            _stringsPosition = new Vector2((float)_rightPanelRect.X + _defaultPadding, (float)_rightPanelRect.Y + _defaultPadding);
        }

        public override void Draw(CanvasAnimatedDrawEventArgs args) {
            base.Draw(args);

            DrawLeftPanelBorder(args);
            DrawRightPanelBorder(args);
            DrawPartySummary(args);
            DrawStrings(args);
        }

        private void DrawLeftPanelBorder(CanvasAnimatedDrawEventArgs args) {
            args.DrawingSession.DrawRoundedRectangle(_leftPanelRect, _borderRadiusX, _borderRadiusY, _borderColor, _borderStrokeWidth);
        }

        private void DrawRightPanelBorder(CanvasAnimatedDrawEventArgs args) {
            args.DrawingSession.DrawRoundedRectangle(_rightPanelRect, _borderRadiusX, _borderRadiusY, _borderColor, _borderStrokeWidth);
        }

        private void DrawPartySummary(CanvasAnimatedDrawEventArgs args) {
            if(Party == null) { return; }
            foreach(Character c in Party.Characters) {

            }
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
