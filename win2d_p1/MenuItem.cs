using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace win2d_p1 {
    public delegate void MenuItemEventHandler();

    class MenuItem {
        public string Text { get; set; }
        public event MenuItemEventHandler Event;

        public MenuItem(string text) {
            Text = text;
        }

        public void InvokeEvent() {
            Event?.Invoke();
        }
    }
}
