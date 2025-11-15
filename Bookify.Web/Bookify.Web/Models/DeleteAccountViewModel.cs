using System.ComponentModel.DataAnnotations;

namespace Bookify.Web.Models
{
    public class DeleteAccountViewModel
    {
        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
    }
}