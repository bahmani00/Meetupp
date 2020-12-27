using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Infrastructure
{
    public static class WebUtility
    {
        public static Guid GetId(this IHttpContextAccessor httpContextAccessor){
            return Guid.Parse(httpContextAccessor.HttpContext.GetRouteData().Values["id"].ToString());
        }
        
    }
}