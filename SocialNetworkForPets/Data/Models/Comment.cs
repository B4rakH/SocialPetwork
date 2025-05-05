using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestApp.Data.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }

        public string CommentText { get; set; }

        //Foreign Key

        [ForeignKey("Person")]
        public int PersonId { get; set; }

        public Person Person { get; set; }

        [ForeignKey("Post")]
        public int PostId { get; set; }

        public Post Post { get; set; }
    }
}
