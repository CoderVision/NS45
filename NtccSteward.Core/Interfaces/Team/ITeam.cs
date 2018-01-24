namespace NtccSteward.Core.Interfaces.Team
{
    public interface ITeam
    {
        int Id { get; set; }

        string Name { get; set; }

        string Desc { get; set; }

        int TeamTypeEnumId { get; set; }

        int TeamPositionEnumTypeId { get; set; }

        int ChurchId { get; set; }

        string Comment { get; set; }
    }
}