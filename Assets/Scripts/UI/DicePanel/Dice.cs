using UnityEngine;

namespace UI.DicePanel
{
    public class Dice
    {
        private readonly int Sides;

        public Dice(int sides)
        {
            Sides = sides;
        }

        public int Roll()
        {
            return Random.Range(0, Sides);
        }
    }
}
