namespace NtccSteward.Core.Interfaces.Team
{
    public interface ITeammate
    {
        int Id { get; set; }
        int ChurchId { get; set; }
        int MemberId { get; set; }
        int TeamId { get; set; }
        int TeamPositionEnumId { get; set; }
        int TeamStatusTypeEnumId { get; set; }
    }
}