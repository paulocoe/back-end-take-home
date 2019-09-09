using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TakeHome.Mediator.Handlers;
using TakeHome.Mediator.Requests;
using TakeHome.Models;
using TakeHome.Services;
using Xunit;

namespace TakeHome.Mediator.Tests
{
    public class GetShortestRouteHandlerTests
    {
        private readonly Mock<ITakeHomeService> _service;
        private readonly GetShortestRouteHandler _sut;

        public GetShortestRouteHandlerTests()
        {
            _service = new Mock<ITakeHomeService>();
            _sut = new GetShortestRouteHandler(_service.Object);
        }
        
        [Theory]
        [InlineData("Y3R", "YYZ", "Invalid Origin")]
        [InlineData("YVR", "Y2Z", "Invalid Destination")]
        [InlineData("Y!R", "YYZ", "Invalid Origin")]
        [InlineData("YVR", "Y#Z", "Invalid Destination")]
        [InlineData(null, "YYZ", "Invalid Origin")]
        [InlineData("YVR", null, "Invalid Destination")]
        [InlineData("", "YYZ", "Invalid Origin")]
        [InlineData("YVR", "", "Invalid Destination")]
        [InlineData("YV", "YYZ", "Invalid Origin")]
        [InlineData("YVR", "YZ", "Invalid Destination")]
        [InlineData("YVRW", "YYZ", "Invalid Origin")]
        [InlineData("YVR", "YYZZ", "Invalid Destination")]
        public async void Should_ReturnInvalidResponse_When_ParameterInvalid(string origin, string destination, string expectedMessage)
        {
            //Act
            var result = await _sut.Handle(new GetShortestRouteRequest(origin, destination), It.IsAny<CancellationToken>());

            //Assert
            Assert.False(result.IsValid);
            Assert.IsType<string>(result.Content);
            Assert.Equal(expectedMessage, result.Content);

        }

        [Fact]        
        public async void Should_ReturnInvalidResponse_When_AirpotIsInvalid()
        {
            //Arrange
            _service.Setup(s =>
                s.GetAiports(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new List<Models.Airport>());

            //Act
            var result = await _sut.Handle(new GetShortestRouteRequest("XXX", "YYZ"), It.IsAny<CancellationToken>());

            //Assert
            Assert.False(result.IsValid);
            Assert.IsType<string>(result.Content);          
        }

        [Theory]
        [InlineData("XXX", "BBB", "", "Invalid Origin")]
        [InlineData("XXX", "YYZ", "YYZ", "Invalid Origin")]
        [InlineData("YVR", "XXX", "YVR", "Invalid Destination")]
        public async void Should_ReturnMessage_When_AirpotIsInvalid(string origin, string destination, string fakeResult, string expectedMessage)
        {
            //Arrange
            var returnList = fakeResult.Split(',').Select(s => new Airport { Iata3 = s }).ToList();

            _service.Setup(s =>
                s.GetAiports(origin, destination)).ReturnsAsync(returnList);

            //Act
            var result = await _sut.Handle(new GetShortestRouteRequest(origin, destination), It.IsAny<CancellationToken>());

            //Assert
            Assert.NotNull(result);
            Assert.False(result.IsValid);
            Assert.Equal(expectedMessage, result.Content);
        }
    }
}
