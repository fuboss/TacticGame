using System.Collections;
using Apex.WorldGeometry;
using JetBrains.Annotations;

namespace Assets.Code.Core.Actions
{
    public class NormalMoveAction : Action
    {
        public NormalMoveAction(Character character, ActionPrice price, [NotNull]Cell cell) : base(character, price)
        {
            _cell = cell;
        }

        public override IEnumerator Execute()
        {
            _character.MoveTo(_cell.position);

            //wait for finish moving
            yield return null;
            while (!_character.navigatorFacade.hasArrivedAtDestination || _character.GetCell() != _cell)
                yield return null;

            yield return null;
            Finished(_character.navigatorFacade.hasArrivedAtDestination);
        }

        protected Cell _cell;
    }
}