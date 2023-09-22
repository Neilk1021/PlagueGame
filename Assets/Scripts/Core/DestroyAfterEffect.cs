using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class DestroyAfterEffect : MonoBehaviour
    {
        [SerializeField] GameObject ParentToDestroy = null;

        ParticleSystem ps;
        private void Start()
        {
            ps = GetComponent<ParticleSystem>();
        }

        private void Update()
        {
            if(!ps.IsAlive()){
                if(ParentToDestroy != null)
                {
                    Destroy(ParentToDestroy);
                }

                Destroy(gameObject);
            }
        }
    }

}