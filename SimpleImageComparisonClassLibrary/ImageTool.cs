using SimpleImageComparisonClassLibrary.ExtensionMethods;
using System.Drawing;

namespace SimpleImageComparisonClassLibrary
{
    /// <summary>
    /// Tool for finding how much two images differ.
    /// </summary>
    public static class ImageTool
    {
        /// <summary>
        /// Gets the difference between two images as a percentage, 
        /// by converting them to 16x16 grayscale images 
        /// and comparing the brightness of each corresponding pixel.
        /// Only pixels with a difference above the threshold will be counted.
        /// </summary>
        /// <returns>The difference between the two images as a percentage</returns>
        /// <param name="image1Path">The path to the first image</param>
        /// <param name="image2Path">The path to the second image</param>
        /// <param name="threshold">What the difference in brightness must be above to count as a difference. Default is 3 (out of 255).</param>
        /// <returns>The difference between the two images as a percentage</returns>
        public static float GetPercentageDifference(string image1Path, string image2Path, int threshold = 3)
        {
            ImageInfo imageInfo1 = new ImageInfo(image1Path);
            ImageInfo imageInfo2 = new ImageInfo(image2Path);

            return imageInfo1.GetPercentageDifference(imageInfo2, threshold);
        }

        /// <summary>
        /// Gets the difference between two images as a percentage, 
        /// by converting them to 16x16 grayscale images 
        /// and comparing the brightness of each corresponding pixel.
        /// Only pixels with a difference above the threshold will be counted.
        /// </summary>
        /// <returns>The difference between the two images as a percentage</returns>
        /// <param name="image1">The first image</param>
        /// <param name="image2">The second image</param>
        /// <param name="threshold">What the difference in brightness must be above to count as a difference. Default is 3 (out of 255).</param>
        /// <returns>The difference between the two images as a percentage</returns>
        public static float GetPercentageDifference(Image image1, Image image2, int threshold = 3)
        {
            return image1.ToImageInfo().GetPercentageDifference(image2.ToImageInfo(), threshold);
        }


        /// <summary>
        /// Gets the difference between two ImageInfo objects as a percentage, 
        /// by comparing their grayscale "pixel" values.
        /// Only pixels with a difference above the threshold will be counted.
        /// </summary>
        /// <returns>The difference between the two ImageInfos as a percentage</returns>
        /// <param name="imageInfo1">The first ImageInfo</param>
        /// <param name="imageInfo2">The second ImageInfo</param>
        /// <param name="threshold">What the difference in brightness must be above to count as a difference. Default is 3 (out of 255).</param>
        /// <returns>The difference between the two ImageInfos as a percentage</returns>
        public static float GetPercentageDifference(ImageInfo imageInfo1, ImageInfo imageInfo2, int threshold = 3)
        {
            return imageInfo1.GetPercentageDifference(imageInfo2, threshold);
        }
    }
}