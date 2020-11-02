using System;
using FactorioModsManager.Services.Implementations;
using NUnit.Framework;

namespace FactorioModsManager.UnitTests.Services.Implementations
{
    [TestFixture]
    public class ArgsServiceTests
    {
        [Test]
        public void Constructor_ConfigArgWithPath_ExtractPath()
        {
            // Arrange
            var expectedPath = @"C:\Test\Path\config.xml";

            // Act
            var argsService = new ArgsService(new[]
            {
               "--config",
               expectedPath,
            });
            var actualPath = argsService.GetArgs().ConfigFilePath;

            // Assert
            Assert.That(actualPath, Is.EqualTo(expectedPath));
        }

        [Test]
        public void Constructor_ConfigArgWithoutPath_ThrowException()
        {
            // Arrange

            // Act
            void Code()
            {
                var argsService = new ArgsService(new[]
                {
                    "--config",
                });
            }

            // Assert
            Assert.That(Code, Throws.TypeOf<Exception>());
        }

        [Test]
        public void Constructor_CreateConfigArg_SetCreateConfigToTrue()
        {
            // Arrage

            // Act
            var argsService = new ArgsService(new[]
            {
                "--create-config",
            });

            // Assert
            Assert.That(argsService.GetArgs().CreateConfig, Is.True);
        }
    }
}
