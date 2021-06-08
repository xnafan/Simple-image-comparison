using NUnit.Framework;
using SimpleImageComparisonClassLibrary;
using System;
using System.Diagnostics;
using System.IO;

namespace TestingImageComparison
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestFindSimilarImages()
        {
            //arrange
            var testImageDirectory = Path.Combine( Directory.GetCurrentDirectory(), "TestImages");
            var whiteImagePath = Path.Combine(testImageDirectory, "White.png");

            //act
            var similarToWhite =  DuplicateImageFinder.FindSimilarImages(whiteImagePath, testImageDirectory, true, 0, 20);
            var halfOfImageDifferent = DuplicateImageFinder.FindSimilarImages(whiteImagePath, testImageDirectory, true, .50f, 0);

            //assert
            Assert.AreEqual(1, similarToWhite.Count);
            Assert.AreEqual(1, halfOfImageDifferent.Count);
        }
    }
}