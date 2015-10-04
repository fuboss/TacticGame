using Assets.Code.Core;
using Assets.Code.Tools;
using Assets.Code.Turns;
using UnityEngine;

namespace Assets.Code.Input
{
    public class InputReciever : MonoSingleton<InputReciever>
    {
        private Vector3 _lastSelectDownPos;
        private Character Current { get { return TeamsManager.Instance.PlayableTeam.CurrentControlled; } }
        private void Awake()
        {
            InputManager.Instance.ClickRB += OnClickRb;
        }

        private void OnClickRb(InputTarget input)
        {
            if (!input.IsValid) return;

            if (Current.ValidateCell(input.cell))
                TeamsManager.Instance.SetTask(new Task(Task.TaskType.Move, input.cell));
            else
            {
                print(Helper.ColorizedText("abort","red"));
            }
        }

    }
}
