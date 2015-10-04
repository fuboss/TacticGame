using System.Collections.Generic;
using System.Linq;
using Apex.WorldGeometry;
using Assets.Code.Core;
using Assets.Code.Tools;
using Assets.Code.Turns;
using UnityEngine;

namespace Assets.Code.Apex
{
    public class MovingOffset : MonoSingleton<MovingOffset>
    {
        public Character Current
        {
            get { return TeamsManager.Instance.PlayableTeam.CurrentControlled; }
        }

        public List<Cell> AllNormalDistanceCells
        {
            get { return _allNormalDistanceCells; }
        }

        public List<Cell> AllLongDistanceCells
        {
            get { return _allLongDistanceCells; }
        }

        public bool Validate(Cell cell)
        {
            return _allNormalDistanceCells.Contains(cell) || _allLongDistanceCells.Contains(cell);
        }

        public void Draw()
        {
            if (Grid == null) return;
            ScanPositions();
            if (_allNormalDistanceCells.Count == 0) return;

            //drawing only bordered cells
            foreach (var cell in _allNormalDistanceCells)
                DrawIfAvailable(cell, false, Color.yellow);

            foreach (var cell in _allLongDistanceCells)
                DrawIfAvailable(cell, true, Color.red);
        }

        public void ScanPositions()
        {
            _allNormalDistanceCells = Grid.GetCellRange(Current.navigatorFacade.position, Current.normalTurn*2 + 1).ToList();

            if (Current.CurrentActionPoints == Current.MaxActionPoints)
            {
                _allLongDistanceCells = Grid.GetCellRange(Current.navigatorFacade.position, Current.longTurn*2 + 1)
                    .Where(c => !_allNormalDistanceCells.Contains(c)).ToList();
            }
            else
                _allLongDistanceCells.Clear();
        }

        private Vector3 _previousUnitPosition;
        private List<Cell> _allNormalDistanceCells = new List<Cell>();
        private List<Cell> _allLongDistanceCells = new List<Cell>();
        private int minX = 0,
                    minZ = 0,
                    maxX = 0,
                    maxZ = 0;

        private IGrid Grid
        {
            get
            {
                if (Current == null || GridManager.instance == null)
                    return null;

                return GridManager.instance.GetGrid(Current.navigatorFacade.position);
            }
        }

        private Cell GetNeighbour(Cell cell, LineLayoutOffset direction)
        {
            switch (direction)
            {
                case LineLayoutOffset.Top:
                    return Grid.GetNeighbour(cell, 0, 1);
                case LineLayoutOffset.Bottom:
                    return Grid.GetNeighbour(cell, 0, -1);
                case LineLayoutOffset.Left:
                    return Grid.GetNeighbour(cell,-1, 0);
                case LineLayoutOffset.Right:
                    return Grid.GetNeighbour(cell, 1, 0);
                default:
                    return null;
            }
        }

        private void DrawIfAvailable(Cell cell, bool isLongDistance, Color color)
        {
            var currAttr = Current.navigatorFacade.attributes;
            //neighbours
            var right = GetNeighbour(cell, LineLayoutOffset.Right);
            var left = GetNeighbour(cell, LineLayoutOffset.Left);
            var bottom = GetNeighbour(cell, LineLayoutOffset.Bottom);
            var top = GetNeighbour(cell, LineLayoutOffset.Top);

            var currentRange = isLongDistance ? _allLongDistanceCells : _allNormalDistanceCells;
            
            if (!currentRange.Contains(right)) right = null;
            if (!currentRange.Contains(left)) left = null;
            if (!currentRange.Contains(bottom)) bottom = null;
            if (!currentRange.Contains(top)) top = null;

            if (cell.isWalkable(currAttr))
            {
                if (right != null)
                {
                    if (!right.isWalkable(currAttr))
                        DrawCoubicLine(cell, LineLayoutOffset.Right, color);
                }
                else
                {
                    right = GetNeighbour(cell, LineLayoutOffset.Right);
                    if (!isLongDistance)
                        DrawCoubicLine(cell, LineLayoutOffset.Right, color);
                    else if (right == null || (!_allNormalDistanceCells.Contains(right)))
                        DrawCoubicLine(cell, LineLayoutOffset.Right, color);
                }

                if (left != null)
                {
                    if (!left.isWalkable(currAttr))
                        DrawCoubicLine(cell, LineLayoutOffset.Left, color);
                }
                else
                {
                    left = GetNeighbour(cell, LineLayoutOffset.Left);
                    if (!isLongDistance)
                        DrawCoubicLine(cell, LineLayoutOffset.Left, color);
                    else if (left == null || (!_allNormalDistanceCells.Contains(left)))
                        DrawCoubicLine(cell, LineLayoutOffset.Left, color);
                }

                if (bottom != null)
                {
                    if (!bottom.isWalkable(currAttr))
                        DrawCoubicLine(cell, LineLayoutOffset.Bottom, color);
                }
                else
                {
                    bottom = GetNeighbour(cell, LineLayoutOffset.Bottom);
                    if (!isLongDistance)
                        DrawCoubicLine(cell, LineLayoutOffset.Bottom, color);
                    else if (bottom == null || (!_allNormalDistanceCells.Contains(bottom)))
                        DrawCoubicLine(cell, LineLayoutOffset.Bottom, color);
                }

                if (top != null)
                {
                    if (!top.isWalkable(currAttr))
                        DrawCoubicLine(cell, LineLayoutOffset.Top, color);
                }
                else
                {
                    top = GetNeighbour(cell, LineLayoutOffset.Top);
                    if (!isLongDistance)
                        DrawCoubicLine(cell, LineLayoutOffset.Top, color);
                    else if (top == null || (!_allNormalDistanceCells.Contains(top)))
                        DrawCoubicLine(cell, LineLayoutOffset.Top, color);
                }
            }
        }


        private void DrawCoubicLine(Cell cell, LineLayoutOffset offset, Color color)
        {
            Vector3 p0 = Vector3.zero, 
                    p1 = Vector3.zero;

            var half = Grid.cellSize*0.5f;
            var lineHeight = 0.2f;
            switch (offset)
            {
                case LineLayoutOffset.Bottom:
                {
                    p0 = new Vector3(cell.position.x - half, lineHeight, cell.position.z - half);
                    p1 = new Vector3(cell.position.x + half, lineHeight, cell.position.z - half);
                    break;
                }
                case LineLayoutOffset.Left:
                {
                    p0 = new Vector3(cell.position.x - half, lineHeight, cell.position.z - half);
                    p1 = new Vector3(cell.position.x - half, lineHeight, cell.position.z + half);
                    break;
                }
                case LineLayoutOffset.Right:
                {
                    p0 = new Vector3(cell.position.x + half, lineHeight, cell.position.z - half);
                    p1 = new Vector3(cell.position.x + half, lineHeight, cell.position.z + half);
                    break;
                }
                case LineLayoutOffset.Top:
                {
                    p0 = new Vector3(cell.position.x - half, lineHeight, cell.position.z + half);
                    p1 = new Vector3(cell.position.x + half, lineHeight, cell.position.z + half);
                    break;
                }
            }

            Debug.DrawLine(p0,p1, color);
        }
    }

    enum LineLayoutOffset
    {
        None,
        Bottom,
        Left,
        Top,
        Right,
    }
}
