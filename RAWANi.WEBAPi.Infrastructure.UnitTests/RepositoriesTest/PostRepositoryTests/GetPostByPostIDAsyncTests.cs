using Microsoft.Extensions.Configuration;
using Moq;
using RAWANi.WEBAPi.Application.Abstractions;
using RAWANi.WEBAPi.Application.Repository;
using RAWANi.WEBAPi.Application.Services;
using RAWANi.WEBAPi.Domain.Entities.Posts;
using RAWANi.WEBAPi.Domain.Models;
using RAWANi.WEBAPi.Infrastructure.Data.DataWrapperFactory;
using RAWANi.WEBAPi.Infrastructure.Repository.Posts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Infrastructure.UnitTests.RepositoriesTest.PostRepositoryTests
{
    public class GetPostByPostIDAsyncTests
    {
        private readonly Mock<IAppLogger<PostRepository>> _mockLogger;
        private readonly Mock<ILoggMessagingService> _mockMessagingService;
        private readonly Mock<IErrorHandler> _mockErrorHandler;
        private readonly Mock<IDatabaseConnectionFactory> _mockConnectionFactory;
        private readonly Mock<IDatabaseConnection> _mockDbConnection;
        private readonly Mock<Data.DataWrapperFactory.IDbCommand> _mockDbCommand;
        private readonly Mock<Data.DataWrapperFactory.IDataReader> _mockDbReader;
        private readonly string _connectionString = "Server=localhost;Database=RAWANiProDb;User Id=sa;Password=sa123456;Encrypt=False;TrustServerCertificate=True;Connection Timeout=30;";

        private readonly IPostRepository _postRepository;

        public GetPostByPostIDAsyncTests()
        {
            _mockLogger = new Mock<IAppLogger<PostRepository>>();
            _mockMessagingService = new Mock<ILoggMessagingService>();
            _mockErrorHandler = new Mock<IErrorHandler>();
            _mockConnectionFactory = new Mock<IDatabaseConnectionFactory>();
            _mockDbConnection = new Mock<IDatabaseConnection>();
            _mockDbCommand = new Mock<Data.DataWrapperFactory.IDbCommand>();
            _mockDbReader = new Mock<Data.DataWrapperFactory.IDataReader>();

            // Mock Database Connection Factory
            _mockConnectionFactory.Setup(cf => cf.CreateConnectionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(_mockDbConnection.Object);

            // Mock Database Connection to return a command
            _mockDbConnection.Setup(db => db.CreateCommand()).Returns(_mockDbCommand.Object);
            _mockDbConnection.Setup(db => db.OpenAsync(It.IsAny<CancellationToken>()))
                             .Returns(Task.CompletedTask);

            // Mock DB Command Behavior
            _mockDbCommand.SetupSet(c => c.CommandText = It.IsAny<string>());
            _mockDbCommand.SetupSet(c => c.CommandType = CommandType.StoredProcedure);
            _mockDbCommand.Setup(c => c.ExecuteReaderAsync(It.IsAny<CancellationToken>()))
                          .ReturnsAsync(_mockDbReader.Object);

            // Mock Configuration
            var mockConfiguration = new Mock<IConfiguration>();

            // Initialize repository
            _postRepository = new PostRepository(
                mockConfiguration.Object,
                _mockLogger.Object,
                _mockMessagingService.Object,
                _mockErrorHandler.Object,
                _mockConnectionFactory.Object,
                _connectionString
            );

            // Setup default reader behavior
            _mockDbReader.SetupSequence(r => r.ReadAsync(It.IsAny<CancellationToken>()))
                         .ReturnsAsync(true)
                         .ReturnsAsync(false);
        }

        [Fact]
        public async Task GetPostByPostIDAsync_ValidPost_ReturnsPost()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var createdAt = DateTime.UtcNow.AddDays(-1);
            var updatedAt = DateTime.UtcNow;

            // Mock reader column ordinals
            _mockDbReader.Setup(r => r.GetOrdinal("PostID")).Returns(0);
            _mockDbReader.Setup(r => r.GetOrdinal("UserProfileID")).Returns(1);
            _mockDbReader.Setup(r => r.GetOrdinal("PostTitle")).Returns(2);
            _mockDbReader.Setup(r => r.GetOrdinal("PostContent")).Returns(3);
            _mockDbReader.Setup(r => r.GetOrdinal("CreatedAt")).Returns(4);
            _mockDbReader.Setup(r => r.GetOrdinal("UpdatedAt")).Returns(5);
            _mockDbReader.Setup(r => r.GetOrdinal("PostImage")).Returns(6);

            // Mock reader values
            _mockDbReader.Setup(r => r.GetGuid(0)).Returns(postId);
            _mockDbReader.Setup(r => r.GetGuid(1)).Returns(userId);
            _mockDbReader.Setup(r => r.GetString(2)).Returns("Test Title");
            _mockDbReader.Setup(r => r.GetString(3)).Returns("Test Content");
            _mockDbReader.Setup(r => r.GetDateTime(4)).Returns(createdAt);
            _mockDbReader.Setup(r => r.GetDateTime(5)).Returns(updatedAt);
            _mockDbReader.Setup(r => r.IsDBNull(6)).Returns(false);
            _mockDbReader.Setup(r => r.GetString(6)).Returns("image.jpg");

            // Act
            var result = await _postRepository.GetPostByPostIDAsync(postId, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(postId, result.Data!.PostID.Value);
            Assert.Equal("Test Title", result.Data.PostTitle);
            _mockLogger.Verify(l => l.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task GetPostByPostIDAsync_PostNotFound_ReturnsFailure()
        {
            // Arrange
            var postId = Guid.NewGuid();
            _mockDbReader.SetupSequence(r => r.ReadAsync(It.IsAny<CancellationToken>()))
                         .ReturnsAsync(false);

            // Act
            var result = await _postRepository.GetPostByPostIDAsync(postId, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NotFound, result.Errors.First().Code);
            Assert.Equal("Post not found", result.Errors.First().Message);
            Assert.Equal($"Post with ID {postId} was not found in the database.", result.Errors.First().Details);
        }

        [Fact]
        public async Task GetPostByPostIDAsync_CancellationHandled_ReturnsFailure()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var cancellationToken = new CancellationToken(true);

            _mockErrorHandler.Setup(e => e.HandleCancelationToken<Post>(It.IsAny<OperationCanceledException>()))
                             .Returns(OperationResult<Post>.Failure(
                                 new Error(ErrorCode.OperationCancelled, "Operation canceled")));

            // Act
            var result = await _postRepository.GetPostByPostIDAsync(postId, cancellationToken);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.OperationCancelled, result.Errors.First().Code);
            _mockLogger.Verify(l => l.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
        }

        [Fact]
        public async Task GetPostByPostIDAsync_DatabaseError_ReturnsFailure()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var exception = new Exception("Database connection failed");

            _mockDbCommand.Setup(c => c.ExecuteReaderAsync(It.IsAny<CancellationToken>()))
                          .ThrowsAsync(exception);

            _mockErrorHandler.Setup(e => e.HandleException<Post>(exception))
                             .Returns(OperationResult<Post>.Failure(
                                 new Error(ErrorCode.UnknownError, "Database error")));

            // Act
            var result = await _postRepository.GetPostByPostIDAsync(postId, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.UnknownError, result.Errors.First().Code);
            _mockLogger.Verify(l => l.LogError(It.IsAny<string>(), It.IsAny<Exception>()), Times.Once);
        }

        [Fact]
        public async Task GetPostByPostIDAsync_NullableImageHandled_ReturnsPost()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var createdAt = DateTime.UtcNow.AddDays(-1);
            var updatedAt = DateTime.UtcNow;

            // Mock reader column ordinals
            _mockDbReader.Setup(r => r.GetOrdinal("PostID")).Returns(0);
            _mockDbReader.Setup(r => r.GetOrdinal("UserProfileID")).Returns(1);
            _mockDbReader.Setup(r => r.GetOrdinal("PostTitle")).Returns(2);
            _mockDbReader.Setup(r => r.GetOrdinal("PostContent")).Returns(3);
            _mockDbReader.Setup(r => r.GetOrdinal("CreatedAt")).Returns(4);
            _mockDbReader.Setup(r => r.GetOrdinal("UpdatedAt")).Returns(5);
            _mockDbReader.Setup(r => r.GetOrdinal("PostImage")).Returns(6);

            // Mock reader values
            _mockDbReader.Setup(r => r.GetGuid(0)).Returns(postId);
            _mockDbReader.Setup(r => r.GetGuid(1)).Returns(userId);
            _mockDbReader.Setup(r => r.GetString(2)).Returns("Test Title");
            _mockDbReader.Setup(r => r.GetString(3)).Returns("Test Content");
            _mockDbReader.Setup(r => r.GetDateTime(4)).Returns(createdAt);
            _mockDbReader.Setup(r => r.GetDateTime(5)).Returns(updatedAt);

            // Setup null image
            _mockDbReader.Setup(r => r.IsDBNull(6)).Returns(true);
            _mockDbReader.Setup(r => r.GetString(6)).Returns(() => null);
           
            // Act
            var result = await _postRepository.GetPostByPostIDAsync(postId, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.Data.ImageLink);
           _mockLogger.Verify(l => l.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()), Times.AtLeastOnce);
        }
    }
}
