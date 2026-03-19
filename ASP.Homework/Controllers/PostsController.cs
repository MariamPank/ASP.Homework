using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialPosts.Data;
using SocialPosts.DTOs;
using SocialPosts.Models;

namespace SocialPosts.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly DataContext _db;

        public PostsController(DataContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult GetAllPosts()
        {
            var posts = _db.Posts
                .Include(x => x.Comments)
                .ToList();

            return Ok(posts);
        }

        [HttpGet("{id}")]
        public IActionResult GetPostById(int id)
        {
            var post = _db.Posts
                .Include(x => x.Comments)
                .FirstOrDefault(x => x.Id == id);

            if (post == null)
                return NotFound("Post not found.");

            return Ok(post);
        }

        [HttpGet("author/{author}")]
        public IActionResult GetPostsByAuthor(string author)
        {
            var posts = _db.Posts
                .Where(x => x.Author.ToLower() == author.ToLower())
                .ToList();

            if (!posts.Any())
                return NotFound("No posts found for this author.");

            return Ok(posts);
        }


        [HttpGet("search")]
        public IActionResult SearchPostsByTitle(string title)
        {
            var posts = _db.Posts
                .Where(x => x.Title.ToLower().Contains(title.ToLower()))
                .ToList();

            if (!posts.Any())
                return NotFound("No matching posts found.");

            return Ok(posts);
        }


        [HttpGet("latest")]
        public IActionResult GetLatestPosts()
        {
            var posts = _db.Posts
                .OrderByDescending(x => x.CreatedDate)
                .Take(5)
                .ToList();

            return Ok(posts);
        }


        [HttpPost]
        public IActionResult CreatePost([FromBody] CreatePostDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
                return BadRequest("Title is required.");

            if (string.IsNullOrWhiteSpace(dto.Content))
                return BadRequest("Content is required.");

            if (string.IsNullOrWhiteSpace(dto.Author))
                return BadRequest("Author is required.");

            var post = new Post
            {
                Title = dto.Title,
                Content = dto.Content,
                Author = dto.Author,
                CreatedDate = DateTime.Now
            };

            _db.Posts.Add(post);
            _db.SaveChanges();

            return Ok(post);
        }


        [HttpPost("create-many")]
        public IActionResult CreateManyPosts([FromBody] List<CreatePostDto> dtos)
        {
            if (dtos == null || !dtos.Any())
                return BadRequest("Post list is empty.");

            var posts = new List<Post>();

            foreach (var dto in dtos)
            {
                if (string.IsNullOrWhiteSpace(dto.Title) ||
                    string.IsNullOrWhiteSpace(dto.Content) ||
                    string.IsNullOrWhiteSpace(dto.Author))
                {
                    return BadRequest("Each post must have Title, Content and Author.");
                }

                posts.Add(new Post
                {
                    Title = dto.Title,
                    Content = dto.Content,
                    Author = dto.Author,
                    CreatedDate = DateTime.Now
                });
            }

            _db.Posts.AddRange(posts);
            _db.SaveChanges();

            return Ok(posts);
        }


        [HttpPost("{id}/comment")]
        public IActionResult AddCommentToPost(int id, [FromBody] CreateCommentDto dto)
        {
            var post = _db.Posts.FirstOrDefault(x => x.Id == id);

            if (post == null)
                return NotFound("Post not found.");

            if (string.IsNullOrWhiteSpace(dto.UserName))
                return BadRequest("UserName is required.");

            if (string.IsNullOrWhiteSpace(dto.Text))
                return BadRequest("Comment text is required.");

            var comment = new Comment
            {
                UserName = dto.UserName,
                Text = dto.Text,
                PostId = id,
                CreatedDate = DateTime.Now
            };

            _db.Comments.Add(comment);
            _db.SaveChanges();

            return Ok(comment);
        }


        [HttpPost("{id}/like")]
        public IActionResult LikePost(int id)
        {
            var post = _db.Posts.FirstOrDefault(x => x.Id == id);

            if (post == null)
                return NotFound("Post not found.");

            post.LikesCount += 1;
            _db.SaveChanges();

            return Ok(new
            {
                Message = "Post liked successfully.",
                LikesCount = post.LikesCount
            });
        }


        [HttpPut("{id}")]
        public IActionResult UpdatePost(int id, [FromBody] UpdatePostDto dto)
        {
            var post = _db.Posts.FirstOrDefault(x => x.Id == id);

            if (post == null)
                return NotFound("Post not found.");

            if (string.IsNullOrWhiteSpace(dto.Title))
                return BadRequest("Title is required.");

            if (string.IsNullOrWhiteSpace(dto.Content))
                return BadRequest("Content is required.");

            if (string.IsNullOrWhiteSpace(dto.Author))
                return BadRequest("Author is required.");

            post.Title = dto.Title;
            post.Content = dto.Content;
            post.Author = dto.Author;

            _db.SaveChanges();

            return Ok(post);
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePost(int id)
        {
            var post = _db.Posts
                .Include(x => x.Comments)
                .FirstOrDefault(x => x.Id == id);

            if (post == null)
                return NotFound("Post not found.");

            _db.Comments.RemoveRange(post.Comments);
            _db.Posts.Remove(post);
            _db.SaveChanges();

            return Ok("Post deleted successfully.");
        }
    }
}
