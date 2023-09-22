using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using RPG.Attributes;
using RPG.Utils;
using RPG.Dialogue;
using System;

namespace RPG.Control
{
    public class AiController : MonoBehaviour
    {
        [SerializeField] float wayPointTolerance = 0.5f; 
        [SerializeField] float EngageDistance = 5f;
        [SerializeField] float shoutDistance = 5f;
        [Range(0,1)]
        [SerializeField] float PatrolSpeedFraction = 0.2f;
        [SerializeField] PatrolPath path;
        GameObject player;
        Fighter fighter;
        Health health;
        Move move;

        LazyValue<Vector3> gaurdLocation;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeSinceLastAggrivated = Mathf.Infinity;
        float timeAtWayPoint = 0;
        [SerializeField] float timeToForget = 3f;
        [SerializeField] float timeToDeagro = 5f;
        [SerializeField] float timeToWait = 2f;
        int wayPointIndex = 0;

        [SerializeField] bool nonHostile = false;

        private void Awake()
        {
            move = GetComponent<Move>();
            health = GetComponent<Health>();
            player = GameObject.FindWithTag("Player");
            fighter = GetComponent<Fighter>();
            gaurdLocation = new LazyValue<Vector3>(InitialLoc);
        }
        private Vector3 InitialLoc()
        {
            return transform.position;
        }

        private void Start()
        {
            if(nonHostile) GetComponent<CombatTarget>().canAttack = false;
            gaurdLocation.ForceInit();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, EngageDistance);

        }

        private void Update()
        {
            if (health.isDead()) return;

            if (IsAggrivated() && fighter.CanAttack(player)){
                fighter.Attack(player);
                timeSinceLastSawPlayer = 0;
                AggravateNearbyEnemies();
            }
            else if(timeSinceLastSawPlayer < timeToForget)
            {
                SuspiciousBehavior();
            }
            else
            {
                GuardBehavior();
            }

            timeSinceLastSawPlayer += Time.deltaTime;
        }

        private void AggravateNearbyEnemies()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0, 8);
            foreach (RaycastHit hit in hits)
            {
                hit.collider.GetComponent<AiController>()?.Aggravate();
            }
        }

        public void Aggravate()
        {
            timeSinceLastAggrivated = 0;
        }

        private void SuspiciousBehavior()
        {
            GetComponent<ActionSchedular>().CancelCurrentAction();
        }

        private void GuardBehavior()
        {
            Vector3 nextPos = gaurdLocation.value;

            if(path != null)
            {
                if (AtWaypoint())
                {
                    timeAtWayPoint += Time.deltaTime;
                    if(timeAtWayPoint >= timeToWait)
                    {
                        CycleWayPoint();
                        timeAtWayPoint = 0;
                    }
                }

                nextPos = GetCurrentWaypoint();
            }

            fighter.Cancel();
            move.StartMove(nextPos, PatrolSpeedFraction);
        }

        public void setEngageDistance(float Dist)
        {
            nonHostile = false;
            GetComponent<CombatTarget>().canAttack = true;
            GetComponent<DialogueTrigger>()?.disable();
            EngageDistance = Dist;
        }

        private Vector3 GetCurrentWaypoint()
        {
           return path.GetWayPoint(wayPointIndex);
        }

        private void CycleWayPoint()
        {
            wayPointIndex = path.GetNexIndex(wayPointIndex);
        }

        private bool AtWaypoint()
        {
            float distanceToWayPoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            if(distanceToWayPoint <= wayPointTolerance)
            {
                return true;
            }
            return false;
        }

        private bool IsAggrivated()
        {
            if(timeSinceLastAggrivated < timeToDeagro)
            {
                timeSinceLastAggrivated += Time.deltaTime;
                return true;
            }

            return Vector3.Distance(transform.position, player.transform.position) <= EngageDistance;
        }
    }
}