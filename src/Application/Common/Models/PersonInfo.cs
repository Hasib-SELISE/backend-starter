namespace Application.Common.Models;

public class PersonInfo
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string DisplayName { get; set; }
    public string Email { get; set; }
    public string PersonalEmail { get; set; }
    public string PhoneNumber { get; set; }
    public string Salutation { get; set; }
    public string[] Roles { get; set; }
    public string ProfileImageId { get; set; }
    public string OrganizationId { get; set; }
    public string Organization { get; set; }
    public string Sex { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string Nationality { get; set; }
}