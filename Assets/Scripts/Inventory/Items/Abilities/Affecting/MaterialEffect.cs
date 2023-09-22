using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RPG.Abilities
{

    [CreateAssetMenu(fileName = "Material Effect", menuName = "Items/Abilities/Effects/Material")]
    public class MaterialEffect : EffectStrategy
    {
        [Serializable]
        public struct MatValues
        {
            public string Name;
            public float FinalValue;
            public float Speed;
        }

        [SerializeField] Material mat;
        [SerializeField] MatValues[] Changes;
        [SerializeField] Material[] OldMaterials;
        [SerializeField] bool AffectUser;
        [SerializeField] float Durration;
        [SerializeField] string RevertString = "_Alpha";
        [SerializeField] float RevertSpeed = 1;

        GameObject[] parents;

        public override void StartEffect(AbilityData data, Action finished)
        {
            if (AffectUser)
            {
                parents = new GameObject[1];
                parents[0] = data.GetUser();
            }
            else 
                parents = data.getTargets().ToArray();

            for (int i = 0; i < parents.Length; i++)
            {
                ChangeMat(parents[i]);
            }

            data.StartCoroutine(Return(Durration, mat));
        }

        void ChangeMat(GameObject parent)
        {
            Renderer[] ChildRend = parent.GetComponentsInChildren<SkinnedMeshRenderer>();
            OldMaterials = new Material[ChildRend.Length];
            for (int i = 0; i < ChildRend.Length; i++)
            {
                OldMaterials[i] = ChildRend[i].material;
                ChildRend[i].material = mat;
            }
        }

        void RevertMat(GameObject parent)
        {
            Renderer[] ChildRend = parent.GetComponentsInChildren<SkinnedMeshRenderer>();
            for (int i = 0; i < ChildRend.Length; i++)
            {
                ChildRend[i].material = OldMaterials[i];
            }
        }

        IEnumerator Return(float Durration_, Material newMat)
        {
            for (int i = 0; i < Changes.Length; i++)
            {
                newMat.SetFloat(Changes[i].Name, 0);
            }

            float t = 0;
            while (Durration_ > t)
            {
                for (int i = 0; i < Changes.Length; i++)
                {
                    newMat.SetFloat(Changes[i].Name, Mathf.Lerp(newMat.GetFloat(Changes[i].Name), Changes[i].FinalValue, Changes[i].Speed * Time.deltaTime));
                }
                t += Time.deltaTime;
                yield return null;
            }

            if (!String.IsNullOrWhiteSpace(RevertString))
            {
                for (int i = 0; i < OldMaterials.Length; i++)
                {
                    OldMaterials[i].SetFloat(RevertString, 0);
                }
                OldMaterials[0].SetFloat(RevertString, 0);
                for (int i = 0; i < parents.Length; i++)
                {
                    RevertMat(parents[i]);
                }

                t = 0;

                while (t < 1)
                {
                    for (int i = 0; i < OldMaterials.Length; i++)
                    {
                        OldMaterials[i].SetFloat(RevertString, t);
                    }
                    t += Time.deltaTime * RevertSpeed;
                    yield return null;
                }
            }
            else
            {
                for (int i = 0; i < parents.Length; i++)
                {
                    RevertMat(parents[i]);
                }

                for (int i = 0; i < OldMaterials.Length; i++)
                {
                    OldMaterials[i].SetFloat(RevertString, 1);
                }
            }

            yield return new WaitForFixedUpdate();
        }
    }
}
