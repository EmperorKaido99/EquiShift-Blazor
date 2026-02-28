namespace EquiShift.App.Models;

public class ShiftAssignment
{
    public DateTime Date { get; set; }
    public ShiftType ShiftType { get; set; }
    public StaffMember? AssignedStaff { get; set; }
    public bool IsShortage { get; set; } = false;
}

public class MonthlySchedule
{
    public int Year { get; set; }
    public int Month { get; set; }
    public List<ShiftAssignment> Assignments { get; set; } = new();
    public List<StaffMember> Staff { get; set; } = new();

    public string MonthName => new DateTime(Year, Month, 1).ToString("MMMM yyyy");
}