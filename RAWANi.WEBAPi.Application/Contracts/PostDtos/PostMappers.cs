using RAWANi.WEBAPi.Application.Contracts.PostDtos.Responses;
using RAWANi.WEBAPi.Domain.Entities.Posts;

namespace RAWANi.WEBAPi.Application.Contracts.PostDtos
{
    public static class PostMappers
    {
        public static PostResponseDto ToPostResponseDto(Post post)
        {
            return new PostResponseDto
            {
                PostID = post.PostID.Value,
                UserProfileID = post.UserProfileID.Value,
                PostTilte = post.PostTitle.Value,
                PostContent = post.PostContent.Value,
                PostImageLink = post.ImageLink,
                CreateAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
            };
        }

        public static PostCommentResponseDto ToPostCommentResponseDto(PostComment comment)
        {
            return new PostCommentResponseDto
            {
                CommentID = comment.CommentID.Value,
                PostID = comment.PostID.Value,
                UserProfileID = comment.UserProfileID.Value,
                CommentContent = comment.CommentContent.Value,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt,
            };
        }
    }
}
