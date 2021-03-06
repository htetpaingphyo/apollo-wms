﻿using ApolloWMS.Models;
using ApolloWMS.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApolloWMS.Controllers
{
    public class Utilities
    {
        private MainDbContext _context;
        private SecurityService _service;

        public Utilities(MainDbContext dbContext)
        {
            _context = dbContext;
            _service = new SecurityService();
        }

        public string UniqueKey => _service.GetUniqueString;

        public bool CheckCompleteMonth(DateTime today)
        {
            return today.Day == DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
        }

        public string GetRoleById(Guid userId)
        {
            string roleName = string.Empty;

            var role = from er in _context.EmployeeRole
                       join e in _context.Employee on er.EmployeeId equals e.EmployeeId
                       join r in _context.Role on er.RoleId equals r.RoleId
                       where e.EmployeeId == userId
                       select r;

            if (role != null)
            {
                foreach (var r in role)
                {
                    roleName = r.RoleName;
                }
            }

            return roleName;
        }

        public List<Balance> GetBalancesByReportId(Guid empId)
        {
            var balances = from b in _context.Balance.ToList()
                           where (from r in _context.Report
                                  where r.ReportTo == empId
                                  select r.EmployeeId).Contains(b.EmployeeId)
                           || b.EmployeeId == empId
                           select b;

            return balances.ToList();
        }

        // Calculate the remaining balance of employee after taking leave.
        public double CalculateBalance(Guid userId, Guid leaveTypeId, DateTime from, DateTime to)
        {
            double totalDays = 0D;

            var emp = _context.Employee.SingleOrDefault(e => e.EmployeeId == userId);

            if (emp != null)
            {
                var hireDate = emp.HiredDate;
                var type = _context.EmployeeType.SingleOrDefault(t => t.EmployeeTypeId == emp.EmployeeTypeId).EmployeeTypeName;
                var leaveType = _context.LeaveType.SingleOrDefault(l => l.LeaveTypeId == leaveTypeId).LeaveTypeName;

                if (leaveType == LeaveTypeFormat.SICK)
                {
                    totalDays = CalculateHolidays(from, to);
                }

                if (leaveType == LeaveTypeFormat.EARNED)
                {
                    if (type == EmployeeTypeFormat.LOCAL)
                    {
                        // 0.833 stands earning points for local employees where they'll get every complete month.
                        if (emp.HiredDate.Value.Year == DateTime.Now.Year)
                        {
                            totalDays = Math.Round(0.833 * (DateTime.Now.Month - hireDate.Value.Month), 2) - CalculateHolidays(from, to);
                        }
                        else
                        {
                            totalDays = Math.Round(0.833 * (DateTime.Now.Month), 2) - CalculateHolidays(from, to);
                        }
                    }
                    else
                    {
                        // 2.083 stands earning points for local employees where they'll get every complete month.
                        if (emp.HiredDate.Value.Year == DateTime.Now.Year)
                        {
                            totalDays = Math.Round(2.083 * (DateTime.Now.Month - hireDate.Value.Month), 2) - CalculateHolidays(from, to);
                        }
                        else
                        {
                            totalDays = Math.Round(2.083 * (DateTime.Now.Month), 2) - CalculateHolidays(from, to);
                        }

                    }
                }
            }

            return totalDays;
        }

        // Calculate the number of days taking rather than Weekends and National Holidays.
        public double CalculateHolidays(DateTime from, DateTime to)
        {
            double totalHours = 0D;
            double totalDays = 0D;

            if (from == to)
            {
                totalDays = (to - from).TotalHours > 4.5 ? 1D : 0.5D;
            }
            else
            {
                while (from <= to)
                {
                    if ((from.DayOfWeek != DayOfWeek.Saturday) && (from.DayOfWeek != DayOfWeek.Sunday))
                    {
                        if (_context.Holidays.SingleOrDefault(d => d.DefinedDate.Date == from.Date) == null)
                        {
                            totalHours++;
                        }
                    }
                    from = from.AddHours(1);
                }
                totalDays = Math.Round(totalHours / 24, 1);
            }

            return totalDays;
        }

        // Get the authorizer's Id from report table.
        public Guid? GetAuthorizerId(Guid reqId)
        {
            return _context.Report.SingleOrDefault(r => r.EmployeeId == reqId).ReportTo;
        }

        public List<Report> GetReportersById(Guid resId)
        {
            return _context.Report.Where(r => r.ReportTo == resId).ToList();
        }

        public string EncryptPassword(string pwdToEncrypt, string salt)
        {
            return _service.Encrypt(pwdToEncrypt, salt);
        }

        public string DecryptPassword(string pwdToDecrypt, string salt)
        {
            return _service.Decrypt(pwdToDecrypt, salt);
        }
    }
}
