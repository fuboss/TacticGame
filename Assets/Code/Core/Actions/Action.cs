using System;
using System.Collections;

namespace Assets.Code.Core.Actions
{
    public abstract class Action
    {
        /// <summary>
        /// Name of this Action
        /// </summary>
        public string name;

        /// <summary>
        /// Fired, when Execute is finished successes or failed
        /// </summary>
        public Action<bool> Finished = delegate (bool b) { };
        
        /// <summary>
        /// Price to execute ability
        /// </summary>
        public ActionPrice Price
        {
            get { return _price; }
            protected set { _price = value; }
        }

        /// <summary>
        /// Get reference to currently running routine
        /// </summary>
        public IEnumerator ActionRoutine
        {
            get { return _routine; }
            protected set { _routine = value; }
        }

        protected Action(Character character, ActionPrice price)
        {
            Price = price;
            _character = character;
        }

        /// <summary>
        /// Execute main logic of action
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerator Execute();

        private IEnumerator _routine;
        private ActionPrice _price;
        protected Character _character;
    }
}