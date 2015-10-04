using System;
using Assets.Code.Tools;
using UnityEngine;

namespace Assets.Code.Turns
{
    public class TurnManager : MonoSingleton<TurnManager>
    {
        public event Action<TeamsManager.Team> NewTurn; 

        [SerializeField]
        private int _currentTurn = 0;

        public void AllTeamHaveNoTurns()
        {
            _currentTurn++;

            if (NewTurn != null)
                NewTurn(TeamsManager.Instance.GetNextTeam(TeamsManager.Instance.PlayableTeam.team).team);
        }
    }
}
