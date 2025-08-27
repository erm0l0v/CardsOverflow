using System.Collections.Generic;

namespace Cards
{
    public class CombinationsConfig
    {
        private Dictionary<HandCategory, uint> scores;

        public CombinationsConfig(Dictionary<HandCategory, uint> scores)
        {
            this.scores = scores;
        }

        public uint GetScore(HandCategory handCategory)
        {
            if (scores.TryGetValue(handCategory, out var score))
            {
                return score;
            }

            return (uint)handCategory;
        }
    }
}