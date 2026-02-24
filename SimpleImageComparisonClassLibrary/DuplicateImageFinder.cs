using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleImageComparisonClassLibrary
{
    public static class DuplicateImageFinder
    {
        // Tilføj de extensions vi vil arbejde med
        private static readonly string[] ImageExtensions =
            { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".webp" };

        /// <summary>
        /// Find all duplicate images in a folder, and possibly subfolders
        /// </summary>
        /// <param name="pathOfImageToCompareTo">The image to compare others to</param>
        /// <param name="pathOfFolderToSearch">The folder to look for duplicates in</param>
        /// <param name="checkSubfolders">Whether to look in subfolders too</param>
        /// <param name="maximumDifferenceInPercentage">Max allowed difference percentage</param>
        /// <param name="threshold">Threshold for pixel comparison</param>
        /// <returns>A list of paths to all the duplicates found</returns>
        public static List<string> FindSimilarImages(
            string pathOfImageToCompareTo,
            string pathOfFolderToSearch,
            bool checkSubfolders = true,
            float maximumDifferenceInPercentage = 0,
            int threshold = 3)
        {
            // Hent kun filer med de ønskede billedextensions
            var allFiles = Directory.GetFiles(pathOfFolderToSearch, "*.*",
                                checkSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                            .Where(f => ImageExtensions.Contains(Path.GetExtension(f).ToLower()))
                            .ToList();

            return FindSimilarImages(pathOfImageToCompareTo, allFiles, maximumDifferenceInPercentage, threshold);
        }

        /// <summary>
        /// Find all duplicate images in a given list
        /// </summary>
        /// <param name="imagePaths">The paths to the images to check for duplicates</param>
        /// <returns>A list of paths to all the duplicates found.</returns>
        public static List<string> FindSimilarImages(
            string pathOfImageToCompareTo,
            IEnumerable<string> imagePaths,
            float maximumDifferenceInPercentage,
            int threshold = 3)
        {
            ImageInfo imageToCompareTo = new ImageInfo(pathOfImageToCompareTo);
            var similarImagesFound = new List<string>();

            Parallel.ForEach(imagePaths, imagePath =>
            {
                var percentageDiff = ImageTool.GetPercentageDifference(imageToCompareTo, new ImageInfo(imagePath), (byte)threshold);
                if (percentageDiff <= maximumDifferenceInPercentage && pathOfImageToCompareTo != imagePath)
                {
                    lock (similarImagesFound) // Parallel-safe add
                    {
                        similarImagesFound.Add(imagePath);
                    }
                }
            });
            return similarImagesFound;
        }
    }
}