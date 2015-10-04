/* Copyright © 2014 Apex Software. All rights reserved. */

using Apex.Units;
using Assets.Code.Core;

namespace Apex.Debugging
{
    using Apex;
    using Apex.Messages;
    using Apex.Services;
    using UnityEngine;

    /// <summary>
    /// A simple navigation monitor, that will log the various navigation events a moving unit may report to the Unity console.
    /// </summary>
    [AddComponentMenu("Apex/Debugging/Steering Monitor")]
    [ApexComponent("Debugging")]
    public class SteeringMonitor : ExtendedMonoBehaviour, IHandleMessage<UnitNavigationEventMessage>
    {
        /// <summary>
        /// The unit filter
        /// </summary>
        public GameObject unitFilter;

        /// <summary>
        /// The event filter
        /// </summary>
        public UnitNavigationEventMessage.Event eventFilter;

        private IUnitFacade _facade;

        /// <summary>
        /// Called on Start and OnEnable, but only one of the two, i.e. at startup it is only called once.
        /// </summary>
        protected override void OnStartAndEnable()
        {
            unitFilter = this.gameObject;
            _facade = GameServices.gameStateManager.GetUnitFacade(unitFilter);
            GameServices.messageBus.Subscribe(this);
        }

        private void OnDisable()
        {
            GameServices.messageBus.Unsubscribe(this);
        }

        void IHandleMessage<UnitNavigationEventMessage>.Handle(UnitNavigationEventMessage message)
        {
            if (unitFilter != null && !unitFilter.Equals(message.entity))
                return;

            if (eventFilter != UnitNavigationEventMessage.Event.None && message.eventCode != eventFilter)
                return;

            switch (message.eventCode)
            {
                case UnitNavigationEventMessage.Event.DestinationReached:
                case UnitNavigationEventMessage.Event.WaypointReached:
                case UnitNavigationEventMessage.Event.NodeReached:
                {
                    //Debug.Log(string.Format("Unit '{0}' ({1}) reports: {2} at position: {3}.", message.entity.name, message.entity.transform.position, message.eventCode, message.destination));
                    break;
                }

                case UnitNavigationEventMessage.Event.StoppedDestinationBlocked:
                case UnitNavigationEventMessage.Event.StoppedNoRouteExists:
                case UnitNavigationEventMessage.Event.StoppedRequestDecayed:
                case UnitNavigationEventMessage.Event.Stuck:
                {
                    Debug.Log(string.Format("Unit '{0}' ({1}) reports: {2} moving towards: {3}.", message.entity.name, message.entity.transform.position, message.eventCode, message.destination));
                    break;
                }
            }
        }
    }
}
