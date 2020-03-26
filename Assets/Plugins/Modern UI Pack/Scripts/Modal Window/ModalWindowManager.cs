using System;
using UnityEngine;

namespace Michsky.UI.ModernUIPack
{
    public class ModalWindowManager : MonoBehaviour
    {
        Animator mwAnimator;
        
        private void Awake()
        {
            mwAnimator = GetComponent<Animator>();
        }

        public void OpenWindow()
        {
            mwAnimator.Play("Fade-in");
        }

        public void CloseWindow()
        {
            mwAnimator.Play("Fade-out");
        }
    }
}