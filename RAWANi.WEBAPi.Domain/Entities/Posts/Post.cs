using RAWANi.WEBAPi.Domain.Entities.Posts.ValueObjects;
using RAWANi.WEBAPi.Domain.Models;
using System;

namespace RAWANi.WEBAPi.Domain.Entities.Posts
{
    /// <summary>
    /// Represents a Post entity in the system.
    /// </summary>
    public class Post
    {
        /// <summary>
        /// Gets the unique identifier of the post.
        /// </summary>
        public GuidID PostID { get; private set; }

        /// <summary>
        /// Gets the unique identifier of the user profile associated with the post.
        /// </summary>
        public GuidID UserProfileID { get; private set; }

        /// <summary>
        /// Gets the title of the post.
        /// </summary>
        public Header PostTitle { get; private set; }

        /// <summary>
        /// Gets the content of the post.
        /// </summary>
        public Bodies PostContent { get; private set; }

        /// <summary>
        /// Gets the image link associated with the post.
        /// </summary>
        public string? ImageLink { get; private set; }

        /// <summary>
        /// Gets the date and time when the post was created.
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// Gets the date and time when the post was last updated.
        /// </summary>
        public DateTime UpdatedAt { get; private set; }

        // Private constructor to enforce the use of the factory method
        private Post() { }

        /// <summary>
        /// Creates a new instance of the <see cref="Post"/> class.
        /// </summary>
        /// <param name="postId">The unique identifier of the post.</param>
        /// <param name="userProfileId">The unique identifier of the user profile.</param>
        /// <param name="postTitle">The title of the post.</param>
        /// <param name="postContent">The content of the post.</param>
        /// <param name="imageLink">The image link associated with the post.</param>
        /// <returns>An <see cref="OperationResult{Post}"/> indicating success or failure.</returns>
        public static OperationResult<Post> Create(
            Guid postId, Guid userProfileId, string postTitle, 
            string postContent, string? imageLink = null)
        {
            // Validate postId
            var postIDResult = GuidID.Create(postId);
            if (!postIDResult.IsSuccess)
                return OperationResult<Post>.Failure(postIDResult.Errors);

            // Validate userProfileId
            var userProfileIDResult = GuidID.Create(userProfileId);
            if (!userProfileIDResult.IsSuccess)
                return OperationResult<Post>.Failure(userProfileIDResult.Errors);

            // Validate postTitle
            var postTitleResult = Header.Create(postTitle);
            if (!postTitleResult.IsSuccess)
                return OperationResult<Post>.Failure(postTitleResult.Errors);

            // Validate postContent
            var postContentResult = Bodies.Create(postContent);
            if (!postContentResult.IsSuccess)
                return OperationResult<Post>.Failure(postContentResult.Errors);

            
            // Create the post
            var post = new Post
            {
                PostID = postIDResult.Data,
                UserProfileID = userProfileIDResult.Data,
                PostTitle = postTitleResult.Data,
                PostContent = postContentResult.Data,
                ImageLink = imageLink,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            return OperationResult<Post>.Success(post);
        }


        public static OperationResult<Post> FetchPostFromDatabase(
            Guid postId, Guid userProfileId, string postTitle,
            string postContent,DateTime createdAt, DateTime updatedAt, string? imageLink = null)
        {
            // Validate postId
            var postIDResult = GuidID.Create(postId);
            if (!postIDResult.IsSuccess)
                return OperationResult<Post>.Failure(postIDResult.Errors);

            // Validate userProfileId
            var userProfileIDResult = GuidID.Create(userProfileId);
            if (!userProfileIDResult.IsSuccess)
                return OperationResult<Post>.Failure(userProfileIDResult.Errors);

            // Validate postTitle
            var postTitleResult = Header.Create(postTitle);
            if (!postTitleResult.IsSuccess)
                return OperationResult<Post>.Failure(postTitleResult.Errors);

            // Validate postContent
            var postContentResult = Bodies.Create(postContent);
            if (!postContentResult.IsSuccess)
                return OperationResult<Post>.Failure(postContentResult.Errors);


            // Create the post
            var post = new Post
            {
                PostID = postIDResult.Data,
                UserProfileID = userProfileIDResult.Data,
                PostTitle = postTitleResult.Data,
                PostContent = postContentResult.Data,
                ImageLink = imageLink,
                CreatedAt = createdAt,
                UpdatedAt = updatedAt
            };

            return OperationResult<Post>.Success(post);
        }
        /// <summary>
        /// Updates the post with new title, content, and image link.
        /// </summary>
        /// <param name="postTitle">The new title of the post.</param>
        /// <param name="postContent">The new content of the post.</param>
        /// <param name="imageLink">The new image link associated with the post.</param>
        /// <returns>An <see cref="OperationResult{Post}"/> indicating success or failure.</returns>
        public OperationResult<Post> Update(string postTitle, string postContent, string? imageLink = null)
        {
            // Validate postTitle
            var postTitleResult = Header.Create(postTitle);
            if (!postTitleResult.IsSuccess)
                return OperationResult<Post>.Failure(postTitleResult.Errors);

            // Validate postContent
            var postContentResult = Bodies.Create(postContent);
            if (!postContentResult.IsSuccess)
                return OperationResult<Post>.Failure(postContentResult.Errors);

            // Update the post
            PostTitle = postTitleResult.Data;
            PostContent = postContentResult.Data;
            ImageLink = imageLink;
            UpdatedAt = DateTime.UtcNow;

            return OperationResult<Post>.Success(this);
        }
    }
}