using System.Collections;
using UnityEngine;
using RPG.Control;
using RPG.Inventory;
using RPG.Saving;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour, IRaycastable, ISaveable
    {
        [SerializeField] Item item;
        [SerializeField] float respawnTime = 5f;
        [SerializeField] float DistanceToPickUp = 5f;
        [SerializeField] bool respawn = false;
        private bool picked = false; 
        [SerializeField] CursorType cursorType = CursorType.PickUp;

        private void OnTriggerEnter(Collider other)
        {
            if(other.tag == "Player")
            {
                //PickUp(other.GetComponent<Fighter>());
            }
        }

        private void PickUp(Fighter fighter)
        {
            //fighter.EquipWeaponItem(weapon);
            StartCoroutine(HideForSeconds(respawnTime));
        }

        IEnumerator HideForSeconds (float seconds)
        {
            HidePickUp();
            yield return new WaitForSeconds(seconds);
            ShowPickUp();
        }

        private void ShowPickUp()
        {
            if(picked && !respawn) { return; }

            GetComponent<Collider>().enabled = true;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
        }

        private void HidePickUp()
        {
            GetComponent<Collider>().enabled = false;
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        public bool HandleRaycast(PlayerController callingCon)
        {
            if (Vector3.Distance(transform.position, callingCon.transform.position) > DistanceToPickUp) return false;
            if(Input.GetMouseButtonDown(0))
            {
                //PickUp(callingCon.GetComponent<Fighter>());
                if (callingCon.PickUp(item))
                {
                    picked = true;
                    StartCoroutine(HideForSeconds(respawnTime));
                }
            }

            return true;
        }

        public CursorType GetCursorType()
        {
            return cursorType;
        }

        public object CaptureState()
        {
            return picked;
        }

        public void RestoreState(object state)
        {
            if((bool)state && respawn == false)
            {
                HidePickUp();
            }
        }
    }

}