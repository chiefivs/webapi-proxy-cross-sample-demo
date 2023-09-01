using System;

namespace WebApiProxy.Abstractions.Attributes
{
    public class RouteAttribute: Attribute
    {
        public string[] Segments;

        public RouteAttribute(string pattern)
        {
            Segments = pattern.Trim('/').Split('/');
        }
    }
}
