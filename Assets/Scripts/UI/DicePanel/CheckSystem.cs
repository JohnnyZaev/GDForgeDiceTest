using System;
using System.Collections.Generic;

namespace UI.DicePanel
{
    /// <summary>
    /// System for dice check mechanic
    /// </summary>
    public class CheckSystem
    {
        public string TitleText;
        public string SubTitleText;
        private Dice _dice;

        public int RollResult
        {
            get => _rollResult;
            set
            {
                _rollResult = value < 20 ? value : 19;
                CheckResult = RollResult >= _currentDifficulty - 1;
            }
        }

        public bool CheckResult;
        public List<int> AdditiveNumbers;

        private int _sides;
        private int _currentDifficulty;
        private int _rollResult;

        public event Action OnStartRolling;
        public event Action OnFinishCheck;

        public CheckSystem(string titleText, string subTitleText, List<int> additiveNumbers, int difficulty)
        {
            TitleText = titleText;
            SubTitleText = subTitleText;
            AdditiveNumbers = additiveNumbers;
            _currentDifficulty = difficulty;
        }
        
        //every system init function
        public void Init(string titleText, string subTitleText, List<int> additiveNumbers, int difficulty)
        {
            TitleText = titleText;
            SubTitleText = subTitleText;
            AdditiveNumbers = additiveNumbers;
            _currentDifficulty = difficulty;

            // open for next changes
            _sides = 20;
            _dice = new Dice(_sides);
        }

        public void RollTheDice()
        {
            OnStartRolling?.Invoke();
            RollResult = _dice.Roll();
        }

        public void FinishCheck()
        {
            OnFinishCheck?.Invoke();
        }
    }
}
