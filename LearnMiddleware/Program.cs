var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Middleware #1
app.Use(async (context, next) =>
{
	await context.Response.WriteAsync("Middleware #1: Before calling next\n");

	await next(context);

	await context.Response.WriteAsync("Middleware #1: After calling next\n");

});

app.Map("/employees", (appBuilder) =>
{
	appBuilder.Use(async (context, next) =>
	{
		await context.Response.WriteAsync("Middleware #5: Before calling next\n");

		await next(context);

		await context.Response.WriteAsync("Middleware #5: After calling next\n");
	});

	appBuilder.Use(async (context, next) =>
	{
		await context.Response.WriteAsync("Middleware #6: Before calling next\n");

		await next(context);

		await context.Response.WriteAsync("Middleware #6: After calling next\n");
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
