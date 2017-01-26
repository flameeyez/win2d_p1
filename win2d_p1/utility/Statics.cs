using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace win2d_p1 {
    static class Statics {
        public static Random Random = new Random(DateTime.Now.Millisecond);
        public static T RandomValue<T>(this List<T> list) {
            return list[Random.Next(list.Count)];
        }
    }
}
