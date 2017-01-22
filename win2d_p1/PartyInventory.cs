using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace win2d_p1 {
    class PartyInventory {
        Dictionary<Item, int> Items = new Dictionary<Item, int>();

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            foreach(KeyValuePair<Item, int> kvp in Items) {
                sb.Append(kvp.Key.Name);
                sb.Append(' ');
                sb.Append(kvp.Value);
                sb.Append("\r\n");
            }
            return sb.ToString();
        }

        public void Add(Item item, int count) {
            int existingCount;
            if(Items.TryGetValue(item, out existingCount)) {
                Items[item] += count;
            }
            else {
                Items.Add(item, count);
            }
        }
    }
}
