using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project._01_Scripts
{
    public class UIManager : MonoBehaviour
    {
        [Header("New Order Popup")]
        [SerializeField] private GameObject newOrderPopup;
        [SerializeField] private float newOrderDisplaySeconds = 1.5f;
        [SerializeField] private Image[] metalPreviewSlots;
        [SerializeField] private Image[] guardPreviewSlots;
        [SerializeField] private Image[] hiltPreviewSlots;

        [Header("Start Screen")]
        [SerializeField] private GameObject startScreenCanvas;
        
        [Header("End Screen")]
        [SerializeField] private GameObject endScreenCanvas;
        [SerializeField] private TMP_Text finalScoreText;

        [Header("Status Text")]
        [SerializeField] private TMP_Text statusText;
        [SerializeField] private string workingMessage = "Weapon In Progress!";
        [SerializeField] private string readyMessage = "Ready to go!";

        public void SetStatus(bool isReady)
        {
            if (statusText == null)
            {
                return;
            }

            statusText.text = isReady ? readyMessage : workingMessage;
        }
        
        void Awake()
        {
            if (newOrderPopup != null) newOrderPopup.SetActive(false);
            if (endScreenCanvas != null) endScreenCanvas.SetActive(false);
            if (startScreenCanvas != null) startScreenCanvas.SetActive(true);
        }

        public void HideStartScreen()
        {
            if (startScreenCanvas != null)
            {
                startScreenCanvas.SetActive(false);
            }
        }

        public void ShowNewOrderPopup(List<Sprite> topSprites, List<Sprite> middleSprites, List<Sprite> bottomSprites)
        {
            if (newOrderPopup == null)
            {
                return;
            }

            FillSlots(metalPreviewSlots, topSprites);
            FillSlots(guardPreviewSlots, middleSprites);
            FillSlots(hiltPreviewSlots, bottomSprites);

            StopAllCoroutines();
            StartCoroutine(NewOrderRoutine());
        }

        void FillSlots(Image[] slots, List<Sprite> sprites)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (i < sprites.Count)
                {
                    slots[i].sprite = sprites[i];
                    slots[i].enabled = true;
                }
                else
                {
                    slots[i].enabled = false;
                }
            }
        }

        IEnumerator NewOrderRoutine()
        {
            newOrderPopup.SetActive(true);
            yield return new WaitForSeconds(newOrderDisplaySeconds);
            newOrderPopup.SetActive(false);
        }

        public void ShowEndScreen(int finalScore)
        {
            if (endScreenCanvas == null)
            {
                return;
            }

            endScreenCanvas.SetActive(true);

            if (finalScoreText != null)
            {
                finalScoreText.text = $"{finalScore}";
            }
        }

        public void HideEndScreen()
        {
            if (endScreenCanvas != null)
            {
                endScreenCanvas.SetActive(false);
            }
        }
    }
}