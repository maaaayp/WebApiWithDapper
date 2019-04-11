using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mix.Ofms.Entity.Identity
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int Id { get; set; }


        public string NickName { get; set; }
        public string Pwd { get; set; }
        public int Role { get; set; }

        public string Phone { get; set; }
    }
}
