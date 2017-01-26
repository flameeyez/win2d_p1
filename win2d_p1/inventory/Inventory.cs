using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace win2d_p1 {
    class Inventory {
        public List<InventorySlot> Slots = new List<InventorySlot>();

        public void Add(InventorySlot slotToAdd) {
            foreach(InventorySlot slot in Slots) {
                if(slot.Item == slotToAdd.Item) {
                    slot.Count += slotToAdd.Count;
                    return;
                }
            }

            Slots.Add(slotToAdd);
        }
    }
}
