using NUnit.Framework;

namespace AdvancedCQRS.Events.Tests
{
    public class StopLossProcessManagerTests2
    {
        [Test]
        public void WhenPositionAcquiredIsPublished_StopLossPriceIsUpdated()
        {
            var publisher = new FakeMessagePublisher();
            var sut = new StopLossProcessManager(publisher);

            sut.Handle(new PositionAcquired { Price = 105 });

            var message = publisher.FindSendMeInMessage<PriceExpiredFromStopLossWindow>();
            Assert.That(message != null);
            //Assert.That(message.DelayInSeconds == 10);
            var message2 = publisher.FindMessage<StopLossPriceUpdated>();
            Assert.That(message2.Price == 95);
        }

        [Test]
        public void GivenPriceUpdated_ThenRemoveIn10SecondsIsPublished()
        {
            var publisher = new FakeMessagePublisher();
            var sut = new StopLossProcessManager(publisher);
            sut.Handle(new PositionAcquired { Price = 100 });
            publisher.Clear();

            var priceUpdated = new PriceUpdated { Price = 95 };
            sut.Handle(priceUpdated);


            var message = publisher.FindSendMeInMessage<PriceExpiredFromStopLossWindow>();
            Assert.That(message != null);
            //Assert.That(message.DelayInSeconds == 10);
        }

        [Test]
        public void GivenPriceUpdated_WhenRemovingIn10Seconds_ThenStopLossPriceUpdated()
        {
            var publisher = new FakeMessagePublisher();
            var sut = new StopLossProcessManager(publisher);
            sut.Handle(new PositionAcquired { Price = 100 });
            var priceUpdated = new PriceUpdated { Price = 101 };
            sut.Handle(priceUpdated);
            publisher.Clear();

            sut.Handle(new PriceExpiredFromStopLossWindow() { Price = 100 });

            StopLossPriceUpdated actual = publisher.FindMessage<StopLossPriceUpdated>();
            Assert.AreEqual(91, actual.Price);
        }

        [Test]
        public void GivePositionAcquired_ThenRemoveFrom13SencondWindowIsPublished()
        {
            var publisher = new FakeMessagePublisher();
            var sut = new StopLossProcessManager(publisher);

            sut.Handle(new PositionAcquired { Price = 100 });

            var message = publisher.FindSendMeInMessage<PriceExpiredFromStopLossHitWindow>();
            //Assert.That(message.DelayInSeconds == 13);
            Assert.That(message.Price == 100);
        }

        [Test]
        public void GivenPositionAcquired_WhenPriceUppdated_ThenRemoveFrom13SencondWindowIsPublished()
        {
            var publisher = new FakeMessagePublisher();
            var sut = new StopLossProcessManager(publisher);
            sut.Handle(new PositionAcquired { Price = 100 });
            publisher.Clear();

            sut.Handle(new PriceUpdated { Price = 90 });

            var message = publisher.FindSendMeInMessage<PriceExpiredFromStopLossHitWindow>();
            //Assert.That(message.DelayInSeconds == 13);
            Assert.That(message.Price == 90);
        }

        [Test]
        public void GivenDecreasingPrice_ThenWeTriggerHitStopLost()
        {
            var publisher = new FakeMessagePublisher();
            var sut = new StopLossProcessManager(publisher);
            sut.Handle(new PositionAcquired { Price = 100 });
            sut.Handle(new PriceUpdated { Price = 89 });
            publisher.Clear();

            sut.Handle(new PriceExpiredFromStopLossHitWindow() { Price = 100 });

            var message = publisher.FindMessage<StopLossHit>();
            Assert.IsInstanceOf<StopLossHit>(message);
        }

        [Test]
        public void GivenDecreasingPrice_ThenWeTriggerHitStopLost_X()
        {
            var publisher = new FakeMessagePublisher();
            var sut = new StopLossProcessManager(publisher);
            sut.Handle(new PositionAcquired { Price = 100 });
            sut.Handle(new PriceUpdated { Price = 89 });
            sut.Handle(new PriceUpdated { Price = 89 });
            publisher.Clear();

            sut.Handle(new PriceExpiredFromStopLossHitWindow { Price = 100 });
            sut.Handle(new PriceExpiredFromStopLossHitWindow { Price = 89 });

            var message = publisher.FindMessage<StopLossHit>();
            Assert.IsInstanceOf<StopLossHit>(message);
        }

        [Test]
        public void GivenDecreasingPrice_ThenWeShouldNotReduceTheStopLossPrice()
        {
            var publisher = new FakeMessagePublisher();
            var sut = new StopLossProcessManager(publisher);
            sut.Handle(new PositionAcquired { Price = 105 });
            sut.Handle(new PriceUpdated { Price = 95 });
            publisher.Clear();

            sut.Handle(new PriceExpiredFromStopLossWindow() { Price = 105 });

            var message = publisher.FindMessage<StopLossPriceUpdated>();
            Assert.That(message.Price == 95);
            //Assert.IsNull(message);
        }
    }
}