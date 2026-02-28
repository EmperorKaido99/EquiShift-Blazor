using EquiShift.App.Models;

namespace EquiShift.App.Services;

public interface IShiftSchedulerService
{
    List<StaffMember> GetStaff();
    void UpdateStaff(List<StaffMember> staff);
    MonthlySchedule GenerateSchedule(int year, int month);
}