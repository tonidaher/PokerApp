using PokerApp.Domain.Common;
using PokerApp.Domain.DataModel;
using PokerAppML.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace PokerApp.Domain
{
    public class CardRecognizer : ICardRecognizer
    {
        private IWindowPositionGetter _windowPositioGetter;
        private IScreenshotTaker _screenshotTaker;
        private readonly double[] BoardX = new double[5] { 0.286 , 0.372, 0.46, 0.548, 0.6375};
        private readonly double CardWidthRatio = 0.02722;
        private readonly double CardHeightRatio = 0.04029;
        private readonly double HoleCardsYRatio = 0.66793;
        private readonly double BoardCardsYRatio = 0.36;
        private ConsumeModel _model;
        public CardRecognizer(IWindowPositionGetter windowsPositionGetter,IScreenshotTaker screenshotTaker)
        {
            _windowPositioGetter = windowsPositionGetter;
            _screenshotTaker = screenshotTaker;
            _model = new ConsumeModel();
            
        }
        public Cards RecognizeHoleCards(string tournamentId, string tournamentName)
        {
            Cards result = null;

            if(!GetWindowBitmap(tournamentId, tournamentName,out var windowBitmap)) return result; 
            
            var cardWidth = (int)(windowBitmap.Width * CardWidthRatio);
                var cardHeight = (int)(windowBitmap.Height * CardHeightRatio);
                var x = (windowBitmap.Width * 629) / 1396;

                var x1 = x;
                var x2 = x1 + cardWidth + 1;
            var name = tournamentName + "_" + tournamentId + "card";

            var imageCard1 = SaveCard(windowBitmap, x1, HoleCardsYRatio, cardWidth, cardHeight, name, 1) ;
            var imageCard2 = SaveCard(windowBitmap, x2, HoleCardsYRatio, cardWidth, cardHeight, name, 2);

            var card1Output = _model.Predict(new ModelInput() { ImageSource = imageCard1.ImageName }).Prediction;
            var card2Output = _model.Predict(new ModelInput() { ImageSource = imageCard2.ImageName }).Prediction;

            if( card1Output!=Constants.None && card2Output != Constants.None)
            {
               var suit1 = GetSuit(imageCard1.ImageBitmap);
               var suit2 = GetSuit(imageCard2.ImageBitmap);
                 result= new Cards(card1Output + suit1, card2Output + suit2);
            }
            return result;
        }
        private bool GetWindowBitmap(string tournamentId, string tournamentName, out Bitmap bitmap)
        {
            bitmap = null;
            var windowHandler = _windowPositioGetter.GetWindowHandler(tournamentId, tournamentName);
            if (windowHandler == null) return false;
            bitmap =  _screenshotTaker.WindowScreen(windowHandler);
            return true;
        }

        public Cards RecognizeBoardCards(string tournamentId, string tournamentName)
        {
            Cards result = null;

            if (!GetWindowBitmap(tournamentId, tournamentName, out var windowBitmap)) return result;

            var cardWidth = (int)(windowBitmap.Width * CardWidthRatio);
            var cardHeight = (int)(windowBitmap.Height * CardHeightRatio);
            var windowWidth = windowBitmap.Width;
          
            var name = tournamentName+"_"+tournamentId +"board";
            List<string> flopCards = new List<string>();
            var validCount = 0;
            for (int i = 0; i < 5; i++)
            {
                var imageCard = SaveCard(windowBitmap,(int)(BoardX[i]*windowWidth),BoardCardsYRatio, cardWidth, cardHeight, name, i+1);
                var cardOutput = _model.Predict(new ModelInput() { ImageSource = imageCard.ImageName }).Prediction;
                if(cardOutput == Constants.None) { continue; }
                var suit = GetSuit(imageCard.ImageBitmap);
                flopCards.Add(cardOutput + suit);
                validCount += 1;
            }
            if (validCount > 2)
            {
                result = new Cards(flopCards);
            }
            
            return result;
        }
        private ImageNameBitmap SaveCard(Bitmap src, int x,double yRatio, int cardWidth, int cardHeight, string name, int number)
        {
            Bitmap target1 = new Bitmap(cardWidth, cardHeight);
            var y = (int)(src.Height * yRatio);
            //var y = src.Height - FooterOffset - (CardHeight/2);
            Rectangle cropRect1 = new Rectangle(x, y, cardWidth, cardHeight);

            using (Graphics g = Graphics.FromImage(target1))
            {
                g.DrawImage(src, new Rectangle(0, 0, target1.Width, target1.Height),
                                 cropRect1,
                                 GraphicsUnit.Pixel);
            }
            var imageName = Path.Combine("temp", name + "_" + number + ".png");
            target1.Save(imageName, ImageFormat.Png);
            return new ImageNameBitmap() { ImageName = imageName, ImageBitmap = target1 };
        }
        private string GetSuit(Bitmap bmp)
        {

            int width = bmp.Width;
            int height = bmp.Height;

            Color p;
            int black = 0; // s
            int red = 0; // h
            int blue = 0; // c
            int green = 0; // d
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    p = bmp.GetPixel(x, y);
                    int a = p.A;
                    int r = p.R;
                    int g = p.G;
                    int b = p.B;
                    int avg = (r + g + b) / 3;
                    if ((r > g) && (r > b)) red += 1;
                    else if ((g > r) && (g > b)) green += 1;
                    else if ((b > r) && (b > r)) blue += 1;
                    else if (avg < 128) black += 1;
                  //  avg = avg < 128 ? 0 : 255;     // Converting gray pixels to either pure black or pure white
                  //  bmp.SetPixel(x, y, Color.FromArgb(a, avg, avg, avg));
                }
            }
            var max = Math.Max(Math.Max(Math.Max(black, green), red), blue);
            if (max == green) return "c";
            if (max == red) return "h";
            if (max == blue) return "d";
            return "s";
        }
    }

    internal class ImageNameBitmap
    {
        public string ImageName { get; set; }
        public Bitmap ImageBitmap { get; set; }
    }
}
