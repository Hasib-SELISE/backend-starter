namespace Application.Common.Models;

public class UserCreationModel
{
    public string ItemId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string DisplayName { get; set; }
    public string Email { get; set; }
    public string[] Roles { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public string PhoneNumber { get; set; }
    public string CountryCode { get; set; }
    public string Language { get; set; }
    public string Salutation { get; set; }

    public bool PersonaEnabled { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public string MailPurpose => "VerifyEmail";
    public string ProfileImageId { get; set; }
    public string CopyEmailTo { get; set; }
    public string DefaultPassword { get; set; }
    public string HostDomain { get; set; }
    public string RegisteredBy { get; set; } = "2";
    public PersonInfo PersonInfo { get; set; }
    
    public UserCreationModel()
    {
        PersonInfo = new PersonInfo();
    }
}