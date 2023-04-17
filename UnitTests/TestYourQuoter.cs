using NUnit.Framework;
using QuoterApp;

namespace UnitTests
{
    public class TestYourQuoter
    {
        [Test]
        public void GetQuote_ReturnsCorrectQuote()
        {
            var quoter = new YourQuoter();
            var instrumentId = "DK50782120";
            var quantity = 100;

            var quote = quoter.GetQuote(instrumentId, quantity);

            Assert.That(quote, Is.EqualTo(9981));
        }

        [Test]
        public void GetQuote_NoMarketOrdersFound_ThrowsException()
        {
            var quoter = new YourQuoter();
            var instrumentId = "TESTINSTRUMENT";
            var quantity = 100;

            Assert.That(() => quoter.GetQuote(instrumentId, quantity), Throws.Exception);
        }

        [Test]
        public void GetQuote_NotEnoughMarketOrders_ThrowsException()
        {
            var quoter = new YourQuoter();
            var instrumentId = "DK50782120";
            var quantity = 100000;

            Assert.That(() => quoter.GetQuote(instrumentId, quantity), Throws.Exception);
        }

        [Test]
        public void GetVolumeWeightedAveragePrice_ReturnsCorrectPrice()
        {
            var quoter = new YourQuoter();
            var instrumentId = "DK50782120";

            var price = quoter.GetVolumeWeightedAveragePrice(instrumentId);

            Assert.That(price, Is.EqualTo(99.94));
        }

        [Test]
        public void GetVolumeWeightedAveragePrice_NoMarketOrdersFound_ThrowsException()
        {
            var quoter = new YourQuoter();
            var instrumentId = "TESTINSTRUMENT";

            Assert.That(() => quoter.GetVolumeWeightedAveragePrice(instrumentId), Throws.Exception);
        }
    }
}