using Microsoft.AspNetCore.Http;

namespace ClassLibrary1.Helpers
{
    public static class HttpContextHelper
    {
        private static IHttpContextAccessor _httpContextAccessor;
        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public static HttpContext HttpContext => _httpContextAccessor.HttpContext;
    }
}