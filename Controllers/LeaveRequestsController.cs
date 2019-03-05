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
    public class LeaveRequestsController : Controller
    {
        private readonly MainDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private Utilities utilities;

        private ISession _session => _httpContextAccessor.HttpContext.Session;
        private string _email = null;

        public LeaveRequestsController(MainDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;

            if (_session.GetString("sid") != null)
            {
                _email = _session.GetString("sid");
            }

            utilities = new Utilities(context);
        }

        // GET: Leave requests history for EXAMINER to examine.
        public async Task<IActionResult> LeaveRequestLog()
        {
            var reqId = _context.Employee.SingleOrDefault(e => e.Email == _email).EmployeeId;
            ViewBag.Role = utilities.GetRoleById(reqId);
            ViewBag.Name = _context.Employee.SingleOrDefault(e => e.Email == _email).FirstName;
            var requests = await _context.LeaveRequest.ToListAsync();

            List<LeaveRequestViewModel> leaveRequestViews = new List<LeaveRequestViewModel>();

            if (requests != null)
            {
                foreach (var req in requests)
                {
                    leaveRequestViews.Add(new LeaveRequestViewModel()
                    {
                        LeaveRequestId = req.LeaveRequestId,
                        LeaveTypeName = _context.LeaveType.SingleOrDefault(l => l.LeaveTypeId == req.LeaveTypeId).LeaveTypeName,
                        From = req.FromTimeOff,
                        To = req.ToTimeOff,
                        Total = req.TotalTimeOff,
                        ReasonForAbsence = req.ReasonForAbsence,
                        EmergencyContact = req.EmergencyContact,
                        Requester = _context.Employee.SingleOrDefault(e => e.EmployeeId == req.RequesterId).Email,
                        RequestedDate = req.RequestedDate,
                        Status = req.Status,
                        Remark = req.Remark
                    });
                }
            }

            return View(leaveRequestViews);
        }

        // GET: Leave requests list for REQUESTER/AUTHORIZER to review.
        public async Task<IActionResult> Index()
        {
            string role = null;
            var reqId = _context.Employee.SingleOrDefault(e => e.Email == _email).EmployeeId;
            ViewBag.Role = role = utilities.GetRoleById(reqId);
            ViewBag.Name = _context.Employee.SingleOrDefault(e => e.Email == _email).FirstName;

            var requests_1 = await _context.LeaveRequest.Where(r => r.Status == RequestStatus.PENDING).ToListAsync();
            List<LeaveRequestViewModel> leaveRequestViews = new List<LeaveRequestViewModel>();

            if (role == RoleType.REQUESTER)
            {
                if (requests_1 != null)
                {
                    foreach (var req in requests_1)
                    {
                        leaveRequestViews.Add(new LeaveRequestViewModel()
                        {
                            LeaveRequestId = req.LeaveRequestId,
                            LeaveTypeName = _context.LeaveType.SingleOrDefault(l => l.LeaveTypeId == req.LeaveTypeId).LeaveTypeName,
                            From = req.FromTimeOff,
                            To = req.ToTimeOff,
                            Total = req.TotalTimeOff,
                            ReasonForAbsence = req.ReasonForAbsence,
                            EmergencyContact = req.EmergencyContact,
                            Requester = _context.Employee.SingleOrDefault(e => e.EmployeeId == req.RequesterId).Email,
                            RequestedDate = req.RequestedDate,
                            Status = req.Status,
                            Remark = req.Remark
                        });
                    }
                }
            }
            else
            {
                var requests_2 = requests_1.Where(r => utilities.GetAuthorizerId(r.RequesterId) == reqId).ToList();
                if (requests_2 != null)
                {
                    foreach (var req in requests_2)
                    {
                        leaveRequestViews.Add(new LeaveRequestViewModel()
                        {
                            LeaveRequestId = req.LeaveRequestId,
                            LeaveTypeName = _context.LeaveType.SingleOrDefault(l => l.LeaveTypeId == req.LeaveTypeId).LeaveTypeName,
                            From = req.FromTimeOff,
                            To = req.ToTimeOff,
                            Total = req.TotalTimeOff,
                            ReasonForAbsence = req.ReasonForAbsence,
                            EmergencyContact = req.EmergencyContact,
                            Requester = _context.Employee.SingleOrDefault(e => e.EmployeeId == req.RequesterId).Email,
                            RequestedDate = req.RequestedDate,
                            Status = req.Status,
                            Remark = req.Remark
                        });
                    }
                }
            }

            return View(leaveRequestViews);
        }

        // GET: Leave request detail.
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveRequest = await _context.LeaveRequest
                .FirstOrDefaultAsync(m => m.LeaveRequestId == id);
            if (leaveRequest == null)
            {
                return NotFound();
            }

            ViewBag.Name = _context.Employee.SingleOrDefault(e => e.Email == _email).FirstName;
            return View(leaveRequest);
        }

        // GET: Create new leave request.
        public IActionResult Create()
        {
            var reqId = _context.Employee.SingleOrDefault(e => e.Email == _email).EmployeeId;
            var role = utilities.GetRoleById(reqId);
            ViewBag.Role = role;
            ViewBag.Name = _context.Employee.SingleOrDefault(e => e.Email == _email).FirstName;
            ViewBag.LeaveTypes = _context.LeaveType.ToList();
            return View();
        }

        // POST: LeaveRequests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LeaveTypeId,FromTimeOff,ToTimeOff,ReasonForAbsence,EmergencyContact")] LeaveRequest leaveRequest)
        {
            var reqId = _context.Employee.SingleOrDefault(e => e.Email == _email).EmployeeId;

            if (ModelState.IsValid)
            {
                leaveRequest.LeaveRequestId = Guid.NewGuid();
                leaveRequest.TotalTimeOff = Convert.ToDecimal(utilities.CalculateHolidays(leaveRequest.FromTimeOff, leaveRequest.ToTimeOff));
                leaveRequest.RequesterId = reqId;
                leaveRequest.RequestedDate = DateTime.Now;
                leaveRequest.Status = RequestStatus.PENDING;

                _context.Add(leaveRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(leaveRequest);
        }

        // GET: LeaveRequests/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveRequest = await _context.LeaveRequest.FindAsync(id);
            if (leaveRequest == null)
            {
                return NotFound();
            }

            var leaveRequestEdit = new LeaveRequestEditModel()
            {
                LeaveRequestId = leaveRequest.LeaveRequestId,
                LeaveTypeId = leaveRequest.LeaveTypeId,
                From = leaveRequest.FromTimeOff,
                To = leaveRequest.ToTimeOff,
                Total = leaveRequest.TotalTimeOff,
                ReasonForAbsence = leaveRequest.ReasonForAbsence,
                EmergencyContact = leaveRequest.EmergencyContact,
                EmergencyContactName = leaveRequest.EmergencyContactName,
                EmergencyContactRS = leaveRequest.EmergencyContactRS,
                RequesterId = leaveRequest.RequesterId,
                RequestedDate = leaveRequest.RequestedDate,
                Status = leaveRequest.Status
            };

            var reqId = _context.Employee.SingleOrDefault(e => e.Email == _email).EmployeeId;
            var role = utilities.GetRoleById(reqId);
            ViewBag.Role = role;
            ViewBag.Name = _context.Employee.SingleOrDefault(e => e.Email == _email).FirstName;
            ViewBag.Requester = _context.Employee.Where(e => e.EmployeeId == leaveRequestEdit.RequesterId).ToList();
            ViewBag.LeaveTypes = _context.LeaveType.ToList();
            return View(leaveRequestEdit);
        }

        // POST: LeaveRequests/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("LeaveRequestId,LeaveTypeId,From,To,Total,ReasonForAbsence,EmergencyContact,EmergencyContactName,EmergencyContactRS,RequesterId,RequestedDate,Status")] LeaveRequestEditModel editModel)
        {
            var leaveRequest = _context.LeaveRequest.SingleOrDefault(l => l.LeaveRequestId == id);
            if (leaveRequest == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    leaveRequest.LeaveRequestId = editModel.LeaveRequestId;
                    leaveRequest.LeaveTypeId = editModel.LeaveTypeId;
                    leaveRequest.FromTimeOff = editModel.From;
                    leaveRequest.ToTimeOff = editModel.To;
                    leaveRequest.TotalTimeOff = Convert.ToDecimal(utilities.CalculateHolidays(leaveRequest.FromTimeOff, leaveRequest.ToTimeOff));
                    leaveRequest.ReasonForAbsence = editModel.ReasonForAbsence;
                    leaveRequest.EmergencyContact = editModel.EmergencyContact;
                    leaveRequest.EmergencyContactName = editModel.EmergencyContactName;
                    leaveRequest.EmergencyContactRS = editModel.EmergencyContactRS;
                    leaveRequest.RequesterId = editModel.RequesterId;
                    leaveRequest.RequestedDate = DateTime.Now;

                    _context.Update(leaveRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LeaveRequestExists(leaveRequest.LeaveRequestId))
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
            return View(leaveRequest);
        }


        // GET:
        public async Task<IActionResult> Permission(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var req = await _context.LeaveRequest.FindAsync(id);
            if (req == null)
            {
                return NotFound();
            }

            var levReqView = new LeaveRequestViewModel()
            {
                LeaveRequestId = req.LeaveRequestId,
                LeaveTypeName = _context.LeaveType.SingleOrDefault(l => l.LeaveTypeId == req.LeaveTypeId).LeaveTypeName,
                From = req.FromTimeOff,
                To = req.ToTimeOff,
                Total = req.TotalTimeOff,
                ReasonForAbsence = req.ReasonForAbsence,
                EmergencyContact = req.EmergencyContact,
                Requester = _context.Employee.SingleOrDefault(e => e.EmployeeId == req.RequesterId).Email,
                RequestedDate = req.RequestedDate,
                Status = req.Status,
                Remark = req.Remark
            };
            return View(levReqView);
        }

        [HttpPost]
        public async Task<IActionResult> Permission(Guid id, [Bind("LeaveRequestId,LeaveTypeName,From,To,Total,ReasonForAbsence,EmergencyContact,EmergencyContactName,EmergencyContactRS,Requester,RequestedDate,Status,Remark")]LeaveRequestViewModel model, string btnStatus)
        {
            if (ModelState.IsValid)
            {
                var req = _context.LeaveRequest.SingleOrDefault(l => l.LeaveRequestId == id);
                if (req == null)
                {
                    return NotFound();
                }

                if (btnStatus.Equals("Approve"))
                {
                    var balance = _context.Balance.SingleOrDefault(b => b.EmployeeId == req.RequesterId && b.LeaveTypeId == req.LeaveTypeId);
                    balance.UsedBalance = Convert.ToDecimal(utilities.CalculateHolidays(req.FromTimeOff, req.ToTimeOff));
                    balance.RemainedBalance = Convert.ToDecimal(utilities.CalculateBalance(req.RequesterId, balance.LeaveTypeId, req.FromTimeOff, req.ToTimeOff));
                    balance.UpdatedBy = null;
                    balance.UpdatedDate = null;

                    _context.Update(balance);
                    await _context.SaveChangesAsync();
                }

                req.Status = btnStatus.Equals("Approve") ? RequestStatus.APPROVED : RequestStatus.REJECTED;
                req.Remark = model.Remark;
                req.AuthorizerId = _context.Employee.SingleOrDefault(e => e.Email == _email).EmployeeId;
                req.AuthorizedDate = DateTime.Now;

                _context.Update(req);
                await _context.SaveChangesAsync();

            }

            return RedirectToAction(nameof(Index));
        }

        // GET: LeaveRequests/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveRequest = await _context.LeaveRequest
                .FirstOrDefaultAsync(m => m.LeaveRequestId == id);
            if (leaveRequest == null)
            {
                return NotFound();
            }

            ViewBag.Name = _context.Employee.SingleOrDefault(e => e.Email == _email).FirstName;
            return View(leaveRequest);
        }

        // POST: LeaveRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var leaveRequest = await _context.LeaveRequest.FindAsync(id);
            _context.LeaveRequest.Remove(leaveRequest);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LeaveRequestExists(Guid id)
        {
            return _context.LeaveRequest.Any(e => e.LeaveRequestId == id);
        }
    }
}
