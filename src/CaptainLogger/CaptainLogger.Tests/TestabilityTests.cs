
using AutoFixture;
using CaptainLogger;
using CaptainLogger.Logic;
using CaptainLogger.Options;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace CaptainLogger.Tests;

public class TestabilityTests
{
    private readonly IFixture _fixture = new Fixture();

    [Fact(DisplayName = "ICaptainLogger can be mocked successfully!")]
    public void CaptainLoggerMockTest()
    {
        var options = Substitute.For<IOptions<RectangleOptions>>();
        options
            .Value
            .Returns(_fixture
                .Build<RectangleOptions>()
                .With(x => x.Width, 150)
                .With(x => x.Height, 150)
                .Create());

        var logger = Substitute.For<ICaptainLogger<Rectangle>>();
        var rect = new Rectangle(logger, options);

        var area = rect.GetArea();
        var perimeter = rect.GetPerimeter();

        Assert.True(area > perimeter);
    }

    public class Rectangle
    {
        private readonly ICaptainLogger _logger;

        public int Width { get; }
        public int Height { get; }
        public bool IsSquare => GetIsSquare();

        public Rectangle(
            ICaptainLogger<Rectangle> logger,
            IOptions<RectangleOptions> options)
        {
            _logger = logger;
            Width = options.Value.Width;
            Height = options.Value.Height;

            logger
                .DebugLog("New rectangle created", Width, Height);
        }

        public int GetArea()
        {
            var area = Width * Height;
            _logger
                .DebugLog("Area calculated", area);

            return area;
        }

        public int GetPerimeter()
        {
            var perimeter = (Width + Height) * 2;
            _logger
                .DebugLog("Perimeter calculated", perimeter);

            return perimeter;
        }

        private bool GetIsSquare()
        {
            var isSquare = Width == Height;

            _logger
                .DebugLog("Check on is square has been requested", isSquare);

            return isSquare;
        }
    }

    public record RectangleOptions(int Width, int Height);
}
