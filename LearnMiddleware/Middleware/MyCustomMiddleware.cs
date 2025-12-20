
namespace LearnMiddleware.Middleware;

public class MyCustomMiddleware : IMiddleware
{
	public async Task InvokeAsync(HttpContext context, RequestDelegate next)
	{
		await context.Response.WriteAsync("MyCustomMiddleware: Before calling next\n");

		await next(context);

		await context.Response.WriteAsync("MyCustomMiddleware: After calling next\n");
	}
}
