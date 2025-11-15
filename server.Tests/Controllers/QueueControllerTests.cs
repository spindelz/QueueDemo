using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using server.Controllers;
using server.Models;
using server.Repositories.Interface;
using System.Net;

namespace server.Tests.Controllers;

public class QueueControllerTests
{
    private readonly Mock<IQueue> _mockQueueRepository;
    private readonly QueueController _controller;

    public QueueControllerTests()
    {
        _mockQueueRepository = new Mock<IQueue>();
        _controller = new QueueController(_mockQueueRepository.Object);
    }

    #region ReceiveQueue Tests

    [Fact]
    public async Task ReceiveQueue_WhenNoExistingQueue_ShouldReturnA0()
    {
        // Arrange
        _mockQueueRepository.Setup(x => x.GetLastestQueue())
            .ReturnsAsync((Models.Queue?)null);
        _mockQueueRepository.Setup(x => x.GenerateQueue())
            .ReturnsAsync("A0");

        // Act
        var result = await _controller.ReceiveQueue();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        okResult.Value.Should().NotBeNull();
        _mockQueueRepository.Verify(x => x.GenerateQueue(), Times.Once);
    }

    [Fact]
    public async Task ReceiveQueue_WhenQueueExistsButNotFull_ShouldReturnNextQueue()
    {
        // Arrange
        var lastQueue = new Models.Queue { Id = 1, Number = "A5", QDate = DateTime.Now };
        _mockQueueRepository.Setup(x => x.GetLastestQueue())
            .ReturnsAsync(lastQueue);
        _mockQueueRepository.Setup(x => x.GenerateQueue())
            .ReturnsAsync("A6");

        // Act
        var result = await _controller.ReceiveQueue();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        _mockQueueRepository.Verify(x => x.GenerateQueue(), Times.Once);
    }

    [Fact]
    public async Task ReceiveQueue_WhenQueueIsFull_ShouldReturnBadRequest()
    {
        // Arrange
        var fullQueue = new Models.Queue { Id = 1, Number = "Z9", QDate = DateTime.Now };
        _mockQueueRepository.Setup(x => x.GetLastestQueue())
            .ReturnsAsync(fullQueue);

        // Act
        var result = await _controller.ReceiveQueue();

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        _mockQueueRepository.Verify(x => x.GenerateQueue(), Times.Never);
    }

    [Fact]
    public async Task ReceiveQueue_WhenExceptionThrown_ShouldReturnInternalServerError()
    {
        // Arrange
        _mockQueueRepository.Setup(x => x.GetLastestQueue())
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.ReceiveQueue();

        // Assert
        var statusCodeResult = result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
    }

    #endregion

    #region GetQueueDetail Tests

    [Fact]
    public async Task GetQueueDetail_WhenQueueExists_ShouldReturnQueueResponse()
    {
        // Arrange
        var queueResponse = new QueueResponse
        {
            Id = 1,
            Number = "A5",
            QDate = "15/11/2025 10:30:00"
        };
        _mockQueueRepository.Setup(x => x.GetQueueDetail("A5"))
            .ReturnsAsync(queueResponse);

        // Act
        var result = await _controller.GetQueueDetail("A5");

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        var returnValue = okResult.Value.Should().BeOfType<QueueResponse>().Subject;
        returnValue.Number.Should().Be("A5");
        returnValue.Id.Should().Be(1);
        _mockQueueRepository.Verify(x => x.GetQueueDetail("A5"), Times.Once);
    }

    [Fact]
    public async Task GetQueueDetail_WhenQueueNotFound_ShouldReturnBadRequest()
    {
        // Arrange
        _mockQueueRepository.Setup(x => x.GetQueueDetail("Z9"))
            .ReturnsAsync((QueueResponse?)null);

        // Act
        var result = await _controller.GetQueueDetail("Z9");

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetQueueDetail_WhenExceptionThrown_ShouldReturnInternalServerError()
    {
        // Arrange
        _mockQueueRepository.Setup(x => x.GetQueueDetail(It.IsAny<string>()))
            .ThrowsAsync(new Exception("Database connection failed"));

        // Act
        var result = await _controller.GetQueueDetail("A0");

        // Assert
        var statusCodeResult = result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task GetQueueDetail_WithDifferentQueueNumbers_ShouldReturnCorrectData()
    {
        // Arrange
        var testCases = new[] { "A0", "B5", "C9", "Z9" };
        
        foreach (var queueNumber in testCases)
        {
            // Arrange
            var queueResponse = new QueueResponse
            {
                Id = 1,
                Number = queueNumber,
                QDate = "15/11/2025 10:30:00"
            };
            _mockQueueRepository.Setup(x => x.GetQueueDetail(queueNumber))
                .ReturnsAsync(queueResponse);

            // Act
            var result = await _controller.GetQueueDetail(queueNumber);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnValue = okResult.Value.Should().BeOfType<QueueResponse>().Subject;
            returnValue.Number.Should().Be(queueNumber);
        }
    }

    #endregion

    #region ClearQueue Tests

    [Fact]
    public async Task ClearQueue_ShouldCallRepositoryAndReturnOk()
    {
        // Arrange
        _mockQueueRepository.Setup(x => x.ClearQueue())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.ClearQueue();

        // Assert
        result.Should().BeOfType<OkResult>();
        var okResult = result as OkResult;
        okResult?.StatusCode.Should().Be((int)HttpStatusCode.OK);
        _mockQueueRepository.Verify(x => x.ClearQueue(), Times.Once);
    }

    [Fact]
    public async Task ClearQueue_WhenExceptionThrown_ShouldReturnInternalServerError()
    {
        // Arrange
        _mockQueueRepository.Setup(x => x.ClearQueue())
            .ThrowsAsync(new Exception("Cannot clear queues"));

        // Act
        var result = await _controller.ClearQueue();

        // Assert
        var statusCodeResult = result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
    }

    #endregion

    #region Integration Scenarios

    [Fact]
    public async Task ReceiveQueue_MultipleCallsSequentially_ShouldGenerateSequentialQueues()
    {
        // Arrange
        var callCount = 0;
        var queueNumbers = new[] { "A0", "A1", "A2", "A3", "A4" };

        // Setup GetLastestQueue to return the last generated queue
        _mockQueueRepository.Setup(x => x.GetLastestQueue())
            .Returns(async () =>
            {
                if (callCount == 0)
                    return null;
                
                return new Models.Queue 
                { 
                    Id = callCount, 
                    Number = queueNumbers[callCount - 1], 
                    QDate = DateTime.Now 
                };
            });

        // Setup GenerateQueue to return next queue number
        _mockQueueRepository.Setup(x => x.GenerateQueue())
            .Returns(async () =>
            {
                var result = queueNumbers[callCount];
                callCount++;
                return result;
            });

        // Act & Assert
        for (int i = 0; i < 3; i++)
        {
            var result = await _controller.ReceiveQueue();
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        _mockQueueRepository.Verify(x => x.GenerateQueue(), Times.Exactly(3));
    }

    #endregion
}
