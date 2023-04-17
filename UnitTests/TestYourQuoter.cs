using NUnit.Framework;
using QuoterApp;

namespace UnitTests
{
    public class TestYourQuoter
    {
        [Test]
        public void GetQuote_ReturnsCorrectQuote()
        {
            // Arrange
            var quoter = new YourQuoter();
            var instrumentId = "DK50782120";
            var quantity = 100;

            // Act
            var quote = quoter.GetQuote(instrumentId, quantity);

            // Assert
            Assert.That(quote, Is.EqualTo(9981));
        }

        [Test]
        public void GetQuote_NoMarketOrdersFound_ThrowsException()
        {
            // Arrange
            var quoter = new YourQuoter();
            var instrumentId = "TESTINSTRUMENT";
            var quantity = 100;

            // Act & Assert
            Assert.That(() => quoter.GetQuote(instrumentId, quantity), Throws.Exception);
        }

        [Test]
        public void GetQuote_NotEnoughMarketOrders_ThrowsException()
        {
            // Arrange
            var quoter = new YourQuoter();
            var instrumentId = "DK50782120";
            var quantity = 100000;

            // Act & Assert
            Assert.That(() => quoter.GetQuote(instrumentId, quantity), Throws.Exception);
        }

        [Test]
        public void GetVolumeWeightedAveragePrice_ReturnsCorrectPrice()
        {
            // Arrange
            var quoter = new YourQuoter();
            var instrumentId = "DK50782120";

            // Act
            var price = quoter.GetVolumeWeightedAveragePrice(instrumentId);

            // Assert
            Assert.AreEqual(99.94, price);
        }

        [Test]
        public void GetVolumeWeightedAveragePrice_NoMarketOrdersFound_ThrowsException()
        {
            // Arrange
            var quoter = new YourQuoter();
            var instrumentId = "TESTINSTRUMENT";

            // Act & Assert
            Assert.That(() => quoter.GetVolumeWeightedAveragePrice(instrumentId), Throws.Exception);
        }
    }
}