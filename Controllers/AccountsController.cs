using ApolloWMS.Models;
using ApolloWMS.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Linq;

namespace ApolloWMS.Controllers
{
    public class AccountsController : Controller
    {
        private readonly MainDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Utilities utilities;

        private ISession _session => _httpContextAccessor.HttpContext.Session;
        private string _email = null;

        public AccountsController(MainDbContext context, IHttpContextAccessor httpContextAccessor)
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
                    var attemptedPassword = loginViewModel.Password;
                    var verifiedPassword = utilities.DecryptPassword(login.Password, login.Salt);
                    if (attemptedPassword == verifiedPassword)
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

        [HttpGet]
        public IActionResult ChangePassword()
        {
            if (_session.GetString("sid") == null)
            {
                return RedirectToAction(nameof(Index));
            }

            _email = _session.GetString("sid");
            var employee = _context.Employee.SingleOrDefault(e => e.Email == _email);

            var chpwd = new ChangePasswordViewModel()
            {
                EmployeeId = employee.EmployeeId,
                CurrentPassword = employee.Password,
                NewPassword = string.Empty,
                ConfirmPassword = string.Empty
            };

            var reqId = _context.Employee.SingleOrDefault(e => e.Email == _email).EmployeeId;
            ViewBag.Role = utilities.GetRoleById(reqId);
            ViewBag.Name = _context.Employee.SingleOrDefault(e => e.Email == _email).FirstName;
            return View(chpwd);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(Guid id, [Bind("EmployeeId,CurrentPassword,NewPassword,ConfirmPassword")] ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var employee = _context.Employee.SingleOrDefault(e => e.EmployeeId == model.EmployeeId);
                if (employee == null)
                {
                    return NotFound();
                }

                if (string.IsNullOrEmpty(employee.Salt))
                {
                    employee.Salt = utilities.UniqueKey;
                }
                employee.Password = utilities.EncryptPassword(model.NewPassword, employee.Salt);

                _context.Employee.Update(employee);
                _context.SaveChanges();
            }
            return RedirectToAction("Create", "LeaveRequests");
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
