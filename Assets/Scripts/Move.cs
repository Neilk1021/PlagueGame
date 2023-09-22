using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;

namespace RPG.Movement
{
    public class Move : MonoBehaviour, IAction, ISaveable
    {
        NavMeshAgent PlayerNav;
        [SerializeField] Animator Anim;
        [SerializeField] float MaxSpeed = 5.66f;
        [SerializeField] float maxPathLength = 35f;
        Health health;


        void Awake()
        {
            Anim = GetComponentInChildren<Animator>();
            health = GetComponent<Health>();
            PlayerNav = GetComponent<NavMeshAgent>();
        }

        void UpdateAnim()
        {
            PlayerNav.enabled = !health.isDead();

            Vector3 localVelocity = transform.InverseTransformDirection(PlayerNav.velocity);
            Anim.SetFloat("Blend", localVelocity.z);

        }

        public void Cancel()
        {
            PlayerNav.isStopped = true;
        }

        public void StartMove(Vector3 Des, float SpeedFraction )
        {
            GetComponent<ActionSchedular>().StartAction(this);
            MoveTo(Des, SpeedFraction);
        }

        public bool CanMoveTo(Vector3 destination)
        {
            NavMeshPath path = new NavMeshPath();
            bool HasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);
            if (!HasPath) return false;
            if (path.status != NavMeshPathStatus.PathComplete) return false;
            if (getPathLength(path) > maxPathLength) return false;

            return true;
        }

        private float getPathLength(NavMeshPath path)
        {
            float total = 0;
            if (path.corners.Length < 2) return total;

            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                total += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }

            return total;
        }

        public void MoveTo(Vector3 Hit, float SpeedFraction)
        {
            PlayerNav.SetDestination(Hit);
            PlayerNav.speed = MaxSpeed * Mathf.Clamp01(SpeedFraction);
            PlayerNav.isStopped = false;
        }

        public void AddForce(Vector3 Position, float Speed, float Durration)
        {
            GetComponent<ActionSchedular>().StartAction(this);
            PlayerNav.enabled = false;
            Vector3 Vel = (Position - transform.position).normalized;
            GetComponent<Rigidbody>().AddForce(Vel * Speed, ForceMode.Impulse);
           StartCoroutine(AddForceTime(Durration));
        }

        // Update is called once per frame
        void Update()
        {
            UpdateAnim();
        }

        public object CaptureState()
        {
            return new SerializableVector3(transform.position);
        }

        public void RestoreState(object state)
        {
            SerializableVector3 pos = (SerializableVector3)state;
            GetComponent<NavMeshAgent>().enabled = false;
            transform.position = pos.ToVector();
            GetComponent<NavMeshAgent>().enabled = true;
            GetComponent<ActionSchedular>().CancelCurrentAction();
        }


        IEnumerator AddForceTime(float Durration)
        {
            yield return new WaitForSeconds(Durration);
            PlayerNav.enabled = true;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
  
    }

}