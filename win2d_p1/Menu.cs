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
    class Menu {
        public List<MenuItem> Items = new List<MenuItem>();
        private int nSelectedItem;

        private Vector2 _position;
        private double _width;
        private double _height;
        private Rect _rect;
        private Color _backgroundColor;
        private Vector2 _stringsPosition;

        private static float _defaultPadding = 10.0f;
        private static float _borderRadiusX = 5.0f;
        private static float _borderRadiusY = 5.0f;
        private static float _borderStrokeWidth = 5.0f;
        private static Color _borderColor = Colors.White;
        private static Color _selectedItemColor = Colors.Red;
        private static Color _unselectedItemColor = Colors.White;

        public Menu(Vector2 position, double width, double height, Color? backgroundColor = null) {
            _position = position;
            _width = width;
            _height = height;
            _rect = new Rect(_position.X, _position.Y, _width, _height);
            _backgroundColor = backgroundColor.HasValue ? backgroundColor.Value : Colors.Blue;
            _stringsPosition = new Vector2(_position.X + _defaultPadding, _position.Y + _defaultPadding);
        }

        public void Draw(CanvasAnimatedDrawEventArgs args) {
            DrawBackground(args);
            DrawBorder(args);
            DrawStrings(args);
        }

        private void DrawBackground(CanvasAnimatedDrawEventArgs args) {
            args.DrawingSession.FillRectangle(_rect, _backgroundColor);
        }

        private void DrawBorder(CanvasAnimatedDrawEventArgs args) {
            args.DrawingSession.DrawRoundedRectangle(_rect, _borderRadiusX, _borderRadiusY, _borderColor, _borderStrokeWidth);
        }

        private void DrawStrings(CanvasAnimatedDrawEventArgs args) {
            float y = _stringsPosition.Y;
            for(int i = 0; i < Items.Count; i++) {
                args.DrawingSession.DrawText(Items[i].Text, new Vector2(_stringsPosition.X, y), i == nSelectedItem ? _selectedItemColor : _unselectedItemColor);
                y += 20.0f + _defaultPadding;
            }
        }

        public void KeyDown(VirtualKey vk) {
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
