﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NuGet.Test.Helpers;
using Sleet.Test.Common;

namespace Sleet.AmazonS3.Tests
{
    public class BasicTests
    {
        [EnvVarExistsFact(AmazonS3TestContext.EnvAccessKeyId)]
        public async Task GivenAStorageAccountVerifyInitSucceeds()
        {
            using (var testContext = new AmazonS3TestContext())
            {
                await testContext.InitAsync();

                var result = await InitCommand.RunAsync(testContext.LocalSettings,
                    testContext.FileSystem,
                    enableCatalog: true,
                    enableSymbols: true,
                    log: testContext.Logger,
                    token: CancellationToken.None);

                result &= await ValidateCommand.RunAsync(testContext.LocalSettings,
                    testContext.FileSystem,
                    testContext.Logger);

                result.Should().BeTrue();

                await testContext.CleanupAsync();
            }
        }

        [EnvVarExistsFact(AmazonS3TestContext.EnvAccessKeyId)]
        public async Task GivenAStorageAccountVerifyPushSucceeds()
        {
            using (var packagesFolder = new TestFolder())
            using (var testContext = new AmazonS3TestContext())
            {
                await testContext.InitAsync();

                var testPackage = new TestNupkg("packageA", "1.0.0");
                var zipFile = testPackage.Save(packagesFolder.Root);

                var result = await InitCommand.RunAsync(testContext.LocalSettings,
                    testContext.FileSystem,
                    enableCatalog: true,
                    enableSymbols: true,
                    log: testContext.Logger,
                    token: CancellationToken.None);

                result &= await PushCommand.RunAsync(testContext.LocalSettings,
                    testContext.FileSystem,
                    new List<string>() { zipFile.FullName },
                    force: false,
                    skipExisting: false,
                    log: testContext.Logger);

                result &= await ValidateCommand.RunAsync(testContext.LocalSettings,
                    testContext.FileSystem,
                    testContext.Logger);

                result.Should().BeTrue();

                await testContext.CleanupAsync();
            }
        }

        [EnvVarExistsFact(AmazonS3TestContext.EnvAccessKeyId)]
        public async Task GivenAStorageAccountVerifyPushAndRemoveSucceed()
        {
            using (var packagesFolder = new TestFolder())
            using (var testContext = new AmazonS3TestContext())
            {
                await testContext.InitAsync();

                var testPackage = new TestNupkg("packageA", "1.0.0");
                var zipFile = testPackage.Save(packagesFolder.Root);

                var result = await InitCommand.RunAsync(testContext.LocalSettings,
                    testContext.FileSystem,
                    enableCatalog: true,
                    enableSymbols: true,
                    log: testContext.Logger,
                    token: CancellationToken.None);

                result &= await PushCommand.RunAsync(testContext.LocalSettings,
                    testContext.FileSystem,
                    new List<string>() { zipFile.FullName },
                    force: false,
                    skipExisting: false,
                    log: testContext.Logger);

                result &= await DeleteCommand.RunAsync(testContext.LocalSettings,
                    testContext.FileSystem,
                    "packageA",
                    "1.0.0",
                    "test",
                    force: true,
                    log: testContext.Logger);

                result &= await ValidateCommand.RunAsync(testContext.LocalSettings,
                    testContext.FileSystem,
                    testContext.Logger);

                result.Should().BeTrue();

                await testContext.CleanupAsync();
            }
        }
    }
}