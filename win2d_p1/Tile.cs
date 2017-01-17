using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace win2d_p1 {
    enum TILE_TYPE {
        GRASS,
        MOUNTAIN,
        DESERT,
        OCEAN,
        RIVER,
        SWAMP,
        DIRT,
        FOREST
    }
    class Tile {
        public static TILE_TYPE[] Impassables = { TILE_TYPE.MOUNTAIN, TILE_TYPE.OCEAN, TILE_TYPE.RIVER };
        public TILE_TYPE TileType { get; set; }
        public bool IsImpassable() {
            return Impassables.Contains(TileType);
        }

        public Tile(TILE_TYPE type, int elevation) {
            TileType = type;
            Elevation = elevation;
        }

        public int Elevation { get; set; }
    }
}
