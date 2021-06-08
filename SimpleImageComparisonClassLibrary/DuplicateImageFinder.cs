using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleImageComparisonClassLibrary
{
    public static class DuplicateImageFinder
    {
        /// <summary>
        /// Find all duplicate images in a folder, and possibly subfolders
        /// IMPORTANT: this method assumes that all files in the folder(s) are images!
        /// </summary>
        /// <param name="folderPath">The folder to look for duplicates in</param>
        /// <param name="checkSubfolders">Whether to look in subfolders too</param>
        /// <returns>A list of paths to all the duplicates found</returns>
        public static List<string> FindSimilarImages(string pathOfImageToCompareTo, string pathOfFolderToSearch, bool checkSubfolders=true, float maximumDifferenceInPercentage=0, int threshold = 3)
        {
            var imagePaths = Directory.GetFiles(pathOfFolderToSearch, "*.*", checkSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).ToList();
            return FindSimilarImages(pathOfImageToCompareTo, imagePaths, maximumDifferenceInPercentage, threshold);
        }

        /// <summary>
        /// Find all duplicate images from in list
        /// </summary>
        /// <param name="imagePaths">The paths to the images to check for duplicates</param>
        /// <returns>A list of paths to all the duplicates found.</returns>
        public static List<string> FindSimilarImages(string pathOfImageToCompareTo, IEnumerable<string> imagePaths, float maximumDifferenceInPercentage, int threshold = 3)
        {
            ImageInfo imageToCompareTo = new ImageInfo(pathOfImageToCompareTo);
            var similarImagesFound = new List<string>();

            Parallel.ForEach(imagePaths, imagePath => 
            {
                var percentageDiff = ImageTool.GetPercentageDifference(imageToCompareTo, new ImageInfo(imagePath), (byte)threshold);
                if (percentageDiff <= maximumDifferenceInPercentage && pathOfImageToCompareTo != imagePath)
                {
                    similarImagesFound.Add(imagePath);
                };
            });
            return similarImagesFound;
        }
    }
}