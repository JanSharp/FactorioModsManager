using FactorioModsManager.Infrastructure;
using FactorioModsManager.Services;
using FactorioModsManager.Services.Implementations;
using Moq;
using NUnit.Framework;

namespace FactorioModsManager.UnitTests.Services.Implementations
{
    [TestFixture]
    public class SyncModsWithPortalServiceTests
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

            var mainService = new SyncModsWithPortalService(modsStorageService: modsStorageService.Object);

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

            var mainService = new SyncModsWithPortalService(modsStorageService: modsStorageService.Object);

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

            var mainService = new SyncModsWithPortalService(modsStorageService: modsStorageService.Object);

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

            var mainService = new SyncModsWithPortalService(modsStorageService: modsStorageService.Object);

            // Act
            mainService.UnmaintainRelease(modName, version, shouldDelete);

            // Assert
            modsStorageService.Verify(mss => mss.DiscardRelease(modName, version), Times.Never);
        }

        //[Test]
        //public void UnmaintainRelease_Overload1()
        //{
        //    // Arrage
        //    var release = new ReleaseData()
        //    {
        //        Mod = new ModData()
        //        {
        //            Name = "ModName",
        //        },
        //        Version = new FactorioVersion(1, 2, 3),
        //    };
        //    var shouldDelete = true;

        //    var mainServiceMock = new Mock<SyncModsWithPortalService>();
        //    mainServiceMock.Setup(s => s.UnmaintainRelease(release.Mod.Name, release.Version, shouldDelete));
        //    var mainService = new SyncModsWithPortalService(mainService: mainServiceMock.Object);

        //    // Act
        //    mainService.UnmaintainRelease(release, shouldDelete);

        //    // Assert
        //    mainServiceMock.Verify(s => s.UnmaintainRelease(release.Mod.Name, release.Version, shouldDelete), Times.Once);
        //}
    }
}
