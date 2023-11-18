using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace UI.DicePanel
{
    // object should be created for every scene where dice can be rolled
    // Panel controls all UI elements and actively communicate with CheckSystem
    public class DicePanel : MonoBehaviour
    {
        [SerializeField] private Canvas dicePanelCanvas;
        [SerializeField] private TMP_Text titlePanel;
        [SerializeField] private TMP_Text subTitlePanel;
        [SerializeField] private TMP_Text resultText;
        [SerializeField] private TMP_Text clickText;
        [SerializeField] private TMP_Text difficultyNumberText;

        [SerializeField] private Button diceButton;
        [SerializeField] private GameObject finalDice;
        [SerializeField] private GameObject addonsPanel;
        [SerializeField] private Button continueButton;

        [SerializeField] private Sprite[] diceSprites;
        
        private List<GameObject> _addonElementsPool = new ();
        
        // can be injected properly in full game
        private CheckSystem _checkSystem;
        private void Awake()
        {
            foreach (Transform element in addonsPanel.transform)
            {
                _addonElementsPool.Add(element.gameObject);
            }
            
            
            // TODO : Delete. For testing only
            // best to inject it ofc)
            //-----------------------------------------
            _checkSystem = new("TitleText", "SubTitleText", new(), Random.Range(1, 21));
            var testList = new List<int> { 1, 3, 2 };
            Init("Investigation", "Dexterity check", testList, Random.Range(1, 21));
            //-----------------------------------------

        }

        public void Init(string titleText, string subTitleText, List<int> additiveNumbers, int difficulty)
        {
            _checkSystem.Init(titleText, subTitleText, additiveNumbers, difficulty);
            difficultyNumberText.text = difficulty.ToString();
            clickText.gameObject.SetActive(true);
            titlePanel.text = _checkSystem.TitleText;
            subTitlePanel.text = _checkSystem.SubTitleText;
            diceButton.gameObject.SetActive(true);
            diceButton.interactable = true;
            finalDice.gameObject.SetActive(false);
            dicePanelCanvas.gameObject.SetActive(true);
            resultText.gameObject.SetActive(false);
            resultText.transform.localScale = Vector3.one * 0.5f;
            
            diceButton.onClick.AddListener(Roll);

            for (int i = 0; i < additiveNumbers.Count; i++)
            {
                _addonElementsPool[i].gameObject.SetActive(true);
                _addonElementsPool[i].GetComponentInChildren<TMP_Text>().text = additiveNumbers[i].ToString();
            }
        }

        private void Roll()
        {
            _checkSystem.RollTheDice();
            diceButton.gameObject.SetActive(false);
            finalDice.gameObject.SetActive(true);
            int xRandom;
            int yRandom;
            do
            {
                xRandom = Random.Range(-1, 2);
                yRandom = Random.Range(-1, 2);
            } while (xRandom == 0 && yRandom == 0);

            finalDice.transform.DOPunchPosition(new Vector3(150 * xRandom, 150 * yRandom, 0), 2f, 5)
                .SetEase(Ease.InOutBounce).onComplete = OnEndRoll;
        }

        private void OnEndRoll()
        {
            diceButton.interactable = false;
            diceButton.gameObject.GetComponent<Image>().sprite = diceSprites[_checkSystem.RollResult];
            diceButton.gameObject.SetActive(true);
            finalDice.gameObject.SetActive(false);
            continueButton.gameObject.SetActive(true);
            clickText.gameObject.SetActive(false);
            resultText.text = _checkSystem.CheckResult ? "SUCCESS" : "FAILURE";
            resultText.color = _checkSystem.CheckResult ? Color.green : Color.red;
            resultText.gameObject.SetActive(true);
            resultText.transform.DOScale(Vector3.one, 1f).SetEase(Ease.InOutBack);
            foreach (var element in _addonElementsPool)
            {
                element.gameObject.SetActive(false);
            }

            RemoveListeners();
        }
        
        // TODO : FOR TESTING ONLY
        //-----------------------------------------
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                var testList = new List<int>();
                int i = Random.Range(0, 5);
                while (i != 0 && testList.Count != 8)
                {
                    testList.Add(i);
                    i = Random.Range(0, 5);
                }
                Init("Investigation", "Dexterity check", testList, Random.Range(1, 21));
            }
        }
        //-----------------------------------------

        private void RemoveListeners()
        {
            diceButton.onClick.RemoveListener(Roll);
        }

        private void Disable()
        {
            RemoveListeners();
            dicePanelCanvas.gameObject.SetActive(false);
        }
    }
}
