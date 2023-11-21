using UnityEngine;

namespace UI.DicePanel
{
    /// <summary>
    /// Simple dice logic implementation
    /// </summary>
    public class Dice
    {
        private readonly int _sides;

        public Dice(int sides)
        {
            _sides = sides;
        }

        public int Roll()
        {
            return Random.Range(0, _sides);
        }
    }
}
