using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Windows.System;
using Windows.UI;

namespace win2d_p1 {
    class MenuPartyInventory : Menu {
        public PartyInventory PartyInventory { get; set; }
        public MenuPartyInventory(PartyInventory partyInventory, Vector2 position, double width, double height, Color? backgroundColor = default(Color?)) : base(position, width, height, backgroundColor) {
            PartyInventory = partyInventory;
        }

        public override void Draw(CanvasAnimatedDrawEventArgs args) {
            base.Draw(args);
            DrawPartyInventory(args);
        }

        private void DrawPartyInventory(CanvasAnimatedDrawEventArgs args) {
            args.DrawingSession.DrawText(PartyInventory.ToString(), new Vector2(_position.X + _defaultPadding, _position.Y + _defaultPadding), _unselectedItemColor);
        }

        public override void KeyDown(VirtualKey vk) {

        }
    }
}
