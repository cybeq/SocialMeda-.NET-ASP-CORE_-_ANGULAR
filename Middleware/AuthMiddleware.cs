using newapi.Services;

namespace newapi.Middleware
{
    public class AuthMiddleware : IMiddleware
    {
        private readonly IJwtService _jwtService;

        public AuthMiddleware(IJwtService jwtService)
        {
            _jwtService = jwtService;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var path = context.Request.Path;

            if (path.HasValue && path.Value.StartsWith("/api/auth"))
            {
                await next(context);
                return;
            }

            var token = context.Request.Headers.Authorization.ToString();

            if (string.IsNullOrEmpty(token))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Missing or invalid authorization key");
                return;
            }

            if (_jwtService.ValidateToken(token) == null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Invalid token");
                return;
            }

            await next(context);


        }
    }
}