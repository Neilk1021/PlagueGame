using UnityEngine;
using UnityEngine.Playables;
using RPG.Core;
using RPG.Control;

namespace RPG.Cinematics
{
    public class CinematicControl : MonoBehaviour
    {
        GameObject player; 
        private void Awake()
        {
            player = GameObject.FindWithTag("Player") ;
        }

        private void OnEnable()
        {
            GetComponent<PlayableDirector>().played += DisableCtrl;
            GetComponent<PlayableDirector>().stopped += EnableCtrl;
        }

        private void OnDisable()
        {
            GetComponent<PlayableDirector>().played -= DisableCtrl;
            GetComponent<PlayableDirector>().stopped -= EnableCtrl;
        }

        void DisableCtrl(PlayableDirector playableDirector)
        {
            Debug.Log("e");
            player.GetComponent<ActionSchedular>().CancelCurrentAction();
            player.GetComponent<PlayerController>().enabled = false;
        }

        void EnableCtrl(PlayableDirector playableDirector)
        {
            player.GetComponent<PlayerController>().enabled = true;
        }
    }
}
