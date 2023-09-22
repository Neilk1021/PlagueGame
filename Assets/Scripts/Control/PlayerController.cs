using UnityEngine;
using RPG.Movement;
using UnityEngine.AI;
using RPG.Attributes;
using UnityEngine.EventSystems;
using System;
using RPG.Combat;
using RPG.Inventory;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        Health health;
        ItemInventory inventory;
        InventoryManager inventoryManager;
        [System.Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [SerializeField]CursorMapping[] cursorMappings = null;
        [SerializeField] float MaxNavMeshProjection = 1f;
        bool InventoryOpened = false;
        ActionStore actionStore;

        [SerializeField] int AbilityCount = 6;

        private void Awake()
        {
            inventoryManager = GameObject.FindObjectOfType<InventoryManager>();
            health = GetComponent<Health>();
            inventory = GetComponent<ItemInventory>();
            actionStore = GetComponent<ActionStore>();
        }

        public static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        private void SetCursor(CursorType cursorType)
        {
            CursorMapping mapping = GetCursorMapping(cursorType);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType type)
        {
            foreach (CursorMapping mapping in cursorMappings)
            {
                if(mapping.type == type)
                {
                    return mapping;
                }
            }
            return cursorMappings[0];
        }

        private bool InteractMove()
        {
            Vector3 target;
            bool HasHit = RaycastNavMesh(out target);
            if (HasHit)
            {
                if (!GetComponent<Move>().CanMoveTo(target)) return false;

                if (Input.GetButton("Fire1"))
                {
                    GetComponent<Move>().StartMove(target, 1);
                }
                SetCursor(CursorType.Movement);
                return true;
            }
            SetCursor(CursorType.None);
            return false;
        }

        public bool InteractWithInventory()
        {
            if (InventoryOpened)
            {
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    RectTransform rect = inventoryManager.GetComponent<RectTransform>();
                    rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, -720);
                    InventoryOpened = false;
                    return false;
                }
                return true;
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                RectTransform rect = inventoryManager.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, 0);
                InventoryOpened = true;
                return true;
            }
            return false;
        }

        private bool RaycastNavMesh(out Vector3 target)
        {
            target = new Vector3();
            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            if (!hasHit) return false;

            NavMeshHit NavHit;
            bool hasCastToNav = NavMesh.SamplePosition(hit.point, out NavHit, MaxNavMeshProjection, NavMesh.AllAreas);
            if (!hasCastToNav) return false;

            target = NavHit.position;

            return true;
        }

        void Update()
        {
            if (InteractWithUI())
            {
                SetCursor(CursorType.Ui);
                return;
            }
            if (health.isDead())
            {
                SetCursor(CursorType.None);
                return;
            }

            UseAbilities();
            if (InteractWithComponenet()) return;
            if(InteractMove()) return;
        }

        private void UseAbilities()
        {
            for (int i = 0; i < AbilityCount; i++)
            {
                if(Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    actionStore.Use(i, gameObject);
                }
            }
        }

        private bool InteractWithComponenet()
        {
            RaycastHit[] hits = RaycastAllSorted();
            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach(IRaycastable raycastable in raycastables)
                {
                    if (raycastable.HandleRaycast(this))
                    {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }
            }
            return false;
        }

        public bool PickUp(Item item)
        {
            if (inventory.AddItem(item)) return true;

            return false;
        }

        RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            float[] Distances = new float[hits.Length];
            for (int i = 0; i < Distances.Length; i++)
            {
                Distances[i] = hits[i].distance;
            }

            Array.Sort(Distances, hits);

            return hits;
        }

        private bool InteractWithUI()
        {
            if (InteractWithInventory()) return true;

            return EventSystem.current.IsPointerOverGameObject();
        }

        private void OnEnable()
        {
            if(inventory != null)
            {
                //inventory.onWeaponUse += GetComponent<Fighter>().EquipWeapon;
                //inventory.onArmorEquip += GetComponent<PlayerModel>().LoadArmor;
                //inventory.onArmorEquip += GetComponent<Health>().EquipArmor;
            }
        }

        private void OnDisable()
        {
            if (inventory != null)
            {
                //inventory.onWeaponUse -= GetComponent<Fighter>().EquipWeapon;
                //inventory.onArmorEquip -= GetComponent<PlayerModel>().LoadArmor;
                //inventory.onArmorEquip -= GetComponent<Health>().EquipArmor;
            }
        }

    }
}
