using System.Linq;
using Cards;
using NUnit.Framework;

namespace Tests
{
    public class ChangedHandRulesMoreTests
    {
        private static CombinationsConfig Cfg(params (HandCategory cat, uint mult)[] rules) =>
            new CombinationsConfig(rules.ToDictionary(x => x.cat, x => x.mult));

        private static uint ScoreOf(CombinationsConfig cfg, string s) =>
            new Hand(s).GetScore(cfg).Score;

        private static bool LeftGreater(CombinationsConfig cfg, string left, string right) =>
            ScoreOf(cfg, left) > ScoreOf(cfg, right);

        [Test]
        public void HighCard_Beats_StraightFlush_When_HighCardBoosted()
        {
            var cfg = Cfg((HandCategory.HighCard, 1000));

            Assert.IsTrue(LeftGreater(cfg,
                "A♠,8♣,Q♦,J♠,10♠",
                "9♠,8♠,7♠,6♠,5♠"
            ));

            var defaultCfg = Cfg();
            Assert.IsFalse(LeftGreater(defaultCfg,
                "A♠,8♣,Q♦,J♠,10♠",
                "9♠,8♠,7♠,6♠,5♠"
            ));
        }

        [Test]
        public void OnePair_Tops_All()
        {
            var cfg = Cfg((HandCategory.OnePair, 2000));

            Assert.IsTrue(LeftGreater(cfg,
                "A♣,A♦,K♣,Q♦,J♠",
                "K♠,Q♠,J♠,10♠,9♠"
            ));

            Assert.IsTrue(LeftGreater(cfg,
                "A♣,A♦,K♣,Q♦,J♠",
                "A♥,A♠,K♦,Q♣,10♠"
            ));
        }

        [Test]
        public void Flush_Beats_FullHouse_When_FullHouse_Demoted()
        {
            var cfg = Cfg(
                (HandCategory.Flush, 500),
                (HandCategory.FullHouse, 100)
            );

            Assert.IsTrue(LeftGreater(cfg,
                "A♣,J♣,9♣,7♣,5♣",
                "Q♦,Q♣,Q♥,2♠,2♣"
            ));
        }

        [Test]
        public void FiveOfAKind_Tops_RoyalFlush_When_Boosted()
        {
            var cfg = Cfg((HandCategory.FiveOfAKind, 3000));

            Assert.IsTrue(LeftGreater(cfg,
                "10♣,10♣,10♣,10♣,10♣",
                "A♠,K♠,Q♠,J♠,10♠"
            ));
        }

        [Test]
        public void FiveOfAKind_Aces_Beats_FiveOfAKind_Kings()
        {
            var cfg = Cfg((HandCategory.FiveOfAKind, 3000));

            Assert.IsTrue(LeftGreater(cfg,
                "A♣,A♣,A♣,A♣,A♣",
                "K♣,K♣,K♣,K♣,K♣"
            ));
        }

        [Test]
        public void StraightFlush_Beats_FiveOfAKind_When_StraightFlush_Boosted()
        {
            var cfg = Cfg(
                (HandCategory.StraightFlush, 1500),
                (HandCategory.FiveOfAKind, 100)
            );

            Assert.IsTrue(LeftGreater(cfg,
                "9♠,8♠,7♠,6♠,5♠",
                "Q♦,Q♦,Q♦,Q♦,Q♦"
            ));
        }

        [Test]
        public void Boosting_HighCard_DoesNotChange_Order_Inside_HighCard()
        {
            var cfg = Cfg((HandCategory.HighCard, 5000));

            Assert.IsTrue(LeftGreater(cfg,
                "A♣,K♦,9♥,5♠,3♣",
                "A♦,K♣,9♠,5♥,2♦"
            ));
        }

        [Test]
        public void Equal_Multipliers_Favor_Raw_Value()
        {
            var cfg = Cfg(
                (HandCategory.Straight, 800),
                (HandCategory.Flush, 800)
            );

            Assert.IsTrue(LeftGreater(cfg,
                "A♣,J♣,9♣,7♣,5♣",
                "K♦,Q♣,J♥,10♠,9♣"
            ));
        }

        [Test]
        public void Input_Order_Does_Not_Matter()
        {
            var cfg = Cfg((HandCategory.HighCard, 1000));

            var a = "A♣,K♦,Q♥,J♠,10♣";
            var b = "10♣,J♠,Q♥,K♦,A♣";

            Assert.AreEqual(ScoreOf(cfg, a), ScoreOf(cfg, b));
        }

        [Test]
        public void Trips_TieBreaks_Work_When_Trips_Boosted()
        {
            var cfg = Cfg((HandCategory.ThreeOfAKind, 1200));

            Assert.IsTrue(LeftGreater(cfg,
                "7♣,7♦,7♥,A♠,K♣",
                "7♣,7♦,7♥,A♠,Q♣"
            ));
        }

        [Test]
        public void Unspecified_Categories_Use_Default_Order()
        {
            var cfg = Cfg((HandCategory.HighCard, 1));

            Assert.IsFalse(LeftGreater(cfg,
                "A♠,K♦,9♥,5♠,3♣",
                "9♠,8♠,7♠,6♠,5♠"
            ));
        }
    }
}