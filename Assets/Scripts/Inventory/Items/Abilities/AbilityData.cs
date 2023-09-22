using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace RPG.Abilities
{
    public class AbilityData
    {
        GameObject user;
        Vector3 TargetPoint;
        IEnumerable<GameObject> targets;
        CinemachineVirtualCamera Camera;

        public CinemachineVirtualCamera GetCamera() { return Camera; }
        public void SetCamera(CinemachineVirtualCamera setCamera) { Camera = setCamera; }

        public AbilityData(GameObject user)
        {
            this.user = user;
        }

        public GameObject GetUser() { return user; }

        public IEnumerable<GameObject> getTargets()
        {
            return targets;
        }

        public void SetTargetedPoint(Vector3 TargetPoint) { this.TargetPoint = TargetPoint; }

        public Vector3 GetTargetedPoint() { return TargetPoint; }

        public void setTargets(IEnumerable<GameObject> targets)
        {
            this.targets = targets;
        }

        public void StartCoroutine(IEnumerator enumerator)
        {
            user.GetComponent<MonoBehaviour>()?.StartCoroutine(enumerator);
        }

    }

}