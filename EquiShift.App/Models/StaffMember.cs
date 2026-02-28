namespace EquiShift.App.Models;

public enum StaffRole
{
    Regular,
    Supervisor,
    Cleaner
}

public enum ShiftType
{
    Day,
    Night
}

public class StaffMember
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public StaffRole Role { get; set; } = StaffRole.Regular;
    public int ShiftsThisMonth { get; set; } = 0;
    public int DayShifts { get; set; } = 0;
    public int NightShifts { get; set; } = 0;
}