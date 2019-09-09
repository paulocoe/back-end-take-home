using FluentValidation.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TakeHome.Mediator.Requests;
using TakeHome.Mediator.Responses;
using TakeHome.Mediator.Validators;
using TakeHome.Models;
using TakeHome.Services;

namespace TakeHome.Mediator.Handlers
{
    public class GetShortestRouteHandler : IRequestHandler<GetShortestRouteRequest, GetShortestRouteResponse>
    {
        private readonly ITakeHomeService _service;

        public GetShortestRouteHandler(ITakeHomeService service)
        {
            _service = service;
        }

        public async Task<GetShortestRouteResponse> Handle(GetShortestRouteRequest request, CancellationToken cancellationToken)
        {
            try
            {
                ValidationResult result = await ValidateRequest(request);

                if (!result.IsValid)
                    return CreateInvalidResponse(result.Errors[0].ErrorMessage);

                request.Origin = request.Origin.ToUpper();
                request.Destination = request.Destination.ToUpper();

                var airports = await _service.GetAiports(request.Origin, request.Destination);

                var validationMessage = ValidateAirports(airports, request.Origin);

                if (!string.IsNullOrEmpty(validationMessage))
                    return CreateInvalidResponse(validationMessage);
                                
                var response = await GetShortestRoute(airports, request.Origin, request.Destination);

                if (response == "No Route")
                    return new GetShortestRouteResponse(true, true, response);

                return new GetShortestRouteResponse(true, false, response);
            }
            catch (Exception)
            {
                //TODO: log here
                return new GetShortestRouteResponse("Something wrong happened, please try again in a few minutes.");
            }
        }
        public string ValidateAirports(List<Airport> airports, string origin)
        {
            string invalidOrigin = "Invalid Origin";

            if (airports == null || airports.Count == 0)
                return invalidOrigin;

            if (airports.Count < 2)
                return airports[0].Iata3 == origin ? "Invalid Destination" : invalidOrigin;

            return null;
        }

        private GetShortestRouteResponse CreateInvalidResponse(string message)
        {
            return new GetShortestRouteResponse(false, false, message);
        }

        private async Task<ValidationResult> ValidateRequest(GetShortestRouteRequest request)
        {
            var requestValidator = new GetShortestRouteRequestValidator();
            return await requestValidator.ValidateAsync(request);
        }

        private async Task<string> GetShortestRoute(List<Airport> airports, string originIata3, string destinationIata3)
        {
            Airport origin; 
            Airport destination;
            int originIndex = -1;
            int destinationIndex = -1;

            if (airports[0].Iata3 == originIata3)
            {
                originIndex = 0;
                destinationIndex = 1;
            }
            else
            {
                originIndex = 1;
                destinationIndex = 0;
            }

            origin = airports[originIndex];
            destination = airports[destinationIndex];
            
            return await _service.GetShortestRoute(origin, destination);
        }
    }
}
