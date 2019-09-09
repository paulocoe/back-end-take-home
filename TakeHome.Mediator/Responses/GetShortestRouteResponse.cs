using System;
using System.Collections.Generic;
using System.Text;

namespace TakeHome.Mediator.Responses
{
    public class GetShortestRouteResponse
    {
        public GetShortestRouteResponse(bool isValid, bool hasNoRoute, string content)
        {
            IsValid = isValid;
            Content = content;
            HasNoRoute = hasNoRoute;            
        }

        public GetShortestRouteResponse(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public bool IsValid { get;}
        public bool HasNoRoute { get; set; }
        public string ErrorMessage { get; set; }
        public string Content { get;}
    }
}
