namespace MIC.Models.Enums
{
    public enum DeviceState
    {
        Unknown = 0,
        Connected = 1,
        Disconnected = 2,
        Error = 3
    }

    public enum RoleType
    {
        Operator,
        Engineer,
        Admin
    }
}