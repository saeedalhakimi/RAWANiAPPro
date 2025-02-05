using Microsoft.Extensions.Configuration;
using Moq;
using RAWANi.WEBAPi.Application.Abstractions;
using RAWANi.WEBAPi.Application.Repository;
using RAWANi.WEBAPi.Application.Services;
using RAWANi.WEBAPi.Domain.Entities.Posts;
using RAWANi.WEBAPi.Domain.Models;
using RAWANi.WEBAPi.Infrastructure.Data.DataWrapperFactory;
using RAWANi.WEBAPi.Infrastructure.Repository.Posts;
using System.Data;


namespace RAWANi.WEBAPi.Infrastructure.UnitTests.RepositoriesTest.Tests
{
    public class CreatePostTests
    {
        private readonly Mock<IAppLogger<PostRepository>> _mockLogger;
        private readonly Mock<ILoggMessagingService> _mockMessagingService;
        private readonly Mock<IErrorHandler> _mockErrorHandler;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IDatabaseConnectionFactory> _mockConnectionFactory;
        private readonly Mock<IDatabaseConnection> _mockDbConnection;
        private readonly Mock<Data.DataWrapperFactory.IDbCommand> _mockDbCommand;
        private readonly string _connectionString = "Server=localhost;Database=RAWANiProDb;User Id=sa;Password=sa123456;Encrypt=False;TrustServerCertificate=True;Connection Timeout=30;";

        private readonly IPostRepository _postRepository;

        public CreatePostTests()
        {
            _mockLogger = new Mock<IAppLogger<PostRepository>>();
            _mockMessagingService = new Mock<ILoggMessagingService>();
            _mockErrorHandler = new Mock<IErrorHandler>();
            _mockConnectionFactory = new Mock<IDatabaseConnectionFactory>();
            _mockDbConnection = new Mock<IDatabaseConnection>();
            _mockDbCommand = new Mock<Data.DataWrapperFactory.IDbCommand>();

            // Mock Configuration to return connection string
            var mockConfiguration = new Mock<IConfiguration>();
            

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
            _mockDbCommand.Setup(c => c.ExecuteNonQueryAsync(It.IsAny<CancellationToken>()))
                          .ReturnsAsync(1); // Simulate successful insert

            // Initialize repository
            _postRepository = new PostRepository(
                mockConfiguration.Object,
                _mockLogger.Object,
                _mockMessagingService.Object,
                _mockErrorHandler.Object,
                _mockConnectionFactory.Object,
                _connectionString
            );
        }

        [Fact]
        public async Task CreatePostAsync_ValidPost_ReturnsSuccess()
        {
            // Arrange
            var post = Post.Create(Guid.NewGuid(), Guid.NewGuid(), "Valid Title", "Valid Content", "https://example.com").Data;
            var cancellationToken = new CancellationToken();

            // Act
            var result = await _postRepository.CreatePostAsync(post, cancellationToken);

            // Assert
            Assert.True(result.IsSuccess);
            _mockLogger.Verify(l => l.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()), Times.AtLeastOnce);
            _mockDbCommand.Verify(c => c.ExecuteNonQueryAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreatePostAsync_NoRowsAffected_ReturnsFailure()
        {
            // Arrange
            var post = Post.Create(Guid.NewGuid(), Guid.NewGuid(), "Valid Title", "Valid Content", "https://example.com").Data;
            var cancellationToken = new CancellationToken();

            // Simulate no rows affected
            _mockDbCommand.Setup(c => c.ExecuteNonQueryAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);

            // Act
            var result = await _postRepository.CreatePostAsync(post, cancellationToken);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Creation Failed", result.Errors.First().Message);
            _mockLogger.Verify(l => l.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
        }

        [Fact]
        public async Task CreatePostAsync_CancelledOperation_ReturnsCancellationHandled()
        {
            // Arrange
            var post = Post.Create(Guid.NewGuid(), Guid.NewGuid(), "Valid Title", "Valid Content", "https://example.com").Data;
            var cancellationToken = new CancellationToken(true); // Trigger cancellation

            // Mock error handler to return a response
            _mockErrorHandler.Setup(e => e.HandleCancelationToken<bool>(It.IsAny<OperationCanceledException>()))
                             .Returns(OperationResult<bool>.Failure(new Error(ErrorCode.OperationCancelled, "Operation canceled", "The operation was canceled")));

            // Act
            var result = await _postRepository.CreatePostAsync(post, cancellationToken);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Operation canceled", result.Errors.First().Message);
            _mockLogger.Verify(l => l.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
        }

        [Fact]
        public async Task CreatePostAsync_ExceptionOccurs_ReturnsFailure()
        {
            // Arrange
            var post = Post.Create(Guid.NewGuid(), Guid.NewGuid(), "Valid Title", "Valid Content", "https://example.com").Data;
            var cancellationToken = new CancellationToken();

            // Simulate exception during execution
            _mockDbCommand.Setup(c => c.ExecuteNonQueryAsync(It.IsAny<CancellationToken>()))
                          .ThrowsAsync(new Exception("Database error"));

            // Mock error handler to return expected response
            _mockErrorHandler.Setup(e => e.HandleException<bool>(It.IsAny<Exception>()))
                             .Returns(OperationResult<bool>.Failure(new Error(ErrorCode.UnknownError, "Unexpected error", "An unexpected error occurred")));

            // Act
            var result = await _postRepository.CreatePostAsync(post, cancellationToken);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Unexpected error", result.Errors.First().Message);
            _mockLogger.Verify(l => l.LogError(It.IsAny<string>(), It.IsAny<Exception>()), Times.Once);
        }
    }
}
