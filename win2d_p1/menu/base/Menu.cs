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
    abstract class Menu : IDrawableUpdatable {
        protected Vector2 _position;
        protected double _width;
        protected double _height;
        protected Rect _rect;
        protected Color _backgroundColor;

        protected static float _defaultPadding = 10.0f;
        protected static float _borderRadiusX = 5.0f;
        protected static float _borderRadiusY = 5.0f;
        protected static float _borderStrokeWidth = 5.0f;
        protected static Color _borderColor = Colors.White;
        protected static Color _selectedItemColor = Colors.Red;
        protected static Color _unselectedItemColor = Colors.White;

        public Menu(Vector2 position, double width, double height, Color? backgroundColor = null) {
            _position = position;
            _width = width;
            _height = height;
            _rect = new Rect(_position.X, _position.Y, _width, _height);
            _backgroundColor = backgroundColor.HasValue ? backgroundColor.Value : Colors.Blue;   
        }

        public virtual void Draw(CanvasAnimatedDrawEventArgs args) {
            DrawBackground(args);
            DrawBorder(args);
        }

        private void DrawBackground(CanvasAnimatedDrawEventArgs args) {
            args.DrawingSession.FillRectangle(_rect, _backgroundColor);
        }

        private void DrawBorder(CanvasAnimatedDrawEventArgs args) {
            args.DrawingSession.DrawRoundedRectangle(_rect, _borderRadiusX, _borderRadiusY, _borderColor, _borderStrokeWidth);
        }

        public abstract void KeyDown(VirtualKey vk);
        public void Update(CanvasAnimatedUpdateEventArgs args) { }
    }
}
