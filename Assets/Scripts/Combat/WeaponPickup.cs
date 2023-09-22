using System.Collections;
using UnityEngine;
using RPG.Control;
using RPG.Inventory;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        [SerializeField] Item item;
        [SerializeField] float respawnTime = 5f;
        [SerializeField] float DistanceToPickUp = 5f;
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
                    StartCoroutine(HideForSeconds(respawnTime));
                }
            }

            return true;
        }

        public CursorType GetCursorType()
        {
            return cursorType;
        }
    }

}