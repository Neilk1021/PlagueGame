using UnityEngine;
using RPG.Attributes;
using RPG.Control;

namespace RPG.Combat
{
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRaycastable
    {
        [HideInInspector]
        public bool canAttack = true;

        public CursorType GetCursorType()
        {
            return CursorType.Combat;
        }

        public bool HandleRaycast(PlayerController callingCon)
        {
            if (!canAttack) return false;

            Fighter playerFighter = callingCon.GetComponent<Fighter>();

            if (!playerFighter.CanAttack(gameObject))return false; 

            if (Input.GetMouseButton(0))
            {
                callingCon.GetComponent<Fighter>().Attack(gameObject);
            }
            return true;
        }
    }
}