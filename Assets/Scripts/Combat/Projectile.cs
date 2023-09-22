using UnityEngine;
using UnityEngine.Events;
using RPG.Attributes;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] UnityEvent onHit;
        [SerializeField] float LifeAfterImpact = .2f;
        [SerializeField] float MaxLifeTime = 10f;
        [SerializeField] float speed = 1;
        [SerializeField] bool isHoming;
        [SerializeField] GameObject ProjectileImpact;
        [SerializeField] GameObject[] DestroyOnHit = null;
        GameObject Instigator;
        float currentDamage = 0;

        Health currentTarget = null; 
        Vector3 targetPos;

        private void Start()
        {
            if (currentTarget == null) return;
            targetPos = getAimLocal();
            transform.LookAt(targetPos);
        }

        private void Update()
        {
            if (currentTarget == null) return;
            if (isHoming && !currentTarget.isDead())
            {
                targetPos = getAimLocal();
                transform.LookAt(targetPos);
            }

            transform.Translate(Vector3.forward * speed * Time.deltaTime); 
        }

        public void SetTarget(Health target, float damage, GameObject instigator)
        {
            currentDamage = damage;
            currentTarget = target;
            Instigator = instigator;

            Destroy(gameObject, MaxLifeTime);
        }

        private Vector3 getAimLocal()
        {
            return currentTarget.GetComponent<Collider>().bounds.center;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Health>() != currentTarget) return;
            if (currentTarget.isDead()) return;

            if(ProjectileImpact != null)
            {
                Instantiate(ProjectileImpact, getAimLocal(), transform.rotation);

            }
            currentTarget.TakeDamage(currentDamage, Instigator);
            onHit?.Invoke();

            speed = 0;
            foreach (GameObject ToDestroy in DestroyOnHit)
            {
                Destroy(ToDestroy);
            }

            Destroy(gameObject, LifeAfterImpact);
        }
    }
}
