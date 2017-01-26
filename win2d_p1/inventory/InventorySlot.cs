using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace win2d_p1 {
    class InventorySlot {
        private CanvasDevice _device;

        private Item _item;
        public Item Item {
            get { return _item; }
            set { _item = value; RefreshTextLayout(); }
        }

        private int _count;
        public int Count {
            get { return _count; }
            set { _count = value; RefreshTextLayout(); }
        }

        private CanvasTextLayout _text;
        public CanvasTextLayout Text { get { return _text; } }

        public InventorySlot(CanvasDevice device, Item item, int count = 1) {
            _device = device;
            _item = item;
            _count = count;
            RefreshTextLayout();
        }

        private void RefreshTextLayout() {
            _text = new CanvasTextLayout(_device, Item.Name + " " + Count.ToString(), Font.Calibri14, 0, 0);
        }

        public void Draw(CanvasAnimatedDrawEventArgs args, Vector2 position) {
            args.DrawingSession.DrawTextLayout(_text, position, Colors.White);
        }

        public void DrawSelected(CanvasAnimatedDrawEventArgs args, Vector2 position) {
            args.DrawingSession.DrawTextLayout(_text, position,Colors.Red);
        }
    }
}
