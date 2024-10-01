using System.ComponentModel.DataAnnotations;

namespace app.Models
{
    public class User
    {
        [Key]
        public long Id { get; set; }

        [Required(ErrorMessage = "Tài khoản không được để trống.")]
        [EmailAddress(ErrorMessage = "Tài khoản phải là định dạng email hợp lệ.")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có tối thiểu 6 ký tự.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{6,}$",
        ErrorMessage = "Mật khẩu phải có ít nhất 1 chữ cái thường, 1 chữ cái in hoa, 1 số và 1 ký tự đặc biệt.")]
        public string? Password { get; set; }

        public string? Name { get; set; }

        public string? Address { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string? ModifiedBy { get; set; }

        public bool Status { get; set; }

    }
}
