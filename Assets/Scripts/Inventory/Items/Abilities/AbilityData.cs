using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using RPG.Core;


namespace RPG.Abilities
{
    public class AbilityData : IAction
    {
        GameObject user;
        Vector3 TargetPoint;
        IEnumerable<GameObject> targets;
        CinemachineVirtualCamera Camera;
        bool cancelled = false; 

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

        public void Cancel()
        {
            cancelled = true;
        }

        public bool isCancelled() { return cancelled;  }
    }

}