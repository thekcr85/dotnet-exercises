static class EmployeesRepository
{
	private static readonly List<Employee> Employees = new()
	{
		new Employee(1, "Alice", "Developer", 60000),
		new Employee(2, "Bob", "Designer", 55000),
		new Employee(3, "Charlie", "Manager", 75000)
	};

	public static List<Employee> GetAll() => Employees;

	public static Employee? GetById(int id) => Employees.FirstOrDefault(e => e.Id == id);

	public static void Add(Employee employee)
	{
		if (employee.Id == 0)
		{
			var nextId = Employees.Any() ? Employees.Max(e => e.Id) + 1 : 1;
			employee.Id = nextId;
		}
		Employees.Add(employee);
	}

	public static void Update(Employee employee)
	{
		var index = Employees.FindIndex(e => e.Id == employee.Id);
		if (index != -1)
		{
			Employees[index] = employee;
		}
	}

	public static void Delete(int id)
	{
		var employee = GetById(id);
		if (employee != null)
		{
			Employees.Remove(employee);
		}
	}
}
