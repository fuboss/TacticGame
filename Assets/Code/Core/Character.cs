using System;
using Apex.Services;
using Apex.Units;
using Apex.WorldGeometry;
using Assets.Code.Apex;
using Assets.Code.Turns;
using UnityEngine;
using Action = Assets.Code.Core.Actions.Action;

namespace Assets.Code.Core
{
    public class Character : MonoBehaviour
    {
        public TeamsManager.Team currentTeam;

        public CharacterSettings settings;
        public int normalTurn { get { return settings.normalTurnDistance; } }
        public int longTurn { get { return settings.longTurnDistance; } }
        public UnitFacade navigatorFacade { get; private set; }

        public int CurrentActionPoints
        {
            get { return _currentActionPoints; }
            protected set
            {
                _currentActionPoints = value;
                if (_currentActionPoints <= 0)
                    JobIsDone(this);
            }
        }

        public virtual bool IsAvailable { get { return _currentActionPoints > 0; } }

        public int MaxActionPoints { get { return settings.actionPoints; } }
        public bool IsBusy {get { return _currentAction != null; } }

        /// <summary>
        /// Fired when CurrentActionPoints of character was over
        /// </summary>
        public event Action<Character> JobIsDone = delegate {};
        public event Action<Character,Action, bool> ActionFinished = delegate {};

        /// <summary>
        /// Get grid cell where is character is placed 
        /// </summary>
        /// <returns></returns>
        public Cell GetCell()
        {
            var grid = GridManager.instance.GetGrid(navigatorFacade.position);
            if (grid != null)
                return grid.GetCell(navigatorFacade.position);
            return null;
        }

        public bool ValidateCell(Cell cell)
        {
            return cell.isWalkable(navigatorFacade.attributes) && MovingOffset.Instance.Validate(cell);
        }
        public void OccupyCurrentCell(bool occupy)
        {
            _model.Obstacle.Toggle(occupy);
        }

        public virtual void Refresh()
        {
            //reset local values on new turn begins
            _currentActionPoints = settings.actionPoints;
            _currentAction = null;
        }

        public virtual void DoAction(Action action)
        {
            if(IsBusy) 
                throw new InvalidOperationException("User is busy: " + _currentAction.name);

            _currentAction = action;
            _currentAction.Finished += OnActionFinished;
            StartCoroutine(_currentAction.Execute());
        }

        /// <summary>
        /// Handle motion and handle model
        /// </summary>
        /// <param name="pos"></param>
        public virtual void MoveTo(Vector3 pos)
        {
            //release previous position
            OccupyCurrentCell(false);

            //handle motion
            navigatorFacade.MoveTo(pos, false);

            //handle model
        }

        [SerializeField]
        private int _currentActionPoints;

        private CharacterModel _model;
        private Action _currentAction;

        protected virtual void OnActionFinished(bool finished)
        {
            CurrentActionPoints -= _currentAction.Price.points;
            if (_currentAction != null)
                ActionFinished(this, _currentAction, finished);
            _currentAction = null;
        }

        #region Unity

        protected virtual void Awake()
        {
            navigatorFacade =
                (UnitFacade)GameServices.gameStateManager.GetUnitFacade(GetComponentInChildren<UnitComponent>().gameObject);
            _model = GetComponentInChildren<CharacterModel>();
            _currentActionPoints = settings.actionPoints;
            name = settings.name + " " + currentTeam + "Team";
        }

        protected virtual void Start()
        {
            OccupyCurrentCell(true);

        }

        #endregion

    }
}
