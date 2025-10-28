using Algorithms.Sections;
using Algorithms.Tools;
using Algorithms.Utilities;
using Emgu.CV;
using Emgu.CV.Structure;
using Framework.View;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using static Framework.Converters.ImageConverter;
using static Framework.Utilities.DataProvider;
using static Framework.Utilities.DrawingHelper;
using static Framework.Utilities.FileHelper;

namespace Framework.ViewModel
{
    public class MenuCommands : BaseVM
    {
        private readonly MainVM _mainVM;

        public MenuCommands(MainVM mainVM)
        {
            _mainVM = mainVM;
        }

        private ImageSource InitialImage
        {
            get => _mainVM.InitialImage;
            set => _mainVM.InitialImage = value;
        }

        private ImageSource ProcessedImage
        {
            get => _mainVM.ProcessedImage;
            set => _mainVM.ProcessedImage = value;
        }

        private double ScaleValue
        {
            get => _mainVM.ScaleValue;
            set => _mainVM.ScaleValue = value;
        }

        #region File

        #region Load grayscale image
        private RelayCommand _loadGrayscaleImageCommand;
        public RelayCommand LoadGrayscaleImageCommand
        {
            get
            {
                if (_loadGrayscaleImageCommand == null)
                    _loadGrayscaleImageCommand = new RelayCommand(LoadGrayscaleImage);
                return _loadGrayscaleImageCommand;
            }
        }

        private void LoadGrayscaleImage(object parameter)
        {
            Clear(parameter);

            string fileName = LoadFileDialog("Select a grayscale picture");
            if (fileName != null)
            {
                GrayInitialImage = new Image<Gray, byte>(fileName);
                InitialImage = Convert(GrayInitialImage);
            }
        }
        #endregion

        #region Load color image
        private ICommand _loadColorImageCommand;
        public ICommand LoadColorImageCommand
        {
            get
            {
                if (_loadColorImageCommand == null)
                    _loadColorImageCommand = new RelayCommand(LoadColorImage);
                return _loadColorImageCommand;
            }
        }

        private void LoadColorImage(object parameter)
        {
            Clear(parameter);

            string fileName = LoadFileDialog("Select a color picture");
            if (fileName != null)
            {
                ColorInitialImage = new Image<Bgr, byte>(fileName);
                InitialImage = Convert(ColorInitialImage);
            }
        }
        #endregion

        #region Save processed image
        private ICommand _saveProcessedImageCommand;
        public ICommand SaveProcessedImageCommand
        {
            get
            {
                if (_saveProcessedImageCommand == null)
                    _saveProcessedImageCommand = new RelayCommand(SaveProcessedImage);
                return _saveProcessedImageCommand;
            }
        }

        private void SaveProcessedImage(object parameter)
        {
            if (GrayProcessedImage == null && ColorProcessedImage == null)
            {
                MessageBox.Show("If you want to save your processed image, " +
                    "please load and process an image first!");
                return;
            }

            string imagePath = SaveFileDialog("image.jpg");
            if (imagePath != null)
            {
                GrayProcessedImage?.Bitmap.Save(imagePath, GetJpegCodec("image/jpeg"), GetEncoderParameter(Encoder.Quality, 100));
                ColorProcessedImage?.Bitmap.Save(imagePath, GetJpegCodec("image/jpeg"), GetEncoderParameter(Encoder.Quality, 100));
                Process.Start(imagePath);
            }
        }
        #endregion

        #region Exit
        private ICommand _exitCommand;
        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                    _exitCommand = new RelayCommand(Exit);
                return _exitCommand;
            }
        }

        private void Exit(object parameter)
        {
            Application.Current.Shutdown();
        }
        #endregion

        #endregion

        #region Edit

        #region Remove drawn shapes from initial canvas
        private ICommand _removeInitialDrawnShapesCommand;
        public ICommand RemoveInitialDrawnShapesCommand
        {
            get
            {
                if (_removeInitialDrawnShapesCommand == null)
                    _removeInitialDrawnShapesCommand = new RelayCommand(RemoveInitialDrawnShapes);
                return _removeInitialDrawnShapesCommand;
            }
        }

        private void RemoveInitialDrawnShapes(object parameter)
        {
            RemoveUiElements(parameter as Canvas);
        }
        #endregion

        #region Remove drawn shapes from processed canvas
        private ICommand _removeProcessedDrawnShapesCommand;
        public ICommand RemoveProcessedDrawnShapesCommand
        {
            get
            {
                if (_removeProcessedDrawnShapesCommand == null)
                    _removeProcessedDrawnShapesCommand = new RelayCommand(RemoveProcessedDrawnShapes);
                return _removeProcessedDrawnShapesCommand;
            }
        }

        private void RemoveProcessedDrawnShapes(object parameter)
        {
            RemoveUiElements(parameter as Canvas);
        }
        #endregion

        #region Remove drawn shapes from both canvases
        private ICommand _removeDrawnShapesCommand;
        public ICommand RemoveDrawnShapesCommand
        {
            get
            {
                if (_removeDrawnShapesCommand == null)
                    _removeDrawnShapesCommand = new RelayCommand(RemoveDrawnShapes);
                return _removeDrawnShapesCommand;
            }
        }

        private void RemoveDrawnShapes(object parameter)
        {
            var canvases = (object[])parameter;
            RemoveUiElements(canvases[0] as Canvas);
            RemoveUiElements(canvases[1] as Canvas);
        }
        #endregion

        #region Clear initial canvas
        private ICommand _clearInitialCanvasCommand;
        public ICommand ClearInitialCanvasCommand
        {
            get
            {
                if (_clearInitialCanvasCommand == null)
                    _clearInitialCanvasCommand = new RelayCommand(ClearInitialCanvas);
                return _clearInitialCanvasCommand;
            }
        }

        private void ClearInitialCanvas(object parameter)
        {
            RemoveUiElements(parameter as Canvas);

            GrayInitialImage = null;
            ColorInitialImage = null;
            InitialImage = null;
        }
        #endregion

        #region Clear processed canvas
        private ICommand _clearProcessedCanvasCommand;
        public ICommand ClearProcessedCanvasCommand
        {
            get
            {
                if (_clearProcessedCanvasCommand == null)
                    _clearProcessedCanvasCommand = new RelayCommand(ClearProcessedCanvas);
                return _clearProcessedCanvasCommand;
            }
        }

        private void ClearProcessedCanvas(object parameter)
        {
            RemoveUiElements(parameter as Canvas);

            GrayProcessedImage = null;
            ColorProcessedImage = null;
            ProcessedImage = null;
        }
        #endregion

        #region Closing all open windows and clear both canvases
        private ICommand _clearCommand;
        public ICommand ClearCommand
        {
            get
            {
                if (_clearCommand == null)
                    _clearCommand = new RelayCommand(Clear);
                return _clearCommand;
            }
        }

        private void Clear(object parameter)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window != Application.Current.MainWindow)
                {
                    window.Close();
                }
            }

            ScaleValue = 1;

            var canvases = (object[])parameter;
            ClearInitialCanvas(canvases[0] as Canvas);
            ClearProcessedCanvas(canvases[1] as Canvas);
        }
        #endregion

        #endregion

        #region Tools

        #region Magnifier
        private ICommand _magnifierCommand;
        public ICommand MagnifierCommand
        {
            get
            {
                if (_magnifierCommand == null)
                    _magnifierCommand = new RelayCommand(Magnifier);
                return _magnifierCommand;
            }
        }

        private void Magnifier(object parameter)
        {
            if (MagnifierOn == true) return;
            if (MouseClickCollection.Count == 0)
            {
                MessageBox.Show("Please select an area first!");
                return;
            }

            MagnifierWindow magnifierWindow = new MagnifierWindow();
            magnifierWindow.Show();
        }
        #endregion

        #region Crop

        private ICommand _cropCommand;
        public ICommand CropCommand
        {
            get
            {
                if (_cropCommand == null)
                    _cropCommand = new RelayCommand(Crop);
                return _cropCommand;
            }
        }

        private void Crop(object parameter)
        {
            Canvas initialCanvas = parameter as Canvas;

            RemoveUiElements(initialCanvas);

            int mouseClicksCount = MouseClickCollection.Count;

            if (mouseClicksCount < 2)
            {
                MessageBox.Show("Please select an area first!");
                return;
            }

            Point firstMouseClick = MouseClickCollection[mouseClicksCount - 2];
            Point lastMouseClick = MouseClickCollection[mouseClicksCount - 1];

            DrawRectangle(initialCanvas, firstMouseClick, lastMouseClick, 3, Brushes.Red, ScaleValue);


            if (GrayInitialImage != null)
            {
                double mean = ComputeMean(GrayInitialImage);
                double mse = ComputeMSE(GrayInitialImage, mean);

                GrayProcessedImage = ProcessCrop(GrayInitialImage, firstMouseClick, lastMouseClick);
                ProcessedImage = Convert(GrayProcessedImage);

                MessageBox.Show($"Mean: {mean}\nMean Squared Error: {mse}");
            }
            if (ColorInitialImage != null)
            {
                // TODO: implement mean and mse for color images
                // double mean = ComputeMean(ColorInitialImage);
                // double mse = ComputeMSE(ColorInitialImage, mean);

                double mean = 0;
                double mse = 0;

                ColorProcessedImage = ProcessCrop(ColorInitialImage, firstMouseClick, lastMouseClick);
                ProcessedImage = Convert(ColorProcessedImage);

                MessageBox.Show($"Mean: {mean}\nMean Squared Error: {mse}");
            }
        }

        private Image<Gray, byte> ProcessCrop(Image<Gray, byte> initialImage, Point firstMouseClick, Point lastMouseClick)
        {
            int width = (int)(lastMouseClick.X - firstMouseClick.X + 1);
            int height = (int)(lastMouseClick.Y - firstMouseClick.Y + 1);
            Image<Gray, byte> croppedImage = new Image<Gray, byte>(width, height);
            for (int i = 0; i < height; ++i)
            {
                for (int j = 0; j < width; ++j)
                {
                    croppedImage.Data[i, j, 0] = initialImage.Data[(int)firstMouseClick.Y + i, (int)firstMouseClick.X + j, 0];
                }
            }
            return croppedImage;
        }

        private Image<Bgr, byte> ProcessCrop(Image<Bgr, byte> initialImage, Point firstMouseClick, Point lastMouseClick)
        {
            int width = (int)(lastMouseClick.X - firstMouseClick.X + 1);
            int height = (int)(lastMouseClick.Y - firstMouseClick.Y + 1);
            Image<Bgr, byte> croppedImage = new Image<Bgr, byte>(width, height);
            for (int i = 0; i < height; ++i)
            {
                for (int j = 0; j < width; ++j)
                {
                    croppedImage.Data[i, j, 0] = initialImage.Data[(int)firstMouseClick.Y + i, (int)firstMouseClick.X + j, 0];
                    croppedImage.Data[i, j, 1] = initialImage.Data[(int)firstMouseClick.Y + i, (int)firstMouseClick.X + j, 1];
                    croppedImage.Data[i, j, 2] = initialImage.Data[(int)firstMouseClick.Y + i, (int)firstMouseClick.X + j, 2];
                }
            }
            return croppedImage;
        }


        private double ComputeMean(Image<Gray, byte> initialImage)
        {
            double mean = 0;
            for (int i = 0; i < initialImage.Height; ++i)
            {
                for (int j = 0; j < initialImage.Width; ++j)
                {
                    //if (i >= 0 && j >= 0 && i < InitialImage.Height && j < InitialImage.Width)
                    mean += (int)initialImage.Data[i, j, 0];
                }
            }

            mean /= (double)(initialImage.Height * initialImage.Width);

            return mean;
        }


        private double ComputeMSE(Image<Gray, byte> initialImage, double mean)
        {
            double mse = 0;
            for (int i = 0; i < initialImage.Height; ++i)
            {
                for (int j = 0; j < initialImage.Width; ++j)
                {
                    mse += ((int)initialImage.Data[i, j, 0] - mean) * ((int)initialImage.Data[i, j, 0] - mean);
                }
            }

            mse /= (double)(initialImage.Height * initialImage.Width);
            mse = Math.Sqrt(mse);
            return mse;
        }

        //private double ComputeMean(Image<Bgr, byte> initialImage, Point firstMouseClick, Point lastMouseClick)
        //{
        //    double mean = 0;
        //    for (int i = (int)firstMouseClick.Y; i <= lastMouseClick.Y; ++i)
        //    {
        //        for (int j = (int)firstMouseClick.X; j <= lastMouseClick.X; ++j)
        //        {
        //            //if (i >= 0 && j >= 0 && i < InitialImage.Height && j < InitialImage.Width)
        //            mean += (int)GrayInitialImage.Data[i, j, 0];
        //        }
        //    }

        //    mean /= (double)((lastMouseClick.Y - firstMouseClick.Y + 1) * (lastMouseClick.X - firstMouseClick.X + 1));

        //    return mean;
        //}

        #endregion

        #region Visualize color levels

        #region Row color levels
        private ICommand _rowColorLevelsCommand;
        public ICommand RowColorLevelsCommand
        {
            get
            {
                if (_rowColorLevelsCommand == null)
                    _rowColorLevelsCommand = new RelayCommand(RowColorLevels);
                return _rowColorLevelsCommand;
            }
        }

        private void RowColorLevels(object parameter)
        {
            if (RowColorLevelsOn == true) return;
            if (MouseClickCollection.Count == 0)
            {
                MessageBox.Show("Please select an area first!");
                return;
            }

            ColorLevelsWindow window = new ColorLevelsWindow(_mainVM, CLevelsType.Row);
            window.Show();
        }
        #endregion

        #region Column color levels
        private ICommand _columnColorLevelsCommand;
        public ICommand ColumnColorLevelsCommand
        {
            get
            {
                if (_columnColorLevelsCommand == null)
                    _columnColorLevelsCommand = new RelayCommand(ColumnColorLevels);
                return _columnColorLevelsCommand;
            }
        }

        private void ColumnColorLevels(object parameter)
        {
            if (ColumnColorLevelsOn == true) return;
            if (MouseClickCollection.Count == 0)
            {
                MessageBox.Show("Please select an area first!");
                return;
            }

            ColorLevelsWindow window = new ColorLevelsWindow(_mainVM, CLevelsType.Column);
            window.Show();
        }
        #endregion

        #endregion

        #region Visualize image histogram

        #region Initial image histogram
        private ICommand _histogramInitialImageCommand;
        public ICommand HistogramInitialImageCommand
        {
            get
            {
                if (_histogramInitialImageCommand == null)
                    _histogramInitialImageCommand = new RelayCommand(HistogramInitialImage);
                return _histogramInitialImageCommand;
            }
        }

        private void HistogramInitialImage(object parameter)
        {
            if (InitialHistogramOn == true) return;
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            HistogramWindow window = null;

            if (ColorInitialImage != null)
            {
                window = new HistogramWindow(_mainVM, ImageType.InitialColor);
            }
            else if (GrayInitialImage != null)
            {
                window = new HistogramWindow(_mainVM, ImageType.InitialGray);
            }

            window.Show();
        }
        #endregion

        #region Processed image histogram
        private ICommand _histogramProcessedImageCommand;
        public ICommand HistogramProcessedImageCommand
        {
            get
            {
                if (_histogramProcessedImageCommand == null)
                    _histogramProcessedImageCommand = new RelayCommand(HistogramProcessedImage);
                return _histogramProcessedImageCommand;
            }
        }

        private void HistogramProcessedImage(object parameter)
        {
            if (ProcessedHistogramOn == true) return;
            if (ProcessedImage == null)
            {
                MessageBox.Show("Please process an image first!");
                return;
            }

            HistogramWindow window = null;

            if (ColorProcessedImage != null)
            {
                window = new HistogramWindow(_mainVM, ImageType.ProcessedColor);
            }
            else if (GrayProcessedImage != null)
            {
                window = new HistogramWindow(_mainVM, ImageType.ProcessedGray);
            }

            window.Show();
        }
        #endregion

        #endregion

        #region Copy image
        private ICommand _copyImageCommand;
        public ICommand CopyImageCommand
        {
            get
            {
                if (_copyImageCommand == null)
                    _copyImageCommand = new RelayCommand(CopyImage);
                return _copyImageCommand;
            }
        }

        private void CopyImage(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter);

            if (ColorInitialImage != null)
            {
                ColorProcessedImage = Tools.Copy(ColorInitialImage);
                ProcessedImage = Convert(ColorProcessedImage);
            }
            else if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.Copy(GrayInitialImage);
                ProcessedImage = Convert(GrayProcessedImage);
            }
        }
        #endregion

        #region Invert image
        private ICommand _invertImageCommand;
        public ICommand InvertImageCommand
        {
            get
            {
                if (_invertImageCommand == null)
                    _invertImageCommand = new RelayCommand(InvertImage);
                return _invertImageCommand;
            }
        }

        private void InvertImage(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter as Canvas);

            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.Invert(GrayInitialImage);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Tools.Invert(ColorInitialImage);
                ProcessedImage = Convert(ColorProcessedImage);
            }
        }
        #endregion

        #region Convert color image to grayscale image
        private ICommand _convertImageToGrayscaleCommand;
        public ICommand ConvertImageToGrayscaleCommand
        {
            get
            {
                if (_convertImageToGrayscaleCommand == null)
                    _convertImageToGrayscaleCommand = new RelayCommand(ConvertImageToGrayscale);
                return _convertImageToGrayscaleCommand;
            }
        }

        private void ConvertImageToGrayscale(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter);

            if (ColorInitialImage != null)
            {
                GrayProcessedImage = Tools.Convert(ColorInitialImage);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else
            {
                MessageBox.Show("It is possible to convert only color images!");
            }
        }
        #endregion

        #region Binary image
        private ICommand _binaryImageCommand;
        public ICommand BinaryImageCommand
        {
            get
            {
                if (_binaryImageCommand == null)
                    _binaryImageCommand = new RelayCommand(BinaryImage);
                return _binaryImageCommand;
            }
        }

        private void BinaryImage(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter as Canvas);

            List<string> labels = new List<string>() { "Threshold: " };
            DialogWindow window = new DialogWindow(_mainVM, labels);
            window.ShowDialog();

            List<double> values = window.GetValues();
            int T = (int)values[0];

            if(T < 10 || T > 145)
            {
                MessageBox.Show("Please add a valid threshold value from [10, 145]!");
                return;
            }

            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.BinaryImage(GrayInitialImage, T);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            if (ColorInitialImage != null)
            {
                MessageBox.Show("Please add a grayscale image!");
            }
        }
        #endregion

        #region Mirror image
        private ICommand _mirrorImageCommand;
        public ICommand MirrorImageCommand
        {
            get
            {
                if (_mirrorImageCommand == null)
                    _mirrorImageCommand = new RelayCommand(MirrorImage);
                return _mirrorImageCommand;
            }
        }

        private void MirrorImage(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter as Canvas);

            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.MirrorImage(GrayInitialImage);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Tools.MirrorImage(ColorInitialImage);
                ProcessedImage = Convert(ColorProcessedImage);
            }
        }
        #endregion

        #region Rotate image right
        private ICommand _rightRotateImageCommand;
        public ICommand RightRotateImageCommand
        {
            get
            {
                if (_rightRotateImageCommand == null)
                    _rightRotateImageCommand = new RelayCommand(RotateImageRight);
                return _rightRotateImageCommand;
            }
        }

        private void RotateImageRight(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter as Canvas);

            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.RotateRight(GrayInitialImage);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Tools.RotateRight(ColorInitialImage);
                ProcessedImage = Convert(ColorProcessedImage);
            }
        }
        #endregion

        #region Rotate image left
        private ICommand _leftRotateImageCommand;
        public ICommand LeftRotateImageCommand
        {
            get
            {
                if (_leftRotateImageCommand == null)
                    _leftRotateImageCommand = new RelayCommand(RotateImageLeft);
                return _leftRotateImageCommand;
            }
        }

        private void RotateImageLeft(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter as Canvas);

            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.RotateLeft(GrayInitialImage);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            else if (ColorInitialImage != null)
            {
                ColorProcessedImage = Tools.RotateLeft(ColorInitialImage);
                ProcessedImage = Convert(ColorProcessedImage);
            }
        }
        #endregion

        #endregion

        #region Pointwise operations

        #region Change Contrast and Brightness

        #region Linear

        private ICommand _linearContrastBrightnessCommand;
        public ICommand LinearContrastBrightnessCommand
        {
            get
            {
                if (_linearContrastBrightnessCommand == null)
                    _linearContrastBrightnessCommand = new RelayCommand(ChangeContrastAndBrightness);
                return _linearContrastBrightnessCommand;
            }
        }

        private void ChangeContrastAndBrightness(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter as Canvas);

            List<string> labels = new List<string>() { "Alpha: ", "Beta: " };
            DialogWindow window = new DialogWindow(_mainVM, labels);
            window.ShowDialog();

            List<double> values = window.GetValues();
            double alpha = values[0];
            double beta = values[1];

            if (alpha <= 0)
            {
                MessageBox.Show("Please add a valid alpha!");
                return;
            }

            byte[] LUT = PointwiseOperations.ContrastBrightnessLinear(alpha, beta);

            if (GrayInitialImage != null)
            {
                GrayProcessedImage = PointwiseOperations.ApplyLUT(GrayInitialImage, LUT);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            if (ColorInitialImage != null)
            {
                MessageBox.Show("Please add a grayscale image!");
            }
        }
        #endregion

        #region Gamma Operator

        private ICommand _gammaContrastBrightnessCommand;
        public ICommand GammaContrastBrightnessCommand
        {
            get
            {
                if (_gammaContrastBrightnessCommand == null)
                    _gammaContrastBrightnessCommand = new RelayCommand(GammaOperatorContrastAndBrightness);
                return _gammaContrastBrightnessCommand;
            }
        }

        private void GammaOperatorContrastAndBrightness(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter as Canvas);

            List<string> labels = new List<string>() { "Gamma: "};
            DialogWindow window = new DialogWindow(_mainVM, labels);
            window.ShowDialog();

            List<double> values = window.GetValues();
            double gamma = values[0];

            if (gamma <= 0)
            {
                MessageBox.Show("Please add a valid gamma!");
                return;
            }

            byte[] LUT = PointwiseOperations.ContrastBrightnessGamma(gamma);

            if (GrayInitialImage != null)
            {
                GrayProcessedImage = PointwiseOperations.ApplyLUT(GrayInitialImage, LUT);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            if (ColorInitialImage != null)
            {
                MessageBox.Show("Please add a grayscale image!");
            }
        }

        #endregion

        #endregion

        #endregion

        #region Thresholding
        #region MinError
        private ICommand _minErrorCommand;
        public ICommand MinErrorCommand;
        {
            get
            {
                if (_minErrorCommand == null)
                    _minErrorCommand = new RelayCommand(BinaryImage);
                return _minErrorCommand;
            }
        }

        private void BinaryImage(object parameter)
        {
            if (InitialImage == null)
            {
                MessageBox.Show("Please add an image!");
                return;
            }

            ClearProcessedCanvas(parameter as Canvas);

            List<string> labels = new List<string>() { "Threshold: " };
            DialogWindow window = new DialogWindow(_mainVM, labels);
            window.ShowDialog();

            List<double> values = window.GetValues();
            int T = (int)values[0];

            if (T < 10 || T > 145)
            {
                MessageBox.Show("Please add a valid threshold value from [10, 145]!");
                return;
            }

            if (GrayInitialImage != null)
            {
                GrayProcessedImage = Tools.BinaryImage(GrayInitialImage, T);
                ProcessedImage = Convert(GrayProcessedImage);
            }
            if (ColorInitialImage != null)
            {
                MessageBox.Show("Please add a grayscale image!");
            }
        }
        #endregion
        #endregion

        #region Filters
        #endregion

        #region Morphological operations
        #endregion

        #region Geometric transformations
        #endregion

        #region Segmentation
        #endregion

        #region Use processed image as initial image
        private ICommand _useProcessedImageAsInitialImageCommand;
        public ICommand UseProcessedImageAsInitialImageCommand
        {
            get
            {
                if (_useProcessedImageAsInitialImageCommand == null)
                    _useProcessedImageAsInitialImageCommand = new RelayCommand(UseProcessedImageAsInitialImage);
                return _useProcessedImageAsInitialImageCommand;
            }
        }

        private void UseProcessedImageAsInitialImage(object parameter)
        {
            if (ProcessedImage == null)
            {
                MessageBox.Show("Please process an image first!");
                return;
            }

            var canvases = (object[])parameter;

            ClearInitialCanvas(canvases[0] as Canvas);

            if (GrayProcessedImage != null)
            {
                GrayInitialImage = GrayProcessedImage;
                InitialImage = Convert(GrayInitialImage);
            }
            else if (ColorProcessedImage != null)
            {
                ColorInitialImage = ColorProcessedImage;
                InitialImage = Convert(ColorInitialImage);
            }

            ClearProcessedCanvas(canvases[1] as Canvas);
        }
        #endregion

    }
}