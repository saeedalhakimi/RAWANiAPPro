using RAWANi.WEBAPi.Domain.Entities.Posts.ValueObjects;
using RAWANi.WEBAPi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Domain.Entities.Posts
{
    public class PostComment
    {
        public GuidID CommentID { get; private set; }
        public GuidID PostID { get; private set; }
        public GuidID UserProfileID { get; private set; }
        public Bodies CommentContent { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set;}

        private PostComment() { }   

        public static OperationResult<PostComment> Create(
            Guid commendId, Guid postId, Guid userProfileId,
            string commentContent)
        {
            var commentID = GuidID.Create(commendId);
            if (!commentID.IsSuccess) return OperationResult<PostComment>
                    .Failure(commentID.Errors);

            var postID = GuidID.Create(postId);
            if (!postID.IsSuccess) return OperationResult<PostComment>
                    .Failure(postID.Errors);

            var userProfileID = GuidID.Create(userProfileId);
            if (!userProfileID.IsSuccess) return OperationResult<PostComment>
                    .Failure(userProfileID.Errors);

            var commentContents = Bodies.Create(commentContent);
            if (!commentContents.IsSuccess) return OperationResult<PostComment>
                    .Failure(commentContents.Errors);

            var createdAt = DateTime.UtcNow;
            var updatedAt = createdAt;

            var postComment = new PostComment
            {
                CommentID = commentID.Data,
                PostID = postID.Data,
                UserProfileID = userProfileID.Data,
                CommentContent = commentContents.Data,
                CreatedAt = createdAt,
                UpdatedAt = updatedAt,
            };

            return OperationResult<PostComment>.Success(postComment);
        }

        public static OperationResult<PostComment> FetchFromDatabase(
            Guid commendId, Guid postId, Guid userProfileId,
            string commentContent, DateTime createdAt, 
            DateTime updatedAt)
        {
            var commentID = GuidID.Create(commendId);
            if (!commentID.IsSuccess) return OperationResult<PostComment>
                    .Failure(commentID.Errors);

            var postID = GuidID.Create(postId);
            if (!postID.IsSuccess) return OperationResult<PostComment>
                    .Failure(postID.Errors);

            var userProfileID = GuidID.Create(userProfileId);
            if (!userProfileID.IsSuccess) return OperationResult<PostComment>
                    .Failure(userProfileID.Errors);

            var commentContents = Bodies.Create(commentContent);
            if (!commentContents.IsSuccess) return OperationResult<PostComment>
                    .Failure(commentContents.Errors);

            var postComment = new PostComment
            {
                CommentID = commentID.Data,
                PostID = postID.Data,
                UserProfileID = userProfileID.Data,
                CommentContent = commentContents.Data,
                CreatedAt = createdAt,
                UpdatedAt = updatedAt,
            };

            return OperationResult<PostComment>.Success(postComment);
        }

        public OperationResult<bool> UpdateComment(string commentContent)
        {
            var commentContents = Bodies.Create(commentContent);
            if (!commentContents.IsSuccess) return OperationResult<bool>
                    .Failure(commentContents.Errors);
            CommentContent = commentContents.Data;
            UpdatedAt = DateTime.UtcNow;
            return OperationResult<bool>.Success(true);
        }
    }
}
