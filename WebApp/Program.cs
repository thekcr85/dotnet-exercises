using System.Text.Json;

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
			var reader = new StreamReader(context.Request.Body);
			var body = await reader.ReadToEndAsync();
			var employee = JsonSerializer.Deserialize<Employee>(body);

			EmployeesRepository.AddEmployee(employee);
		}
		else
		{
		}
	}
	else
	{
		await context.Response.WriteAsync($"The method {context.Request.Method} is not supported.\r\n");
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
		if (employee is null)
		{
			throw new ArgumentNullException(nameof(employee));
		}

		employees.Add(employee);
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
