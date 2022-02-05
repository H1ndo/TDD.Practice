using System;
using System.Collections.Generic;
using System.Linq;
using DeskBooker.Core.DataInterface;
using DeskBooker.Core.Domain;
using Moq;
using Xunit;

namespace DeskBooker.Core.Processor
{
    public class DeskBookingRequestProcessorTests
    {
        private readonly DeskBookingRequestProcessor processor;
        private readonly Mock<IDeskBookingRepository> deskBookingRepositoryMock;
        private readonly Mock<IDeskRepository> deskRepositoryMock;
        private readonly DeskBookingRequest request;
        private readonly List<Desk> availableDesks;

        public DeskBookingRequestProcessorTests()
        {
            request = new DeskBookingRequest
            {
                FirstName = "Jim",
                LastName = "Beam",
                Email = "whiskey@jb.com",
                Date = new DateTime(2020, 1, 20)
            };
            availableDesks = new List<Desk> { new Desk { Id = 6 } };
            deskBookingRepositoryMock = new Mock<IDeskBookingRepository>();
            deskRepositoryMock = new Mock<IDeskRepository>();
            deskRepositoryMock.Setup(x => x.GetAvailableDesks(request.Date))
                .Returns(availableDesks);

            processor = new DeskBookingRequestProcessor(deskBookingRepositoryMock.Object, deskRepositoryMock.Object);
        }

        [Fact]
        public void ShouldReturnDeskBookingResultWithRequestValues()
        {
            // arrange


            // act
            DeskBookingResult result = processor.BookDesk(request);

            // assert
            Assert.NotNull(result);
            Assert.Equal(request.FirstName, result.FirstName);
            Assert.Equal(request.LastName, result.LastName);
            Assert.Equal(request.Email, result.Email);
            Assert.Equal(request.Date, result.Date);
        }

        [Fact]
        public void ShouldThrowNullExceptionWhenRequestIsNull()
        {
            // arrange

            // act

            // assert

            var exception = Assert.Throws<ArgumentNullException>(() => processor.BookDesk(null));

            Assert.Equal("request", exception.ParamName);
        }

        [Fact]
        public void ShouldSaveDeskBooking()
        {
            DeskBooking savedDeskBooking = null;

            deskBookingRepositoryMock.Setup(x => x.Save(It.IsAny<DeskBooking>()))
                .Callback<DeskBooking>(deskBooking =>
                savedDeskBooking = deskBooking);

            processor.BookDesk(request);

            deskBookingRepositoryMock.Verify(x => x.Save(It.IsAny<DeskBooking>()), Times.Once);

            Assert.NotNull(savedDeskBooking);
            Assert.Equal(request.FirstName, savedDeskBooking.FirstName);
            Assert.Equal(request.LastName, savedDeskBooking.LastName);
            Assert.Equal(request.Email, savedDeskBooking.Email);
            Assert.Equal(request.Date, savedDeskBooking.Date);
            Assert.Equal(availableDesks.First().Id, savedDeskBooking.DeskId);
        }

        [Fact]
        public void ShouldNotSaveDeskBookingIfNoDeskAvailable()
        {
            availableDesks.Clear();

            processor.BookDesk(request);

            deskBookingRepositoryMock.Verify(x => x.Save(It.IsAny<DeskBooking>()), Times.Never);

        }

        [Theory]
        [InlineData(DeskBookingResultCode.Success, true)]
        [InlineData(DeskBookingResultCode.NoDeskAvailable, false)]
        public void ShouldReturnExpectedResultCode(DeskBookingResultCode expectedResultCode, bool isDeskAvailable)
        {
            if (!isDeskAvailable)
            {
                availableDesks.Clear();
            }

            var result = processor.BookDesk(request);

            Assert.Equal(expectedResultCode, result.Code);
        }

        [Theory]
        [InlineData(5, true)]
        [InlineData(null, false)]
        public void ShouldReturnExpectedDeskBookingId(int? expectedBookingId, bool isDeskAvailable)
        {
            if (!isDeskAvailable)
            {
                availableDesks.Clear();
            }
            else
            {
                deskBookingRepositoryMock.Setup(x => x.Save(It.IsAny<DeskBooking>()))
                    .Callback<DeskBooking>(deskBooking =>
                    {
                        deskBooking.Id = expectedBookingId.Value;
                    });
            }

            var result = processor.BookDesk(request);

            Assert.Equal(expectedBookingId, result.DeskBookingId);
        }

    }
}
