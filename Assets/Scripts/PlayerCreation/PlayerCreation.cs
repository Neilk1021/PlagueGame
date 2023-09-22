using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.PlayerCreation
{
    public class PlayerCreation : MonoBehaviour
    {
        [SerializeField] Material[] playerMat;

        [Header("Colors")]
        [SerializeField] Color[] SkinTones;
        [SerializeField] Color[] StubleTones;
        [SerializeField] Color[] HairTones;
        [SerializeField] Color[] EyeColor;
        [SerializeField] Color[] BodyPaintColor;

        [Header("Meshes")]
        [SerializeField] GameObject[] HairPiecesMale;
        [SerializeField] GameObject[] HeadPiecesMale;

        int CurrentSkinTone = 0;
        int CurrentHairTone = 0;
        int CurrentEyeColor = 0;
        int CurrentBodyPaintColor = 0;

        int CurrentHairPiece = 0;
        int CurrentHeadPiece = 0;

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Y))
            {
                if (CurrentSkinTone == SkinTones.Length - 1)
                    CurrentSkinTone = 0;
                else
                    CurrentSkinTone++;

                for (int i = 0; i < playerMat.Length; i++)
                {
                    playerMat[i].SetColor("_Color_Skin", SkinTones[CurrentSkinTone]);
                    playerMat[i].SetColor("_Color_Stubble", StubleTones[CurrentSkinTone]);
                }
            }

            if (Input.GetKeyUp(KeyCode.U))
            {
                if (CurrentHairTone == HairTones.Length - 1)
                    CurrentHairTone = 0;
                else
                    CurrentHairTone++;

                for (int i = 0; i < playerMat.Length; i++)
                {
                    playerMat[i].SetColor("_Color_Hair", HairTones[CurrentHairTone]);
                }
            }

            if (Input.GetKeyUp(KeyCode.I))
            {
                if (CurrentEyeColor == EyeColor.Length - 1)
                    CurrentEyeColor = 0;
                else
                    CurrentEyeColor++;

                for (int i = 0; i < playerMat.Length; i++)
                {
                    playerMat[i].SetColor("_Color_Eyes", EyeColor[CurrentEyeColor]);
                }
            }

            if (Input.GetKeyUp(KeyCode.O))
            {
                HairPiecesMale[CurrentHairPiece].SetActive(false);

                if (CurrentHairPiece == HairPiecesMale.Length - 1)
                    CurrentHairPiece = 0;
                else
                    CurrentHairPiece++;

                HairPiecesMale[CurrentHairPiece].SetActive(true);

            }

            if (Input.GetKeyUp(KeyCode.P))
            {
                HeadPiecesMale[CurrentHeadPiece].SetActive(false);

                if (CurrentHeadPiece == HeadPiecesMale.Length - 1)
                    CurrentHeadPiece = 0;
                else
                    CurrentHeadPiece++;

                HeadPiecesMale[CurrentHeadPiece].SetActive(true);

            }
        }
    }
}