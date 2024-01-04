namespace Application.Common.Models;

public class UserUpdateModel
{
    public string DisplayName { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string ProfileImageId { get; set; }
    public bool? Active { get; set; }
    public string[] Roles { get; set; }
    public string UserId { get; set; }
    public bool? TwoFactorEnabled { get; set; }
    public string CountryCode { get; set; }
    public string[] Tags { get; set; }
    public string Language { get; set; }
    public string Salutation { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Sex { get; set; }
    public bool? PersonaEnabled { get; set; }
    public bool DontDeletePreviousRoles { get; set; }
    public bool InstantChangeLoggedInUserRoles { get; set; }
    public string EmailChangeTemplateName { get; set; }
    public PersonInfo PersonInfo { get; set; }
}