using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ShowHideUI : MonoBehaviour
{
    [SerializeField] GameObject Ui;

    [SerializeField] KeyCode activateKey;

    public void changeVisibility()
    {
        bool val = Ui.activeInHierarchy;

        Ui.SetActive(!val);
    }

    private void Update()
    {
        if (Input.GetKeyDown(activateKey)) { changeVisibility(); }
    }
}
