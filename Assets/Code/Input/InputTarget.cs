using System;
using Apex.WorldGeometry;
using Assets.Code.Core;
using UnityEngine;

namespace Assets.Code.Input
{
    public class InputTarget
    {
        public readonly bool isOverHUD;
        public readonly Cell cell;
        public readonly Character character;

        public bool IsValid { get { return cell != null || character != null; } }

        /// <summary>
        /// Position of selected Character or Cell
        /// </summary>
        public Vector3 UnifiedPoint
        {
            get
            {
                if (character != null)
                    return character.navigatorFacade.position;
                if (cell != null)
                    return cell.position;

                throw new NotSupportedException("Not supported");
            }
        }

        public InputTarget()
        {
            isOverHUD = true;
        }

        public InputTarget(Character character, Cell cell)
        {
            if (character != null || cell != null)
            {
                isOverHUD = false;
                this.character = character;
                this.cell = cell;
            }
            else
                isOverHUD = true;
        }

        public InputTarget(Character character) : this(character, character.GetCell())
        { }

        public InputTarget(Cell cell) : this(null, cell)
        { }
    }
}