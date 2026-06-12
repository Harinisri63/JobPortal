namespace JobPortal.Shared.Interfaces;

internal interface INotifiable
{
    void SendNotification(string message);
}
