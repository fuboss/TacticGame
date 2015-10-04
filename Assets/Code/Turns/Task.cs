using System;
using Apex.WorldGeometry;

namespace Assets.Code.Turns
{
    [Serializable]
    public class Task
    {
        public TaskType Type { get; private set; }
        public Cell Cell { get; private set; }

        public Task(TaskType type, Cell cell)
        {
            Type = type;
            Cell = cell;
        }

        public enum TaskType
        {
            Move,
            Attack,
            Heal,
            Use,
        }
    }
}