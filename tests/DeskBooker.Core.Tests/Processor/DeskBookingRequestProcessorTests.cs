using System;
using DeskBooker.Core.Domain;
using Xunit;

namespace DeskBooker.Core.Processor
{
    public class DeskBookingRequestProcessorTests
    {
        private readonly DeskBookingRequestProcessor processor;

        public DeskBookingRequestProcessorTests()
        {
            processor = new DeskBookingRequestProcessor();
        }

        [Fact]
        public void ShouldReturnDeskBookingResultWithRequestValues()
        {
            // arrange
            var request = new DeskBookingRequest
            {
                FirstName = "Jim",
                LastName = "Beam",
                Email = "whiskey@jb.com",
                Date = new DateTime(2020, 1, 20)
            };

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
    }
}