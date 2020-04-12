using System;
using Dash.Scripts.Config;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dash.Scripts.UIManager
{
    public class GuanQiaUIManager : MonoBehaviour
    {
        public Button piPei;
        public Button duiWu;
        public TextMeshProUGUI title;
        public Button left;
        public Button right;
        public AudioSource audioSource;
        public Animator animator;
        public Image image;
        public TextMeshProUGUI textContent;
        public DuiWuListUIManager duiWuList;

        private void Awake()
        {
            duiWu.onClick.AddListener(() => { duiWuList.Open(); });
        }

        private int currentId;

        public void Open()
        {
            animator.Play("Fade-in");
            Load(0);
        }

        public void Close()
        {
            animator.Play("Fade-out");
            audioSource.Stop();
        }

        private void Load(int id)
        {
            currentId = id;
            var info = GameInfoManager.guanQiaInfoTable[id];
            audioSource.clip = info.music;
            image.sprite = info.image;
            audioSource.time = 0;
            audioSource.Play();
            animator.Play("Load-in");
            title.text = info.displayName;
            if (info.miaoShu)
            {
                textContent.text = info.miaoShu.text;
            }
            else
            {
                textContent.text = "";
            }
        }
    }
}
