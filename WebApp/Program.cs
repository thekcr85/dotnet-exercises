using System.Text.Json;
using static EmployeesRepository;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.Run(async (HttpContext context) =>
{
	if (context.Request.Method == "GET")
	{
		if (context.Request.Path.StartsWithSegments("/employees"))
		{
			var employees = EmployeesRepository.GetEmployees();

			foreach (var employee in employees)
			{
				await context.Response.WriteAsync($"ID: {employee.Id}) Name: {employee.Name}\tPosition: {employee.Position}\tSalary: {employee.Salary}\r\n");
			}
		}
		else if (context.Request.Path == "/" || string.IsNullOrEmpty(context.Request.Path))
		{
			await context.Response.WriteAsync($"The method is: {context.Request.Method}\r\n");
			await context.Response.WriteAsync($"The path is: {context.Request.Path}\r\n");

			await context.Response.WriteAsync("\r\nHeaders:\r\n");
			foreach (var key in context.Request.Headers.Keys)
			{
				await context.Response.WriteAsync($"{key}: {context.Request.Headers[key]}\r\n");
			}
		}
	}
	else if (context.Request.Method == "POST")
	{
		if (context.Request.Path.StartsWithSegments("/employees"))
		{
			using var reader = new StreamReader(context.Request.Body); // Using 'using' to ensure proper disposal after use to prevent memory leaks so that the stream is closed after reading.
			var body = await reader.ReadToEndAsync();
			var employee = JsonSerializer.Deserialize<Employee>(body);

			EmployeesRepository.AddEmployee(employee);
		}
		else
		{
		}
	}
	else if (context.Request.Method == "PUT")
	{
		if (context.Request.Path.StartsWithSegments("/employees"))
		{
			using var reader = new StreamReader(context.Request.Body);
			var body = await reader.ReadToEndAsync();
			var employee = JsonSerializer.Deserialize<Employee>(body);

			var result = EmployeesRepository.UpdateEmployee(employee);
			if (result)
			{
				await context.Response.WriteAsync("Employee updated successfully.");
			}
			else
			{
				await context.Response.WriteAsync("Employee not found.");
			}
		}

	}
	else if (context.Request.Method == "DELETE")
	{
		if (context.Request.Path.StartsWithSegments("/employees"))
		{
			if (context.Request.Query.ContainsKey("id"))
			{
				var id = context.Request.Query["id"];
				if (int.TryParse(id, out int employeeId))
				{
					var result = EmployeesRepository.DeleteEmployee(employeeId);
					if (result)
					{
						await context.Response.WriteAsync("Employee deleted successfully.");
					}
					else
					{
						await context.Response.WriteAsync("Employee not found.");
					}

				}

			}
		}

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

	public static List<Employee> GetEmployees() => employees;

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

