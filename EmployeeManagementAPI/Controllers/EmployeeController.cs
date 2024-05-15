﻿using EmployeeManagementAPI.Models.ViewModels;

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
        public async Task<IActionResult> Index(string? searchName, string? sortColumn, string? sortDirection)
        {
            try
            {
                var employees = await _employeeRepository.ListAsync(searchName, sortColumn, sortDirection);
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
    }
}
