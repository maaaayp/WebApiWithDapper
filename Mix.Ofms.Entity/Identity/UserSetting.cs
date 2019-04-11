using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mix.Ofms.Entity.Identity
{
    [Table("UserSetting")]
    public class UserSetting
    {
        [Key]
        public int Id { get; set; }

        public bool OpenAunt { get; set; }

        public long UserId { get; set; }


    }
}
