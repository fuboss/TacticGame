using Apex.WorldGeometry;
using JetBrains.Annotations;

namespace Assets.Code.Core.Actions
{
    public class SprintMoveAction : NormalMoveAction
    {
        public SprintMoveAction(Character character, ActionPrice price, [NotNull]Cell cell) : base(character, price, cell)
        {
        }

        //public override IEnumerator Execute()
        //{
        //    throw new NotImplementedException();
        //}
    }
}