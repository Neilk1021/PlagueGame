using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class ActionSchedular : MonoBehaviour
    {
        IAction Last_Action;

        public void StartAction(IAction action)
        {
            if (Last_Action == action) return;

            if (Last_Action != null)
            {
                Last_Action.Cancel();
            }

            Last_Action = action;  
        }

        public void CancelCurrentAction()
        {
            StartAction(null);
        }
    }

}