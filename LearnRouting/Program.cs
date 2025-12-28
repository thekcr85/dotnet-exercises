var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
	endpoints.MapGet("/", async (HttpContext context) =>
	{
		await context.Response.WriteAsync("Welcome to the Employee Management System");
	});

	endpoints.MapGet("/employees", async (HttpContext context) =>
	{
		var employees = EmployeesRepository.GetAll();
		await context.Response.WriteAsJsonAsync(employees);
	});

	endpoints.MapGet("/employees/{id:int}", async (HttpContext context) =>
	{
		var id = Convert.ToInt32(context.Request.RouteValues["id"]);
		var employee = EmployeesRepository.GetById(id);
		if (employee != null)
		{
			await context.Response.WriteAsJsonAsync(employee);
		}
		else
		{
			context.Response.StatusCode = StatusCodes.Status404NotFound;
			await context.Response.WriteAsync("Employee not found");
		}
	});

	endpoints.MapPost("/employees", async (HttpContext context) =>
	{
		var employee = await context.Request.ReadFromJsonAsync<Employee>();
		if (employee != null)
		{
			EmployeesRepository.Add(employee);
			context.Response.StatusCode = StatusCodes.Status201Created;
			context.Response.Headers.Location = $"/employees/{employee.Id}";
			await context.Response.WriteAsJsonAsync(employee);
		}
		else
		{
			context.Response.StatusCode = StatusCodes.Status400BadRequest;
			await context.Response.WriteAsync("Invalid employee data");
		}
	});

	endpoints.MapPut("/employees/{id:int}", async (HttpContext context) =>
	{
		var id = Convert.ToInt32(context.Request.RouteValues["id"]);
		var employee = await context.Request.ReadFromJsonAsync<Employee>();
		if (employee == null)
		{
			context.Response.StatusCode = StatusCodes.Status400BadRequest;
			await context.Response.WriteAsync("Invalid employee data");
			return;
		}

		var existing = EmployeesRepository.GetById(id);
		if (existing == null)
		{
			context.Response.StatusCode = StatusCodes.Status404NotFound;
			await context.Response.WriteAsync("Employee not found");
			return;
		}

		employee.Id = id;
		EmployeesRepository.Update(employee);
		context.Response.StatusCode = StatusCodes.Status204NoContent;
	});

	endpoints.MapDelete("/employees/{id:int}", async (HttpContext context) =>
	{
		var id = Convert.ToInt32(context.Request.RouteValues["id"]);
		var existing = EmployeesRepository.GetById(id);
		if (existing == null)
		{
			context.Response.StatusCode = StatusCodes.Status404NotFound;
			await context.Response.WriteAsync("Employee not found");
			return;
		}

		EmployeesRepository.Delete(id);
		context.Response.StatusCode = StatusCodes.Status204NoContent;
	});
});

app.Run();
