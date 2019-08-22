using ExtCore.Data.Abstractions;
using FluentAssertions;
using Manufactures.Application.Helpers;
using Manufactures.Application.StockCards.EventHandlers.Warping;
using Manufactures.Domain.Shared.ValueObjects;
using Manufactures.Domain.StockCard;
using Manufactures.Domain.StockCard.Events.Warping;
using Manufactures.Domain.StockCard.ReadModels;
using Manufactures.Domain.StockCard.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Manufactures.Tests.StockCards.EventHandlers.DailyOperations.Warping
{
    public class MoveInBeamStockWarpingEventHandlerTests : IDisposable
    {
        private MockRepository mockRepository;

        private Mock<IStorage> mockStorage;

        private Mock<IStockCardRepository> _stockCardRepository;

        public MoveInBeamStockWarpingEventHandlerTests()
        {
            //Set up mock object
            this.mockRepository = new MockRepository(MockBehavior.Default);
            this.mockStorage = this.mockRepository.Create<IStorage>();
            this._stockCardRepository = this.mockRepository.Create<IStockCardRepository>();
            this.mockStorage.Setup(x => x.Save());
            this.mockStorage.Setup(x => x.GetRepository<IStockCardRepository>()).Returns(_stockCardRepository.Object);
        }

        public void Dispose()
        {
            this.mockRepository.VerifyAll();
        }

        private MoveInBeamStockWarpingEventHandler CreateMoveInBeamStockWarpingEventHandler()
        {
            return new MoveInBeamStockWarpingEventHandler(
                this.mockStorage.Object);
        }

        [Fact]
        public async void RunUnitTest()
        {
            await MoveIn_Stock_Card_Warping_Should_Success_Without_Existing_MoveOut_StockCard();
            await MoveIn_StockCard_Warping_Should_Success_With_Existing_MoveOUt_StockCard();
        }

        internal async Task MoveIn_Stock_Card_Warping_Should_Success_Without_Existing_MoveOut_StockCard()
        {
            // Arrange
            var moveInBeamStockWarpingEventHandler = this.CreateMoveInBeamStockWarpingEventHandler();
            MoveInBeamStockWarpingEvent notification = new MoveInBeamStockWarpingEvent();
            notification.BeamId = new BeamId(Guid.NewGuid());
            notification.DailyOperationId = new DailyOperationId(Guid.NewGuid());
            notification.DateTimeOperation = DateTimeOffset.UtcNow;
            notification.StockNumber = "Testing-MovIn-Warping";

            //Setup mock object result for find query linq
            _stockCardRepository
                .Setup(x => x.Find(It.IsAny<Expression<Func<StockCardReadModel, bool>>>())).Returns(new List<StockCardDocument>());

            //Set Cancellation Token
            CancellationToken cancellationToken = CancellationToken.None;

            // Act
            await moveInBeamStockWarpingEventHandler.Handle(
                notification,
                cancellationToken);

            // Assert
            moveInBeamStockWarpingEventHandler.ReturnResult().Should().Equals(true);
        }

        internal async Task MoveIn_StockCard_Warping_Should_Success_With_Existing_MoveOUt_StockCard()
        {
            // Arrange
            var moveInBeamStockWarpingEventHandler = this.CreateMoveInBeamStockWarpingEventHandler();
            MoveInBeamStockWarpingEvent notification = new MoveInBeamStockWarpingEvent();
            notification.BeamId = new BeamId(Guid.NewGuid());
            notification.DailyOperationId = new DailyOperationId(Guid.NewGuid());
            notification.DateTimeOperation = DateTimeOffset.UtcNow;
            notification.StockNumber = "Testing-MovIn-Warping";

            //Add dummy object to test with existing moveOut Stockcard
            var existingMoveOutStockCard = 
                new StockCardDocument(Guid.NewGuid(), 
                                      "Testing-moveout-warping", 
                                      new DailyOperationId(Guid.NewGuid()), 
                                      DateTimeOffset.UtcNow, 
                                      notification.BeamId, 
                                      false, 
                                      true, 
                                      StockCardStatus.WARPING_STOCK, 
                                      StockCardStatus.MOVEOUT_STOCK);

            //Setup mock object result for find query linq
            _stockCardRepository
                .Setup(x => x.Find(It.IsAny<Expression<Func<StockCardReadModel, bool>>>())).Returns(new List<StockCardDocument>() { existingMoveOutStockCard });

            //Set Cancellation Token
            CancellationToken cancellationToken = CancellationToken.None;

            // Act
            await moveInBeamStockWarpingEventHandler.Handle(
                notification,
                cancellationToken);

            // Assert
            moveInBeamStockWarpingEventHandler.ReturnResult().Should().Equals(true);
        }
    }
}
