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
    public class GetPostsWithPaginationTests
    {
        private readonly Mock<IAppLogger<PostRepository>> _mockLogger;
        private readonly Mock<ILoggMessagingService> _mockMessagingService;
        private readonly Mock<IErrorHandler> _mockErrorHandler;
        private readonly Mock<IDatabaseConnectionFactory> _mockConnectionFactory;
        private readonly Mock<IDatabaseConnection> _mockDbConnection;
        private readonly Mock<Data.DataWrapperFactory.IDbCommand> _mockDbCommand;
        private readonly Mock<Data.DataWrapperFactory.IDataReader> _mockDataReader;
        private readonly string _connectionString = "Server=localhost;Database=RAWANiProDb;User Id=sa;Password=sa123456;Encrypt=False;TrustServerCertificate=True;Connection Timeout=30;";

        private readonly IPostRepository _postRepository;

        public GetPostsWithPaginationTests()
        {
            _mockLogger = new Mock<IAppLogger<PostRepository>>();
            _mockMessagingService = new Mock<ILoggMessagingService>();
            _mockErrorHandler = new Mock<IErrorHandler>();
            _mockConnectionFactory = new Mock<IDatabaseConnectionFactory>();
            _mockDbConnection = new Mock<IDatabaseConnection>();
            _mockDbCommand = new Mock<Data.DataWrapperFactory.IDbCommand>();
            _mockDataReader = new Mock<Data.DataWrapperFactory.IDataReader>();

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
            _mockDbCommand.Setup(c => c.ExecuteReaderAsync(It.IsAny<CancellationToken>()))
                          .ReturnsAsync(_mockDataReader.Object);

            // Mock Data Reader Behavior
            _mockDataReader.SetupSequence(r => r.ReadAsync(It.IsAny<CancellationToken>()))
                           .ReturnsAsync(true)
                           .ReturnsAsync(false);
            _mockDataReader.Setup(r => r.GetGuid(It.IsAny<int>())).Returns(Guid.NewGuid());
            _mockDataReader.Setup(r => r.GetString(It.IsAny<int>())).Returns("Sample Data");
            _mockDataReader.Setup(r => r.GetDateTime(It.IsAny<int>())).Returns(DateTime.UtcNow);
            _mockDataReader.Setup(r => r.IsDBNull(It.IsAny<int>())).Returns(false);

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
        public async Task GetPostsWithPaginationAsync_ValidRequest_ReturnsPosts()
        {
            // Arrange
            var userProfileId = Guid.NewGuid();
            var pageNumber = 1;
            var pageSize = 10;
            var sortColumn = "CreatedAt";
            var sortDirection = "ASC";
            var cancellationToken = new CancellationToken();

            // Act
            var result = await _postRepository.GetPostsWithPaginationAsync(userProfileId, pageNumber, pageSize, sortColumn, sortDirection, cancellationToken);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotEmpty(result.Data);
            _mockLogger.Verify(l => l.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()), Times.AtLeastOnce);
            _mockDbCommand.Verify(c => c.ExecuteReaderAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetPostsWithPaginationAsync_NoPostsFound_ReturnsEmptyList()
        {
            // Arrange
            var userProfileId = Guid.NewGuid();
            var pageNumber = 1;
            var pageSize = 10;
            var sortColumn = "CreatedAt";
            var sortDirection = "ASC";
            var cancellationToken = new CancellationToken();

            // Simulate no posts found
            _mockDataReader.SetupSequence(r => r.ReadAsync(It.IsAny<CancellationToken>()))
                           .ReturnsAsync(false);

            // Act
            var result = await _postRepository.GetPostsWithPaginationAsync(userProfileId, pageNumber, pageSize, sortColumn, sortDirection, cancellationToken);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Empty(result.Data);
            _mockLogger.Verify(l => l.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task GetPostsWithPaginationAsync_CancelledOperation_ReturnsCancellationHandled()
        {
            // Arrange
            var userProfileId = Guid.NewGuid();
            var pageNumber = 1;
            var pageSize = 10;
            var sortColumn = "CreatedAt";
            var sortDirection = "ASC";
            var cancellationToken = new CancellationToken(true); // Trigger cancellation

            // Mock error handler to return a response
            _mockErrorHandler.Setup(e => e.HandleCancelationToken<IEnumerable<Post>>(It.IsAny<OperationCanceledException>()))
                             .Returns(OperationResult<IEnumerable<Post>>.Failure(new Error(ErrorCode.OperationCancelled, "Operation canceled", "The operation was canceled")));

            // Act
            var result = await _postRepository.GetPostsWithPaginationAsync(userProfileId, pageNumber, pageSize, sortColumn, sortDirection, cancellationToken);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Operation canceled", result.Errors.First().Message);
            _mockLogger.Verify(l => l.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
        }

        [Fact]
        public async Task GetPostsWithPaginationAsync_ExceptionOccurs_ReturnsFailure()
        {
            // Arrange
            var userProfileId = Guid.NewGuid();
            var pageNumber = 1;
            var pageSize = 10;
            var sortColumn = "CreatedAt";
            var sortDirection = "ASC";
            var cancellationToken = new CancellationToken();

            // Simulate exception during execution
            _mockDbCommand.Setup(c => c.ExecuteReaderAsync(It.IsAny<CancellationToken>()))
                          .ThrowsAsync(new Exception("Database error"));

            // Mock error handler to return expected response
            _mockErrorHandler.Setup(e => e.HandleException<IEnumerable<Post>>(It.IsAny<Exception>()))
                             .Returns(OperationResult<IEnumerable<Post>>.Failure(new Error(ErrorCode.UnknownError, "Unexpected error", "An unexpected error occurred")));

            // Act
            var result = await _postRepository.GetPostsWithPaginationAsync(userProfileId, pageNumber, pageSize, sortColumn, sortDirection, cancellationToken);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Unexpected error", result.Errors.First().Message);
            _mockLogger.Verify(l => l.LogError(It.IsAny<string>(), It.IsAny<Exception>()), Times.Once);
        }
    }
}
