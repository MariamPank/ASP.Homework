using SocialPosts.Common.Entity;

namespace SocialPosts.Models
{
    public class Comment : Entity
    {
        public string UserName { get; set; } = "";
        public string Text { get; set; } = "";

        public int PostId { get; set; }
        public Post Post { get; set; }
    }
}
