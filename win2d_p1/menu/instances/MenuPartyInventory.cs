using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Windows.System;
using Windows.UI;
using System.Diagnostics;
using Microsoft.Graphics.Canvas.Text;

namespace win2d_p1 {
    class MenuPartyInventory : Menu {
        private int nSelectedItem;
        private static int nItemsPerRow = 4;
        private Vector2 _stringsPosition;
        public Inventory PartyInventory { get; set; }

        private int _itemOffset = 0;
        private int _maxOffset {
            get {
                if(PartyInventory.Slots.Count <= _maxItemsPerPage) { return 0; }
                else {
                    int totalRows = PartyInventory.Slots.Count / nItemsPerRow;
                    if(PartyInventory.Slots.Count % nItemsPerRow != 0) { totalRows++; }
                    int rowsPerPage = _maxItemsPerPage / nItemsPerRow;
                    return (totalRows - rowsPerPage) * nItemsPerRow;
                }
            }
        }
        private int _itemsPerColumn { get { return (int)(_height / _heightOfMenuItem); } }
        private int _maxItemsPerPage { get { return nItemsPerRow * _itemsPerColumn; } }
        private float _heightOfMenuItem { get { return 20.0f + _defaultPadding; } }
        private int nItemsOnLastRow { get { return PartyInventory.Slots.Count % nItemsPerRow; } }

        public MenuPartyInventory(Inventory partyInventory, Vector2 position, double width, double height, Color? backgroundColor = default(Color?)) : base(position, width, height, backgroundColor) {
            _stringsPosition = new Vector2(_position.X + _defaultPadding, _position.Y + _defaultPadding);
            PartyInventory = partyInventory;
        }

        public override void Draw(CanvasAnimatedDrawEventArgs args) {
            base.Draw(args);
            DrawPartyInventory(args);
        }

        private void DrawPartyInventory(CanvasAnimatedDrawEventArgs args) {
            Stopwatch s1 = Stopwatch.StartNew();
            float x;
            float y = _stringsPosition.Y;
            for(int i = _itemOffset; i < PartyInventory.Slots.Count && i < _maxItemsPerPage + _itemOffset; i++) {
                x = _stringsPosition.X + (i % nItemsPerRow) * (float)_width / nItemsPerRow;

                if(i == nSelectedItem) {
                    PartyInventory.Slots[i].DrawSelected(args, new Vector2(x, y));
                }
                else {
                    PartyInventory.Slots[i].Draw(args, new Vector2(x, y));
                }

                if((i + 1) % nItemsPerRow == 0) {
                    y += 20.0f + _defaultPadding;
                }
            }
            s1.Stop();

            // draw debug timing
            CanvasTextLayout text = new CanvasTextLayout(args.DrawingSession.Device, s1.ElapsedMilliseconds.ToString() + "ms", Font.Calibri14, 0, 0);
            args.DrawingSession.DrawTextLayout(text, new Vector2(_position.X + (float)_width - (float)text.LayoutBounds.Width - _defaultPadding, _position.Y + _defaultPadding), Colors.White);

            // deduce number of items that can fit into a column
            // measure sample string
            // drawable height / sample string height + padding
            // add arbitrarily large x
        }

        public override void KeyDown(VirtualKey vk) {
            switch(vk) {
                case VirtualKey.Down:
                    // if only one row, do nothing
                    if(PartyInventory.Slots.Count > nItemsPerRow) {
                        int nColumnPosition = nSelectedItem % nItemsPerRow;
                        nSelectedItem += nItemsPerRow;
                        if(nSelectedItem >= PartyInventory.Slots.Count) {
                            // if selected item isn't on last row, scroll down and select last item
                            if(nItemsOnLastRow > 0 && nColumnPosition >= nItemsOnLastRow) {
                                if(_itemOffset != _maxOffset) {
                                    _itemOffset += nItemsPerRow;
                                }
                                nSelectedItem = PartyInventory.Slots.Count - 1;
                            }
                            // otherwise, scroll to the top
                            else {
                                _itemOffset = 0;
                                nSelectedItem = nColumnPosition;
                            }
                        }
                        else if(nSelectedItem - _itemOffset >= _maxItemsPerPage) {
                            // selected item in bounds, but no longer on screen
                            // scroll down
                            _itemOffset += nItemsPerRow;
                        }
                    }
                    break;
                case VirtualKey.Up:
                    if(PartyInventory.Slots.Count > nItemsPerRow) {
                        int nColumnPosition = nSelectedItem % nItemsPerRow;
                        nSelectedItem -= nItemsPerRow;
                        if(nSelectedItem < 0) {
                            // scroll to bottom
                            _itemOffset = _maxOffset;
                            if(nItemsOnLastRow == 0) {
                                // full row; can simply add total count to move to end and maintain cursor position
                                nSelectedItem += PartyInventory.Slots.Count;
                            }
                            else {
                                // not full row
                                // move to cursor position on final row and check to see if item occupies slot
                                nSelectedItem = PartyInventory.Slots.Count - nItemsOnLastRow + nColumnPosition;
                                if(nColumnPosition >= nItemsOnLastRow) {
                                    // column position doesn't contain an item; need to move up a row
                                    nSelectedItem -= nItemsPerRow;
                                }
                            }
                        }
                        else if(nSelectedItem < _itemOffset) {
                            // still in bounds; scroll up
                            _itemOffset -= nItemsPerRow;
                        }
                    }
                    break;
                case VirtualKey.Right:
                    nSelectedItem++;
                    if(nSelectedItem >= PartyInventory.Slots.Count) {
                        nSelectedItem -= PartyInventory.Slots.Count;
                        _itemOffset = 0;
                    }
                    else if(PartyInventory.Slots.Count > _maxItemsPerPage && SelectedItemBelowBottomRow) {
                        _itemOffset += nItemsPerRow;
                    }
                    break;
                case VirtualKey.Left:
                    nSelectedItem--;
                    if(nSelectedItem < 0) {
                        nSelectedItem += PartyInventory.Slots.Count;
                        _itemOffset = _maxOffset;
                    }
                    else if(nSelectedItem < _itemOffset) {
                        _itemOffset -= nItemsPerRow;
                    }
                    break;
                case VirtualKey.Enter:
                    // invoke menu item
                    //Items[nSelectedItem].InvokeEvent();
                    break;
            }
        }

        private bool SelectedItemBelowBottomRow { get { return nSelectedItem >= _itemOffset + _maxItemsPerPage; } }
    }
}
