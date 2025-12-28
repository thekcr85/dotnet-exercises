
var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RouteOptions>(options =>
{
	options.ConstraintMap.Add("position", typeof(PositionConstraint));
});

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

	endpoints.MapGet("/employees/positions/{position:position}", async (HttpContext context) =>
	{
		await context.Response.WriteAsync($"Get employees under position: {context.Request.RouteValues["position"]}");
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