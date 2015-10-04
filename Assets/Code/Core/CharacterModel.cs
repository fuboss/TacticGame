using Apex.WorldGeometry;
using UnityEngine;

namespace Assets.Code.Core
{
    public class CharacterModel : MonoBehaviour
    {
        public Collider BodyCollider
        {
            get { return _collider; }
        }

        public DynamicObstacle Obstacle
        {
            get { return _obstacle; }
        }

        public Character Owner
        {
            get { return _owner; }
        }

        private Collider _collider;
        private DynamicObstacle _obstacle;
        private Character _owner;

        protected virtual void Awake()
        {
            _owner = GetComponentInParent<Character>();
            _collider = GetComponent<Collider>();
            _obstacle = GetComponent<DynamicObstacle>();
        }
    }
}