using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Windows.Globalization;
using Windows.Media.Ocr;
using ZXing;
using ZXing.Common;
using WindowTextExtractor.Extensions;
using BitmapDecoder = Windows.Graphics.Imaging.BitmapDecoder;

namespace WindowTextExtractor.Utils
{
    static class ImageUtils
    {
        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using var graphics = Graphics.FromImage(destImage);
            graphics.CompositingMode = CompositingMode.SourceCopy;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            using var wrapMode = new ImageAttributes();
            wrapMode.SetWrapMode(WrapMode.TileFlipXY);
            graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);

            return destImage;
        }

        public static string ExtractBarcodes(Bitmap bitmap)
        {
            var barcodeReader = new BarcodeReader
            {
                AutoRotate = true,
                Options = new DecodingOptions { TryHarder = true }
            };

            var result = barcodeReader.Decode(bitmap);
            return result?.Text ?? string.Empty;
        }

        public static async Task<string> ExtractTextAsync(Bitmap image, string language = null)
        {
            var selectedLanguage = string.IsNullOrEmpty(language) ? GetCurrentLanguage() : new Language(language);
            if (selectedLanguage == null || !OcrEngine.IsLanguageSupported(selectedLanguage))
            {
                return string.Empty;
            }

            var isSpaceJoiningOCRLang = true;

            if (selectedLanguage.LanguageTag.StartsWith("zh-", StringComparison.InvariantCultureIgnoreCase) == true)
            {
                isSpaceJoiningOCRLang = false;
            }
            else if (selectedLanguage.LanguageTag.Equals("ja", StringComparison.InvariantCultureIgnoreCase) == true)
            {
                isSpaceJoiningOCRLang = false;
            }

            var scale = await GetIdealScaleFactorAsync(image, selectedLanguage).ConfigureAwait(false);
            var scaledBitmap = ScaleBitmapUniform(image, scale);
            var text = new StringBuilder();

            using var memory = new MemoryStream();
            scaledBitmap.Save(memory, ImageFormat.Bmp);
            memory.Position = 0;
            var randomAccessStream = memory.AsRandomAccessStream();
            var bmpDecoder = await BitmapDecoder.CreateAsync(randomAccessStream).AsTask().ConfigureAwait(false);
         
            using var softwareBmp = await bmpDecoder.GetSoftwareBitmapAsync().AsTask().ConfigureAwait(false);
            await memory.FlushAsync();

            var ocrEngine = OcrEngine.TryCreateFromLanguage(selectedLanguage);
            var ocrResult = await ocrEngine.RecognizeAsync(softwareBmp).AsTask().ConfigureAwait(false);

            var heightsList = new List<double>();

            foreach (var ocrLine in ocrResult.Lines)
            {
                GetTextFromOcrLine(ocrLine, isSpaceJoiningOCRLang, text);
            }

            var lang = XmlLanguage.GetLanguage(selectedLanguage.LanguageTag);
            var culture = lang.GetEquivalentCulture();
            if (culture.TextInfo.IsRightToLeft)
            {
                ReverseWordsForRightToLeft(text);
            }

            return text.ToString();
        }

        private async static Task<double> GetIdealScaleFactorAsync(Bitmap image, Language selectedLanguage)
        {
            using var memory = new MemoryStream();
            var heightsList = new List<double>();
            double scaleFactor = 1.5;
            image.Save(memory, ImageFormat.Bmp);
            memory.Position = 0;
            var bmpDecoder = await BitmapDecoder.CreateAsync(memory.AsRandomAccessStream()).AsTask().ConfigureAwait(false);
         
            using var softwareBmp = await bmpDecoder.GetSoftwareBitmapAsync().AsTask().ConfigureAwait(false);
            if (selectedLanguage is null)
            {
                selectedLanguage = GetCurrentLanguage();
            }

            memory.Flush();
            var ocrEngine = OcrEngine.TryCreateFromLanguage(selectedLanguage);
            var ocrResult = await ocrEngine.RecognizeAsync(softwareBmp);

            foreach (var ocrLine in ocrResult.Lines)
            {
                foreach (var ocrWord in ocrLine.Words)
                {
                    heightsList.Add(ocrWord.BoundingRect.Height);
                }
            }

            double lineHeight = 10;

            if (heightsList.Count > 0)
            {
                lineHeight = heightsList.Average();
            }

            // Ideal Line Height is 40px
            const double idealLineHeight = 40.0;

            scaleFactor = idealLineHeight / lineHeight;

            if (image.Width * scaleFactor > OcrEngine.MaxImageDimension || image.Height * scaleFactor > OcrEngine.MaxImageDimension)
            {
                var largerDim = Math.Max(image.Width, image.Height);
                // find the largest possible scale factor, because the ideal scale factor is too high

                scaleFactor = OcrEngine.MaxImageDimension / largerDim;
            }

            return scaleFactor;
        }

        private static void ReverseWordsForRightToLeft(StringBuilder text)
        {
            var textListLines = text.ToString().Split(new char[] { '\n', '\r' });
            var regexSpaceJoiningWord = new Regex(@"(^[\p{L}-[\p{Lo}]]|\p{Nd}$)|.{2,}");

            text.Clear();
            foreach (string textLine in textListLines)
            {
                var firstWord = true;
                var isPrevWordSpaceJoining = false;
                var wordArray = textLine.Split().ToList();
                wordArray.Reverse();

                foreach (string wordText in wordArray)
                {
                    var isThisWordSpaceJoining = regexSpaceJoiningWord.IsMatch(wordText);

                    if (firstWord || (!isThisWordSpaceJoining && !isPrevWordSpaceJoining))
                    {
                        text.Append(wordText);
                    }
                    else
                    {
                        text.Append(' ').Append(wordText);
                    }

                    firstWord = false;
                    isPrevWordSpaceJoining = isThisWordSpaceJoining;
                }

                if (textLine.Length > 0)
                {
                    text.Append(Environment.NewLine);
                }
            }
        }

        private static Language GetCurrentLanguage()
        {
            // use currently selected Language
            var inputLang = InputLanguage.CurrentInputLanguage.Culture.Name;
            var selectedLanguage = new Language(inputLang);

            var possibleLanguages = OcrEngine.AvailableRecognizerLanguages.ToList();

            if (possibleLanguages.Count() < 1)
            {
                return null;
            }

            if (possibleLanguages.All(l => l.LanguageTag != selectedLanguage.LanguageTag))
            {
                var similarLanguages = possibleLanguages.Where(la => la.NativeName == selectedLanguage.NativeName).ToList();

                if (similarLanguages != null)
                {
                    selectedLanguage = similarLanguages.Count() > 0 ? similarLanguages.FirstOrDefault() : possibleLanguages.FirstOrDefault();
                }
            }

            return selectedLanguage;
        }

        private static Bitmap ScaleBitmapUniform(Bitmap passedBitmap, double scale)
        {
            using var memory = new MemoryStream();
            passedBitmap.Save(memory, ImageFormat.Bmp);
            memory.Position = 0;
            var bitmapimage = new BitmapImage();
            bitmapimage.BeginInit();
            bitmapimage.StreamSource = memory;
            bitmapimage.CacheOption = BitmapCacheOption.None;
            bitmapimage.EndInit();
            bitmapimage.Freeze();

            memory.Flush();

            var tbmpImg = new TransformedBitmap();
            tbmpImg.BeginInit();
            tbmpImg.Source = bitmapimage;
            tbmpImg.Transform = new ScaleTransform(scale, scale);
            tbmpImg.EndInit();
            tbmpImg.Freeze();
            return BitmapSourceToBitmap(tbmpImg);
        }

        private static Bitmap BitmapSourceToBitmap(BitmapSource source)
        {
            var bmp = new Bitmap(source.PixelWidth, source.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            var data = bmp.LockBits(new Rectangle(System.Drawing.Point.Empty, bmp.Size), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            source.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
            bmp.UnlockBits(data);
            return bmp;
        }

        private static void GetTextFromOcrLine(OcrLine ocrLine, bool isSpaceJoiningOCRLang, StringBuilder text)
        {
            // (when OCR language is zh or ja)
            // matches words in a space-joining language, which contains:
            // - one letter that is not in "other letters" (CJK characters are "other letters")
            // - one number digit
            // - any words longer than one character
            // Chinese and Japanese characters are single-character words
            // when a word is one punctuation/symbol, join it without spaces

            if (isSpaceJoiningOCRLang == true)
            {
                text.AppendLine(ocrLine.Text);
            }
            else
            {
                var isFirstWord = true;
                var isPrevWordSpaceJoining = false;
                var regexSpaceJoiningWord = new Regex(@"(^[\p{L}-[\p{Lo}]]|\p{Nd}$)|.{2,}");

                foreach (var ocrWord in ocrLine.Words)
                {
                    var wordString = ocrWord.Text.TryFixEveryWordLetterNumberErrors();
                    var isThisWordSpaceJoining = regexSpaceJoiningWord.IsMatch(wordString);

                    if (isFirstWord || (!isThisWordSpaceJoining && !isPrevWordSpaceJoining))
                    {
                        text.Append(wordString);
                    }
                    else
                    {
                        text.Append(' ').Append(wordString);
                    }

                    isFirstWord = false;
                    isPrevWordSpaceJoining = isThisWordSpaceJoining;
                }
            }
        }
    }
}