using ApolloWMS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ApolloWMS.Controllers
{
    public class HolidaysController : Controller
    {
        private readonly MainDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private ISession _session => _httpContextAccessor.HttpContext.Session;
        private string _email = null;

        public HolidaysController(MainDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;

            if (_session.GetString("sid") != null)
            {
                _email = _session.GetString("sid");
            }
        }

        // GET: Holidays
        public async Task<IActionResult> Index()
        {
            ViewBag.Name = _context.Employee.SingleOrDefault(e => e.Email == _email).FirstName;
            return View(await _context.Holidays.ToListAsync());
        }

        // GET: Holidays/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var holidays = await _context.Holidays
                .FirstOrDefaultAsync(m => m.HolidayId == id);
            if (holidays == null)
            {
                return NotFound();
            }

            ViewBag.Name = _context.Employee.SingleOrDefault(e => e.Email == _email).FirstName;
            return View(holidays);
        }

        // GET: Holidays/Create
        public IActionResult Create()
        {
            ViewBag.Name = _context.Employee.SingleOrDefault(e => e.Email == _email).FirstName;
            return View();
        }

        // POST: Holidays/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HolidayName,DefinedDate,EncompassedYear,UpdatedBy,UpdatedDate")] Holidays holidays)
        {
            if (ModelState.IsValid)
            {
                holidays.HolidayId = Guid.NewGuid();
                _context.Add(holidays);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(holidays);
        }

        // GET: Holidays/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var holidays = await _context.Holidays.FindAsync(id);
            if (holidays == null)
            {
                return NotFound();
            }

            ViewBag.Name = _context.Employee.SingleOrDefault(e => e.Email == _email).FirstName;
            return View(holidays);
        }

        // POST: Holidays/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("HolidayId,HolidayName,DefinedDate,EncompassedYear")] Holidays holidays)
        {
            if (id != holidays.HolidayId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                holidays.UpdatedBy = "ADMIN";
                holidays.UpdatedDate = DateTime.Now;

                try
                {
                    _context.Update(holidays);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HolidaysExists(holidays.HolidayId))
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
            return View(holidays);
        }

        // GET: Holidays/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var holidays = await _context.Holidays
                .FirstOrDefaultAsync(m => m.HolidayId == id);
            if (holidays == null)
            {
                return NotFound();
            }

            ViewBag.Name = _context.Employee.SingleOrDefault(e => e.Email == _email).FirstName;
            return View(holidays);
        }

        // POST: Holidays/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var holidays = await _context.Holidays.FindAsync(id);
            _context.Holidays.Remove(holidays);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HolidaysExists(Guid id)
        {
            return _context.Holidays.Any(e => e.HolidayId == id);
        }
    }
}
