using System;
using System.IO;
using FluentAssertions;
using GUIApps.Shared.HelperMethods;
using Xunit;

namespace GUIApps.Tests
{
    public class ExifGpsParserTests
    {
        [Fact]
        public void TryGetGps_WithNullArray_Should_ReturnNull()
        {
            // Arrange
            byte[] data = null!;

            // Act
            var result = ExifGpsParser.TryGetGps(data);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void TryGetGps_WithEmptyArray_Should_ReturnNull()
        {
            // Arrange
            byte[] data = Array.Empty<byte>();

            // Act
            var result = ExifGpsParser.TryGetGps(data);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void TryGetGps_WithCorruptData_Should_ReturnNull()
        {
            // Arrange
            byte[] data = new byte[] { 0x00, 0x01, 0x02, 0xFF, 0xFE };

            // Act
            var result = ExifGpsParser.TryGetGps(data);

            // Assert
            result.Should().BeNull();
        }

        // Ideally, here we would include a base64 encoded string of a minimal JPEG image with EXIF GPS data
        // and test if it returns correct Lat/Lon values.
    }
}
