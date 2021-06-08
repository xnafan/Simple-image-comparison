using SimpleImageComparisonClassLibrary.ExtensionMethods;
using System.Drawing;
using System;
using System.Linq;

namespace SimpleImageComparisonClassLibrary
{
    /// <summary>
    /// ImageInfo is a class which can analyze and store information about an bitmap for easy comparison of images.
    /// The information is stored as a 16x16 px grayscaled version of the image.
    /// ImageInfo can also store the path of the source image, if the ImageInfo was created from an image file.
    /// </summary>
    public class ImageInfo : IEquatable<ImageInfo>
    {
        
        #region Variables and properties
        /// <summary>
        /// The path to the image file the ImageInfo was constructred from.
        /// This value is NULL if the ImageInfo was constructed from an Image object.
        /// </summary>
        public string ImagePath { get; private set; }

        private byte[,] _grayValues;
        public byte[,] GrayValues { 
            get => _grayValues; 
            private set { _grayValues = value; CalculateAverageBrightness(); } 
        }
        /// <summary>
        /// The average brightness in the image
        /// </summary>
        public int AverageBrightness { get; private set; }
        #endregion


        #region Constructors
        public ImageInfo(string imagePath)
        {
            ImagePath = imagePath;
            try
            {
                using (Image image = Image.FromFile(imagePath))
                {
                    GrayValues = image.GetGrayScaleValues();
                }
            }
            catch (System.Exception ex)
            {
                throw new System.Exception($"An error occurred loading the file '{ImagePath}'. The error is '{ex.Message}'.");
            }
        }

        public ImageInfo(Image image)
        {
            GrayValues = image.GetGrayScaleValues();
        }
        #endregion


        #region Methods
        private void CalculateAverageBrightness()
        {
            AverageBrightness = (int)GrayValues.GetAverage();
        }

        public override string ToString()
        {
            return $"ImageInfo {{ Path:'{ImagePath ?? "none"}', AverageBrightness:{AverageBrightness}}}";
        }
        #endregion


        #region IEquatable implementation

        public bool Equals(ImageInfo other)
        {
            if (other == null) throw new ArgumentException("Cannot compare to null!");
            if (other is ImageInfo)
            {
                return GetHashCode() == other.GetHashCode();
            }
            else
            {
                throw new ArgumentException($"Cannot compare ImageInfo with {other.GetType().Name}");
            }
        }

        public override int GetHashCode()
        {
            var values = GrayValues.All();
            int hashCode = values.Count();
            foreach (int value in values)
            {
                hashCode = unchecked(hashCode * 314159 + value);
            }
            return hashCode;
        }
        #endregion

    }
}