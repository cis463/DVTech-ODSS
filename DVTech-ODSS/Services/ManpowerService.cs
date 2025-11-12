using DVTech_ODSS.Data;
using DVTech_ODSS.Models;
using Microsoft.EntityFrameworkCore;

namespace DVTech_ODSS.Services
{
    public class ManpowerService
    {
        private readonly ApplicationDbContext _context;

        public ManpowerService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===== EMPLOYEE METHODS =====

        public async Task<List<Employee>> GetAllEmployeesAsync()
        {
            return await _context.Employees
                .Where(e => e.IsActive)
                .OrderBy(e => e.FullName)
                .ToListAsync();
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            return await _context.Employees.FindAsync(id);
        }

        public async Task<bool> AddEmployeeAsync(Employee employee)
        {
            try
            {
                employee.DateHired = DateTime.Now;
                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateEmployeeAsync(Employee employee)
        {
            try
            {
                _context.Employees.Update(employee);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            try
            {
                var employee = await GetEmployeeByIdAsync(id);
                if (employee != null)
                {
                    employee.IsActive = false;
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Employee>> GetEmployeesByDepartmentAsync(string department)
        {
            return await _context.Employees
                .Where(e => e.IsActive && e.Department == department)
                .OrderBy(e => e.FullName)
                .ToListAsync();
        }

        // ===== SCHEDULE METHODS =====

        public async Task<List<Schedule>> GetAllSchedulesAsync()
        {
            return await _context.Schedules
                .Include(s => s.Employee)
                .Where(s => s.IsActive)
                .OrderByDescending(s => s.Date)
                .ToListAsync();
        }

        public async Task<Schedule?> GetScheduleByIdAsync(int id)
        {
            return await _context.Schedules
                .Include(s => s.Employee)
                .FirstOrDefaultAsync(s => s.ScheduleId == id);
        }

        public async Task<bool> AddScheduleAsync(Schedule schedule)
        {
            try
            {
                _context.Schedules.Add(schedule);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateScheduleAsync(Schedule schedule)
        {
            try
            {
                _context.Schedules.Update(schedule);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteScheduleAsync(int id)
        {
            try
            {
                var schedule = await GetScheduleByIdAsync(id);
                if (schedule != null)
                {
                    schedule.IsActive = false;
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Schedule>> GetSchedulesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Schedules
                .Include(s => s.Employee)
                .Where(s => s.IsActive && s.Date >= startDate && s.Date <= endDate)
                .OrderBy(s => s.Date)
                .ThenBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<List<Schedule>> GetSchedulesByEmployeeAsync(int employeeId)
        {
            return await _context.Schedules
                .Include(s => s.Employee)
                .Where(s => s.IsActive && s.EmployeeId == employeeId)
                .OrderByDescending(s => s.Date)
                .ToListAsync();
        }

        public async Task<List<Schedule>> GetSchedulesByDateAsync(DateTime date)
        {
            return await _context.Schedules
                .Include(s => s.Employee)
                .Where(s => s.IsActive && s.Date.Date == date.Date)
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }
    }
}