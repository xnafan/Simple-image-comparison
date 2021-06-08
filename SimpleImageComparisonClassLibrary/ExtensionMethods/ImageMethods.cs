using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;

//For reference:
//http://www.nbdtech.com/Blog/archive/2008/04/27/Calculating-the-Perceived-Brightness-of-a-Color.aspx


namespace SimpleImageComparisonClassLibrary.ExtensionMethods
{
    public static class ImageMethods
    {

        #region Properties
        //the font to use for the DifferenceImages
        private static readonly Font DefaultFont = new Font("Arial", 8);

        //the brushes to use for the DifferenceImages
        private static Brush[] brushes = new Brush[256];

        //the colormatrix used to grayscale an image
        //http://www.switchonthecode.com/tutorials/csharp-tutorial-convert-a-color-image-to-grayscale
        static readonly ColorMatrix ColorMatrix = new ColorMatrix(new float[][]
        {
            new float[] {.3f, .3f, .3f, 0, 0},
            new float[] {.59f, .59f, .59f, 0, 0},
            new float[] {.11f, .11f, .11f, 0, 0},
            new float[] {0, 0, 0, 1, 0},
            new float[] {0, 0, 0, 0, 1}
        });

        #endregion


        #region Constructor
        static ImageMethods()
        {
            //Create the brushes in varying intensities
            for (int i = 0; i < 256; i++)
            {
                brushes[i] = new SolidBrush(Color.FromArgb(255, i, i / 3, i / 2));
            }
        } 
        #endregion


        #region Difference methods
        /// <summary>
        /// Gets the difference between two images as a percentage
        /// </summary>
        /// <param name="img1">The first image</param>
        /// <param name="img2">The image to compare to</param>
        /// <param name="threshold">How big a difference (out of 255) will be ignored - the default is 3.</param>
        /// <returns>The difference between the two images as a percentage</returns>
        public static float GetPercentageDifference(this Image img1, Image img2, int threshold = 3)
        {
            return GetPercentageDifference(new ImageInfo(img1), new ImageInfo(img2));
        }

        /// <summary>
        /// Gets the difference between two images as a percentage
        /// </summary>
        /// <param name="img1">The first imageinfo</param>
        /// <param name="img2">The imageinfo to compare to</param>
        /// <param name="threshold">How big a difference (out of 255) will be ignored - the default is 3.</param>
        /// <returns>The difference between the two images as a percentage</returns>
        public static float GetPercentageDifference(this ImageInfo img1, ImageInfo img2, int threshold = 3)
        {
            byte[,] differences = img1.GrayValues.GetDifferences(img2.GrayValues);
            int numberOfPixels = differences.GetLength(0) * differences.GetLength(1);
            float diffPixels = differences.All().Count(b => b > threshold);
            return diffPixels / numberOfPixels;
        }


        /// <summary>
        /// Finds the differences between two images and returns them in a doublearray
        /// </summary>
        /// <param name="img1">The first image</param>
        /// <param name="img2">The image to compare with</param>
        /// <returns>the differences between the two images as a doublearray</returns>
        public static byte[,] GetDifferences(this Image img1, Image img2, int arraySize = 16)
        {
            return new ImageInfo(img1).GetDifferences(new ImageInfo(img2));
        }


        /// <summary>
        /// Finds the differences between two images and returns them in a doublearray
        /// </summary>
        /// <param name="imageInfo1">The first ImageInfo</param>
        /// <param name="imageInfo2">The ImageInfo to compare with</param>
        /// <returns>the differences between the two imageinfos as a doublearray</returns>
        public static byte[,] GetDifferences(this ImageInfo imageInfo1, ImageInfo imageInfo2)
        {
            return imageInfo1.GrayValues.GetDifferences(imageInfo2.GrayValues);
        }

        #endregion


        #region Visualization methods

        /// <summary>
        /// Gets an image which displays the differences between two images
        /// </summary>
        /// <param name="img1">The first image</param>
        /// <param name="img2">The image to compare with</param>
        /// <param name="adjustColorSchemeToMaxDifferenceFound">Whether to adjust the color indicating maximum difference (usually 255) to the maximum difference found in this case.
        /// E.g. if the maximum difference found is 12, then a true value in adjustColorSchemeToMaxDifferenceFound would result in 0 being black, 6 being dark pink, and 12 being bright pink.
        /// A false value would still have differences of 255 as bright pink resulting in the 12 difference still being very dark.</param>
        /// <param name="percentages">Whether to write percentages in each of the 255 squares (true) or the absolute value (false)</param>
        /// <returns>an image which displays the differences between two images</returns>
        public static Bitmap GetDifferenceImage(this Image img1, Image img2, bool adjustColorSchemeToMaxDifferenceFound = false, bool absoluteText = false)
        {
            //create a 16x16 tiles image with information about how much the two images differ
            int cellsize = 16;  //each tile is 16 pixels wide and high
            Bitmap bmp = new Bitmap(16 * cellsize + 1, 16 * cellsize + 1); //16 blocks * 16 pixels + a borderpixel at left/bottom

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.FillRectangle(Brushes.Black, 0, 0, bmp.Width, bmp.Height);
                byte[,] differences = img1.GetDifferences(img2);
                byte maxDifference = 255;

                //if wanted - adjust the color scheme, by finding the new maximum difference
                if (adjustColorSchemeToMaxDifferenceFound)
                {
                    maxDifference = 0;
                    foreach (byte b in differences)
                    {
                        if (b > maxDifference)
                        {
                            maxDifference = b;
                        }
                    }

                    if (maxDifference == 0)
                    {
                        maxDifference = 1;
                    }
                }

                DrawDifferencesToBitmap(absoluteText, cellsize, g, differences, maxDifference);
            }
            return bmp;
        }

        private static void DrawDifferencesToBitmap(bool absoluteText, int cellsize, Graphics g, byte[,] differences, byte maxDifference)
        {
            for (int y = 0; y < differences.GetLength(1); y++)
            {
                for (int x = 0; x < differences.GetLength(0); x++)
                {
                    byte cellValue = differences[x, y];
                    string cellText;

                    if (absoluteText)
                    {
                        cellText = cellValue.ToString();
                    }
                    else
                    {
                        cellText = string.Format("{0}%", (int)cellValue);
                    }

                    float percentageDifference = (float)differences[x, y] / maxDifference;
                    int colorIndex = (int)(255 * percentageDifference);

                    g.FillRectangle(brushes[colorIndex], x * cellsize, y * cellsize, cellsize, cellsize);
                    g.DrawRectangle(Pens.Blue, x * cellsize, y * cellsize, cellsize, cellsize);
                    SizeF size = g.MeasureString(cellText, DefaultFont);
                    g.DrawString(cellText, DefaultFont, Brushes.Black, x * cellsize + cellsize / 2 - size.Width / 2 + 1, y * cellsize + cellsize / 2 - size.Height / 2 + 1);
                    g.DrawString(cellText, DefaultFont, Brushes.White, x * cellsize + cellsize / 2 - size.Width / 2, y * cellsize + cellsize / 2 - size.Height / 2);
                }
            }
        }


        #endregion


        /// <summary>
        /// Gets the lightness of the image in 256 sections (16x16)
        /// </summary>
        /// <param name="img">The image to get the lightness for</param>
        /// <returns>A doublearray (16x16) containing the lightness of the 256 sections</returns>
        public static byte[,] GetGrayScaleValues(this Image img, int arraySize = 16)
        {
            using (Bitmap thisOne = (Bitmap)img.GetResizedVersion(arraySize, arraySize).GetGrayScaleVersion())
            {
                byte[,] grayScale = new byte[arraySize, arraySize];

                for (int y = 0; y < arraySize; y++)
                {
                    for (int x = 0; x < arraySize; x++)
                    {
                        grayScale[x, y] = (byte)Math.Abs(thisOne.GetPixel(x, y).R);
                    }
                }
                return grayScale;
            }
        }

        #region Image conversion methods

        /// <summary>
        /// Gets a grayscaled version of an image
        /// </summary>
        /// <param name="original">The image to grayscale</param>
        /// <returns>A grayscale version of the image</returns>
        public static Image GetGrayScaleVersion(this Image original)
        {
            //http://www.switchonthecode.com/tutorials/csharp-tutorial-convert-a-color-image-to-grayscale
            //create a blank bitmap the same size as original
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            //get a graphics object from the new image
            using (Graphics g = Graphics.FromImage(newBitmap))
            {
                //create some image attributes
                ImageAttributes attributes = new ImageAttributes();

                //set the color matrix attribute
                attributes.SetColorMatrix(ColorMatrix);

                //draw the original image on the new image
                //using the grayscale color matrix
                g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
                   0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
            }
            return newBitmap;

        }

        /// <summary>
        /// Gets a resized copy of an image
        /// </summary>
        /// <param name="originalImage">The image to resize</param>
        /// <param name="newWidth">The new width in pixels</param>
        /// <param name="newHeight">The new height in pixels</param>
        /// <returns>A resized version of the original image</returns>
        public static Image GetResizedVersion(this Image originalImage, int newWidth, int newHeight, bool highSpeed = true)
        {
            Image smallVersion = new Bitmap(newWidth, newHeight);
            using (Graphics g = Graphics.FromImage(smallVersion))
            {
                if (highSpeed)
                {
                    g.SmoothingMode = SmoothingMode.Default;
                    g.InterpolationMode = InterpolationMode.Low;
                    g.PixelOffsetMode = PixelOffsetMode.HighSpeed;
                }
                else
                {
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                }

                g.DrawImage(originalImage, 0, 0, newWidth, newHeight);
            }

            return smallVersion;
        }

        #endregion


        #region Histogram methods
        /// <summary>
        /// Gets a bitmap with the RGB histograms of a bitmap
        /// </summary>
        /// <param name="bitmap">The bitmap to get the histogram for</param>
        /// <returns>A bitmap with the histogram for R, G and B values</returns>
        public static Bitmap GetRgbHistogramBitmap(this Bitmap bitmap)
        {
            return GetRgbHistogram(bitmap).Visualize();
        }

        /// <summary>
        /// Get a histogram for a bitmap
        /// </summary>
        /// <param name="bitmap">The bitmap to get the histogram for</param>
        /// <returns>A histogram for the bitmap</returns>
        public static Histogram GetRgbHistogram(this Bitmap bitmap)
        {
            return Histogram.FromBitmap(bitmap);
        }
        #endregion


        #region ImageInfo methods

        public static ImageInfo ToImageInfo(this Image image) => new ImageInfo(image);

        #endregion

    }
}