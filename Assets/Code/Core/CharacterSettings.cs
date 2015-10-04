using System;
using System.Collections.Generic;
using Action = Assets.Code.Core.Actions.Action;

namespace Assets.Code.Core
{
    [Serializable]
    public class CharacterSettings
    {
        public string name;
        public int actionPoints;

        public int normalTurnDistance;
        public int longTurnDistance;
        
    }
}