using NUnit.Framework;

namespace AdvancedCQRS.Events.Tests
{
    public class StopLossProcessManagerTests
    {
        private FakeMessagePublisher _publisher;
        private StopLossProcessManager _sut;

        [SetUp]
        public void SetUp()
        {
            _publisher = new FakeMessagePublisher();
            _sut = new StopLossProcessManager(_publisher);

            When();
        }

        protected virtual void When()
        {
        }

        public class WhenAcquiringPosition : StopLossProcessManagerTests
        {
            protected override void When()
            {
                _sut.Handle(new PositionAcquired { Price = 105 });
            }

            [Test]
            public void ShouldUpdateStopLossPrice()
            {
                var message = _publisher.FindMessage<StopLossPriceUpdated>();
                Assert.That(message, Is.Not.Null);
                Assert.That(message.Price, Is.EqualTo(95));
            }

            [Test]
            public void ShouldSendRemoveMinimumPricesMessage()
            {
                var message = _publisher.FindSendMeInMessage<PriceExpiredFromStopLossWindow>();
                Assert.That(message, Is.Not.Null);
                Assert.That(message.Price, Is.EqualTo(105));
            }

            [Test]
            public void ShouldSendRemoveMaximumPricesMessage()
            {
                var message = _publisher.FindSendMeInMessage<PriceExpiredFromStopLossHitWindow>();
                Assert.That(message, Is.Not.Null);
                Assert.That(message.Price, Is.EqualTo(105));
            }
        }

        public class WhenPriceUpdatedToLower : StopLossProcessManagerTests
        {
            protected override void When()
            {
                _sut.Handle(new PositionAcquired { Price = 100 });
                _publisher.Clear();

                _sut.Handle(new PriceUpdated { Price = 95 });
            }

            [Test]
            public void ShouldSendRemoveMinimumPricesMessage()
            {
                var message = _publisher.FindSendMeInMessage<PriceExpiredFromStopLossWindow>();
                Assert.That(message, Is.Not.Null);
                Assert.That(message.Price, Is.EqualTo(95));
            }

            [Test]
            public void ShouldSendRemoveMaximumPricesMessage()
            {
                var message = _publisher.FindSendMeInMessage<PriceExpiredFromStopLossHitWindow>();
                Assert.That(message, Is.Not.Null);
                Assert.That(message.Price, Is.EqualTo(95));
            }

            [Test]
            public void ShouldNotSendStopLossPriceUpdatedMessage()
            {
                var message = _publisher.FindMessage<StopLossPriceUpdated>();
                Assert.That(message, Is.Null);
            }
        }

        public class WhenPriceUpdatedToHigher : StopLossProcessManagerTests
        {
            protected override void When()
            {
                _sut.Handle(new PositionAcquired { Price = 100 });
                _publisher.Clear();

                _sut.Handle(new PriceUpdated { Price = 105 });
            }

            [Test]
            public void ShouldSendRemoveMinimumPricesMessage()
            {
                var message = _publisher.FindSendMeInMessage<PriceExpiredFromStopLossWindow>();
                Assert.That(message, Is.Not.Null);
                Assert.That(message.Price, Is.EqualTo(105));
            }

            [Test]
            public void ShouldSendRemoveMaximumPricesMessage()
            {
                var message = _publisher.FindSendMeInMessage<PriceExpiredFromStopLossHitWindow>();
                Assert.That(message, Is.Not.Null);
                Assert.That(message.Price, Is.EqualTo(105));
            }

            [Test]
            public void ShouldNotSendStopLossPriceUpdatedMessage()
            {
                var message = _publisher.FindMessage<StopLossPriceUpdated>();
                Assert.That(message, Is.Null);
            }
        }

        public class WhenStopLossPriceExpired : StopLossProcessManagerTests
        {
            protected override void When()
            {
                _sut.Handle(new PositionAcquired { Price = 100 });
                _sut.Handle(new PriceUpdated { Price = 105 });
                _publisher.Clear();

                _sut.Handle(new PriceExpiredFromStopLossWindow { Price = 100 });
            }

            [Test]
            public void ShouldSendStopLossPriceUpdatedMessage()
            {
                var message = _publisher.FindMessage<StopLossPriceUpdated>();
                Assert.That(message, Is.Not.Null);
                Assert.That(message.Price, Is.EqualTo(95));
            }
        }

        public class WhenStopLossHitExpired : StopLossProcessManagerTests
        {
            protected override void When()
            {
                _sut.Handle(new PositionAcquired { Price = 100 });
                _sut.Handle(new PriceUpdated { Price = 105 });
                //_sut.Handle(new PriceExpiredFromStopLossWindow { Price = 100 });
                _sut.Handle(new PriceExpiredFromStopLossHitWindow { Price = 100 });
                //_sut.Handle(new PriceUpdated { Price = 110 });
                //_sut.Handle(new PriceExpiredFromStopLossWindow { Price = 105 });
                _sut.Handle(new PriceExpiredFromStopLossHitWindow { Price = 105 });
                _sut.Handle(new PriceUpdated { Price = 90 });
                //_sut.Handle(new PriceUpdated { Price = 90 });
                //_sut.Handle(new PriceUpdated { Price = 90 });
                //_sut.Handle(new PriceExpiredFromStopLossWindow { Price = 110 });
                //_sut.Handle(new PriceExpiredFromStopLossHitWindow { Price = 110 });
                //_sut.Handle(new PriceExpiredFromStopLossWindow { Price = 90 });
                //_sut.Handle(new PriceExpiredFromStopLossHitWindow { Price = 90 });
                _sut.Handle(new PriceUpdated { Price = 88 });
                _sut.Handle(new PriceUpdated { Price = 85 });
                _sut.Handle(new PriceUpdated { Price = 80 });
                _publisher.Clear();
                
                _sut.Handle(new PriceExpiredFromStopLossHitWindow { Price = 90 });
            }

            [Test]
            public void ShouldTriggerStopLossHit()
            {
                var message = _publisher.FindMessage<StopLossHit>();
                Assert.That(message, Is.Not.Null);
                Assert.That(message.Price, Is.EqualTo(80));
            }

            [Test]
            public void ShouldTriggerSingleStopLossHit()
            {
                _sut.Handle(new PriceExpiredFromStopLossHitWindow { Price = 90 });

                var message = _publisher.FindMessage<StopLossHit>();
                Assert.That(message, Is.Not.Null);
                Assert.That(message.Price, Is.EqualTo(80));
            }
        }
    }
}