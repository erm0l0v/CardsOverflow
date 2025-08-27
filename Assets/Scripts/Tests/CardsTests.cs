using Cards;
using NUnit.Framework;

namespace Tests
{
    public class CardsTests
    {
        [TestCase("10♣", "9♣", ExpectedResult = true)]
        [TestCase("A♣", "K♣", ExpectedResult = true)]
        public bool Card_Compare(string left, string right)
            => new Card(left).CompareTo(new Card(right)) == 1;
    }
}
