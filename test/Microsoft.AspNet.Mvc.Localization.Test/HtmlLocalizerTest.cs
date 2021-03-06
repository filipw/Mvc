﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.AspNet.Testing;
using Microsoft.Framework.Localization;
using Microsoft.Framework.WebEncoders.Testing;
using Moq;
using Xunit;

namespace Microsoft.AspNet.Mvc.Localization.Test
{
    public class HtmlLocalizerTest
    {
        [Fact]
        public void HtmlLocalizer_UseIndexer_ReturnsLocalizedString()
        {
            // Arrange
            var localizedString = new LocalizedString("Hello", "Bonjour");
            var stringLocalizer = new Mock<IStringLocalizer>();
            stringLocalizer.Setup(s => s["Hello"]).Returns(localizedString);

            var htmlLocalizer = new HtmlLocalizer(stringLocalizer.Object, new CommonTestEncoder());

            // Act
            var actualLocalizedString = htmlLocalizer["Hello"];

            // Assert
            Assert.Equal(localizedString, actualLocalizedString);
        }

        [Fact]
        public void HtmlLocalizer_UseIndexerWithArguments_ReturnsLocalizedString()
        {
            // Arrange
            var localizedString = new LocalizedString("Hello", "Bonjour test");

            var stringLocalizer = new Mock<IStringLocalizer>();
            stringLocalizer.Setup(s => s["Hello", "test"]).Returns(localizedString);

            var htmlLocalizer = new HtmlLocalizer(stringLocalizer.Object, new CommonTestEncoder());

            // Act
            var actualLocalizedString = htmlLocalizer["Hello", "test"];

            // Assert
            Assert.Equal(localizedString, actualLocalizedString);
        }

        public static IEnumerable<object[]> HtmlData
        {
            get
            {
                yield return new object[] { "Bonjour {0} {{{{ }}", new object[] { "test" }, "Bonjour HtmlEncode[[test]] {{ }" };
                yield return new object[] { "Bonjour {{0}}", new object[] { "{0}" }, "Bonjour {0}" };
                yield return new object[] { "Bonjour {0:x}", new object[] { 10 }, "Bonjour HtmlEncode[[a]]" };
                yield return new object[] { "Bonjour {0:x}}}", new object[] { 10 }, "Bonjour HtmlEncode[[x}]]" };
                yield return new object[] { "Bonjour {{0:x}}", new object[] { 10 }, "Bonjour {0:x}" };
                yield return new object[] { "{{ Bonjour {{{0:x}}}", new object[] { 10 }, "{ Bonjour {HtmlEncode[[x}]]" };
                yield return new object[] { "}} Bonjour {{{0:x}}}", new object[] { 10 }, "} Bonjour {HtmlEncode[[x}]]" };
                yield return new object[] { "}} Bonjour", new object[] { }, "} Bonjour" };
                yield return new object[] { "{{ {0} }}", new object[] { 10 }, "{ HtmlEncode[[10]] }" };
                yield return new object[] {
                    "Bonjour {{{0:x}}} {1:yyyy}",
                    new object[] { 10, new DateTime(2015, 10, 10) },
                    "Bonjour {HtmlEncode[[x}]] HtmlEncode[[2015]]"
                };
                yield return new object[] {
                    "Bonjour {{{0:x}}} Bienvenue {{1:yyyy}}",
                    new object[] { 10, new DateTime(2015, 10, 10) },
                    "Bonjour {HtmlEncode[[x}]] Bienvenue {1:yyyy}"
                };
                yield return new object[] {
                    "Bonjour {0,6} Bienvenue {{1:yyyy}}",
                    new object[] { 10, new DateTime(2015, 10, 10) },
                    "Bonjour HtmlEncode[[    10]] Bienvenue {1:yyyy}"
                };
                if (!TestPlatformHelper.IsMono)
                {
                    // Mono doesn't deal well with custom format strings, even valid ones
                    yield return new object[] { "{0:{{000}}}", new object[] { 10 }, "HtmlEncode[[{010}]]" };
                    yield return new object[] {
                    "Bonjour {0:'{{characters that should be escaped}}b'###'b'}",
                    new object[] { 10 },
                    "Bonjour HtmlEncode[[{characters that should be escaped}b10b]]"
                    };
                }
            }
        }

        [Theory]
        [MemberData(nameof(HtmlData))]
        public void HtmlLocalizer_HtmlWithArguments_ReturnsLocalizedHtml(
            string format,
            object[] arguments,
            string expectedText)
        {
            // Arrange
            var localizedString = new LocalizedString("Hello", format);

            var stringLocalizer = new Mock<IStringLocalizer>();
            stringLocalizer.Setup(s => s["Hello"]).Returns(localizedString);

            var htmlLocalizer = new HtmlLocalizer(stringLocalizer.Object, new CommonTestEncoder());

            // Act
            var localizedHtmlString = htmlLocalizer.Html("Hello", arguments);

            // Assert
            Assert.NotNull(localizedHtmlString);
            Assert.Equal(expectedText, localizedHtmlString.Value);
        }

        [Theory]
        [InlineData("{")]
        [InlineData("{0")]
        public void HtmlLocalizer_HtmlWithInvalidResourcestring_ThrowsException(string format)
        {
            // Arrange
            var localizedString = new LocalizedString("Hello", format);

            var stringLocalizer = new Mock<IStringLocalizer>();
            stringLocalizer.Setup(s => s["Hello"]).Returns(localizedString);

            var htmlLocalizer = new HtmlLocalizer(stringLocalizer.Object, new CommonTestEncoder());

            // Act
            var exception = Assert.Throws<FormatException>(() => htmlLocalizer.Html("Hello", new object[] { }));

            // Assert
            Assert.NotNull(exception);
            Assert.Equal("Input string was not in a correct format.", exception.Message);
        }
    }

    public class TestClass
    {

    }
}
