﻿/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using Piranha.Services;
using System;
using System.IO;
using Xunit;

namespace Piranha.ImageSharp.Tests
{
    [Collection("Integration tests")]
    public class MediaRepository : BaseTests
    {
        private Guid imageId;

        protected override void Init() {
            using (var api = CreateApi()) {
                App.Init(api);

                // Add media
                using (var stream = File.OpenRead("../../../Assets/HLD_Screenshot_01_mech_1080.png")) {
                    var image1 = new Models.StreamMediaContent() {
                        Filename = "HLD_Screenshot_01_mech_1080.png",
                        Data = stream
                    };
                    api.Media.Save(image1);

                    imageId = image1.Id.Value;
                }
            }
        }
        protected override void Cleanup() {
            using (var api = CreateApi()) {
                api.Media.Delete(imageId);
            }
        }

        [Fact]
        public void GetOriginal() {
            using (var api = CreateApi()) {
                var media = api.Media.GetById(imageId);

                Assert.NotNull(media);
                Assert.Equal($"~/uploads/{imageId}-{media.Filename}", media.PublicUrl);
            }
        }

        [Fact]
        public void GetScaled() {
            using (var api = CreateApi()) {
                var url = api.Media.EnsureVersion(imageId, 640);

                Assert.NotNull(url);
                Assert.Equal($"~/uploads/{imageId}-HLD_Screenshot_01_mech_1080_640.png", url);
            }
        }

        [Fact]
        public void GetCropped() {
            using (var api = CreateApi()) {
                var url = api.Media.EnsureVersion(imageId, 640, 300);

                Assert.NotNull(url);
                Assert.Equal($"~/uploads/{imageId}-HLD_Screenshot_01_mech_1080_640x300.png", url);
            }
        }

        [Fact]
        public void GetScaledOrgSize() {
            using (var api = CreateApi()) {
                var url = api.Media.EnsureVersion(imageId, 1920);

                Assert.NotNull(url);
                Assert.Equal($"~/uploads/{imageId}-HLD_Screenshot_01_mech_1080.png", url);
            }
        }

        [Fact]
        public void GetCroppedOrgSize() {
            using (var api = CreateApi()) {
                var url = api.Media.EnsureVersion(imageId, 1920, 1080);

                Assert.NotNull(url);
                Assert.Equal($"~/uploads/{imageId}-HLD_Screenshot_01_mech_1080.png", url);
            }
        }

        private IApi CreateApi()
        {
            var factory = new ContentFactory(services);

            return new Api(GetDb(), factory, new ContentServiceFactory(factory), storage, null, processor);
        }
    }
}
