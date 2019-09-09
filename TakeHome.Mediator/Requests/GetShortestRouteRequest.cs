using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using TakeHome.Mediator.Responses;
using TakeHome.Services;

namespace TakeHome.Mediator.Requests
{
    public class GetShortestRouteRequest : IRequest<GetShortestRouteResponse>
    {
        public GetShortestRouteRequest(string origin, string destination)
        {
            Origin = origin;
            Destination = destination;
        }
        public string Origin { get; set; }
        public string Destination { get; set; }

        public override bool Equals(object obj)
        {
            var castedObject = obj as GetShortestRouteRequest;
            if (castedObject == null)
                return base.Equals(obj);

            return this.Origin == castedObject.Origin && this.Destination == castedObject.Destination;
        }

    }
}
