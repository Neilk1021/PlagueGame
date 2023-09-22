using System.Collections;
using UnityEngine;
using RPG.Control;
using System.Collections.Generic;
using System;

namespace RPG.Abilities.Targeting
{
    [CreateAssetMenu(fileName = "Delayed Click Targeting", menuName = "Items/Abilities/Targeting/DelayedClickTargeting")]
    public class DelayedClickTargeting : TargetingStrategy
    {
        [SerializeField] Texture2D cursorTexture;
        [SerializeField] Vector2 cursorHotspot;
        [Header("Sphere Cast")]
        [SerializeField] LayerMask layerMask;
        [SerializeField] float areaAffectRadius;
        [SerializeField] GameObject TargetingCircle;

        Transform targetingPrefabInstance = null;

        public override void StartTargeting(AbilityData data, Action finished)
        {
            PlayerController playerController = data.GetUser().GetComponent<PlayerController>();
            if(targetingPrefabInstance == null) targetingPrefabInstance = Instantiate(TargetingCircle).transform;
            playerController.StartCoroutine(Targeting(data, playerController, finished));
        }

        private IEnumerator Targeting(AbilityData data, PlayerController playerController, Action finished)
        {
            playerController.enabled = false;
            targetingPrefabInstance.gameObject.SetActive(true);
            targetingPrefabInstance.GetChild(0).localScale = new Vector3(areaAffectRadius * 2 *.4f, 1, areaAffectRadius * 2*.4f);
            while (true)
            {
                Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
                RaycastHit raycastHit;
                if (!Physics.Raycast(PlayerController.GetMouseRay(), out raycastHit, 1000, layerMask)) yield return null;
                targetingPrefabInstance.position = raycastHit.point;

                if (Input.GetMouseButtonUp(0))
                {
                    playerController.enabled = true;
                    data.setTargets(GetGameObjectsInRadius(raycastHit.point));
                    data.SetTargetedPoint(raycastHit.point);
                    targetingPrefabInstance.gameObject.SetActive(false);
                    finished();
                    yield break;
                }

                yield return null;
            }
        }

        IEnumerable<GameObject> GetGameObjectsInRadius(Vector3 Point)
        {
             RaycastHit[] hits = Physics.SphereCastAll(Point, areaAffectRadius, Vector3.up, 0);
             foreach (RaycastHit hit in hits)
             {
                 yield return hit.collider.gameObject;
             }
        }
    }
}