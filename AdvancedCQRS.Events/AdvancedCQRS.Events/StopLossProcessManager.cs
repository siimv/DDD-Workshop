using System.Collections.Generic;
using System.Linq;

namespace AdvancedCQRS.Events
{
    public class StopLossProcessManager
    {
        private readonly IMessagePublisher _publisher;
        private readonly List<int> _possibleStopLossPrices = new List<int>();
        private readonly List<int> _currentPrices = new List<int>();
        private int _currentStopLossPrice;
        private bool _isStopLossHit;

        public StopLossProcessManager(IMessagePublisher publisher)
        {
            _publisher = publisher;
        }

        public void Handle(PositionAcquired message)
        {
            _possibleStopLossPrices.Add(message.Price);
            _currentPrices.Add(message.Price);

            UpdateStopLossPrice();

            _publisher.Publish(new StopLossPriceUpdated { Price = _currentStopLossPrice });
            _publisher.Publish(new SendMessageIn { MessageToSend = new PriceExpiredFromStopLossWindow { Price = message.Price } });
            _publisher.Publish(new SendMessageIn { MessageToSend = new PriceExpiredFromStopLossHitWindow { Price = message.Price } });
        }

        public void Handle(PriceUpdated message)
        {
            _possibleStopLossPrices.Add(message.Price);
            _currentPrices.Add(message.Price);

            _publisher.Publish(new SendMessageIn { MessageToSend = new PriceExpiredFromStopLossWindow { Price = message.Price } });
            _publisher.Publish(new SendMessageIn { MessageToSend = new PriceExpiredFromStopLossHitWindow { Price = message.Price } });
        }

        public void Handle(PriceExpiredFromStopLossHitWindow message)
        {
            _currentPrices.Remove(message.Price);

            if (_isStopLossHit || !_currentPrices.Any()) return;

            if (_currentPrices.Max() < _currentStopLossPrice)
            {
                _isStopLossHit = true;
                _publisher.Publish(new StopLossHit { Price = _currentPrices.Last() });
            }
        }

        public void Handle(PriceExpiredFromStopLossWindow message)
        {
            _possibleStopLossPrices.Remove(message.Price);

            UpdateStopLossPrice();

            _publisher.Publish(new StopLossPriceUpdated { Price = _currentStopLossPrice });
        }

        private void UpdateStopLossPrice()
        {
            var stopLossPrice = _possibleStopLossPrices.Min() - 10;
            if (stopLossPrice > _currentStopLossPrice)
            {
                _currentStopLossPrice = stopLossPrice;
            }
        }
    }
}