using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using RPG.Control;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        [SerializeField] int SceneToLoad;
        [SerializeField] float FadeTime, LoadTime, WaitTime;
        [SerializeField] Transform spawnPoint;
        SavingWrapper savingWrapper;
        PlayerController playerController;

        enum DestinationId
        {
            A, B, C, D
        }

        private void Start()
        {
            playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            savingWrapper = FindObjectOfType<SavingWrapper>();
        }

        [SerializeField] DestinationId Destination; 

        private void OnTriggerEnter(Collider other)
        {
            if(other.tag == "Player")
            {
                StartCoroutine(Transition());
            }
        }

        private IEnumerator Transition()
        {
            if(SceneToLoad < 0)
            {
                Debug.LogError("SceneLoad out of index");
                yield break;
            }

            SceneFade fade = FindObjectOfType<SceneFade>();

            playerController.enabled = false; 

            yield return fade.FadeOut(FadeTime);
            savingWrapper.Save();

            DontDestroyOnLoad(gameObject);
            yield return SceneManager.LoadSceneAsync(SceneToLoad);

            savingWrapper.Load();
            playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            playerController.enabled = false;

            Portal otherPortal = GetOtherPortal();
            updatePlayer(otherPortal);
            savingWrapper.Save();
            yield return new WaitForSeconds(WaitTime);
            fade.FadeIn(LoadTime);

            playerController.enabled = true;
            Debug.Log("SceneLoaded");
            Destroy(gameObject);
        }

        private void updatePlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<NavMeshAgent>().Warp(otherPortal.spawnPoint.position);
            player.transform.rotation = otherPortal.spawnPoint.rotation;
        }

        private Portal GetOtherPortal()
        {
            foreach(Portal portal in FindObjectsOfType<Portal>())
            {
                if (portal == this || portal.Destination != Destination) continue;

                return portal;
            }

            return null;
        }
    }

}