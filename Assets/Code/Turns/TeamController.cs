using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Apex.WorldGeometry;
using Assets.Code.Apex;
using Assets.Code.Core;
using Assets.Code.Core.Actions;
using UnityEngine;
using Action = Assets.Code.Core.Actions.Action;

namespace Assets.Code.Turns
{
    [Serializable]
    public class TeamController
    {
        public TeamsManager.Team team;
        public List<Character> characters = new List<Character>();
        public Character CurrentControlled
        {
            get { return _currentControlled; }
            private set
            {   
                //reset previous
                if (_currentControlled != null)
                {
                    _currentControlled.JobIsDone -= CurrentCharacterOnJobIsDone;
                    _currentControlled.navigatorFacade.isSelected = false;
                }
                _currentControlled = value;
                if (_currentControlled != null)
                {
                    _currentControlled.navigatorFacade.isSelected = true;
                    _currentControlled.JobIsDone += CurrentCharacterOnJobIsDone;
                }
            }
        }

        public bool IsAvailable { get { return characters.Any(c => c.IsAvailable); } }

        public TeamController(TeamsManager.Team team)
        {
            this.team = team;
        }

        public TeamController(TeamsManager.Team team, IEnumerable<Character> chars ) :this(team)
        {
            characters = chars as List<Character>;
        }

        /// <summary>
        /// Setup all characters in this team
        /// </summary>
        public void Init()
        {
            if(_inited) return;
            if (!characters.Any())
            {
                Debug.LogError(team + " is empty! Can't init team");
                return;
            }

            foreach (var character in characters)
                character.currentTeam = team;

            TeamsManager.Instance.PlayableTeamChanged += OnTeamChanged;
            _inited = true;
        }

        /// <summary>
        /// Setting a new action for current Character to do
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public IEnumerator SetTask(Task task)
        {
            var character = CurrentControlled;
            if (character == null)
                throw new InvalidOperationException("CurrentControlled is null");

            var action = TaskToAction(task, character);
            if (action == null)
                throw new InvalidOperationException("action is null");

            //waiting for the end current action of character
            while (character.IsBusy)
                yield return null;

            character.DoAction(action);
            character.ActionFinished += CurrentCharacterOnActionFinished;

            //waiting for the end if new action
            while (character.IsBusy)
                yield return null;
        }

        public void SelectNextCharacter()
        {
            var nextCharacter = characters.FirstOrDefault(ch => ch.IsAvailable);

            //if there are no available characters
            if (nextCharacter == null)
            {
                foreach (var character in characters)
                    character.navigatorFacade.isSelected = false;
                TeamsManager.Instance.FinishTurnForTeam(this);
            }
            else
                CurrentControlled = nextCharacter;
        }

        private Character _currentControlled;
        private bool _inited;

        private void OnTeamChanged(TeamsManager.Team turnOfTeam)
        {
            if(turnOfTeam != team) return;

            foreach (var character in characters)
                character.Refresh();

            CurrentControlled = characters.FirstOrDefault();
        }

        private void CurrentCharacterOnJobIsDone(Character character)
        {
            CurrentControlled.JobIsDone -= CurrentCharacterOnJobIsDone;
            SelectNextCharacter();
        }

        private void CurrentCharacterOnActionFinished(Character character, Action action, bool finished)
        {
            character.ActionFinished -= CurrentCharacterOnActionFinished;
//            Debug.Log(string.Format("{0} is {1} action {2}",character.name, (finished?"finished":"failed"), action.name));
            character.OccupyCurrentCell(true);
        }

        private Action TaskToAction(Task task, Character character)
        {
            Action action = null;
            if (task.Type == Task.TaskType.Move)
            {
                var isNormalMoving = MovingOffset.Instance.AllNormalDistanceCells.Contains(task.Cell);
                var fistMove = character.CurrentActionPoints >= character.MaxActionPoints;
                int price = 0;

                if (fistMove)
                {
                    price = isNormalMoving ? 5 : 10;
                    if (isNormalMoving)
                        action = new NormalMoveAction(character, new ActionPrice() {points = price}, task.Cell);
                    else
                        action = new SprintMoveAction(character, new ActionPrice() {points = price}, task.Cell);
                }
                else
                {
                    price = character.CurrentActionPoints;
                    action = new NormalMoveAction(character, new ActionPrice() { points = price }, task.Cell);
                }
            }
            return action;
        }
    }
}