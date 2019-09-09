using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using TakeHome.Mediator.Requests;
using TakeHome.Mediator.Responses;
using TakeHome.Services;
using TakeHome.Web.Api.Controllers;
using Xunit;

namespace TakeHome.Web.Api.Tests
{
    public class TakeHomeControllerTests
    {
        private readonly Mock<IMediator> _mediator;
        private TakeHomeController sut;

        public TakeHomeControllerTests()
        {
            _mediator = new Mock<IMediator>();
            sut = new TakeHomeController(_mediator.Object);
        }

        [Fact]
        public async void Should_ReturnOk_When_ShortestRouteIsFound()
        {
            //Arrange
            _mediator.Setup(s =>
                  s.Send(It.IsAny<GetShortestRouteRequest>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(new GetShortestRouteResponse(true, false, ""));
            
            //Act
            var result = await sut.Get(new GetRequest { Origin = It.IsAny<string>(), Destination = It.IsAny<string>() });
            var okResult = result as ObjectResult;

            //Assert
            Assert.NotNull(okResult);
            Assert.True(okResult is OkObjectResult);
            Assert.IsType<string>(okResult.Value);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        }

        [Fact]
        public async void Should_ReturnNotFound_When_ShortestRouteIsNotFound()
        {
            _mediator.Setup(s =>
                 s.Send(It.IsAny<GetShortestRouteRequest>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(new GetShortestRouteResponse(true, true, ""));
            
            //Act
            var result = await sut.Get(new GetRequest { Origin = It.IsAny<string>(), Destination = It.IsAny<string>() });
            var notFoundResult = result as ObjectResult;

            //Assert
            Assert.NotNull(notFoundResult);
            Assert.True(notFoundResult is NotFoundObjectResult);
            Assert.IsType<string>(notFoundResult.Value);
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        }

        [Fact]
        public async void Should_ReturnBadRequest_When_RequestHasInvalidParameters()
        {
            //Arrange
            _mediator.Setup(s =>
                 s.Send(It.IsAny<GetShortestRouteRequest>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(new GetShortestRouteResponse(false, false, ""));

            //Act
            var result = await sut.Get(new GetRequest { Origin = It.IsAny<string>(), Destination = It.IsAny<string>() });
            var badRequestResult = result as ObjectResult;

            //Assert
            Assert.NotNull(badRequestResult);
            Assert.True(badRequestResult is BadRequestObjectResult);
            Assert.IsType<string>(badRequestResult.Value);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        }

        [Fact]
        public async void Should_ReturnInternalServerError_When_RequestThrowsException()
        {
            //Arrange
            _mediator.Setup(s =>
                 s.Send(It.IsAny<GetShortestRouteRequest>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(new GetShortestRouteResponse(" "));
            
            //Act
            var result = await sut.Get(new GetRequest { Origin = It.IsAny<string>(), Destination = It.IsAny<string>() });
            var errorResult = result as ObjectResult;

            //Assert
            Assert.NotNull(errorResult);
            Assert.IsType<string>(errorResult.Value);
            Assert.Equal(StatusCodes.Status500InternalServerError, errorResult.StatusCode);
        }
    }
}
