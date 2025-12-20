using LearnMiddleware.Middleware;
using System.Reflection.PortableExecutable;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddTransient<MyCustomMiddleware>();
var app = builder.Build();

app.UseMiddleware<MyCustomMiddleware>();

// Middleware #1
app.Use(async (context, next) =>
{
	await context.Response.WriteAsync("Middleware #1: Before calling next\n");

	await next(context);

	await context.Response.WriteAsync("Middleware #1: After calling next\n");

});

//app.MapWhen(
//	context => context.Request.Query.ContainsKey("id"), 
//	appBuilder =>
//	{
//		appBuilder.Use(async (context, next) =>
//		{
//			await context.Response.WriteAsync($"Middleware #7: Before calling next - Id: {context.Request.Query["id"].ToString()}\n");

//			await next(context);

//			await context.Response.WriteAsync($"Middleware #7: After calling next - Id: {context.Request.Query["id"].ToString()}\n");
//		});

//		appBuilder.Use(async (context, next) =>
//		{
//			await context.Response.WriteAsync($"Middleware #8: Before calling next - Id: {context.Request.Query["id"].ToString()}\n");

//			await next(context);

//			await context.Response.WriteAsync($"Middleware #8: After calling next - Id: {context.Request.Query["id"].ToString()}\n");
//		});
//	});

app.UseWhen(
	context => context.Request.Query.ContainsKey("id"),
	appBuilder =>
	{
		appBuilder.Use(async (context, next) =>
		{
			await context.Response.WriteAsync($"Middleware #9: Before calling next - Id: {context.Request.Query["id"].ToString()}\n");

			await next(context);

			await context.Response.WriteAsync($"Middleware #9: After calling next - Id: {context.Request.Query["id"].ToString()}\n");
		});

		appBuilder.Use(async (context, next) =>
		{
			await context.Response.WriteAsync($"Middleware #10: Before calling next - Id: {context.Request.Query["id"].ToString()}\n");

			await next(context);

			await context.Response.WriteAsync($"Middleware #10: After calling next - Id: {context.Request.Query["id"].ToString()}\n");
		});
	});

// Middleware #2
app.Use(async (context, next) =>
{
	await context.Response.WriteAsync("Middleware #2: Before calling next\n");

	await next(context);

	await context.Response.WriteAsync("Middleware #2: After calling next\n");

});
//app.Run(async (context) =>
//{
//	await context.Response.WriteAsync("Middleware #2: Processed\n");
//});

// Middleware #3
app.Use(async (context, next) =>
{
	await context.Response.WriteAsync("Middleware #3: Before calling next\n");

	await next(context);

	await context.Response.WriteAsync("Middleware #3: After calling next\n");

});

app.Run();
