using System.Text;

namespace EmployeeManagementAPI.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
		private readonly IDepartmentRepository _departmentRepository;
        private readonly IPositionRepository _positionRepository;
		private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(IEmployeeRepository employeeRepository, 
            IDepartmentRepository departmentRepository, 
            IPositionRepository positionRepository, 
            ILogger<EmployeeController> logger)
        {
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
            _positionRepository = positionRepository;
			_logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? sortColumn, string? sortOrder, string? searchName, List<string> positions)
        {
			ViewBag.DateOfBirthSortParam = sortOrder == "asc" && sortColumn == "DateOfBirth" ? "desc" : "asc";
			ViewBag.HireDateSortParam = sortOrder == "asc" && sortColumn == "HireDate" ? "desc" : "asc";
			ViewBag.SalarySortParam = sortOrder == "asc" && sortColumn == "Salary" ? "desc" : "asc";

			ViewBag.CurrentSortColumn = sortColumn;
			ViewBag.CurrentSortOrder = sortOrder;

			ViewBag.Positions = await _positionRepository.ListAsync();
			try
            {
                var employees = await _employeeRepository.ListAsync(sortOrder, sortColumn, searchName, positions);
                _logger.LogInformation($"Employees (count = {employees.Count}) were received");
                return View(employees);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex.Message, ex);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(500);
            }
        }

		[HttpGet]
		public async Task<IActionResult> Create()
		{
			try
			{
				var departments = await _departmentRepository.ListAsync();
				var positions = await _positionRepository.ListAsync();

                var model = new EmployeeCreateViewModel
                {
                    Positions = positions.ToList(),
                    Departments = departments.ToList()
                };

                return View(model);
			}
			catch (ArgumentException ex)
			{
				_logger.LogError(ex.Message, ex);
				return BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message, ex);
				return StatusCode(500);
			}			
		}

		[HttpPost]
        public async Task<IActionResult> Create(EmployeeCreateViewModel employee)
        {
            try
            {
                await _employeeRepository.AddAsync(employee);
                _logger.LogInformation($"Added new employee - {employee.FullName}");
                return RedirectToAction("Index");
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex.Message, ex);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(500);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var employee = await _employeeRepository.GetAsync(id);
                if(employee.Id <= 0) 
                {
                    TempData["errorMessage"] = $"Employee with id = {id} does not exist";
                    return RedirectToAction("Index");
                }
                return View(employee);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex.Message, ex);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(500);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var employee = await _employeeRepository.GetAsync(id);
                if (employee.Id <= 0)
                {
                    TempData["errorMessage"] = $"Employee with id = {id} does not exist";
                    return RedirectToAction("Index");
                }

                var result = await _employeeRepository.DeleteAsync(id);
                return RedirectToAction("Index");
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex.Message, ex);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(500);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Salary(List<string> positionsFilter, List<string> departmentsFilter, string? startYear, string? endYear)
        {
            try
            {
                var employeeSalaries = await _employeeRepository.SalaryReportAsync(positionsFilter, departmentsFilter, startYear, endYear);

                var departmentsList = await _departmentRepository.ListAsync();
                var positionsList = await _positionRepository.ListAsync();
                var model = new FilterSelectionViewModel
                {
                    Positions = positionsList.ToList(),
                    Departments = departmentsList.ToList(),
                    Report = employeeSalaries.ToList(),
                    TotalSum = employeeSalaries.Sum(employee => employee.Salary)
                };

                return View(model);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex.Message, ex);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(500);
            }
        }
        [HttpGet]
        public async Task<IActionResult> DownloadReport()
        {
            return Ok();
        }


        [HttpPost]
		public async Task<IActionResult> Edit(Employee model)
		{
			try
			{
				var result = await _employeeRepository.UpdateAsync(model);
				if (!result)
				{
					throw new ArgumentException("User dont updated");
				}

				_logger.LogInformation($"Updated employee - {model.Id}");
				return RedirectToAction("Index");
			}
			catch (ArgumentException ex)
			{
				_logger.LogError(ex.Message, ex);
				return BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message, ex);
				return StatusCode(500);
			}
		}
	}
}
