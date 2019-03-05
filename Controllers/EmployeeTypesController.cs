using ApolloWMS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ApolloWMS.Controllers
{
    public class EmployeeTypesController : Controller
    {
        private readonly MainDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private ISession _session => _httpContextAccessor.HttpContext.Session;
        private string _email = null;

        public EmployeeTypesController(MainDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;

            if (_session.GetString("sid") != null)
            {
                _email = _session.GetString("sid");
            }
        }

        // GET: EmployeeTypes
        public async Task<IActionResult> Index()
        {
            ViewBag.Name = _context.Employee.SingleOrDefault(e => e.Email == _email).FirstName;
            return View(await _context.EmployeeType.ToListAsync());
        }

        // GET: EmployeeTypes/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeType = await _context.EmployeeType
                .FirstOrDefaultAsync(m => m.EmployeeTypeId == id);
            if (employeeType == null)
            {
                return NotFound();
            }

            ViewBag.Name = _context.Employee.SingleOrDefault(e => e.Email == _email).FirstName;
            return View(employeeType);
        }

        // GET: EmployeeTypes/Create
        public IActionResult Create()
        {
            ViewBag.Name = _context.Employee.SingleOrDefault(e => e.Email == _email).FirstName;
            return View();
        }

        // POST: EmployeeTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeTypeName")] EmployeeType employeeType)
        {
            if (ModelState.IsValid)
            {
                employeeType.EmployeeTypeId = Guid.NewGuid();
                _context.Add(employeeType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(employeeType);
        }

        // GET: EmployeeTypes/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeType = await _context.EmployeeType.FindAsync(id);
            if (employeeType == null)
            {
                return NotFound();
            }

            ViewBag.Name = _context.Employee.SingleOrDefault(e => e.Email == _email).FirstName;
            return View(employeeType);
        }

        // POST: EmployeeTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("EmployeeTypeId,EmployeeTypeName")] EmployeeType employeeType)
        {
            if (id != employeeType.EmployeeTypeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employeeType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeTypeExists(employeeType.EmployeeTypeId))
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
            return View(employeeType);
        }

        // GET: EmployeeTypes/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeType = await _context.EmployeeType
                .FirstOrDefaultAsync(m => m.EmployeeTypeId == id);
            if (employeeType == null)
            {
                return NotFound();
            }

            ViewBag.Name = _context.Employee.SingleOrDefault(e => e.Email == _email).FirstName;
            return View(employeeType);
        }

        // POST: EmployeeTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var employeeType = await _context.EmployeeType.FindAsync(id);
            _context.EmployeeType.Remove(employeeType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeTypeExists(Guid id)
        {
            return _context.EmployeeType.Any(e => e.EmployeeTypeId == id);
        }
    }
}
