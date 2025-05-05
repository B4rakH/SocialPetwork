using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestApp.Data.Models
{
    public class Friendship
    {
        //Foreign Key

        [ForeignKey("Person")]
        public int Person1Id { get; set; }

        public Person Person1 { get; set; }

        [ForeignKey("Person")]
        public int Person2Id { get; set; }

        public Person Person2 { get; set; }
    }
}
