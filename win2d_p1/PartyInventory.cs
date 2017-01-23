using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace win2d_p1 {
    class PartyInventory {
        public List<PartyInventorySlot> Slots = new List<PartyInventorySlot>();

        public void Add(PartyInventorySlot slotToAdd) {
            foreach(PartyInventorySlot slot in Slots) {
                if(slot.Item == slotToAdd.Item) {
                    slot.Count += slotToAdd.Count;
                    return;
                }
            }

            Slots.Add(slotToAdd);
        }
    }
}
