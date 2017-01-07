namespace NtccSteward.Core.Models.Message
{
    public interface IRecipientGroup
    {
        int ChurchId { get; set; }
        string Description { get; set; }
        int ID { get; set; }
        string Name { get; set; }
    }
}