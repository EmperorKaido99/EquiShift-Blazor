using EquiShift.App.Models;

namespace EquiShift.App.Services;

public class ShiftSchedulerService : IShiftSchedulerService
{
    private List<StaffMember> _staff = new()
    {
        new StaffMember { Id = 1,  Name = "Alice",   Role = StaffRole.Supervisor },
        new StaffMember { Id = 2,  Name = "Bob",     Role = StaffRole.Cleaner },
        new StaffMember { Id = 3,  Name = "Carol",   Role = StaffRole.Regular },
        new StaffMember { Id = 4,  Name = "David",   Role = StaffRole.Regular },
        new StaffMember { Id = 5,  Name = "Eve",     Role = StaffRole.Regular },
        new StaffMember { Id = 6,  Name = "Frank",   Role = StaffRole.Regular },
        new StaffMember { Id = 7,  Name = "Grace",   Role = StaffRole.Regular },
        new StaffMember { Id = 8,  Name = "Hank",    Role = StaffRole.Regular },
        new StaffMember { Id = 9,  Name = "Iris",    Role = StaffRole.Regular },
        new StaffMember { Id = 10, Name = "Jack",    Role = StaffRole.Regular },
    };

    public List<StaffMember> GetStaff() => _staff;

    public void UpdateStaff(List<StaffMember> staff) => _staff = staff;

    public MonthlySchedule GenerateSchedule(int year, int month)
    {
        // Reset counters
        var staff = _staff.Select(s => new StaffMember
        {
            Id = s.Id,
            Name = s.Name,
            Role = s.Role
        }).ToList();

        var schedule = new MonthlySchedule { Year = year, Month = month, Staff = staff };
        var daysInMonth = DateTime.DaysInMonth(year, month);

        var supervisor = staff.FirstOrDefault(s => s.Role == StaffRole.Supervisor);
        var cleaner    = staff.FirstOrDefault(s => s.Role == StaffRole.Cleaner);
        var regulars   = staff.Where(s => s.Role == StaffRole.Regular).ToList();

        for (int day = 1; day <= daysInMonth; day++)
        {
            var date = new DateTime(year, month, day);
            bool isWeekday = date.DayOfWeek is not DayOfWeek.Saturday and not DayOfWeek.Sunday;
            bool isCleanerDay = date.DayOfWeek is DayOfWeek.Monday or DayOfWeek.Wednesday or DayOfWeek.Friday;

            // Day shift
            StaffMember? dayWorker = null;

            if (supervisor != null && isWeekday)
            {
                dayWorker = supervisor;
                supervisor.ShiftsThisMonth++;
                supervisor.DayShifts++;
            }
            else
            {
                dayWorker = PickLeastUsed(regulars, ShiftType.Day);
                if (dayWorker != null)
                {
                    dayWorker.ShiftsThisMonth++;
                    dayWorker.DayShifts++;
                }
            }

            schedule.Assignments.Add(new ShiftAssignment
            {
                Date = date,
                ShiftType = ShiftType.Day,
                AssignedStaff = dayWorker,
                IsShortage = dayWorker == null
            });

            // Cleaner day shift (Mon/Wed/Fri)
            if (cleaner != null && isCleanerDay)
            {
                cleaner.ShiftsThisMonth++;
                cleaner.DayShifts++;
                schedule.Assignments.Add(new ShiftAssignment
                {
                    Date = date,
                    ShiftType = ShiftType.Day,
                    AssignedStaff = cleaner,
                    IsShortage = false
                });
            }

            // Night shift — pick a regular who hasn't worked today
            var availableForNight = regulars
                .Where(r => !schedule.Assignments
                    .Any(a => a.Date == date && a.AssignedStaff?.Id == r.Id))
                .ToList();

            var nightWorker = PickLeastUsed(availableForNight, ShiftType.Night);
            if (nightWorker != null)
            {
                nightWorker.ShiftsThisMonth++;
                nightWorker.NightShifts++;
            }

            schedule.Assignments.Add(new ShiftAssignment
            {
                Date = date,
                ShiftType = ShiftType.Night,
                AssignedStaff = nightWorker,
                IsShortage = nightWorker == null
            });
        }

        return schedule;
    }

    private StaffMember? PickLeastUsed(List<StaffMember> pool, ShiftType preferredBalance)
    {
        if (!pool.Any()) return null;

        return preferredBalance == ShiftType.Day
            ? pool.OrderBy(s => s.ShiftsThisMonth).ThenBy(s => s.DayShifts).First()
            : pool.OrderBy(s => s.ShiftsThisMonth).ThenBy(s => s.NightShifts).First();
    }
}