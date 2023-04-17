using System;
using System.Collections.Generic;
using System.Threading;

namespace QuoterApp
{
    public class YourQuoter : IQuoter
    {
        private List<MarketOrder> _marketOrders;

        public YourQuoter()
        {
            _marketOrders = new List<MarketOrder>();
            var marketOrderSource = new HardcodedMarketOrderSource();
            RetrieveMarketOrders(marketOrderSource, 5);

            // Refreshed market orders every 10 seconds on a separate thread
            var thread = new Thread(() =>
            {
                while(true)
                {
                    marketOrderSource = new HardcodedMarketOrderSource();
                    RetrieveMarketOrders(marketOrderSource, 60);
                }
            });
            thread.Start();
        }

        public void RetrieveMarketOrders(IMarketOrderSource marketOrderSource, int timeoutLimit = 10)
        {
            var tempOrders = new List<MarketOrder>();
            while (true)
            {
                object _lastOrder = null;
                Thread thread = new Thread(() =>
                {
                    var result = marketOrderSource.GetNextMarketOrder();
                    _lastOrder = result;
                });
                thread.Start();
                thread.Join(TimeSpan.FromSeconds(timeoutLimit));

                var order = (MarketOrder)_lastOrder;
                try
                {
                    if (order == null || order == new MarketOrder())
                    {
                        // No more orders to retrieve, break the loop
                        break;
                    }
                    tempOrders.Add(order);
                }
                catch (Exception e)
                {
                    throw new Exception($"Error while adding order to market orders list: {e.Message}");
                }
            }
            _marketOrders = tempOrders;
        }

        // Calculates the best price for a given quantity of an instrument
        public double GetQuote(string instrumentId, int quantity)
        {
            var orders = new List<MarketOrder>();
            foreach (var order in _marketOrders)
            {
                try
                {
                    if (order.InstrumentId == instrumentId)
                    {
                        orders.Add(order);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error while retrieving market orders for instrument {instrumentId}: {e.Message}");
                }
            }

            if (orders.Count == 0)
            {
                throw new ArgumentException($"No market orders found for instrument {instrumentId}");
            }

            orders.Sort((a, b) => a.Price.CompareTo(b.Price));
            var totalQuantity = 0;
            var totalPrice = 0.0;
            foreach (var o in orders)
            {
                try
                {
                    if (totalQuantity + o.Quantity <= quantity)
                    {
                        totalPrice += o.Quantity * o.Price;
                        totalQuantity += o.Quantity;
                    }
                    else
                    {
                        totalPrice += (quantity - totalQuantity) * o.Price;
                        totalQuantity = quantity;
                        break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error while calculating quote for instrument {instrumentId}: {e.Message}");
                }
            }

            if (totalQuantity < quantity)
            {
                throw new ArgumentException($"Not enough market orders found for instrument {instrumentId} to satisfy the requested quantity of {quantity}");
            }

            return Math.Round(totalPrice, 2);
        }

        // Calculates the volume weighted average price for an instrument
        public double GetVolumeWeightedAveragePrice(string instrumentId)
        {
            var orders = new List<MarketOrder>();
            foreach (var order in _marketOrders)
            {
                try
                {
                    if (order.InstrumentId == instrumentId)
                    {
                        orders.Add(order);
                    }
                }
                catch (Exception e)
                {
                    throw new Exception($"Error while retrieving market orders for instrument {instrumentId}: {e.Message}");
                }
            }

            if (orders.Count == 0)
            {
                throw new ArgumentException($"No market orders found for instrument {instrumentId}");
            }

            double totalPrice = 0.0;
            int totalQuantity = 0;
            foreach (var order in orders)
            {
                try
                {
                    totalPrice += order.Price * order.Quantity;
                    totalQuantity += order.Quantity;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error while calculating volume weighted average price for instrument {instrumentId}: {e.Message}");
                }
            }
            return Math.Round(totalPrice / totalQuantity, 2);
        }
    }
}
