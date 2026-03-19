using SocialPosts.Common.Entity;

namespace SocialPosts.Models
{
    public class Post : Entity
    {
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public string Author { get; set; } = "";
        public int LikesCount { get; set; } = 0;

        public List<Comment> Comments { get; set; } = new();
    }
}
