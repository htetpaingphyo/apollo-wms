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
    public class BalancesController : Controller
    {
        private readonly MainDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Utilities utilities;

        private ISession _session => _httpContextAccessor.HttpContext.Session;
        private string _email = null;

        public BalancesController(MainDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;

            if (_session.GetString("sid") != null)
            {
                _email = _session.GetString("sid");
            }

            utilities = new Utilities(context);
        }

        // GET: Balances
        public async Task<IActionResult> Index()
        {
            var reqId = _context.Employee.SingleOrDefault(e => e.Email == _email).EmployeeId;
            ViewBag.Name = _context.Employee.SingleOrDefault(e => e.Email == _email).FirstName;
            var balances = await _context.Balance.ToListAsync();
            List<BalanceViewModel> balanceViews = new List<BalanceViewModel>();

            // Not quite comprehending what it's purpose!
            //
            //if (!utilities.
            //        GetRoleById(
            //            _context.Employee.SingleOrDefault(e => e.Email == _email).EmployeeId
            //                ).Equals(RoleType.EXAMINER))
            //{
            //    foreach (var role in utilities.GetReportersById(reqId))
            //    {
            //        // intentionally left blank...
            //    }
            //}

            if (utilities.GetRoleById(reqId) == RoleType.REQUESTER)
            {
                balances = _context.Balance.Where(b => b.EmployeeId == reqId).ToList();
            }

            if (utilities.GetRoleById(reqId) == RoleType.AUTHORIZER)
            {
                balances = utilities.GetBalancesByReportId(reqId);
            }

            foreach (var bal in balances)
            {
                balanceViews.Add(new BalanceViewModel()
                {
                    BalanceId = bal.BalanceId,
                    EmployeeName = $"{_context.Employee.SingleOrDefault(e => e.EmployeeId == bal.EmployeeId).LastName}, " +
                                    $"{_context.Employee.SingleOrDefault(e => e.EmployeeId == bal.EmployeeId).FirstName}",
                    LeaveType = _context.LeaveType.SingleOrDefault(l => l.LeaveTypeId == bal.LeaveTypeId).LeaveTypeName,
                    TotalBalance = bal.TotalBalance,
                    UsedBalance = bal.UsedBalance,
                    RemainedBalance = bal.RemainedBalance,
                    IsEdited = bal.IsEdited
                });
            }

            return View(balanceViews);
        }

        // GET: Balances/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var balance = await _context.Balance
                .FirstOrDefaultAsync(m => m.BalanceId == id);
            if (balance == null)
            {
                return NotFound();
            }

            ViewBag.Name = _context.Employee.SingleOrDefault(e => e.Email == _email).FirstName;
            return View(balance);
        }

        // GET: Balances/Create
        public IActionResult Create()
        {
            ViewBag.Employees = _context.Employee.ToList();
            ViewBag.LeaveTypes = _context.LeaveType.ToList();
            ViewBag.Name = _context.Employee.SingleOrDefault(e => e.Email == _email).FirstName;
            return View();
        }

        // POST: Balances/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BalanceId,EmployeeId,LeaveTypeId,TotalBalance,UsedBalance,RemainedBalance,IsEdited,UpdatedBy,UpdatedDate")] Balance balance)
        {
            if (ModelState.IsValid)
            {
                balance.BalanceId = Guid.NewGuid();
                _context.Add(balance);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(balance);
        }

        // GET: Balances/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var balance = await _context.Balance.FindAsync(id);
            if (balance == null)
            {
                return NotFound();
            }

            ViewBag.Employees = _context.Employee.ToList();
            ViewBag.LeaveTypes = _context.LeaveType.ToList();
            ViewBag.Name = _context.Employee.SingleOrDefault(e => e.Email == _email).FirstName;
            return View(balance);
        }

        // POST: Balances/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("BalanceId,EmployeeId,LeaveTypeId,TotalBalance,UsedBalance,RemainedBalance,IsEdited,UpdatedBy,UpdatedDate")] Balance balance)
        {
            if (id != balance.BalanceId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(balance);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BalanceExists(balance.BalanceId))
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
            return View(balance);
        }

        // GET: Balances/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var balance = await _context.Balance
                .FirstOrDefaultAsync(m => m.BalanceId == id);
            if (balance == null)
            {
                return NotFound();
            }

            ViewBag.Name = _context.Employee.SingleOrDefault(e => e.Email == _email).FirstName;
            return View(balance);
        }

        // POST: Balances/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var balance = await _context.Balance.FindAsync(id);
            _context.Balance.Remove(balance);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BalanceExists(Guid id)
        {
            return _context.Balance.Any(e => e.BalanceId == id);
        }
    }
}
