using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;
namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        const string defaultSaveFile = "Save";
        SavingSystem saveSystem;
        [SerializeField] float FadeTime = 2f;

        public void Awake()
        {
            saveSystem = GetComponent<SavingSystem>();
            StartCoroutine(LoadLastScene());
        }

        private IEnumerator LoadLastScene()
        {
            yield return saveSystem.LoadLastScene(defaultSaveFile);
            SceneFade fade = FindObjectOfType<SceneFade>();
            fade.InstantFadeOut();

            yield return fade.FadeIn(FadeTime);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
                Save();

            if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }
        }

        public void Load()
        {
            saveSystem.Load(defaultSaveFile);
        }

        public void Save()
        {
            saveSystem.Save(defaultSaveFile);
        }
    }
}