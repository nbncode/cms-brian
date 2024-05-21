using System;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace CMS_Brian.Models
{
    public class ResponseUserProfile
    {
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string PhoneNumber { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Emailaddress { get; set; }
        [Required]
        public string Address { get; set; }
        public string UserImage { get; set; }
        public string Role { get; set; }
        public string DateOfBirth { get; set; }
        public string Gender { get; set; }
        public int CompleteProfile { get; set; }

    }
    public class ResponseUserImageModel
    {
        public string UserId { get; set; }
        public string UserImage64 { get; set; }
    }
    public class ResponseImage
    {
        public string Image { get; set; }
    }
    public class UserProfiles
    {
        #region Variables
        CMSBrianEntities db = new CMSBrianEntities();

        #endregion

        #region Get User Profile
        public ResponseUserProfile GetUserProfile(string UserId, string Role)
        {
            int completefield = 0;
            int totalfield = 0;
            var userdetails = new ResponseUserProfile();
            if (!string.IsNullOrEmpty(UserId))
            {
                var data = (from a in db.AspNetUsers
                            join u in db.UserDetails on a.Id equals u.UserId
                            where a.Id == UserId
                            select new
                            {
                                Email = a.UserName,
                                Phone = u.PhoneNumber,
                                FirstName = u.FirstName,
                                LastName = u.LastName,
                                Address = u.Address,
                                UserImage = u.UserImage,
                                Gender=u.Gender,
                                DateOfBirth=u.DateofBirth,
                            }).FirstOrDefault();

                if (data != null)
                {
                    userdetails.FirstName = data.FirstName;
                    userdetails.LastName = data.LastName;
                    userdetails.PhoneNumber = data.Phone;
                    userdetails.Emailaddress = data.Email;
                    userdetails.Address = data.Address;
                    userdetails.DateOfBirth = data.DateOfBirth;
                    userdetails.Gender = data.Gender;
                    userdetails.Role = Role;
                    userdetails.UserImage = data.UserImage;
                }
                // calculate profile complete
                totalfield = totalfield + 1;
                if (!string.IsNullOrEmpty(userdetails.FirstName)) { completefield = completefield + 1; }
                totalfield = totalfield + 1;
                if (!string.IsNullOrEmpty(userdetails.LastName)) { completefield = completefield + 1; }
                totalfield = totalfield + 1;
                if (!string.IsNullOrEmpty(userdetails.PhoneNumber)) { completefield = completefield + 1; }
                totalfield = totalfield + 1;
                if (!string.IsNullOrEmpty(userdetails.Emailaddress)) { completefield = completefield + 1; }
                totalfield = totalfield + 1;
                if (!string.IsNullOrEmpty(userdetails.Address)) { completefield = completefield + 1; }
                totalfield = totalfield + 1;
                if (!string.IsNullOrEmpty(userdetails.UserImage)) { completefield = completefield + 1; }
                totalfield = totalfield + 1;
                if (!string.IsNullOrEmpty(userdetails.DateOfBirth)) { completefield = completefield + 1; }
                totalfield = totalfield + 1;
                if (!string.IsNullOrEmpty(userdetails.Gender)) { completefield = completefield + 1; }

                userdetails.CompleteProfile = completefield * 100 / totalfield;
            }
            return userdetails;
        }
        #endregion

        #region Update User Profile
        public bool UpdateUserProfile(ResponseUserProfile model, string UserId)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(UserId) && model != null)
            {
                var detail = db.UserDetails.Where(u => u.UserId == UserId).FirstOrDefault();
                var user = db.AspNetUsers.Where(a => a.Id == UserId).FirstOrDefault();
                if (detail != null && user != null)
                {
                    detail.FirstName = model.FirstName;
                    detail.LastName = model.LastName;
                    user.Email = model.Emailaddress;
                    detail.Address = model.Address;
                    detail.UserImage = model.UserImage;
                    detail.DateofBirth = model.DateOfBirth;
                    detail.PhoneNumber = model.PhoneNumber;
                    detail.Gender = model.Gender;
                    db.SaveChanges();
                    result = true;
                }
            }
            return result;
        }
        #endregion

        #region Save User Image and Update
        public bool SaveUserImage(ResponseUserImageModel model, out string Imagepath)
        {
            bool result = false;
            Image image = new Bitmap(800, 600);
            ImageFormat format = ImageFormat.Png;
            image = new Bitmap(new MemoryStream(Convert.FromBase64String(model.UserImage64)));
            using (Image imageToExport = image)

            {
                String path = HttpContext.Current.Server.MapPath("/Content/attachment/"); //Path
                                                                                          //Check if directory exist
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path); //Create directory if it doesn't exist
                }

                string imageName = Guid.NewGuid() + "_" + Path.GetFileName("_Base64" + ".Png");
                //set the image path
                string imgPath = Path.Combine(path, imageName);
                imageToExport.Save(imgPath, format);

                Imagepath = "/Content/attachment/" + imageName;
                result = UpdateUserImage(model, Imagepath);

            }
            return result;
        }

        public bool UpdateUserImage(ResponseUserImageModel model, string Imagepath)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(model.UserId) && model != null)
            {
                var detail = db.UserDetails.Where(u => u.UserId == model.UserId).FirstOrDefault();
                if (detail != null && Imagepath != null)
                {
                    detail.UserImage = Imagepath;
                    db.SaveChanges();
                    result = true;
                }
            }
            return result;
        }

        #endregion

    }


}