using ApolloWMS.Models;
using ApolloWMS.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;

namespace ApolloWMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly MainDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Utilities utilities;

        private ISession _session => _httpContextAccessor.HttpContext.Session;
        private string _email = null;

        public HomeController(MainDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            utilities = new Utilities(context);
        }

        public IActionResult Index()
        {
            if (_session.GetString("sid") != null)
            {
                return RedirectToAction("Create", "LeaveRequests");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login([Bind("Email, Password")] LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                var login = _context.Employee.SingleOrDefault(e => e.Email == loginViewModel.Email);
                if (login != null)
                {
                    if (login.Password == loginViewModel.Password)
                    {
                        // set user's email to session state
                        _email = login.Email;
                        HttpContext.Session.SetString("sid", _email);

                        var reqId = _context.Employee.SingleOrDefault(e => e.Email == _email).EmployeeId;
                        var role = utilities.GetRoleById(reqId);

                        if (role == RoleType.REQUESTER)
                        {
                            return RedirectToAction("Create", "LeaveRequests");
                        }
                        else if (role == RoleType.EXAMINER)
                        {
                            return RedirectToAction("LeaveRequestLog", "LeaveRequests");
                        }
                        else
                        {
                            return RedirectToAction("Index", "LeaveRequests");
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }
            }

            return View();
        }

        public IActionResult Logout()
        {
            _session.Clear();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
