# Simple-image-comparison
This project was made to create a very simple way to compare images and find duplicates while allowing for an adjustable amount of difference between images.
It is based on my 2012 project which was published in an article at The Code Project : https://www.codeproject.com/Articles/374386/Simple-image-comparison-in-NET - but has been simplified, parallized and converted to .NET Core.

Sample usage:

FIND DIFFERENCE BETWEEN TWO IMAGES ///////////

//get the difference between two images as a percentage
float difference = ImageTool.GetPercentageDifference(@"c:\image1.png", @"c:\image2.jpg");

//get the difference between two images as a percentage, ignoring differences beneath 10 levels of brightness (of 255)
int thresholdLevel = 10;
float difference = ImageTool.GetPercentageDifference(@"c:\image1.png", @"c:\image2.jpg", thresholdLevel);


FIND DUPLICATE IMAGES ///////////

//find duplicates of an image
List<string> duplicates = DuplicateImageFinder.FindSimilarImages(@"C:\myImage.png", @"C:\randomImagefolder\");

//find duplicates of an image in a folder, ignoring subfulders 
//  and allowing for up to 10% of the images to differ from the one we're searching with
bool lookInSubfolders = false;
float maximumDifferenceInPercentage = .1f; //max 10% of the area may differ
List<string> duplicates = DuplicateImageFinder.FindSimilarImages(@"C:\myImage.png", @"C:\randomImagefolder\", lookInSubfolders, maximumDifferenceInPercentage);



- Jakob Farian Krarup
