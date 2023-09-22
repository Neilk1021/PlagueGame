using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Utils
{
    public class MousePos : MonoBehaviour
    {
        /// <summary>
        /// Gets mouse position in world where z = 0
        /// </summary>
        /// <returns>Vector 2 of mouse posistion (Z = 0)</returns>
        public static Vector3 getMouseWorldPos()
        {
            Vector3 vec = getMouseWorldPosWithZ(Input.mousePosition, Camera.main);
            vec.z = 0;
            return vec;
        }

        public static Vector3 getMouseWorldPosWithZ() { return getMouseWorldPosWithZ(Input.mousePosition, Camera.main); }

        public static Vector3 getMouseWorldPosWithZ(Camera worldCamera) { return getMouseWorldPosWithZ(Input.mousePosition, worldCamera); }

        public static Vector3 getMouseWorldPosWithZ(Vector3 screenPos, Camera worldCamera)
        {
            Vector3 worldPos = worldCamera.ScreenToWorldPoint(screenPos);
            return worldPos;
        }
    }

}