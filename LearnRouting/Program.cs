
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.Use(async (context, next) =>
{
	await next(context);
});

app.UseRouting();

app.Use(async (context, next) =>
{
	await next(context);
});

app.UseEndpoints(endpoints =>
{
	endpoints.MapGet("/employees", async (HttpContext context) =>
	{
		await context.Response.WriteAsync("Get employees");
	});

	endpoints.MapPost("/employees", async (HttpContext context) =>
	{
		await context.Response.WriteAsync("Create an employee");
	});

	endpoints.MapPut("/employees/", async (HttpContext context) =>
	{
		await context.Response.WriteAsync("Update an employee");
	});

	endpoints.MapDelete("/employees/{id}", async (HttpContext context) =>
	{
		await context.Response.WriteAsync($"Delete an employee with id: {context.Request.RouteValues["id"]} ");
	});

	endpoints.MapGet("/{category=jeans}/{size=medium}/{id?}", async (HttpContext context) =>
	{
		await context.Response.WriteAsync($"Get category: {context.Request.RouteValues["category"]} in size: {context.Request.RouteValues["size"]} with id: {context.Request.RouteValues["id"]} ");
	});

	endpoints.MapGet("/employees/{id:int}", async (HttpContext context) =>
	{
		await context.Response.WriteAsync($"Get employee: {context.Request.RouteValues["id"]}");
	});

	endpoints.MapGet("/employees/{name}", async (HttpContext context) =>
	{
		await context.Response.WriteAsync($"Get employee: {context.Request.RouteValues["name"]}");
	});
});

app.Run();

class PositionConstraint : IRouteConstraint
{
	public bool Match(HttpContext? httpContext, IRouter? route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
	{
		if (!values.ContainsKey(routeKey))
		{
			return false;
		}

		if (values[routeKey] is null)
		{
			return false;
		}

		if (values[routeKey].ToString().Equals("manager", StringComparison.OrdinalIgnoreCase) || values[routeKey].ToString().Equals("developer", StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}

		return false;
	}
}