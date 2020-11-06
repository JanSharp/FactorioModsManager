using System;
using System.Collections.Generic;
using FactorioModsManager.Infrastructure;
using FactorioModsManager.Services.Implementations;
using FluentAssertions;
using NUnit.Framework;

namespace FactorioModsManager.UnitTests.Services.Implementations
{
    [TestFixture]
    public class ArgsServiceTests
    {
        const string InvalidArg = "--foo";

        const string ConfigArg = "--config";
        const string CreateConfigArg = "--create-config";
        const string ExtractModsPathArg = "--extract-mods-path";
        const string ModListPathArg = "--mod-list-path";
        const string SaveFilePathArg = "--save-file-path";
        const string ExtractModNameArg = "--extract-mod-name";
        const string ExtractModNameShortArg = "-e";
        const string DoNotExtractDependenciesArg = "--do-not-extract-dependencies";
        const string TargetFactorioVersionToExtractArg = "--target-factorio-version-to-extract";
        const string TargetFactorioVersionToExtractShortArg = "-t";

        const string ConfigExtraArg = @"C:\Test\Path\config.xml";
        const string ExtractModsPathExtraArg = @"C:\Test\Path\ExtractedMods";
        const string ModListPathExtraArg = @"C:\Test\Path\mod-list.json";
        const string SaveFilePathExtraArg = @"C:\Test\Path\save.zip";
        const string ExtractModNameExtraArgFoo = "FooMod";
        const string ExtractModNameExtraArgBar = "BarMod";


        [Test]
        public void ParseArgs_UnexpectedArg_ThrowException()
        {
            // Arrange

            // Act
            static void Code()
            {
                var programArgs = ArgsService.ParseArgs(new[]
                {
                    InvalidArg,
                });
            }

            // Assert
            Assert.That(Code, Throws.TypeOf<Exception>());
        }


        [Test]
        public void ParseArgs_OnlyExtractModsPathSet_ThrowException()
        {
            // Arrange
            var args = new[]
            {
                ExtractModsPathArg,
                ExtractModsPathExtraArg,
            };

            // Act
            Action act = () => ArgsService.ParseArgs(args);

            // Assert
            act.Should().ThrowExactly<Exception>();
        }

        [Test]
        public void ParseArgs_OnlyModListPathSet_ThrowException()
        {
            // Arrange
            var args = new[]
            {
                ModListPathArg,
                ModListPathExtraArg,
            };

            // Act
            Action act = () => ArgsService.ParseArgs(args);

            // Assert
            act.Should().ThrowExactly<Exception>();
        }

        [Test]
        public void ParseArgs_OnlySaveFilePathSet_ThrowException()
        {
            // Arrange
            var args = new[]
            {
                SaveFilePathArg,
                SaveFilePathExtraArg,
            };

            // Act
            Action act = () => ArgsService.ParseArgs(args);

            // Assert
            act.Should().ThrowExactly<Exception>();
        }

        [Test]
        public void ParseArgs_OnlyExtractModNameSet_ThrowException()
        {
            // Arrange
            var args = new[]
            {
                ExtractModNameArg,
                ExtractModNameExtraArgFoo,
            };

            // Act
            Action act = () => ArgsService.ParseArgs(args);

            // Assert
            act.Should().ThrowExactly<Exception>();
        }


        [Test]
        public void ParseArgs_ConfigArgWithoutPath_ThrowException()
        {
            // Arrange
            var args = new[]
            {
                ConfigArg,
            };

            // Act
            Action act = () => ArgsService.ParseArgs(args);

            // Assert
            act.Should().ThrowExactly<Exception>();
        }

        [Test]
        public void ParseArgs_ExtractModsPathWithoutPath_ThrowException()
        {
            // Arrange
            var args = new[]
            {
                ExtractModsPathArg,
            };

            // Act
            Action act = () => ArgsService.ParseArgs(args);

            // Assert
            act.Should().ThrowExactly<Exception>();
        }

        [Test]
        public void ParseArgs_ModListPathWithoutPath_ThrowException()
        {
            // Arrange
            var args = new[]
            {
                ModListPathArg,
            };

            // Act
            Action act = () => ArgsService.ParseArgs(args);

            // Assert
            act.Should().ThrowExactly<Exception>();
        }

        [Test]
        public void ParseArgs_SaveFilePathWithoutPath_ThrowException()
        {
            // Arrange
            var args = new[]
            {
                SaveFilePathArg,
            };

            // Act
            Action act = () => ArgsService.ParseArgs(args);

            // Assert
            act.Should().ThrowExactly<Exception>();
        }

        [Test]
        public void ParseArgs_ExtractModNameWithoutName_ThrowException()
        {
            // Arrange
            var args = new[]
            {
                ExtractModNameArg,
            };

            // Act
            Action act = () => ArgsService.ParseArgs(args);

            // Assert
            act.Should().ThrowExactly<Exception>();
        }

        [Test]
        public void ParseArgs_TargetFactorioVersionToExtractWithoutName_ThrowException()
        {
            // Arrange
            var args = new[]
            {
                TargetFactorioVersionToExtractArg,
            };

            // Act
            Action act = () => ArgsService.ParseArgs(args);

            // Assert
            act.Should().ThrowExactly<Exception>();
        }


        [Test]
        public void ParseArgs_ExtractModsPathAndModListPathAndSaveFilePathAndExtractModNameSet_ThrowException()
        {
            // Arrange
            var args = new[]
            {
                ExtractModsPathArg,
                ExtractModsPathExtraArg,
                ModListPathArg,
                ModListPathExtraArg,
                SaveFilePathArg,
                SaveFilePathExtraArg,
                ExtractModNameArg,
                ExtractModNameExtraArgFoo,
            };

            // Act
            Action act = () => ArgsService.ParseArgs(args);

            // Assert
            act.Should().ThrowExactly<Exception>();
        }

        [Test]
        public void ParseArgs_ExtractModsPathAndModListPathAndSaveFilePathSet_ThrowException()
        {
            // Arrange
            var args = new[]
            {
                ExtractModsPathArg,
                ExtractModsPathExtraArg,
                ModListPathArg,
                ModListPathExtraArg,
                SaveFilePathArg,
                SaveFilePathExtraArg,
            };

            // Act
            Action act = () => ArgsService.ParseArgs(args);

            // Assert
            act.Should().ThrowExactly<Exception>();
        }

        [Test]
        public void ParseArgs_ExtractModsPathAndModListPathAndExtractModNameSet_ThrowException()
        {
            // Arrange
            var args = new[]
            {
                ExtractModsPathArg,
                ExtractModsPathExtraArg,
                ModListPathArg,
                ModListPathExtraArg,
                ExtractModNameArg,
                ExtractModNameExtraArgFoo,
            };

            // Act
            Action act = () => ArgsService.ParseArgs(args);

            // Assert
            act.Should().ThrowExactly<Exception>();
        }

        [Test]
        public void ParseArgs_ExtractModsPathAndSaveFilePathAndExtractModNameSet_ThrowException()
        {
            // Arrange
            var args = new[]
            {
                ExtractModsPathArg,
                ExtractModsPathExtraArg,
                SaveFilePathArg,
                SaveFilePathExtraArg,
                ExtractModNameArg,
                ExtractModNameExtraArgFoo,
            };

            // Act
            Action act = () => ArgsService.ParseArgs(args);

            // Assert
            act.Should().ThrowExactly<Exception>();
        }


        [Test]
        public void ParseArgs_TargetFactorioVersionToExtractWithInvalidVersion_ThrowException()
        {
            // Arrange
            var args = new[]
            {
                TargetFactorioVersionToExtractArg,
                "foo",
            };

            // Act
            Action act = () => ArgsService.ParseArgs(args);

            // Assert
            act.Should().Throw<Exception>();
        }


        [Test]
        public void ParseArgs_ConfigArgWithPath_GetPath()
        {
            // Arrage
            var expected = new ProgramArgs()
            {
                ConfigFilePath = ConfigExtraArg,
            };
            var args = new[]
            {
                ConfigArg,
                ConfigExtraArg,
            };

            // Act
            var actual = ArgsService.ParseArgs(args);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void ParseArgs_CreateConfigArg_SetCreateConfigToTrue()
        {
            // Arrage
            var expected = new ProgramArgs()
            {
                CreateConfig = true,
            };
            var args = new[]
            {
                CreateConfigArg,
            };

            // Act
            var actual = ArgsService.ParseArgs(args);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void ParseArgs_ExtractModsPathAndModListPathSet_GetPaths()
        {
            // Arrage
            var expected = new ProgramArgs()
            {
                ExtractModsPath = ExtractModsPathExtraArg,
                ModListPath = ModListPathExtraArg,
            };
            var args = new[]
            {
                ExtractModsPathArg,
                ExtractModsPathExtraArg,
                ModListPathArg,
                ModListPathExtraArg,
            };

            // Act
            var actual = ArgsService.ParseArgs(args);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void ParseArgs_ExtractModsPathAndSaveFilePathSet_GetPaths()
        {
            // Arrage
            var expected = new ProgramArgs()
            {
                ExtractModsPath = ExtractModsPathExtraArg,
                SaveFilePath = SaveFilePathExtraArg,
            };
            var args = new[]
            {
                ExtractModsPathArg,
                ExtractModsPathExtraArg,
                SaveFilePathArg,
                SaveFilePathExtraArg,
            };

            // Act
            var actual = ArgsService.ParseArgs(args);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void ParseArgs_ExtractModsPathAndTwoExtractModNameSet_GetPathsAndModNames()
        {
            // Arrage
            var expected = new ProgramArgs()
            {
                ExtractModsPath = ExtractModsPathExtraArg,
                ModNamesToExtract = new List<string>()
                {
                    ExtractModNameExtraArgFoo,
                    ExtractModNameExtraArgBar,
                },
            };
            var args = new[]
            {
                ExtractModsPathArg,
                ExtractModsPathExtraArg,
                ExtractModNameArg,
                ExtractModNameExtraArgFoo,
                ExtractModNameArg,
                ExtractModNameExtraArgBar,
            };

            // Act
            var actual = ArgsService.ParseArgs(args);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void ParseArgs_ExtractModsPathAndTwoExtractModNameSetUsingNormalAndShortArg_GetPathsAndModNames()
        {
            // Arrage
            var expected = new ProgramArgs()
            {
                ExtractModsPath = ExtractModsPathExtraArg,
                ModNamesToExtract = new List<string>()
                {
                    ExtractModNameExtraArgFoo,
                    ExtractModNameExtraArgBar,
                },
            };
            var args = new[]
            {
                ExtractModsPathArg,
                ExtractModsPathExtraArg,
                ExtractModNameArg,
                ExtractModNameExtraArgFoo,
                ExtractModNameShortArg,
                ExtractModNameExtraArgBar,
            };

            // Act
            var actual = ArgsService.ParseArgs(args);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void ParseArgs_TargetFactorioVersionToExtractUsingNormalAndShortArg_GetVersions()
        {
            // Arrage
            var expected = new ProgramArgs()
            {
                TargetFactorioVersionsToExtract =
                {
                    new FactorioVersion(1, 0),
                    new FactorioVersion(0, 18),
                }
            };
            var args = new[]
            {
                TargetFactorioVersionToExtractArg,
                "1.0",
                TargetFactorioVersionToExtractShortArg,
                "0.18",
            };

            // Act
            var actual = ArgsService.ParseArgs(args);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void ParseArgs_DoNotExtractDependenciesArg_SetDoNotExtractDependenciesToTrue()
        {
            // Arrage
            var expected = new ProgramArgs()
            {
                DoNotExtractDependencies = true,
            };
            var args = new[]
            {
                DoNotExtractDependenciesArg,
            };

            // Act
            var actual = ArgsService.ParseArgs(args);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }
    }
}
