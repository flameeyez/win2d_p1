using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;

namespace win2d_p1 {
    class Map {
        public long DebugCreationTimeMilliseconds { get; set; }
        private CanvasRenderTarget RenderTarget { get; set; }

        private static int nNumberOfPasses = 100;
        private static int nBaseDropsPerPass = 30;
        private static int nDropsVariance = 50;
        private static int nBonusDropRate = 50;
        private static int nBonusDrops = 100;
        public static readonly int TileSizeInPixels = 32;
        public Tile[,] Tiles { get; set; }

        private int _widthInTiles;
        public int WidthInTiles { get { return _widthInTiles; } }
        private int _heightInTiles;
        public int HeightInTiles { get { return _heightInTiles; } }

        public Map(CanvasDevice device, int rows, int columns) {
            Stopwatch s = Stopwatch.StartNew();

            _widthInTiles = columns;
            _heightInTiles = rows;

            List<TILE_TYPE> tileTypes = Enum.GetValues(typeof(TILE_TYPE)).OfType<TILE_TYPE>().ToList();
            int[,] elevationMap = new int[rows, columns];

            for(int i = 0; i < nNumberOfPasses; i++) {
                int x = Statics.Random.Next(columns);
                int y = Statics.Random.Next(rows);

                int nDropsPerPass = nBaseDropsPerPass + Statics.Random.Next(nDropsVariance) + (Statics.Random.Next(nBonusDropRate) == 0 ? nBonusDrops : 0);
                for(int j = 0; j < nDropsPerPass; j++) {
                    elevationMap[y, x] += 1;
                    Settle(elevationMap, x, y);
                }
            }

            int nNumberOfBlurs = 3;
            for(int i = 0; i < nNumberOfBlurs; i++) {
                elevationMap = Blur(elevationMap);
            }

            Tiles = new Tile[rows, columns];
            for(int column = 0; column < columns; column++) {
                for(int row = 0; row < rows; row++) {
                    if(elevationMap[row, column] == 0) {
                        Tiles[row, column] = new Tile(TILE_TYPE.OCEAN, elevationMap[row, column]);
                    }
                    else if(elevationMap[row, column] < 2) {
                        Tiles[row, column] = new Tile(TILE_TYPE.DIRT, elevationMap[row, column]);
                    }
                    else if(elevationMap[row, column] < 6) {
                        Tiles[row, column] = new Tile(TILE_TYPE.FOREST, elevationMap[row, column]);
                    }
                    else {
                        Tiles[row, column] = new Tile(TILE_TYPE.MOUNTAIN, elevationMap[row, column]);
                    }

                    //TILE_TYPE type = tileTypes.RandomValue();
                }
            }

            SaveWorldImage(device);
            s.Stop();
            DebugCreationTimeMilliseconds = s.ElapsedMilliseconds;
        }

        private void Settle(int[,] elevationMap, int x, int y) {
            if(y > 0 && x > 0 && (elevationMap[y, x] - elevationMap[y - 1, x - 1] > 1)) {
                // move nw and re-settle
                elevationMap[y, x]--;
                elevationMap[y - 1, x - 1]++;
                Settle(elevationMap, x - 1, y - 1);
            }
            else if(y > 0 && elevationMap[y, x] - elevationMap[y - 1, x] > 1) {
                // move n and re-settle
                elevationMap[y, x]--;
                elevationMap[y - 1, x]++;
                Settle(elevationMap, x, y - 1);
            }
            else if(y > 0 && x < elevationMap.GetLength(1) - 1 && elevationMap[y, x] - elevationMap[y - 1, x + 1] > 1) {
                // move ne and re-settle
                elevationMap[y, x]--;
                elevationMap[y - 1, x + 1]++;
                Settle(elevationMap, x + 1, y - 1);
            }
            else if(x > 0 && elevationMap[y, x] - elevationMap[y, x - 1] > 1) {
                // move w and re-settle
                elevationMap[y, x]--;
                elevationMap[y, x - 1]++;
                Settle(elevationMap, x - 1, y);
            }
            else if(x < elevationMap.GetLength(1) - 1 && elevationMap[y, x] - elevationMap[y, x + 1] > 1) {
                // move e and re-settle
                elevationMap[y, x]--;
                elevationMap[y, x + 1]++;
                Settle(elevationMap, x + 1, y);
            }
            else if(x > 0 && y < elevationMap.GetLength(0) - 1 && elevationMap[y, x] - elevationMap[y + 1, x - 1] > 1) {
                // move sw and re-settle
                elevationMap[y, x]--;
                elevationMap[y + 1, x - 1]++;
                Settle(elevationMap, x - 1, y + 1);
            }
            else if(y < elevationMap.GetLength(0) - 1 && elevationMap[y, x] - elevationMap[y + 1, x] > 1) {
                // move s and re-settle
                elevationMap[y, x]--;
                elevationMap[y + 1, x]++;
                Settle(elevationMap, x, y + 1);
            }
            else if(y < elevationMap.GetLength(0) - 1 && x < elevationMap.GetLength(1) - 1 && elevationMap[y, x] - elevationMap[y + 1, x + 1] > 1) {
                // move se and re-settle
                elevationMap[y, x]--;
                elevationMap[y + 1, x + 1]++;
                Settle(elevationMap, x + 1, y + 1);
            }
        }

        public void Draw(CanvasAnimatedDrawEventArgs args) {
            args.DrawingSession.DrawImage(RenderTarget);
        }

        private void SaveWorldImage(CanvasDevice device) {
            RenderTarget = new CanvasRenderTarget(device, WidthInTiles * TileSizeInPixels, HeightInTiles * TileSizeInPixels, 96);
            using(CanvasDrawingSession ds = RenderTarget.CreateDrawingSession()) {
                for(int column = 0; column < Tiles.GetLength(1); column++) {
                    for(int row = 0; row < Tiles.GetLength(0); row++) {
                        CanvasBitmap bitmap = Images.Grass;
                        switch(Tiles[row, column].TileType) {
                            case TILE_TYPE.GRASS:
                                bitmap = Images.Grass;
                                break;
                            case TILE_TYPE.DIRT:
                                bitmap = Images.Desert;
                                break;
                            case TILE_TYPE.DESERT:
                                break;
                            case TILE_TYPE.OCEAN:
                                bitmap = Images.Ocean;
                                break;
                            case TILE_TYPE.RIVER:
                                break;
                            case TILE_TYPE.SWAMP:
                                break;
                            case TILE_TYPE.FOREST:
                                break;
                            case TILE_TYPE.MOUNTAIN:
                                bitmap = Images.Mountains;
                                break;
                            default:
                                break;
                        }

                        double tilePositionX = column * TileSizeInPixels;
                        double tilePositionY = row * TileSizeInPixels;

                        ds.DrawImage(bitmap, new Rect(tilePositionX, tilePositionY, TileSizeInPixels, TileSizeInPixels));
                    }
                }
            }
        }

        public void Update(CanvasAnimatedUpdateEventArgs args) {

        }

        public bool IsImpassable(int row, int column) {
            return Tiles[row, column].IsImpassable();
        }

        public bool IsValidRow(int row) {
            return (row >= 0) && (row < Tiles.GetLength(0));
        }

        public bool IsValidColumn(int column) {
            return (column >= 0) && (column < Tiles.GetLength(1));
        }

        private static Dictionary<int, float> kernel;
        static Map() {
            kernel = new Dictionary<int, float>();
            kernel.Add(-3, 0.006f);
            kernel.Add(-2, 0.061f);
            kernel.Add(-1, 0.242f);
            kernel.Add(0, 0.383f);
            kernel.Add(1, 0.242f);
            kernel.Add(2, 0.061f);
            kernel.Add(3, 0.006f);
        }

        private int[,] Blur(int[,] elevationMap) {
            int x = elevationMap.GetLength(0);
            int y = elevationMap.GetLength(1);
            int[,] intermediate = new int[x, y];
            int[,] final = new int[x, y];

            for(int i = 0; i < x; i++) {
                for(int j = 0; j < y; j++) {
                    intermediate[i, j] = ComputeX(elevationMap, i, j, x);
                }
            }

            for(int i = 0; i < x; i++) {
                for(int j = 0; j < y; j++) {
                    final[i, j] = ComputeY(intermediate, i, j, y);
                }
            }

            return final;
        }

        private int ComputeX(int[,] elevationMap, int x, int y, int arrayLengthX) {
            float value = 0.0f;
            foreach(var k in kernel) {
                if(x + k.Key >= 0 && x + k.Key < arrayLengthX) {
                    value += elevationMap[x + k.Key, y] * k.Value;
                }
                else {
                    value += elevationMap[x, y] * k.Value;
                }
            }

            return (int)Math.Round(value, 0);
        }

        private int ComputeY(int[,] elevationMap, int x, int y, int arrayLengthY) {
            float value = 0.0f;
            foreach(var k in kernel) {
                if(y + k.Key >= 0 && y + k.Key < arrayLengthY) {
                    value += elevationMap[x, y + k.Key] * k.Value;
                }
                else {
                    value += elevationMap[x, y] * k.Value;
                }
            }

            return (int)Math.Round(value, 0);
        }
    }
}


