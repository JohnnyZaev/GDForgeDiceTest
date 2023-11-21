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
        [Header("UIObjects")] 
        [SerializeField] private Canvas dicePanelCanvas;
        
        [SerializeField] private TMP_Text titlePanel;
        [SerializeField] private TMP_Text subTitlePanel;
        [SerializeField] private TMP_Text resultText;
        [SerializeField] private TMP_Text clickText;
        [SerializeField] private TMP_Text difficultyNumberText;

        [SerializeField] private Button diceButton;
        [SerializeField] private Button continueButton;
        
        [SerializeField] private GameObject finalDice;
        [SerializeField] private GameObject addonsPanel;

        [Header("Sprites")]
        [SerializeField] private Sprite[] diceSprites;

        [Header("DesignersChoice")] 
        [SerializeField] private float resultTextStarterSize = 0.5f;
        [SerializeField] private float boardRadius = 150f;
        [SerializeField] private float diceRollDuration = 2f;
        [SerializeField] private int diceRollVibrato = 5;
        [SerializeField] private float elementsAnimationDuration = 0.5f;
        [SerializeField] private float diceNumberUpdateAnimationDuration = 0.5f;
        [SerializeField] private int diceNumberUpdateAnimationVibrato = 2;
        [SerializeField] private float resultCheckTextAnimationDuration = 1f;
        [SerializeField] private string successText = "SUCCESS";
        [SerializeField] private string failureText = "FAILURE";
        
        private readonly List<GameObject> _addonElementsPool = new ();
        private int _addonElementsIndex;
        private Vector3 _diceStarterSize;
        
        // can be injected properly in full game
        private CheckSystem _checkSystem;
        
        private void Awake()
        {
            foreach (Transform element in addonsPanel.transform)
            {
                _addonElementsPool.Add(element.gameObject);
            }

            _diceStarterSize = diceButton.transform.localScale;
            
            
            // TODO : Delete. For testing only
            // best to inject it ofc)
            //-----------------------------------------
            _checkSystem = new("TitleText", "SubTitleText", new(), Random.Range(1, 21));
            var testList = new List<int> { 1, 3, 2 };
            Init("Investigation", "Dexterity check", testList, Random.Range(1, 21));
            //-----------------------------------------

        }

        /// <summary>
        /// Should be called to start dice check
        /// </summary>
        /// <param name="titleText"></param>
        /// <param name="subTitleText"></param>
        /// <param name="additiveNumbers"></param>
        /// <param name="difficulty"></param>
        public void Init(string titleText, string subTitleText, List<int> additiveNumbers, int difficulty)
        {
            AddonElementsInit();

            CheckSystemInit(titleText, subTitleText, additiveNumbers, difficulty);
            DiceButtonInit();
            
            clickText.gameObject.SetActive(true);
            finalDice.gameObject.SetActive(false);
            resultText.gameObject.SetActive(false);
            resultText.transform.localScale = Vector3.one * resultTextStarterSize;
            
            dicePanelCanvas.gameObject.SetActive(true);
            
            continueButton.onClick.AddListener(OnContinueButtonClicked);

            for (var i = 0; i < additiveNumbers.Count; i++)
            {
                _addonElementsPool[i].gameObject.SetActive(true);
                _addonElementsPool[i].GetComponentInChildren<TMP_Text>().text = additiveNumbers[i].ToString();
            }
        }

        private void CheckSystemInit(string titleText, string subTitleText, List<int> additiveNumbers, int difficulty)
        {
            _checkSystem.Init(titleText, subTitleText, additiveNumbers, difficulty);
            difficultyNumberText.text = difficulty.ToString();
            titlePanel.text = _checkSystem.TitleText;
            subTitlePanel.text = _checkSystem.SubTitleText;
        }

        private void AddonElementsInit()
        {
            _addonElementsIndex = -1;
            foreach (var element in _addonElementsPool)
            {
                element.transform.localScale = Vector3.one;
                element.gameObject.SetActive(false);
            }
        }

        private void DiceButtonInit()
        {
            diceButton.onClick.AddListener(Roll);
            diceButton.transform.localScale = _diceStarterSize;
            diceButton.gameObject.SetActive(true);
            diceButton.interactable = true;
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

            finalDice.transform.DOPunchPosition(new Vector3(boardRadius * xRandom, boardRadius * yRandom, 0), diceRollDuration, diceRollVibrato)
                .SetEase(Ease.InOutBounce).onComplete = OnEndRoll;
        }

        private void OnEndRoll()
        {
            UpdateDice();
            finalDice.gameObject.SetActive(false);
            clickText.gameObject.SetActive(false);
            UpgradesAnimation();
        }

        private void UpgradesAnimation()
        {
            if (_addonElementsPool.Count > 0)
            {
                DisableElements();
            }
        }

        private void DisableElements()
        {
            _addonElementsIndex++;
            if (_checkSystem.AdditiveNumbers.Count > _addonElementsIndex + 1)
            {
                _addonElementsPool[_addonElementsIndex].transform.DOScale(Vector3.zero, elementsAnimationDuration).SetEase(Ease.InOutBack).onComplete =
                    DisableElements;
            }
            else
            {
                _addonElementsPool[_addonElementsIndex].transform.DOScale(Vector3.zero, elementsAnimationDuration).SetEase(Ease.InOutBack).onComplete = Disable;
            }

            _checkSystem.RollResult += _checkSystem.AdditiveNumbers[_addonElementsIndex];
            UpdateDice();
        }

        private void UpdateDice()
        {
            diceButton.interactable = false;
            diceButton.gameObject.GetComponent<Image>().sprite = diceSprites[_checkSystem.RollResult];
            diceButton.gameObject.SetActive(true);
            diceButton.transform.DOPunchScale(Vector3.one, diceNumberUpdateAnimationDuration, diceNumberUpdateAnimationVibrato);
        }

        private void RemoveListeners()
        {
            diceButton.onClick.RemoveListener(Roll);
            continueButton.onClick.RemoveListener(OnContinueButtonClicked);
        }

        private void OnContinueButtonClicked()
        {
            dicePanelCanvas.gameObject.SetActive(false);
            _checkSystem.FinishCheck();
            RemoveListeners();
        }

        private void Disable()
        {
            resultText.text = _checkSystem.CheckResult ? successText : failureText;
            resultText.color = _checkSystem.CheckResult ? Color.green : Color.red;
            resultText.gameObject.SetActive(true);
            resultText.transform.DOScale(Vector3.one, resultCheckTextAnimationDuration).SetEase(Ease.InOutBack);
            continueButton.gameObject.SetActive(true);
        }

        // TODO : FOR TESTING ONLY
        //-----------------------------------------
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                var testList = new List<int>();
                int i = Random.Range(0, 5);
                while (i != 0 && testList.Count != 4)
                {
                    testList.Add(i);
                    i = Random.Range(0, 5);
                }
                Init("Investigation", "Dexterity check", testList, Random.Range(1, 21));
            }
            if (Input.GetKeyDown(KeyCode.Escape))
                Application.Quit();
        }
        //-----------------------------------------
    }
}
