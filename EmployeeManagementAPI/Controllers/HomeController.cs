namespace EmployeeManagementAPI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICompanyInfoRepository _companyInfoRepository;

        public HomeController(ICompanyInfoRepository companyInfoRepository, ILogger<HomeController> logger)
        {
            _logger = logger;
            _companyInfoRepository = companyInfoRepository;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _companyInfoRepository.GetAsync();
            return View(result);
        }
    }
}
