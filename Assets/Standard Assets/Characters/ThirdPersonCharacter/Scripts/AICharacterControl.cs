using UnityEngine;

namespace Assets.Standard_Assets.Characters.ThirdPersonCharacter.Scripts
{
    [RequireComponent(typeof (UnityStandardAssets.Characters.ThirdPerson.ThirdPersonCharacter))]
    public class AICharacterControl : MonoBehaviour
    {
        public UnityStandardAssets.Characters.ThirdPerson.ThirdPersonCharacter character { get; private set; } // the character we are controlling
        public Vector3 target; // target to aim for

        private void Start()
        {
            character = GetComponent<UnityStandardAssets.Characters.ThirdPerson.ThirdPersonCharacter>();
        }

        private void Update()
        {
            if (target != Vector3.zero)
            {
                if ((target - transform.position).magnitude > 0)
                {
                    character.transform.LookAt(target);
                    var translating = (target - transform.position).normalized*30*Time.deltaTime;
                    character.transform.Translate(translating);
                    character.Move(target, false,false);
                }
                else
                    target = Vector3.zero;
            }

        }


        public void SetTarget(Vector3 target)
        {
            this.target = target;
        }
    }
}
