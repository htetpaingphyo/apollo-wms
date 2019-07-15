using ApolloWMS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ApolloWMS.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly MainDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Utilities utilities;

        private ISession _session => _httpContextAccessor.HttpContext.Session;
        private string _email = null;

        public EmployeesController(MainDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;

            if (_session.GetString("sid") != null)
            {
                _email = _session.GetString("sid");
            }

            utilities = new Utilities(context);
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            ViewBag.Name = _context.Employee.SingleOrDefault(e => e.Email == _email).FirstName;
            return View(await _context.Employee.ToListAsync());
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employee
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            ViewBag.Name = _context.Employee.SingleOrDefault(e => e.Email == _email).FirstName;
            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            ViewBag.Name = _context.Employee.SingleOrDefault(e => e.Email == _email).FirstName;
            ViewBag.Departments = _context.Department.ToList();
            ViewBag.EmpTypes = _context.EmployeeType.ToList();

            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Email,Password,Gender,DepartmentId,Designation,Region,EmployeeTypeId,HiredDate,ContactNumber,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                employee.EmployeeId = Guid.NewGuid();
                employee.Salt = utilities.UniqueKey;
                employee.Password = utilities.EncryptPassword(employee.Password, employee.Salt);
                employee.CreatedBy = _email;
                employee.CreatedDate = DateTime.Now;
                employee.UpdatedBy = null;
                employee.UpdatedDate = null;

                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employee.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            ViewBag.EmpTypes = _context.EmployeeType.ToList();
            ViewBag.Departments = _context.Department.ToList();
            ViewBag.Name = _context.Employee.SingleOrDefault(e => e.Email == _email).FirstName;
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("EmployeeId,FirstName,LastName,Email,Password,Gender,DepartmentId,Designation,Region,EmployeeTypeId,HiredDate,ContactNumber,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate")] Employee employee)
        {
            if (id != employee.EmployeeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (string.IsNullOrEmpty(employee.Salt))
                    {
                        employee.Salt = utilities.UniqueKey;
                    }
                    employee.Password = utilities.EncryptPassword(employee.Password, employee.Salt);
                    employee.UpdatedBy = _email;
                    employee.UpdatedDate = DateTime.Now;

                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.EmployeeId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employee
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            ViewBag.Name = _context.Employee.SingleOrDefault(e => e.Email == _email).FirstName;
            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var employee = await _context.Employee.FindAsync(id);
            _context.Employee.Remove(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(Guid id)
        {
            return _context.Employee.Any(e => e.EmployeeId == id);
        }
    }
}
