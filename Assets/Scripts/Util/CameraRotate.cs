using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    [SerializeField] float speed;

    void Update()
    {
        transform.eulerAngles += new Vector3(0, speed * Time.deltaTime, 0);
    }
}
