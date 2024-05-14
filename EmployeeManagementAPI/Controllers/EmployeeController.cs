namespace EmployeeManagementAPI.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeDAL _employeeDAL;
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(IEmployeeDAL employeeDAL, ILogger<EmployeeController> logger)
        {
            _employeeDAL = employeeDAL;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var employees = await _employeeDAL.GetAllAsync();
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
    }
}
