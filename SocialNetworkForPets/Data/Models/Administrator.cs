using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestApp.Data.Models
{
    public class Administrator
    {
        //Foreign Key
        [ForeignKey("Person")]
        public int AdminId { get; set; }

        public Person Admin { get; set; }
    }
}
