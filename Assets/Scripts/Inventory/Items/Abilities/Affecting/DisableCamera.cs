using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace RPG.Abilities{
    [CreateAssetMenu(fileName = "Disable Camera Effect", menuName = "Items/Abilities/Effects/DisableCamera")]
    public class DisableCamera : EffectStrategy
    {
        [SerializeField] float Durration;
        [SerializeField] float Reduction;
        float Default;
        public override void StartEffect(AbilityData data, Action finished)
        {
            if (data.GetCamera() == null) { data.SetCamera(GameObject.FindGameObjectWithTag("FollowCam").GetComponent<CinemachineVirtualCamera>()); }

            Default = data.GetCamera().GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping;

            data.GetCamera().GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping = Reduction;
            data.GetCamera().GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping = Reduction;
            data.GetCamera().GetCinemachineComponent<CinemachineFramingTransposer>().m_ZDamping = Reduction;
            data.StartCoroutine(Enable(Durration, Default, data));
        }

        public IEnumerator Enable(float Durration_, float Restore_, AbilityData data)
        {
            float current = data.GetCamera().GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping;
            yield return new WaitForSeconds(Durration_);

            float t = 0;

            while (t < 3)
            {
                float point = Mathf.Lerp(current, Restore_, t/3);
                data.GetCamera().GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping = point;
                data.GetCamera().GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping = point;
                data.GetCamera().GetCinemachineComponent<CinemachineFramingTransposer>().m_ZDamping = point;
                t += Time.deltaTime;
            }
        }

    }
}
