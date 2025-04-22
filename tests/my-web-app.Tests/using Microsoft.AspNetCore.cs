using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using DevOpsDemo.Controllers;
using DevOpsDemo.Models;
using DevOpsDemo.Services;
using Xunit;

namespace DevOpsDemo.Tests
{
    public class HomeControllerTests
    {
        private readonly Mock<ILogger<HomeController>> _mockLogger;
        private readonly Mock<IMessageService> _mockMessageService;
        private readonly HomeController _controller;

        public HomeControllerTests()
        {
            _mockLogger = new Mock<ILogger<HomeController>>();
            _mockMessageService = new Mock<IMessageService>();
            _controller = new HomeController(_mockLogger.Object, _mockMessageService.Object);
        }

        [Fact]
        public void Index_ReturnsViewWithMessage()
        {
            // Arrange
            string expectedMessage = "Test Welcome Message";
            _mockMessageService.Setup(service => service.GetWelcomeMessage()).Returns(expectedMessage);

            // Act
            var result = _controller.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedMessage, result.ViewBag.Message);
        }

        [Fact]
        public void Feedback_Get_ReturnsView()
        {
            // Act
            var result = _controller.Feedback() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<FeedbackModel>(result.Model);
        }

        [Fact]
        public void Feedback_Post_WithValidModel_ReturnsConfirmationView()
        {
            // Arrange
            var model = new FeedbackModel
            {
                Name = "Test User",
                Email = "test@example.com",
                Message = "This is a test message",
                Rating = 5
            };

            // Act
            var result = _controller.Feedback(model) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Confirmation", result.ViewName);
            Assert.Equal(model, result.Model);
            Assert.Equal("Thank you for your feedback!", result.ViewBag.SuccessMessage);
        }

        [Fact]
        public void Feedback_Post_WithInvalidModel_ReturnsSameView()
        {
            // Arrange
            var model = new FeedbackModel(); // Empty model will fail validation
            _controller.ModelState.AddModelError("Name", "Required");

            // Act
            var result = _controller.Feedback(model) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.ViewName); // Returns the default view
            Assert.Equal(model, result.Model);
        }
    }

    public class MessageServiceTests
    {
        [Fact]
        public void GetWelcomeMessage_ReturnsCorrectMessage()
        {
            // Arrange
            var service = new MessageService();
            
            // Act
            var result = service.GetWelcomeMessage();
            
            // Assert
            Assert.Equal("Welcome to the DevOps Pipeline Demo!", result);
        }
    }
}