using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.Run(async (HttpContext context) =>
{
	if (context.Request.Path.StartsWithSegments("/employees"))
	{
		if (context.Request.Method == "GET")
		{
			if (context.Request.Query.ContainsKey("id"))
			{
				var id = context.Request.Query["id"];
				if (!string.IsNullOrEmpty(id))
				{
					if (int.TryParse(id, out int employeeId))
					{
						var employee = EmployeesRepository.GetEmployees().FirstOrDefault(e => e.Id == employeeId);
						if (employee != null)
						{
							context.Response.StatusCode = 200;
							await context.Response.WriteAsync($"ID: {employee.Id}) Name: {employee.Name}\tPosition: {employee.Position}\tSalary: {employee.Salary}");
						}
						else
						{
							context.Response.StatusCode = 404;
							await context.Response.WriteAsync("Employee not found.");
						}
						return;
					}
				}
			}

			var employees = EmployeesRepository.GetEmployees();

			context.Response.StatusCode = 200;
			foreach (var employee in employees)
			{
				await context.Response.WriteAsync($"ID: {employee.Id}) Name: {employee.Name}\tPosition: {employee.Position}\tSalary: {employee.Salary}");
			}


		}
		else if (context.Request.Method == "POST")
		{
			using var reader = new StreamReader(context.Request.Body); // Using 'using' to ensure proper disposal after use to prevent memory leaks so that the stream is closed after reading.
			var body = await reader.ReadToEndAsync();

			try
			{
				var employee = JsonSerializer.Deserialize<Employee>(body);

				if (employee is null || employee.Id <= 0)
				{
					context.Response.StatusCode = 400;
					return;
				}
				EmployeesRepository.AddEmployee(employee);

				context.Response.StatusCode = 201;
				await context.Response.WriteAsync("Employee added successfully.");
			}
			catch (Exception ex)
			{
				context.Response.StatusCode = 400;
				await context.Response.WriteAsync($"{ex.ToString()}");
				return;
			}
		}
		else if (context.Request.Method == "PUT")
		{
			using var reader = new StreamReader(context.Request.Body);
			var body = await reader.ReadToEndAsync();
			var employee = JsonSerializer.Deserialize<Employee>(body);

			var result = EmployeesRepository.UpdateEmployee(employee);
			if (result)
			{
				context.Response.StatusCode = 204;
				await context.Response.WriteAsync("Employee updated successfully.");

				return;
			}
			else
			{
				await context.Response.WriteAsync("Employee not found.");
			}
		}
		else if (context.Request.Method == "DELETE")
		{
			if (context.Request.Query.ContainsKey("id"))
			{
				var id = context.Request.Query["id"];
				if (int.TryParse(id, out int employeeId))
				{
					if (context.Request.Headers["Authorization"] == "Michael")
					{
						var result = EmployeesRepository.DeleteEmployee(employeeId);
						if (result)
						{
							await context.Response.WriteAsync("Employee deleted successfully.");
						}
						else
						{
							context.Response.StatusCode = 404;
							await context.Response.WriteAsync("Employee not found.");
						}
					}
					else
					{
						context.Response.StatusCode = 401;
						await context.Response.WriteAsync($"You are not authorized to delete employee {employeeId}.");
					}
				}
			}
		}
	}
	else if (context.Request.Path == "/")
	{
		context.Response.ContentType = "text/html";

		await context.Response.WriteAsync($"The method is: {context.Request.Method}<br>");
		await context.Response.WriteAsync($"The path is: {context.Request.Path}<br><br>");

		await context.Response.WriteAsync("<h1>Headers:</h1>");
		await context.Response.WriteAsync("<ol>");
		foreach (var key in context.Request.Headers.Keys)
		{
			await context.Response.WriteAsync($"<li><b>{key}:</b> {context.Request.Headers[key]}</li><br>");
		}
		await context.Response.WriteAsync("</ol>");
	}
	else
	{
		context.Response.StatusCode = 404;
	}
});

app.Run();

static class EmployeesRepository
{
	private static readonly List<Employee> employees = new()
	{
		new Employee(1, "James", "Manager", 12000),
		new Employee(2, "Robert", "Developer", 30000),
		new Employee(3, "Susan", "Recruiter", 34000)
	};

	public static List<Employee> GetEmployees() => employees.OrderBy(e => e.Id).ToList();

	public static void AddEmployee(Employee? employee)
	{
		ArgumentNullException.ThrowIfNull(employee);

		employees.Add(employee);
	}

	public static bool UpdateEmployee(Employee? employee)
	{
		ArgumentNullException.ThrowIfNull(employee);

		var existingEmployee = employees.FirstOrDefault(e => e.Id == employee.Id);

		if (existingEmployee != null)
		{
			existingEmployee.Name = employee.Name;
			existingEmployee.Position = employee.Position;
			existingEmployee.Salary = employee.Salary;
			return true;
		}

		return false;
	}

	public static bool DeleteEmployee(int id)
	{
		var employee = employees.FirstOrDefault(e => e.Id == id);

		if (employee != null)
		{
			employees.Remove(employee);
			return true;
		}

		return false;
	}
}

public class Employee
{
	public Employee(int id, string name, string position, double salary)
	{
		Id = id;
		Name = name;
		Position = position;
		Salary = salary;
	}

	public int Id { get; set; }
	public string Name { get; set; }
	public string Position { get; set; }
	public double Salary { get; set; }
}

