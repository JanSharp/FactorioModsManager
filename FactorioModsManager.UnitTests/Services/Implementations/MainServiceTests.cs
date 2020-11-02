using FactorioModsManager.Infrastructure;
using FactorioModsManager.Services;
using FactorioModsManager.Services.Implementations;
using Moq;
using NUnit.Framework;

namespace FactorioModsManager.UnitTests.Services.Implementations
{
    [TestFixture]
    public class MainServiceTests
    {
        [Test]
        public void UnmaintainRelease_ShouldDeleteAndIsStored_DiscardRelease()
        {
            // Arrange
            var modName = "TestMod";
            var version = new FactorioVersion(1, 2, 3);
            var shouldDelete = true;
            var isStored = true;

            var modsStorageService = new Mock<IModsStorageService>();
            modsStorageService.Setup(mss => mss.ReleaseIsStored(modName, version)).Returns(isStored);

            var mainService = new MainService(null, null, null, null, null, modsStorageService.Object);

            // Act
            mainService.UnmaintainRelease(modName, version, shouldDelete);

            // Assert
            modsStorageService.Verify(mss => mss.DiscardRelease(modName, version), Times.Once);
        }

        [Test]
        public void UnmaintainRelease_ShouldNotDeleteAndIsStored_DoNothing()
        {
            // Arrange
            var modName = "TestMod";
            var version = new FactorioVersion(1, 2, 3);
            var shouldDelete = false;
            var isStored = true;

            var modsStorageService = new Mock<IModsStorageService>();
            modsStorageService.Setup(mss => mss.ReleaseIsStored(modName, version)).Returns(isStored);

            var mainService = new MainService(null, null, null, null, null, modsStorageService.Object);

            // Act
            mainService.UnmaintainRelease(modName, version, shouldDelete);

            // Assert
            modsStorageService.Verify(mss => mss.DiscardRelease(modName, version), Times.Never);
        }

        [Test]
        public void UnmaintainRelease_ShouldDeleteAndIsNotStored_DoNothing()
        {
            // Arrange
            var modName = "TestMod";
            var version = new FactorioVersion(1, 2, 3);
            var shouldDelete = true;
            var isStored = false;

            var modsStorageService = new Mock<IModsStorageService>();
            modsStorageService.Setup(mss => mss.ReleaseIsStored(modName, version)).Returns(isStored);

            var mainService = new MainService(null, null, null, null, null, modsStorageService.Object);

            // Act
            mainService.UnmaintainRelease(modName, version, shouldDelete);

            // Assert
            modsStorageService.Verify(mss => mss.DiscardRelease(modName, version), Times.Never);
        }

        [Test]
        public void UnmaintainRelease_ShouldNotDeleteAndIsNotStored_DoNothing()
        {
            // Arrange
            var modName = "TestMod";
            var version = new FactorioVersion(1, 2, 3);
            var shouldDelete = false;
            var isStored = false;

            var modsStorageService = new Mock<IModsStorageService>();
            modsStorageService.Setup(mss => mss.ReleaseIsStored(modName, version)).Returns(isStored);

            var mainService = new MainService(null, null, null, null, null, modsStorageService.Object);

            // Act
            mainService.UnmaintainRelease(modName, version, shouldDelete);

            // Assert
            modsStorageService.Verify(mss => mss.DiscardRelease(modName, version), Times.Never);
        }
    }
}
