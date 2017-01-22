using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI;

namespace win2d_p1 {
    class MenuApplyItemToPartyMember : Menu {
        public MenuApplyItemToPartyMember(Vector2 position, double width, double height, Color? backgroundColor = default(Color?)) : base(position, width, height, backgroundColor) {
        }

        public override void KeyDown(VirtualKey vk) {
            throw new NotImplementedException();
        }
    }
}
