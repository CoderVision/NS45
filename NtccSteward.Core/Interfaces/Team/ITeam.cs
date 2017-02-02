namespace NtccSteward.Core.Interfaces.Team
{
    public interface ITeam
    {
        int ChurchId { get; set; }
        int Id { get; set; }
        string Name { get; set; }
        int TeamPositionEnumTypeId { get; set; }
        int TeamTypeEnumId { get; set; }
    }
}