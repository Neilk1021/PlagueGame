using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverseNotDoneMove : MonoBehaviour
{
  /*  void UpdateAnim()
    {
        PlayerNav.enabled = !health.isDead();

        Vector3 localVelocity = transform.InverseTransformDirection(PlayerNav.velocity);

        Anim.SetFloat("Blend", localVelocity.z);
        if (InverseKinematics)
        {
            if (!runningLeft && !runningRight && Vector3.Distance(pos[1].position, LegTargets[1].position) > StepDistance)
            {
                StartCoroutine(UpdatePosLeft(pos[1].position));
                runningLeft = true;
            }

            if (!runningRight && !runningLeft && Vector3.Distance(pos[0].position, LegTargets[0].position) > StepDistance)
            {
                StartCoroutine(UpdatePosRight(pos[0].position));
                runningRight = true;
            }
            LegTargets[0].position = LastLegPos[0];
            LegTargets[1].position = LastLegPos[1];
        }



        [Header("InverseWalking")]
        [SerializeField] float StepSpeed = 0.5f;
        [SerializeField] float StepDistance = 0.5f;
        [SerializeField] Transform mesh;

        public InverseKinematics[] Legs;
    public Transform[] LegTargets;
    public Vector3[] LastLegPos;

    public Transform[] pos;
    Vector3 velocityL;
    Vector3 velocityR;
    bool runningLeft = false;
    bool runningRight = false;
    void Awake()
    {
        health = GetComponent<Health>();
        Anim = GetComponent<Animator>();
        PlayerNav = GetComponent<NavMeshAgent>();
        Legs = GetComponentsInChildren<InverseKinematics>();

        int Count = 0;
        for (int i = 0; i < Legs.Length; i++)
        {
            if (!Legs[i].IsFoot)
            {
                LegTargets[Count] = Legs[i].GetTarget();
                LastLegPos[Count] = LegTargets[Count].position;
                Count++;
            }
        }
    }

}*/
}
