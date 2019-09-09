using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TakeHome.Data;
using TakeHome.Models;
using TakeHome.Services.Tests.Fakes;
using Xunit;

namespace TakeHome.Services.Tests
{
    public class TakeHomeServiceTests
    {
        private readonly ITakeHomeService _service;
        private readonly Mock<ITakeHomeRepository> _repository;
        public TakeHomeServiceTests()
        {
            _repository = new Mock<ITakeHomeRepository>();

            //Arrange
            _repository.Setup(s =>
                s.GetRoutes("JFK", "YFR")).ReturnsAsync(RoutesFake.RoutesA);

            _repository.Setup(s =>
                s.GetRoutes("JFK", "YYZ")).ReturnsAsync(RoutesFake.RoutesB);

            _repository.Setup(s =>
                s.GetRoutes("YYZ", "YJT")).ReturnsAsync(RoutesFake.RoutesC);

            _repository.Setup(s =>
                s.GetRoutes("YYZ", "GRU")).ReturnsAsync(RoutesFake.RoutesD);

            _repository.Setup(s =>
                s.GetRoutes("YYZ", "XYZ")).ReturnsAsync(RoutesFake.RoutesE);

            _repository.Setup(s =>
                s.GetRoutesInRange(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>())).ReturnsAsync(RoutesFake.RoutesInRange);

            _service = new TakeHomeService(_repository.Object);
        }

        [Fact]
        public async Task Should_HaveOneConnection_When_OriginHasDirectRouteToDestination()
        {
            //Act
            var shortestRoute = await _service.GetShortestRoute(new Airport { Iata3 = "JFK" }, new Airport { Iata3 = "YYZ" });

            //Assert
            _repository.Verify(m => m.GetRoutes(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _repository.Verify(m => m.GetRoutesInRange(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never);

            Assert.NotNull(shortestRoute);
            Assert.Equal(2, shortestRoute.Split(" -> ").Length);
        }

        [Fact]
        public async Task Should_HaveMoreThanOneConnection_When_OriginHasNoDirectRouteToDestination()
        {
            //Arrange
            var origin = new Airport
            {
                Iata3 = "YYZ",
                Coordinates = new Coordinates{ Latitude = 43.67720032, Longitude = -79.63059998 }
            };

            var destination = new Airport
            {
                Iata3 = "YJT",
                Coordinates = new Coordinates { Latitude = 48.5442009, Longitude = -58.54999924 }
            };

            //Act
            var shortestRoute = await _service.GetShortestRoute(origin, destination);

            //Assert
            _repository.Verify(m => m.GetRoutes(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _repository.Verify(m => m.GetRoutesInRange(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>()), Times.Once);

            Assert.NotNull(shortestRoute);
            Assert.True(shortestRoute.Split(" -> ").Length > 2);
        }

        [Fact]
        public async Task Should_HaveNoRoute_When_OriginHasNoRouteToDestination()
        {
            //Arrange
            var origin = new Airport
            {
                Iata3 = "YYZ",
                Coordinates = new Coordinates { Latitude = 43.67720032, Longitude = -79.63059998 }
            };

            var destination = new Airport
            {
                Iata3 = "GRU",
                Coordinates = new Coordinates { Latitude = 48.5442009, Longitude = -58.54999924 }
            };

            //Act
            var shortestRoute = await _service.GetShortestRoute(origin, destination);

            //Assert
            _repository.Verify(m => m.GetRoutes(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _repository.Verify(m => m.GetRoutesInRange(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>()), Times.Once);

            Assert.NotNull(shortestRoute);
            Assert.Equal("No Route", shortestRoute);
        }

        [Fact]
        public async Task Should_HaveNoRoute_When_AirportHasNoRoutAtAll()
        {
            //Arrange
            var origin = new Airport
            {
                Iata3 = "YYZ",
                Coordinates = new Coordinates { Latitude = 43.67720032, Longitude = -79.63059998 }
            };

            var destination = new Airport
            {
                Iata3 = "XYZ",
                Coordinates = new Coordinates { Latitude = 48.5442009, Longitude = -58.54999924 }
            };

            //Act
            var shortestRoute = await _service.GetShortestRoute(origin, destination);

            //Assert
            _repository.Verify(m => m.GetRoutes(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _repository.Verify(m => m.GetRoutesInRange(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never);

            Assert.NotNull(shortestRoute);
            Assert.Equal("No Route", shortestRoute);
        }

        [Theory]
        [InlineData("XXX", "BBB", "")]
        [InlineData("XXX", "YYZ", "YYZ")]
        [InlineData("YVR", "XXX", "YVR")]
        public async void Should_ReturnLessThanTwoItems_When_AirpotIsInvalid(string origin, string destination, string fakeResult)
        {
            //Arrange
            var returnList = fakeResult.Split(',').Select(s => new Airport { Iata3 = s}).ToList();

            _repository.Setup(s =>
                s.GetAiports(origin, destination)).ReturnsAsync(returnList);

            //Act
            var airports = await _service.GetAiports(origin, destination);

            //Assert
            Assert.NotNull(airports);
            Assert.True(airports.Count < 2);
        }

    }
}
