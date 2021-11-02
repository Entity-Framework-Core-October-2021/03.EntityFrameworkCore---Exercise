using SoftUni.Data;
using SoftUni.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SoftUni
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            using SoftUniContext context = new SoftUniContext();

            //string result = GetEmployeesFullInformation(context); //- 03
            //string result = GetEmployeesWithSalaryOver50000(context); //- 04
            //string result = GetEmployeesFromResearchAndDevelopment(context); //- 05
            //string result = AddNewAddressToEmployee(context); //- 06
            //string result = GetEmployeesInPeriod(context); //- 07
            //string result = GetAddressesByTown(context); //- 08
            //string result = GetEmployee147(context); //- 09
            //string result = GetDepartmentsWithMoreThan5Employees(context); //- 10
            //string result = GetLatestProjects(context); //- 11
            //string result = IncreaseSalaries(context); //- 12
            //string result = GetEmployeesByFirstNameStartingWithSa(context); //- 13
            //string result = DeleteProjectById(context); //- 14
            string result = RemoveTown(context); //- 54

            Console.WriteLine(result);
        }

        //03. Employees Full Information
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            var employees = context.Employees
                                   .Select(e => new
                                   {
                                       e.FirstName,
                                       e.LastName,
                                       e.MiddleName,
                                       e.JobTitle,
                                       e.Salary
                                   })
                                   .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} {employee.MiddleName} {employee.JobTitle} {employee.Salary:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        //04. Employees with Salary Over 50 000
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            var employees = context.Employees
                                   .Select(e => new
                                   {
                                       e.FirstName,
                                       e.Salary
                                   })
                                   .Where(e => e.Salary > 50000)
                                   .OrderBy(e => e.FirstName)
                                   .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} - {employee.Salary:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        //05. Employees from Research and Development
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            var employees = context.Employees
                                   .Where(e => e.Department.Name == "Research and Development")
                                   .OrderBy(e => e.Salary)
                                   .ThenByDescending(e => e.FirstName)
                                   .Select(e => new
                                   {
                                       e.FirstName,
                                       e.LastName,
                                       DepartmentName = e.Department.Name,
                                       e.Salary
                                   })
                                   .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} from {employee.DepartmentName} - ${employee.Salary:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        //06. Adding a New Address and Updating Employee
        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            Address newAddress = new Address()
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };
            context.Addresses.Add(newAddress);

            var nakov = context.Employees.First(e => e.LastName == "Nakov");
            nakov.Address = newAddress;

            context.SaveChanges();

            var employees = context.Employees
                                   .OrderByDescending(e => e.AddressId)
                                   .Select(e => new
                                   {
                                       AddressText = e.Address.AddressText
                                   })
                                   .Take(10);

            StringBuilder sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.AddressText}");
            }

            return sb.ToString().TrimEnd();
        }

        //07. Employees and Projects
        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            var employees = context.Employees
                                   .Where(e => e.EmployeesProjects.Any(ep => ep.Project.StartDate.Year >= 2001 && ep.Project.StartDate.Year <= 2003))
                                   .Take(10)
                                   .Select(e => new
                                   {
                                       e.FirstName,
                                       e.LastName,
                                       ManagerFirstName = e.Manager.FirstName,
                                       ManagerLastName = e.Manager.LastName,
                                       Projects = e.EmployeesProjects
                                                   .Select(ep => new
                                                   {
                                                       ProjectName = ep.Project.Name,
                                                       StartDate = ep.Project.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture),
                                                       EndDate = ep.Project.EndDate.HasValue ? ep
                                                                                               .Project
                                                                                               .EndDate
                                                                                               .Value
                                                                                               .ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)
                                                                                               : "not finished"
                                                   })
                                   })
                                   .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} - Manager: {employee.ManagerFirstName} {employee.ManagerLastName}");

                foreach (var project in employee.Projects)
                {
                    sb.AppendLine($"--{project.ProjectName} - {project.StartDate} - {project.EndDate}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        //08. Addresses by Town
        public static string GetAddressesByTown(SoftUniContext context)
        {
            var addresses = context.Addresses
                                   .Select(a => new
                                   {
                                       a.AddressText,
                                       TownName = a.Town.Name,
                                       EmployeesCount = a.Employees.Count
                                   })
                                   .OrderByDescending(a => a.EmployeesCount)
                                   .ThenBy(t => t.TownName)
                                   .ThenBy(at => at.AddressText)
                                   .Take(10);

            StringBuilder sb = new StringBuilder();

            foreach (var address in addresses)
            {
                sb.AppendLine($"{address.AddressText}, {address.TownName} - {address.EmployeesCount} employees");
            }

            return sb.ToString().TrimEnd();
        }

        //09. Employee 147
        public static string GetEmployee147(SoftUniContext context)
        {
            var employee147 = context.Employees
                                  .Where(e => e.EmployeeId == 147)
                                  .Select(e => new
                                  {
                                      e.FirstName,
                                      e.LastName,
                                      e.JobTitle,
                                      Projects = e.EmployeesProjects
                                                    .Select(ep => new
                                                    {
                                                        ProjectName = ep.Project.Name
                                                    })
                                                    .OrderBy(ep => ep.ProjectName)
                                                    .ToList()
                                  })
                                  .Single();

            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"{employee147.FirstName} {employee147.LastName} - {employee147.JobTitle}");

            foreach (var project in employee147.Projects)
            {
                sb.AppendLine($"{project.ProjectName}");
            }

            return sb.ToString().TrimEnd();
        }

        //10. Departments with More Than 5 Employees
        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            var departments = context.Departments
                                     .Where(d => d.Employees.Count > 5)
                                     .OrderBy(d => d.Employees.Count)
                                     .ThenBy(d => d.Name)
                                     .Select(d => new
                                     {
                                         d.Name,
                                         ManagerFirstName = d.Manager.FirstName,
                                         ManagerLastName = d.Manager.LastName,
                                         Employees = d.Employees
                                                             .Select(e => new
                                                             {
                                                                 e.FirstName,
                                                                 e.LastName,
                                                                 e.JobTitle
                                                             })
                                                             .OrderBy(e => e.FirstName)
                                                             .ThenBy(e => e.LastName)
                                                             .ToList()
                                     })
                                     .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var department in departments)
            {
                sb.AppendLine($"{department.Name} - {department.ManagerFirstName} {department.ManagerLastName}");

                foreach (var employee in department.Employees)
                {
                    sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        //11. Find Latest 10 Projects
        public static string GetLatestProjects(SoftUniContext context)
        {
            var projects = context.Projects
                                  .OrderByDescending(p => p.StartDate)
                                  .Take(10)
                                  .Select(p => new
                                  {
                                      p.Name,
                                      p.Description,
                                      StartDate = p.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)
                                  })
                                  .OrderBy(p => p.Name)
                                  .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var project in projects)
            {
                sb.AppendLine(project.Name);
                sb.AppendLine(project.Description);
                sb.AppendLine(project.StartDate);
            }

            return sb.ToString().TrimEnd();
        }

        //12. Increase Salaries
        public static string IncreaseSalaries(SoftUniContext context)
        {
            decimal salaryIncreasement = 1.12M;

            var targetingDepartments = new string[]
            {
                "Engineering",
                "Tool Design",
                "Marketing",
                "Information Services"
            };

            var targetingEmployees = context.Employees
                                   .Where(e => targetingDepartments.Contains(e.Department.Name))
                                   .ToList();

            foreach (var employee in targetingEmployees)
            {
                employee.Salary *= salaryIncreasement;
            }

            context.SaveChanges();

            var employees = targetingEmployees
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.Salary
                })
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} (${employee.Salary:f2})");
            }

            return sb.ToString().TrimEnd();
        }

        //13. Find Employees by First Name Starting With Sa
        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            var employees = context.Employees
                                   .Where(e => e.FirstName.StartsWith("Sa"))
                                    .OrderBy(e => e.FirstName)
                                    .ThenBy(e => e.LastName)
                                    .Select(e => new
                                    {
                                        e.FirstName,
                                        e.LastName,
                                        e.JobTitle,
                                        e.Salary
                                    })
                                    .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle} - (${employee.Salary:F2})");
            }

            return sb.ToString().TrimEnd();

        }

        //14. Delete Project by Id
        public static string DeleteProjectById(SoftUniContext context)
        {
            var employeesProjects = context.EmployeesProjects.First(x => x.ProjectId == 2);

            context.EmployeesProjects.Remove(employeesProjects);

            var project = context.Projects.First(x => x.ProjectId == 2);

            context.Projects.Remove(project);

            context.SaveChanges();

            var projects = context.Projects
                .Select(p => p.Name)
                .Take(10)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var p in projects)
            {
                sb.AppendLine(p);
            }

            return sb.ToString().TrimEnd();
        }

        //15. Remove Town
        public static string RemoveTown(SoftUniContext context)
        {
            var townNameToDelete = "Seattle";

            var townToDelete = context.Towns
                .Where(t => t.Name == townNameToDelete)
                .FirstOrDefault();

            var targetingAddresses = context.Addresses
                .Where(a => a.Town.Name == townNameToDelete)
                .ToList();

            var employeesLivingOnTargetingAddresses = context.Employees
                .Where(e => targetingAddresses.Contains(e.Address))
                .ToList();

            employeesLivingOnTargetingAddresses.ForEach(e => e.Address = null);
            targetingAddresses.ForEach(a => context.Remove(a));
            context.Remove(townToDelete);

            context.SaveChanges();

            return $"{targetingAddresses.Count} addresses in Seattle were deleted";

        }
    }
}