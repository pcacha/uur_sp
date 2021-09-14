using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;
using WPF_Malovani.Models;
using WPF_Malovani.Utility;
using WPF_Malovani.Views;

namespace WPF_Malovani.ViewModels
{
    public enum AvailableShapes
    {
        Curve,
        Line,
        Ellipse,
        Rectangle,
        Text
    }

    public enum AvailableModes
    {
        Paint,
        Grab,
        Delete
    }

    public enum AvailableThickness
    {
        Thin,
        Medium,
        Thick
    }

    public delegate void MouseActionDelegate(int x, int y);
    public delegate void MouseMoveActionDelegate(int x, int y, MouseButtonState buttonState);

    class PaintVM: ObservableObject
    {
        private readonly int thinThickness = 1;
        private readonly int mediumThickness = 3;
        private readonly int thickThickness = 6;

        private int startX;
        private int startY;
        private int endX;
        private int endY;

        private SolidColorBrush colorBrush;
        private Color color;
        private FontFamily fontFamily;
        private int fontSize;

        private MyCurve myCurve;
        private bool curveIsPainted;
        private string picturePath;
        private Visual canvas;
        private bool isPainted;

        private int ThicknessValue
        {
            get
            {
                switch(SelectedThickness)
                {
                    case AvailableThickness.Thin:
                        return thinThickness;
                    case AvailableThickness.Medium:
                        return mediumThickness;
                    case AvailableThickness.Thick:
                        return thickThickness;
                    default:
                        return thickThickness;
                }
            }
        }

        public int PixelsX { get; private set; }
        public int PixelsY { get; private set; }
        public string Dimensions { get; private set; }

        private bool fillShapes;
        public bool FillShapes
        {
            get
            {
                return fillShapes;
            }
            private set
            {
                OnPropertyChanged(ref fillShapes, value);
            }
        }

        private AvailableShapes selectedShapeMode;
        public AvailableShapes SelectedShapeMode
        {
            get
            {
                return selectedShapeMode;
            }
            private set
            {
                OnPropertyChanged(ref selectedShapeMode, value);
            }
        }

        private AvailableModes selectedMode;
        public AvailableModes SelectedMode
        {
            get
            {
                return selectedMode;
            }
            private set
            {
                OnPropertyChanged(ref selectedMode, value);
            }
        }

        private AvailableThickness selectedThickness;
        public AvailableThickness SelectedThickness
        {
            get
            {
                return selectedThickness;
            }
            private set
            {
                OnPropertyChanged(ref selectedThickness, value);
            }
        }

        private bool imageInserted;
        public bool ImageInserted
        {
            get
            {
                return imageInserted;
            }
            private set
            {
                OnPropertyChanged(ref imageInserted, value);
            }
        }

        public ObservableCollection<FrameworkElement> Shapes { get; private set; }
        private List<IPaintable> myShapes;

        private List<FrameworkElement> hitShapes;
        private List<IPaintable> hitMyShapes;        

        public RelayCommand<object> FillShapesCommand { get; private set; }
        public RelayCommand<AvailableShapes> SelectShapeMode { get; private set; }
        public RelayCommand<AvailableModes> SelectMode { get; private set; }
        public RelayCommand<AvailableThickness> SelectThickness { get; private set; }
        public RelayCommand<object> Customize { get; private set; }
        public RelayCommand<object> OpenPicture { get; private set; }
        public RelayCommand<object> Save { get; private set; }
        public RelayCommand<Visual> LoadCanvas { get; private set; }

        public MouseActionDelegate MouseDown { get; private set; }
        public MouseActionDelegate MouseUp { get; private set; }
        public MouseMoveActionDelegate MouseMove { get; private set; }        

        public PaintVM(int pixelsX, int pixelsY)
        {
            PixelsX = pixelsX;
            PixelsY = pixelsY;
            Dimensions = String.Format($"{PixelsX}x{PixelsY}");

            Initialize();
        }

        public PaintVM(string path)
        {
            Initialize();

            List<IPaintable> temporaryMyShapes = new List<IPaintable>();
            #region xml load
            try
            {
                XDocument project = XDocument.Load(path);
                XElement root = project.Element("project");
                PixelsX = int.Parse(root.Element("pixelsX").Value);
                PixelsY = int.Parse(root.Element("pixelsY").Value);
                Dimensions = string.Format($"{PixelsX}x{PixelsY}");
                if(bool.Parse(root.Element("picture").Attribute("inserted").Value))
                {
                    string picturePath = root.Element("picture").Value;
                    if (File.Exists(picturePath))
                    {
                        InsertPicture(picturePath);
                    }
                }

                foreach(XElement element in root.Element("shapes").Elements())
                {
                    switch(element.Name.LocalName)
                    {
                        case "curve":
                            temporaryMyShapes.Add(LoadMyCurve(element));
                            break;
                        case "ellipse":
                            temporaryMyShapes.Add(LoadMyEllipse(element));
                            break;
                        case "rectangle":
                            temporaryMyShapes.Add(LoadMyRectangle(element));
                            break;
                        case "textBlock":
                            temporaryMyShapes.Add(LoadMyTextBlock(element));
                            break;
                        case "line":
                            temporaryMyShapes.Add(LoadMyLine(element));
                            break;
                    }
                }
        }
            catch(Exception)
            {
                MessageBox.Show("Načtení souboru selhalo.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                PixelsX = 1280;
                PixelsY = 720;
                Dimensions = String.Format($"{PixelsX}x{PixelsY}");

                temporaryMyShapes.Clear();
            }
    #endregion

            #region load to displayed shapes
            myShapes = temporaryMyShapes;
            foreach(IPaintable p in myShapes)
            {
                if(p is MyCurve)
                {
                    MyCurve mc = (p as MyCurve);
                    for(int i = 0; i < mc.Count; i++)
                    {
                        MyCurvePart part = mc.GetPart(i);
                        Shapes.Add(new Line()
                        {
                            X1 = part.Start.X,
                            Y1 = part.Start.Y,
                            X2 = part.End.X,
                            Y2 = part.End.Y,
                            StrokeThickness = mc.Thickness,
                            Stroke = new SolidColorBrush(mc.Color)
                        });
                    }
                }
                else if(p is MyEllipse)
                {
                    MyEllipse el = (p as MyEllipse);

                    Ellipse e = new Ellipse()
                    {
                        Width = el.Width,
                        Height = el.Height,
                        StrokeThickness = el.Thickness,
                        Stroke = new SolidColorBrush(el.Color),
                        Fill = (el.Filled) ? new SolidColorBrush(el.Color) : null
                    };
                    Canvas.SetLeft(e, el.Start.X);
                    Canvas.SetTop(e, el.Start.Y);
                    Shapes.Add(e);
                }
                else if(p is MyLine)
                {
                    Shapes.Add(new Line()
                    {
                        X1 = p.Start.X,
                        Y1 = p.Start.Y,
                        X2 = (p as MyLine).End.X,
                        Y2 = (p as MyLine).End.Y,
                        StrokeThickness = (p as MyLine).Thickness,
                        Stroke = new SolidColorBrush(p.Color)
                    });
                }
                else if(p is MyRectangle)
                {
                    MyRectangle rect = (p as MyRectangle);

                    Rectangle r = new Rectangle()
                    {
                        Width = rect.Width,
                        Height = rect.Height,
                        StrokeThickness = rect.Thickness,
                        Stroke = new SolidColorBrush(rect.Color),
                        Fill = (rect.Filled) ? new SolidColorBrush(rect.Color) : null
                    };
                    Canvas.SetLeft(r, rect.Start.X);
                    Canvas.SetTop(r, rect.Start.Y);
                    Shapes.Add(r);
                }
                else if(p is MyTextBlock)
                {
                    MyTextBlock tb = (p as MyTextBlock);

                    TextBlock t = new TextBlock()
                    {
                        Text = tb.Text,
                        Foreground = new SolidColorBrush(tb.Color),
                        FontFamily = tb.FontFamily,
                        FontSize = tb.FontSize,
                        Width = tb.Width,
                        Height = tb.Height
                    };
                    Canvas.SetLeft(t, tb.Start.X);
                    Canvas.SetTop(t, tb.Start.Y);
                    Shapes.Add(t);
                }                
            }
            #endregion
        }

        #region load shapes logic
        private MyCurve LoadMyCurve(XElement e)
        {
            Color color = LoadColor(e);
            int thickness = int.Parse(e.Element("thickness").Value);
            MyCurve mc = new MyCurve(color, thickness);
            foreach(XElement part in e.Element("parts").Elements())
            {
                mc.Add(LoadMyCurvePart(part, mc));
            }
            return mc;
        }

        private MyCurvePart LoadMyCurvePart(XElement e, MyCurve mc)
        {
            int x1 = int.Parse(e.Element("start").Element("x").Value);
            int y1 = int.Parse(e.Element("start").Element("y").Value);
            int x2 = int.Parse(e.Element("end").Element("x").Value);
            int y2 = int.Parse(e.Element("end").Element("y").Value);

            return new MyCurvePart(x1, y1, x2, y2, mc);
        }

        private MyEllipse LoadMyEllipse(XElement e)
        {
            Color color = LoadColor(e);
            int x = int.Parse(e.Element("start").Element("x").Value);
            int y = int.Parse(e.Element("start").Element("y").Value);
            int width = int.Parse(e.Element("width").Value);
            int height = int.Parse(e.Element("height").Value);
            int thickness = int.Parse(e.Element("thickness").Value);
            bool filled = bool.Parse(e.Element("filled").Value);

            return new MyEllipse(x, y, width, height, thickness, color, filled, true);
        }

        private MyRectangle LoadMyRectangle(XElement e)
        {
            Color color = LoadColor(e);
            int x = int.Parse(e.Element("start").Element("x").Value);
            int y = int.Parse(e.Element("start").Element("y").Value);
            int width = int.Parse(e.Element("width").Value);
            int height = int.Parse(e.Element("height").Value);
            int thickness = int.Parse(e.Element("thickness").Value);
            bool filled = bool.Parse(e.Element("filled").Value);

            return new MyRectangle(x, y, width, height, thickness, color, filled, true);
        }

        private MyTextBlock LoadMyTextBlock(XElement e)
        {
            int x = int.Parse(e.Element("start").Element("x").Value);
            int y = int.Parse(e.Element("start").Element("y").Value);
            string text = e.Element("text").Value;
            int widht = int.Parse(e.Element("width").Value);
            int height = int.Parse(e.Element("height").Value);
            Color color = LoadColor(e);
            FontFamily ff = new FontFamily(e.Element("fontFamily").Value);
            int fs = int.Parse(e.Element("fontSize").Value);
            return new MyTextBlock(x, y, text, widht, height, color, ff, fs);
        }

        private MyLine LoadMyLine(XElement e)
        {
            Color color = LoadColor(e);
            int x1 = int.Parse(e.Element("start").Element("x").Value);
            int y1 = int.Parse(e.Element("start").Element("y").Value);
            int x2 = int.Parse(e.Element("end").Element("x").Value);
            int y2 = int.Parse(e.Element("end").Element("y").Value);
            int thickness = int.Parse(e.Element("thickness").Value); 

            return new MyLine(x1, y1, x2, y2, thickness, color);
        }

        private Color LoadColor(XElement e)
        {
            Color color = new Color()
            {
                R = byte.Parse(e.Element("color").Element("r").Value),
                G = byte.Parse(e.Element("color").Element("g").Value),
                B = byte.Parse(e.Element("color").Element("b").Value),
                A = byte.Parse(e.Element("color").Element("a").Value)
            };
            return color;
        }        
        #endregion

        private void Initialize()
        {
            startX = 0;
            startY = 0;
            endX = 0;
            endY = 0;

            colorBrush = new SolidColorBrush(Colors.Black);
            color = Colors.Black;
            fontFamily = new FontFamily("Arial");
            fontSize = 12;
            isPainted = false;

            myCurve = new MyCurve(color, ThicknessValue);
            curveIsPainted = false;

            FillShapes = false;
            SelectedShapeMode = AvailableShapes.Curve;
            selectedMode = AvailableModes.Paint;
            SelectedThickness = AvailableThickness.Thin;
            ImageInserted = false;

            Shapes = new ObservableCollection<FrameworkElement>();
            myShapes = new List<IPaintable>();

            hitShapes = new List<FrameworkElement>();
            hitMyShapes = new List<IPaintable>();

            FillShapesCommand = new RelayCommand<object>(FillShapesCommandExecute);
            SelectShapeMode = new RelayCommand<AvailableShapes>(SelectShapeModeExecute);
            SelectMode = new RelayCommand<AvailableModes>(SelectModeExecute);
            SelectThickness = new RelayCommand<AvailableThickness>(SelectThicknessExecute);
            Customize = new RelayCommand<object>(CustomizeExecute);
            OpenPicture = new RelayCommand<object>(OpenPictureExecute);
            Save = new RelayCommand<object>(SaveExecute);
            LoadCanvas = new RelayCommand<Visual>(LoadCanvasExecute);

            MouseDown = MouseDownAction;
            MouseUp = MouseUpAction;
            MouseMove = MouseMoveAction;
        }

        private void FillShapesCommandExecute(object param)
        {
            FillShapes = !FillShapes;
        }

        private void SelectShapeModeExecute(AvailableShapes sender)
        {
            SelectedShapeMode = sender;
        }

        private void SelectModeExecute(AvailableModes sender)
        {
            SelectedMode = sender;
        }

        private void SelectThicknessExecute(AvailableThickness sender)
        {
            SelectedThickness = sender;
        }

        private void CustomizeExecute(object param)
        {
            CustomizeVM customizeVM = new CustomizeVM(color, fontFamily, fontSize);
            CustomizeWindow customizeWindow = new CustomizeWindow();
            customizeWindow.DataContext = customizeVM;

            customizeWindow.ShowDialog();

            if (customizeVM.ClosedProperly)
            {
                colorBrush = new SolidColorBrush(customizeVM.SelectedColor);
                color = customizeVM.SelectedColor;
                fontFamily = customizeVM.SelectedFontFamily;
                fontSize = customizeVM.SelectedFontSize;
            }
        }

        private void OpenPictureExecute(object param)
        {
            if (!ImageInserted)
            {                
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Obrázek (*.jpg, *.png)|*.jpg;*.png";

                if (ofd.ShowDialog() == true)
                {
                    InsertPicture(ofd.FileName);
                }
            }
            else
            {
                Shapes.RemoveAt(0);
                ImageInserted = false;
                picturePath = null;
            }
        }

        private void InsertPicture(string path)
        {
            BitmapImage bitmapImage = new BitmapImage(new Uri(path));
            Image image = new Image()
            {
                Source = bitmapImage
            };
            Shapes.Insert(0, image);

            ImageInserted = true;
            picturePath = path;
        }

        private void SaveExecute(object param)
        {
            RenderTargetBitmap rtb = new RenderTargetBitmap(PixelsX, PixelsY, 96d, 96d, PixelFormats.Default);
            rtb.Render(canvas);
            SaveVM saveVM = new SaveVM(PixelsX, PixelsY, myShapes, picturePath, rtb);
            SaveWindow saveWindow = new SaveWindow();
            saveWindow.DataContext = saveVM;

            saveWindow.ShowDialog();
        }

        private void LoadCanvasExecute(Visual visual)
        {
            canvas = visual;
        }

        private void MouseDownAction(int x, int y)
        {
            startX = x;
            startY = y;

            if (SelectedMode != AvailableModes.Paint)
            {                
               SaveHitShapes();
            }
            else if(SelectedMode == AvailableModes.Paint && SelectedShapeMode == AvailableShapes.Curve)
            {
                myCurve = new MyCurve(color, ThicknessValue);
                curveIsPainted = true;
            }
            else if (SelectedMode == AvailableModes.Paint)
            {
                isPainted = true;
            }
        }

        private void MouseUpAction(int x, int y)
        {
            endX = x;
            endY = y;

            switch(SelectedMode)
            {
                case AvailableModes.Paint:
                    Paint();
                    break;
                case AvailableModes.Grab:
                    Grab();
                    break;
                case AvailableModes.Delete:
                    Delete();
                    break;
            }
        }

        #region paint actions
        private void Paint()
        {
            if(isPainted)
            {
                switch (SelectedShapeMode)
                {
                    case AvailableShapes.Curve:
                        PaintCurve();
                        break;
                    case AvailableShapes.Line:
                        PaintLine();
                        break;
                    case AvailableShapes.Rectangle:
                        PaintRectangle();
                        break;
                    case AvailableShapes.Ellipse:
                        PaintEllipse();
                        break;
                    case AvailableShapes.Text:
                        PaintText();
                        break;
                }
            }            
            isPainted = false;
        }

        private void PaintCurve()
        {
            if(((Math.Abs(startX - endX) >= 1 || Math.Abs(startY - endY) >= 1) && myCurve.Count == 0) || myCurve.Count > 0)
            {
                myCurve.Add(new MyCurvePart(startX, startY, endX, endY, myCurve));
                myShapes.Add(myCurve);
                Line l = new Line()
                {
                    X1 = startX,
                    Y1 = startY,
                    X2 = endX,
                    Y2 = endY,
                    StrokeThickness = ThicknessValue,
                    Stroke = colorBrush,
                    StrokeEndLineCap = PenLineCap.Round
                };
                Shapes.Add(l);
            }            

            curveIsPainted = false;
        }

        private void PaintLine()
        {
            if(Math.Abs(startX - endX) >= 1 || Math.Abs(startY - endY) >= 1)
            {
                myShapes.Add(new MyLine(startX, startY, endX, endY, ThicknessValue, color));
                Line l = new Line()
                {
                    X1 = startX,
                    Y1 = startY,
                    X2 = endX,
                    Y2 = endY,
                    StrokeThickness = ThicknessValue,
                    Stroke = colorBrush
                };
                Shapes.Add(l);
            }            
        }

        private void PaintEllipse()
        {
            MyEllipse myEllipse = new MyEllipse(startX, startY, endX, endY, ThicknessValue, color, FillShapes);
            myShapes.Add(myEllipse);
            Ellipse e = new Ellipse()
            {
                Width = myEllipse.Width,
                Height = myEllipse.Height,
                StrokeThickness = ThicknessValue,
                Stroke = colorBrush,
                Fill = (FillShapes) ? colorBrush : null
            };
            Canvas.SetLeft(e, myEllipse.Start.X);
            Canvas.SetTop(e, myEllipse.Start.Y);
            Shapes.Add(e);
        }

        private void PaintRectangle()
        {
            MyRectangle myRectangle = new MyRectangle(startX, startY, endX, endY, ThicknessValue, color, FillShapes);
            myShapes.Add(myRectangle);
            Rectangle r = new Rectangle()
            {
                Width = myRectangle.Width,
                Height = myRectangle.Height,
                StrokeThickness = ThicknessValue,
                Stroke = colorBrush,
                Fill = (FillShapes) ? colorBrush : null
            };
            Canvas.SetLeft(r, myRectangle.Start.X);
            Canvas.SetTop(r, myRectangle.Start.Y);
            Shapes.Add(r);
        }

        private void PaintText()
        {
            TypeTextWindow textWindow = new TypeTextWindow();
            TypeTextVM textViewModel = new TypeTextVM();

            textWindow.DataContext = textViewModel;

            textWindow.ShowDialog();

            if(textViewModel.ClosedProperly)
            {
                string text = textViewModel.Text;

                TextBlock tb = new TextBlock()
                {
                    Text = text,
                    Foreground = new SolidColorBrush(color),
                    FontFamily = fontFamily,
                    FontSize = fontSize                   
                };
                Canvas.SetLeft(tb, startX);
                Canvas.SetTop(tb, startY);
                Shapes.Add(tb);

                Typeface typeface = new Typeface(fontFamily, tb.FontStyle, tb.FontWeight, tb.FontStretch);
                FormattedText formattedText = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, fontSize, tb.Foreground, new NumberSubstitution());
                MyTextBlock myTextBlock = new MyTextBlock(startX, startY, text, (int)formattedText.Width, (int)formattedText.Height, color, fontFamily, fontSize);
                myShapes.Add(myTextBlock);

                tb.Width = (int)formattedText.Width;
                tb.Height = (int)formattedText.Height;
            }
        }
        #endregion

        #region hit test
        private void SaveHitShapes()
        {
            //opportunity 1) - all shapes, throughout the visibility tree, get hit 
            //hitMyShapes = myShapes.FindAll(s => s.Hit(startX, startY));

            //opportunity 2) - only first shape get hit
            hitMyShapes.Clear();
            for (int i = myShapes.Count - 1; i > -1; i--)
            {
                if (myShapes[i].Hit(startX, startY))
                {
                    hitMyShapes.Add(myShapes[i]);
                    break;
                }
            }

            hitShapes.Clear();
            for (int i = 0; i < hitMyShapes.Count; i++)
            {
                if(!(hitMyShapes[i] is MyCurve))
                {
                    for (int j = 0; j < Shapes.Count; j++)
                    {
                        if (hitMyShapes[i].CompareTo(Shapes[j]))
                        {
                            hitShapes.Add(Shapes[j]);
                            break;
                        }
                    }
                }
                else
                {
                    MyCurve curve = (hitMyShapes[i] as MyCurve);
                    MyCurvePart firstPart = curve.GetPart(0);
                    int startIndex = 0;                   
                    for(int j = 0; j < Shapes.Count; j++)
                    {
                        if(firstPart.CompareTo(Shapes[j]))
                        {
                            startIndex = j;
                        }
                    }
                    for(int j = startIndex; j < startIndex + curve.Count; j++)
                    {
                        hitShapes.Add(Shapes[j]);
                    }
                }
            }
        }
        #endregion

        #region grab actions
        private void Grab()
        {
            MyVector vector = new MyVector(startX, startY, endX, endY);

            int shapesCounter = 0;
            for (int i = 0; i < hitMyShapes.Count; i++)
            {                
                FrameworkElement fe = hitShapes[shapesCounter] as FrameworkElement;
                if(hitMyShapes[i] is MyCurve)
                {
                    for(int j = 0; j < (hitMyShapes[i] as MyCurve).Count; j++)
                    {
                        GrabLine(fe as Line, vector);
                        shapesCounter++;
                        if(j != (hitMyShapes[i] as MyCurve).Count - 1)
                        {
                            fe = hitShapes[shapesCounter] as FrameworkElement;
                        }                        
                    }
                }
                else if (fe is Line)
                {
                    GrabLine(fe as Line, vector);
                    shapesCounter++;
                }
                else if (fe is Ellipse)
                {
                    GrabEllipse(fe as Ellipse, hitMyShapes[i].Start.X, hitMyShapes[i].Start.Y, vector);
                    shapesCounter++;
                }
                else if (fe is Rectangle)
                {
                    GrabRectangle(fe as Rectangle, hitMyShapes[i].Start.X, hitMyShapes[i].Start.Y, vector);
                    shapesCounter++;
                }
                else if (fe is TextBlock)
                {
                    GrabTextBlock(fe as TextBlock, hitMyShapes[i].Start.X, hitMyShapes[i].Start.Y, vector);
                    shapesCounter++;
                }

                hitMyShapes[i].Move(vector);
            }            
        }

        private void GrabLine(Line line, MyVector vector)
        {
            line.X1 += vector.X;
            line.X2 += vector.X;
            line.Y1 += vector.Y;
            line.Y2 += vector.Y;
        }

        private void GrabEllipse(Ellipse ellipse, int actualX, int actualY, MyVector vector)
        {
            Canvas.SetLeft(ellipse, actualX + vector.X);
            Canvas.SetTop(ellipse, actualY + vector.Y);                     
        }

        private void GrabRectangle(Rectangle rectangle, int actualX, int actualY, MyVector vector)
        {
            Canvas.SetLeft(rectangle, actualX + vector.X);
            Canvas.SetTop(rectangle, actualY + vector.Y);
        }

        private void GrabTextBlock(TextBlock textBlock, int actualX, int actualY, MyVector vector)
        {
            Canvas.SetLeft(textBlock, actualX + vector.X);
            Canvas.SetTop(textBlock, actualY + vector.Y);
        }
        #endregion

        #region delete actions
        private void Delete()
        {
            for(int i = 0; i < hitMyShapes.Count; i++)
            {
                for(int j = 0; j < myShapes.Count; j++)
                {
                    if(hitMyShapes[i] == myShapes[j])
                    {
                        myShapes.RemoveAt(j);
                    }
                }
            }

            for (int i = 0; i < hitShapes.Count; i++)
            {
                for (int j = 0; j < Shapes.Count; j++)
                {
                    if (hitShapes[i] == Shapes[j])
                    {
                        Shapes.RemoveAt(j);
                    }
                }
            }            
        }
        #endregion

        private void MouseMoveAction(int x, int y, MouseButtonState buttonState)
        {            
            if(SelectedShapeMode == AvailableShapes.Curve && (Math.Abs(startX - x) > 1 || Math.Abs(startY - y) > 1) && curveIsPainted && buttonState == MouseButtonState.Pressed)
            {
                myCurve.Add(new MyCurvePart(startX, startY, x, y, myCurve));
                Line l = new Line()
                {
                    X1 = startX,
                    Y1 = startY,
                    X2 = x,
                    Y2 = y,
                    StrokeThickness = ThicknessValue,
                    Stroke = colorBrush,
                    StrokeEndLineCap = PenLineCap.Round
                };
                Shapes.Add(l);

                startX = x;
                startY = y;
            }    
            else if(SelectedShapeMode == AvailableShapes.Curve && curveIsPainted && buttonState == MouseButtonState.Released)
            {
                endX = startX;
                endY = startY;
                PaintCurve();
            }
        }       
    }
}
