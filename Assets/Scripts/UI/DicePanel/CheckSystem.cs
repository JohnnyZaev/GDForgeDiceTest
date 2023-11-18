using System;
using System.Collections.Generic;

namespace UI.DicePanel
{
    public class CheckSystem
    {
        public string TitleText;
        public string SubTitleText;
        private Dice _dice;
        public int RollResult;
        public bool CheckResult;
        public List<int> AdditiveNumbers;

        private int _sides;
        private int _currentDifficulty;

        public event Action OnStartRolling;

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
            CheckResult = RollResult >= _currentDifficulty;
        }
        
    }
}
