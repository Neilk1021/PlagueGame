using UnityEngine;
using UnityEngine.Playables;
using RPG.Saving;

namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour, ISaveable
    {
        bool hasPassed = false;

        public object CaptureState()
        {
            return hasPassed;
        }

        public void RestoreState(object state)
        {
            hasPassed = (bool)state;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(!hasPassed && other.tag == "Player")
            {
                GetComponent<PlayableDirector>().Play();
                hasPassed = true; 
            }
        }
    }
}