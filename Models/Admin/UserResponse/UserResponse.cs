using System.ComponentModel.DataAnnotations;

namespace CMS_Brian.Models.Admin.UserResponse
{
    public class ClsAdministratorResponse
    {
        public string UserId { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public bool IsApproved { get; set; }
        public string Status { get; set; }
        public string UserImage { get; set; }
        public string AvailableDays { get; set; }
        public string AssignClasses { get; set; }
        public string EmailId { get; set; }
    }
    public class ClsInstructorResponse
    {
        public string UserId { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public bool IsApproved { get; set; }
        public string Status { get; set; }
        public string UserImage { get; set; }
        public string AvailableDays { get; set; }
    }

    public class ClsAdministrator
    {
        public string UserId { get; set; }
        
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string PhoneNumber { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"^((?=.*[a-z])(?=.*[A-Z])(?=.*\d)).+$", ErrorMessage = "Password must contains 1 Capital letter, 1 Special characters and length should be greater then 6.")]
        public string Password { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }        
        public string Address { get; set; }
        public bool IsApproved { get; set; }
        public string Status { get; set; }
        public string UserImage { get; set; }
        public string AvailableDaysIds { get; set; }

    }
    public class clsAvailableDays
    {
        public string UserId { get; set; }
        public string AvailableDaysIds { get; set; }
    }

    public class ClsInstructor
    {
        public string UserId { get; set; }
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string PhoneNumber { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"^((?=.*[a-z])(?=.*[A-Z])(?=.*\d)).+$", ErrorMessage = "Password must contains 1 Capital letter, 1 Special characters and length should be greater then 6.")]
        public string Password { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string Address { get; set; }
        public bool IsApproved { get; set; }
        public string Status { get; set; }
        public string UserImage { get; set; }
        public string AvailableDaysIds { get; set; }

    }

    public class ClsClient
    {
        public int? ClientId { get; set; }
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string PhoneNumber { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string Address { get; set; }
        public string DateOfBirth { get; set; }
        public string Gender { get; set; }
        public int? JudgeId { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string SignForm { get; set; }

    }

    public class clsApproveUser
    {
        public string UserId { get; set; }
        public bool Approved { get; set; }
        public bool Locked { get; set; }
        public string NewPassword { get; set; }
    }
    public class ChangePasswordModel
    {
        public string UserId { get; set; }
        [Required(ErrorMessage = "Please Enter New Password")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "Please Enter Confirm Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

    }
    public class ClientResponseList
    {
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public string DateofBirth { get; set; }
        public string JudgeName { get; set; }
        public string Status { get; set; }
        public string Address { get; set; }
        public int Classes { get; set; }
        public string SignedForm { get; set; }
        public string Gender { get; set; }
        public decimal TotalPayment { get; set; }
    }

    public class ClientDetail 
    {
        public int ClientId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DateofBirth { get; set; }
        public string JudgeName { get; set; }
        public string Status { get; set; }
        public string Address { get; set; }
        public string SignedForm { get; set; }
        public string Gender { get; set; }
        public string PhoneNumber { get; set; }
    }
}