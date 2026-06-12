namespace JobPortal.Shared.Interfaces;

internal interface IUserProfile
{
    void UpdateProfile();
    void Display();
    string GetProfileSummary();
}
