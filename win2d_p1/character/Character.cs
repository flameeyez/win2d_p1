using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace win2d_p1 {
    class Character {
        public string Name { get; set; }
        public Attributes Attributes { get; set; }

        public Character(string name) {
            Name = name;
        }
    }
}
