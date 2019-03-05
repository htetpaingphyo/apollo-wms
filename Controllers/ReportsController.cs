using ApolloWMS.Models;
using ApolloWMS.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApolloWMS.Controllers
{
    public class ReportsController : Controller
    {
        private readonly MainDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Utilities utilities;

        private ISession _session => _httpContextAccessor.HttpContext.Session;
        private string _email = null;

        public ReportsController(MainDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;

            if (_session.GetString("sid") != null)
            {
                _email = _session.GetString("sid");
            }

            utilities = new Utilities(context);
        }

        // GET: Reports
        public async Task<IActionResult> Index()
        {
            var reqId = _context.Employee.SingleOrDefault(e => e.Email == _email).EmployeeId;
            ViewBag.Role = utilities.GetRoleById(reqId);
            ViewBag.Name = _context.Employee.SingleOrDefault(e => e.Email == _email).FirstName;

            var reports = await _context.Report.ToListAsync();
            List<ReportViewModel> models = new List<ReportViewModel>();

            foreach (var rpt in reports)
            {
                models.Add(new ReportViewModel()
                {
                    ReportId = rpt.ReportId,
                    Email = _context.Employee.SingleOrDefault(e => e.EmployeeId == rpt.EmployeeId).Email,
                    ReportingLevel = rpt.ReportingLevel,
                    ReportTo = _context.Employee.SingleOrDefault(e => e.EmployeeId == rpt.ReportTo).Email,
                });
            }

            return View(models);
        }

        // GET: Reports/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var report = await _context.Report
                .FirstOrDefaultAsync(m => m.ReportId == id);
            if (report == null)
            {
                return NotFound();
            }

            ViewBag.Name = _context.Employee.SingleOrDefault(e => e.Email == _email).FirstName;
            return View(report);
        }

        // GET: Reports/Create
        public IActionResult Create()
        {
            ViewBag.Employees = _context.Employee.ToList();
            ViewBag.Name = _context.Employee.SingleOrDefault(e => e.Email == _email).FirstName;
            return View();
        }

        // POST: Reports/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReportId,EmployeeId,ReportingLevel,ReportTo")] Report report)
        {
            if (ModelState.IsValid)
            {
                report.ReportId = Guid.NewGuid();
                _context.Add(report);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(report);
        }

        // GET: Reports/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var report = await _context.Report.FindAsync(id);
            if (report == null)
            {
                return NotFound();
            }

            ViewBag.Employees = _context.Employee.ToList();
            ViewBag.Name = _context.Employee.SingleOrDefault(e => e.Email == _email).FirstName;
            return View(report);
        }

        // POST: Reports/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ReportId,EmployeeId,ReportingLevel,ReportTo")] Report report)
        {
            if (id != report.ReportId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(report);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReportExists(report.ReportId))
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
            return View(report);
        }

        // GET: Reports/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var report = await _context.Report
                .FirstOrDefaultAsync(m => m.ReportId == id);
            if (report == null)
            {
                return NotFound();
            }

            ViewBag.Name = _context.Employee.SingleOrDefault(e => e.Email == _email).FirstName;
            return View(report);
        }

        // POST: Reports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var report = await _context.Report.FindAsync(id);
            _context.Report.Remove(report);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReportExists(Guid id)
        {
            return _context.Report.Any(e => e.ReportId == id);
        }
    }
}
