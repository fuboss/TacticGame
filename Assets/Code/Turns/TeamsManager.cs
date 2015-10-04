using System;
using System.Collections;
using Assets.Code.Apex;
using Assets.Code.Tools;
using JetBrains.Annotations;

namespace Assets.Code.Turns
{
    public class TeamsManager : MonoSingleton<TeamsManager>
    {
        public TeamController FirstTeam;
        public TeamController SecondTeam;

        public bool CanSetTask
        {
            get
            {
                if (_currentHandlingTask == null) return true;
                if (_currentHandlingTask.MoveNext()) return false;
                _currentHandlingTask = null;
                return true;
            }
        }

        public TeamController PlayableTeam
        {
            get { return _currentPlayableTeam; }
            set
            {
                if (_currentPlayableTeam != value)
                {
                    _currentPlayableTeam = value;
                    if (PlayableTeamChanged != null)
                        PlayableTeamChanged(_currentPlayableTeam.team);
                }
            }
        }

        public event Action<Team> PlayableTeamChanged;

        public TeamController GetNextTeam(Team current)
        {
            return current == Team.Second ? FirstTeam : SecondTeam;
        }

        public TeamController GetTeam(Team team)
        {
            return team == Team.First ? FirstTeam : SecondTeam;
        }

        public void SetTask([NotNull] Task task)
        {
            if (!CanSetTask) return;

            _currentHandlingTask = PlayableTeam.SetTask(task);
            StartCoroutine(_currentHandlingTask);
        }

        public void FinishTurnForTeam([NotNull] TeamController teamController)
        {
            if (!FirstTeam.IsAvailable && !SecondTeam.IsAvailable)
            {
                TurnManager.Instance.AllTeamHaveNoTurns();
                return;
            }

            //switch to nextTeam
            PlayableTeam = teamController == FirstTeam ? SecondTeam : FirstTeam;
        }

        private TeamController _currentPlayableTeam;
        private IEnumerator _currentHandlingTask;
        private MovingOffset _movingOffset;

        private void OnNewTurn(Team team)
        {
            PlayableTeam = GetTeam(team);
        }

        private void Start()
        {
            _movingOffset = MovingOffset.Instance;
            FirstTeam.Init();
            SecondTeam.Init();

            PlayableTeam = FirstTeam;
            TurnManager.Instance.NewTurn += OnNewTurn;
        }

        private void Update()
        {
//            _movingOffset.ScanPositions();
            var curCharacter = PlayableTeam.CurrentControlled;
            if(curCharacter == null) return;

            if (!curCharacter.IsBusy && curCharacter.IsAvailable)
                _movingOffset.Draw();
        }

        public enum Team
        {
            First,
            Second
        }
    }
}