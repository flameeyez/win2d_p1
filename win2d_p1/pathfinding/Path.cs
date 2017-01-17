using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace win2d_p1 {
    class Path {
        private static readonly int CreationAbortThresholdMilliseconds = 200;

        public DateTime DebugCreationTime;

        public int OpenSetMaximumCount { get; set; }

        public Vector2RowColumn Start { get; set; }
        public Vector2RowColumn Destination { get; set; }

        public int DebugInsertCount { get; set; }
        public int DebugClashes { get; set; }
        public TimeSpan DebugTimeToCreate { get; set; }

        // add nodes in reverse order
        public Stack<Vector2RowColumn> Nodes = null;

        public Path() {
            DebugCreationTime = DateTime.Now;
            Nodes = new Stack<Vector2RowColumn>();
        }

        public static Path Create(Map map, Vector2RowColumn start, Vector2RowColumn destination) {
            DateTime StartTime = DateTime.Now;
            TimeSpan ElapsedTime = TimeSpan.Zero;

            if(destination == null) { return null; }
            if(start.Equals(destination)) { return null; }
            if(map.IsImpassable(destination.Row, destination.Column)) { return null; }

            // DEBUG
            Path returnPath = new Path();

            DateTime DebugCreationTime = DateTime.Now;

            returnPath.Start = start;
            returnPath.Destination = destination;

            BinaryHeap _openSet = new BinaryHeap(500);
            PathNodeCollection _closedSet = new PathNodeCollection();

            // TODO: fix maximum heap size
            PathNode startingNode = new PathNode(start, null, destination);

            //DebugInsertCount++;
            _openSet.Insert(startingNode);

            while(_openSet.CurrentSize > 0) {
                ElapsedTime = DateTime.Now - StartTime;
                if(ElapsedTime.TotalMilliseconds > CreationAbortThresholdMilliseconds) {
                    return null;
                }

                //if (returnPath.OpenSet.CurrentSize >= 500) 
                //{
                //    // bail?
                //    return null;
                //}

                PathNode currentNode = _openSet.GetRootValue();
                if(currentNode.Coordinates.Equals(destination)) {
                    // destination reached; build stack
                    while(currentNode.ParentNode != null) {
                        returnPath.Nodes.Push(currentNode.Coordinates);
                        currentNode = currentNode.ParentNode;
                    }

                    returnPath.DebugTimeToCreate = DateTime.Now - DebugCreationTime;

                    return returnPath;
                }

                _closedSet.Add(_openSet.RemoveRoot().Value);

                for(int row = currentNode.Coordinates.Row - 1; row <= currentNode.Coordinates.Row + 1; row++) {
                    // TODO: fix this?
                    if(!map.IsValidRow(row)) { continue; }

                    for(int column = currentNode.Coordinates.Column - 1; column <= currentNode.Coordinates.Column + 1; column++) {
                        // TODO: fix this?
                        if(!map.IsValidColumn(column)) { continue; }
                        //if (Globals.Impassables.IndexOf(map.Tiles[row, column].Character) != -1) { continue; }
                        if(map.IsImpassable(row, column)) { continue; }
                        if(_closedSet.Contains(row, column)) { continue; }

                        // valid tile
                        PathNode newNode = new PathNode(row, column, currentNode, destination);
                        // check if exists node in OpenSet with current coordinates
                        PathNode compareNode = _openSet.GetNode(newNode.Coordinates);
                        if(compareNode != null) {
                            // TODO: fix?
                            returnPath.DebugClashes++;
                            // if(returnPath.Clashes++ > 200) { return null; }

                            if(newNode.G < compareNode.G) {
                                compareNode.ParentNode = newNode.ParentNode;
                                compareNode.CalculateG();
                                compareNode.CalculateH(destination);
                            }
                        }
                        else {
                            // no match found; add new node
                            _openSet.Insert(newNode);
                        }
                    }
                }
            }

            // OpenSet is empty; no path found
            return null;
        }
    }
}


////////////////
//
// DEBUG
//returnPath.OpenSetMaximumCount = returnPath.OpenSet.MaximumCount;
//
////////////////
