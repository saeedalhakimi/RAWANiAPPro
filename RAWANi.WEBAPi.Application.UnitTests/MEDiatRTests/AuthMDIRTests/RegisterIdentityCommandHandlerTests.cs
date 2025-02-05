//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Moq;
//using RAWANi.WEBAPi.Application.Abstractions;
//using RAWANi.WEBAPi.Application.Data.DbContexts;
//using RAWANi.WEBAPi.Application.MEDiatR.AuthMDIR.CommandHandlers;
//using RAWANi.WEBAPi.Application.MEDiatR.AuthMDIR.Commands;
//using RAWANi.WEBAPi.Application.Services;
//using System;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Xunit;

//namespace RAWANi.WEBAPi.Application.UnitTests.MEDiatRTests.AuthMDIRTests
//{
//    public class RegisterIdentityCommandHandlerTests
//    {
//        private readonly Mock<DataContext> _mockContext;
//        private readonly Mock<UserManager<IdentityUser>> _mockUserManager;
//        private readonly Mock<RoleManager<IdentityRole>> _mockRoleManager;
//        private readonly Mock<IAppLogger<RegisterIdentityCommandHandler>> _mockLogger;
//        private readonly Mock<ILoggMessagingService> _mockMessagingService;
//        private readonly Mock<IJwtService> _mockJwtService;
//        private readonly Mock<IFileService> _mockFileService;
//        private readonly Mock<IErrorHandler> _mockErrorHandler;
//        private readonly Mock<IEmailService> _mockEmailService;

//        private readonly RegisterIdentityCommandHandler _handler;

//        public RegisterIdentityCommandHandlerTests()
//        {
//            // Use an in-memory database for testing
//            var options = new DbContextOptionsBuilder<DataContext>()
//                .UseInMemoryDatabase(Guid.NewGuid().ToString())  // Unique DB name for each test run
//                .Options;

//            _mockContext = new Mock<DataContext>(options); // Mock DataContext

//            // Initialize mocks for the dependencies of RegisterIdentityCommandHandler
//            _mockUserManager = new Mock<UserManager<IdentityUser>>(
//                Mock.Of<IUserStore<IdentityUser>>(),
//                null, null, null, null, null, null, null, null);
//            _mockRoleManager = new Mock<RoleManager<IdentityRole>>(
//                Mock.Of<IRoleStore<IdentityRole>>(),
//                null, null, null, null);
//            _mockLogger = new Mock<IAppLogger<RegisterIdentityCommandHandler>>();
//            _mockMessagingService = new Mock<ILoggMessagingService>();
//            _mockJwtService = new Mock<IJwtService>();
//            _mockFileService = new Mock<IFileService>();
//            _mockErrorHandler = new Mock<IErrorHandler>();
//            _mockEmailService = new Mock<IEmailService>();

//            // Create the handler with mocked dependencies
//            _handler = new RegisterIdentityCommandHandler(
//                _mockContext.Object,
//                _mockUserManager.Object,
//                _mockRoleManager.Object,
//                _mockLogger.Object,
//                _mockMessagingService.Object,
//                _mockJwtService.Object,
//                _mockFileService.Object,
//                _mockErrorHandler.Object,
//                _mockEmailService.Object
//            );
//        }

//        [Fact]
//        public async Task Handle_UserAlreadyExists_ReturnsFailure()
//        {
//            // Arrange
//            var request = new RegisterIdentityCommand
//            {
//                Username = "testuser",
//                Password = "Password123!",
//                PhoneNumber = "123456789",
//                ProfilePicture = null, // Mock as needed
//            };

//            // Set up the mock to return an existing user for the username
//            var mockIdentityUser = new IdentityUser { UserName = "testuser" };
//            _mockUserManager.Setup(x => x.FindByNameAsync(request.Username))
//                            .ReturnsAsync(mockIdentityUser);

//            // Act
//            var result = await _handler.Handle(request, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal("User already exists", result.Errors.First().Message);
//        }
//    }
//}
