using RAWANi.WEBAPi.Domain.Entities.Posts;
using RAWANi.WEBAPi.Domain.Entities.Posts.ValueObjects;
using RAWANi.WEBAPi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Domain.UnitTests.IntitiesTests.PostTest
{
    public class PostTests
    {
        [Fact]
        public void Create_ValidPost_ShouldReturnSuccess()
        {
            // Arrange
            var postId = PostGuid.CreateNew().Value;
            var userProfileId = UserProfileGuid.CreateNew().Value;
            var postTitle = "Valid Post Title";
            var postContent = "This is a valid post content.";
            var imageLink = "https://example.com/image.jpg";

            // Act
            var result = Post.Create(postId, userProfileId, postTitle, postContent, imageLink);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(postId, result.Data.PostID.Value);
            Assert.Equal(userProfileId, result.Data.UserProfileID.Value);
            Assert.Equal(postTitle, result.Data.PostTitle.Value);
            Assert.Equal(postContent, result.Data.PostContent.Value);
            Assert.Equal(imageLink, result.Data.ImageLink);
        }

        [Fact]
        public void Create_InvalidPostId_ShouldReturnFailure()
        {
            // Arrange
            var postId = Guid.Empty; // Invalid ID
            var userProfileId = UserProfileGuid.CreateNew().Value;
            var postTitle = "Valid Post Title";
            var postContent = "Valid content";
            var imageLink = string.Empty;

            // Act
            var result = Post.Create(postId, userProfileId, postTitle, postContent, imageLink);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(result.IsError);
            Assert.Equal(ErrorCode.InvalidInput, result.Errors.First().Code);
            Assert.Contains("Invalid Input.", result.Errors.First().Message);
            Assert.Contains("Guid cannot be empty.", result.Errors.First().Details);
        }

        [Fact]
        public void Create_InvalidUserProfileId_ShouldReturnFailure()
        {
            // Arrange
            var postId = PostGuid.CreateNew();
            var userProfileId = Guid.Empty; // Invalid ID
            var postTitle = "Valid Post Title";
            var postContent = "Valid content";
            var imageLink = string.Empty;

            // Act
            var result = Post.Create(postId, userProfileId, postTitle, postContent, imageLink);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(result.IsError);
            Assert.Equal(ErrorCode.InvalidInput, result.Errors.First().Code);
            Assert.Contains("Invalid Input.", result.Errors.First().Message);
            Assert.Contains("Guid cannot be empty.", result.Errors.First().Details);
        }

        [Fact]
        public void Create_EmptyPostTitle_ShouldReturnFailure()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var userProfileId = Guid.NewGuid();
            var postTitle = ""; // Empty title
            var postContent = "Valid content";
            var imageLink = string.Empty;

            // Act
            var result = Post.Create(postId, userProfileId, postTitle, postContent, imageLink);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains(result.Errors, error => error.Message.Contains("Invalid", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public void Create_EmptyPostContent_ShouldReturnFailure()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var userProfileId = Guid.NewGuid();
            var postTitle = "Valid Post Title";
            var postContent = ""; // Empty content
            var imageLink = string.Empty;

            // Act
            var result = Post.Create(postId, userProfileId, postTitle, postContent, imageLink);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains(result.Errors, error => error.Message.Contains("Invalid", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public void Update_ValidData_ShouldReturnSuccess()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var userProfileId = Guid.NewGuid();
            var postTitle = "Original Title";
            var postContent = "Original Content";
            var imageLink = "https://example.com/original.jpg";

            var createResult = Post.Create(postId, userProfileId, postTitle, postContent, imageLink);
            var post = createResult.Data;

            // New data for update
            var newTitle = "Updated Title";
            var newContent = "Updated Content";
            var newImageLink = "https://example.com/updated.jpg";

            // Act
            var updateResult = post.Update(newTitle, newContent, newImageLink);

            // Assert
            Assert.True(updateResult.IsSuccess);
            Assert.Equal(newTitle, post.PostTitle.Value);
            Assert.Equal(newContent, post.PostContent.Value);
            Assert.Equal(newImageLink, post.ImageLink);
        }

        [Fact]
        public void Update_InvalidTitle_ShouldReturnFailure()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var userProfileId = Guid.NewGuid();
            var postTitle = "Original Title";
            var postContent = "Original Content";
            var imageLink = "https://example.com/original.jpg";

            var createResult = Post.Create(postId, userProfileId, postTitle, postContent, imageLink);
            var post = createResult.Data;

            // Act
            var updateResult = post.Update("", "Updated Content", "https://example.com/updated.jpg");

            // Assert
            Assert.False(updateResult.IsSuccess);
            Assert.Contains(updateResult.Errors, error => error.Message.Contains("Invalid", StringComparison.OrdinalIgnoreCase));
        }
        [Fact]
        public void Update_InvalidContent_ShouldReturnFailure()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var userProfileId = Guid.NewGuid();
            var postTitle = "Original Title";
            var postContent = "Original Content";
            var imageLink = "https://example.com/original.jpg";

            var createResult = Post.Create(postId, userProfileId, postTitle, postContent, imageLink);
            var post = createResult.Data;

            // Act
            var updateResult = post.Update("Updated Title", "", "https://example.com/updated.jpg");

            // Assert
            Assert.False(updateResult.IsSuccess);
            Assert.Contains(updateResult.Errors, error => error.Message.Contains("Invalid", StringComparison.OrdinalIgnoreCase));
        }
    }
}
