using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.Storage;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using System.Threading;
using Windows.UI.Xaml.Shapes;
using Windows.UI.ViewManagement;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace GUI_Final
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        int x1;
        int y1;
        int x2;
        int y2;

        int BoxType;

        bool DarkMode;

        public MainPage()
        {
            DarkMode = false; // Default to light mode.
            ResourceDictionary skin1 = new ResourceDictionary();
            skin1.Source = new Uri("ms-appx:///DefaultSkin.xaml");
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(skin1);

            this.InitializeComponent();
            Pages list = Pages.PagesInstance;
            if (list.currentPage > 0)
            {
                Back.IsEnabled = true;
            }
            List<PageShapes> empty = new List<PageShapes>();
            list.pages.Add(empty);
            DrawingCanvas.PointerPressed += new PointerEventHandler(DrawStart);
            //DrawingCanvas.PointerReleased += new PointerEventHandler(DrawEnd); // Found a way around needing this, it caused issues.
            BoxType = 0; // 0 means nothing is selected, do nothing.
        }

        public void colorButton(object sender, RoutedEventArgs e)
        {
            String skin = "";
            if (DarkMode == false)
            { 
                DarkMode = true;
                if (sender.Equals(Dark))
                {
                    skin = "Dark";
                }
            }
            else
            {
                DarkMode = false;
                if (sender.Equals(Dark))
                {
                    skin = "Default";
                }
            }
            ResourceDictionary appSkin = new ResourceDictionary();
            appSkin.Source = new Uri("ms-appx:///" + skin + "Skin.xaml");
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(appSkin);

            Style CanvasStyle = (Style)Application.Current.Resources["CanvasStyle"];
            DrawingCanvas.Style = CanvasStyle;

            Style StackPanelStyle = (Style)Application.Current.Resources["StackPanelStyle"];
            Stackpanel.Style = StackPanelStyle;

            Style GridStyle = (Style)Application.Current.Resources["GridStyle"];
            Griddy.Style = GridStyle;

            Style CommandStyle = (Style)Application.Current.Resources["CommandBarStyle"];
            command.Style = CommandStyle;
            command2.Style = CommandStyle;

            Style AppBarButtonStyle = (Style)Application.Current.Resources["AppBarButtonStyle"];
            Pointer.Style = AppBarButtonStyle;
            Text.Style = AppBarButtonStyle;
            Media.Style = AppBarButtonStyle;
            Back.Style = AppBarButtonStyle;
            Next.Style = AppBarButtonStyle;
            Undo.Style = AppBarButtonStyle;
            Redo.Style = AppBarButtonStyle;
            Dark.Style = AppBarButtonStyle;
            FileButton.Style = AppBarButtonStyle;
            HelpButton.Style = AppBarButtonStyle;
            InfoButton.Style = AppBarButtonStyle;
            Pages list = Pages.PagesInstance;

            if (list.pages[list.currentPage].Count > 0)
            {
                DrawingCanvas.Children.Clear();
                List<PageShapes> shapes = new List<PageShapes>(list.undoes);
                list.pages[list.currentPage] = shapes;
                foreach (PageShapes shp in shapes)
                {
                    switch (shp.Box)
                    {
                        case 1:
                            if(!DrawingCanvas.Children.Contains(shp.words))
                            {
                                DrawingCanvas.Children.Add(shp.words);
                            }
                            break;

                        case 2:
                            if (shp.media != null)
                            {
                                DrawingCanvas.Children.Add(shp.media);
                            }
                            else
                            {
                                if (!DrawingCanvas.Children.Contains(shp.rectangle))
                                {
                                    DrawingCanvas.Children.Add(shp.rectangle);
                                }
                                if (shp.image != null)
                                {
                                    shp.rectangle.Fill = shp.image;
                                }
                                else
                                {
                                    if (DarkMode == false)
                                    {
                                        shp.rectangle.Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                                    }
                                    else
                                    {
                                        shp.rectangle.Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255));
                                    }
                                }
                            }
                            break;

                        default:
                            break;
                    }
                }
            }
            else
            {
                list.pages[list.currentPage].Clear();
            }
        }

        private void DrawStart(object sender, PointerRoutedEventArgs e)
        {
            if (BoxType != 0)
            {
                PointerPoint point = e.GetCurrentPoint(DrawingCanvas);
                x1 = (int)point.Position.X;
                y1 = (int)point.Position.Y;
            }
            DrawingCanvas.PointerReleased += DrawEnd;
        }

        private void DrawEnd(object sender, PointerRoutedEventArgs e)
        {
            PointerPoint point = e.GetCurrentPoint(DrawingCanvas);
            x2 = (int)point.Position.X;
            y2 = (int)point.Position.Y;

            if (((x2 != x1) || (y2 != y1)) && ((x1 > 0) && (y1 > 0))) // Just to avoid any weirdness happening, you can't just click and release the same pixel and do something.
            {
                Pages list = Pages.PagesInstance;
                PageShapes shp = new PageShapes();
                if (BoxType == 1)
                {
                    TextBox textBox = new TextBox();
                    if (x2 < x1)
                    {
                        textBox.Width = x1 - x2;
                        Canvas.SetLeft(textBox, x2);
                    }
                    else
                    {
                        textBox.Width = x2 - x1;
                        Canvas.SetLeft(textBox, x1);
                    }
                    if (y2 < y1)
                    {
                        textBox.Height = y1 - y2;
                        Canvas.SetTop(textBox, y2);
                    }
                    else
                    {
                        textBox.Height = y2 - y1;
                        Canvas.SetTop(textBox, y1);
                    }
                    shp.Box = 1;
                    list.pages[list.currentPage].Add(shp);
                    textBox.TextWrapping = TextWrapping.Wrap;

                    textBox.SelectionChanged += SaveText;
                    shp.words = textBox;
                    list.pages[list.currentPage].Add(shp);
                    //List<PageShapes> stamp = new List<PageShapes>(list.pages[list.currentPage]);
                    list.undoes.Add(shp);

                    DrawingCanvas.Children.Add(textBox);
                }
                else if (BoxType == 2)
                {
                    Windows.UI.Xaml.Shapes.Rectangle rect = new Windows.UI.Xaml.Shapes.Rectangle();
                    if (x2 < x1)
                    {
                        rect.Width = x1 - x2;
                        Canvas.SetLeft(rect, x2);
                    }
                    else
                    {
                        rect.Width = x2 - x1;
                        Canvas.SetLeft(rect, x1);
                    }
                    if (y2 < y1)
                    {
                        rect.Height = y1 - y2;
                        Canvas.SetTop(rect, y2);
                    }
                    else
                    {
                        rect.Height = y2 - y1;
                        Canvas.SetTop(rect, y1);
                    }
                    if (DarkMode == false)
                    {
                        rect.Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                    }
                    else
                    {
                        rect.Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255));
                    }
                    rect.Stroke = new SolidColorBrush(Windows.UI.Color.FromArgb(0, 0, 0, 0));
                    shp.Box = 2;

                    rect.PointerPressed += AddMedia;

                    shp.rectangle = rect; // Testing this
                    list.pages[list.currentPage].Add(shp);
                    //List<PageShapes> stamp = new List<PageShapes>(list.pages[list.currentPage]);
                    PageShapes shp2 = new PageShapes(shp);
                    list.undoes.Add(shp2);

                    DrawingCanvas.Children.Add(rect);
                }
                x1 = 0;
                y1 = 0;
                x2 = 0;
                y2 = 0;
                Next.IsEnabled = true;

                list.redoes.Clear();
            }
            DrawingCanvas.PointerReleased -= DrawEnd;
        }

        private void SaveText(object sender, RoutedEventArgs e)
        {
            Pages list = Pages.PagesInstance;
            PageShapes shp = list.pages[list.currentPage].Last();
            shp.words = sender as TextBox;
        }

        private async void AddMedia(object sender, PointerRoutedEventArgs e)
        {
            // Opens File Explorer and sets the view to thumbnail, so it's easier to see the images.
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            // All supported media at the moment.
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".webp");
            openPicker.FileTypeFilter.Add(".gif");
            openPicker.FileTypeFilter.Add(".mp4");

            StorageFile file = await openPicker.PickSingleFileAsync();

            if (file != null)
            {
                // Somehow this works, don't ask me, I didn't think it would...
                Pages list = Pages.PagesInstance;
                PageShapes shp = new PageShapes();
                shp.Box = 2;
                // Making sure the rectangle both exists, and is valid.
                Windows.UI.Xaml.Shapes.Rectangle rect = sender as Windows.UI.Xaml.Shapes.Rectangle;
                if (rect == null)
                    return;

                if (file.FileType == ".mp4")
                {
                    // Create and configure the MediaElement
                    MediaElement mediaElement = new MediaElement
                    {
                        AutoPlay = false,
                        IsLooping = false,
                        Stretch = Stretch.Fill // Ensure the video stretches to fit the rectangle
                    };
                    // Adding basically a "button" to the video, just click it and it will play.
                    mediaElement.PointerPressed += PlayVideo;
                    //mediaElement.MediaEnded += StopVideo;

                    var stream = await file.OpenAsync(FileAccessMode.Read);
                    mediaElement.SetSource(stream, file.ContentType);

                    // Adjust MediaElement size and position to match the rectangle. Getters are so nice!
                    Canvas.SetLeft(mediaElement, Canvas.GetLeft(rect));
                    Canvas.SetTop(mediaElement, Canvas.GetTop(rect));
                    mediaElement.Width = rect.Width;
                    mediaElement.Height = rect.Height;

                    shp.media = mediaElement;
                    DrawingCanvas.Children.Add(mediaElement);
                    
                }
                else
                {
                    // Load the image into a BitmapImage and use it to fill the rectangle.
                    using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read))
                    {
                        BitmapImage bitmapImage = new BitmapImage();
                        await bitmapImage.SetSourceAsync(fileStream);

                        ImageBrush imageBrush = new ImageBrush
                        {
                            ImageSource = bitmapImage
                        };

                        

                        shp.image = imageBrush;
                        rect.Fill = imageBrush;
                        rect.PointerPressed -= AddMedia;
                    }
                }
                shp.rectangle = rect;
                list.pages[list.currentPage].Add(shp);
                //List<PageShapes> stamp = new List<PageShapes>(list.pages[list.currentPage]);
                if (list.undoes.Contains(shp))
                {
                    list.undoes.Remove(shp);
                }
                PageShapes shp2 = new PageShapes(shp);
                list.undoes.Add(shp2);
                list.redoes.Clear();
            }
        }

        private void PlayVideo(object sender, PointerRoutedEventArgs e)
        {
            MediaElement media = sender as MediaElement;
            if (media == null)
                return;

            media.Play();
            // Removing the play button and replacing it with a pause button.
            media.PointerPressed -= PlayVideo;
            media.PointerPressed += StopVideo;
        }

        private void StopVideo(object sender, PointerRoutedEventArgs e)
        {
            MediaElement media = sender as MediaElement;
            if (media == null)
                return;

            media.Pause();
            // Removing the pause button and replacing it with a play button.
            media.PointerPressed -= StopVideo;
            media.PointerPressed += PlayVideo;
        }

        private void UndoButton(object sender, RoutedEventArgs e)
        {
            Pages list = Pages.PagesInstance;
            if (list.undoes.Count > 0)
            {
                DrawingCanvas.Children.Clear();
                list.redoes.Add(list.pages[list.currentPage].Last());
                list.pages[list.currentPage].Remove(list.pages[list.currentPage].Last());
                list.undoes = list.pages[list.currentPage];
                {
                    List<PageShapes> shapes = new List<PageShapes>(list.undoes);
                    list.pages[list.currentPage] = shapes;
                    foreach (PageShapes shp in shapes)
                    {
                        switch (shp.Box)
                        {
                            case 1:
                                if (!DrawingCanvas.Children.Contains(shp.words))
                                {
                                    DrawingCanvas.Children.Add(shp.words);
                                }
                                break;

                            case 2:
                                if (shp.media != null)
                                {
                                    DrawingCanvas.Children.Add(shp.media);
                                }
                                else
                                {
                                    if (!DrawingCanvas.Children.Contains(shp.rectangle))
                                    {
                                        DrawingCanvas.Children.Add(shp.rectangle);
                                    }
                                    if (shp.image != null)
                                    {
                                        shp.rectangle.Fill = shp.image;
                                    }
                                    else
                                    {
                                        if (DarkMode == false)
                                        {
                                            shp.rectangle.Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                                        }
                                        else
                                        {
                                            shp.rectangle.Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255));
                                        }
                                    }
                                }
                                break;

                            default:
                                break;
                        }
                    }
                }
            }
            else
            {
                list.pages[list.currentPage].Clear();
            }
        }

        private void RedoButton(object sender, RoutedEventArgs e)
        {
            Pages list = Pages.PagesInstance;
            if (list.redoes.Count > 0)
            {
                PageShapes shape = list.redoes.Last();
                //list.undoes.Clear();
                list.undoes.Add(list.redoes.Last());
                list.redoes.Remove(list.redoes.Last());
                PageShapes shp = new PageShapes(shape);
                list.pages[list.currentPage].Add(shp);
                switch (shape.Box)
                {
                    case 1:
                        if (!DrawingCanvas.Children.Contains(shp.words))
                        {
                            DrawingCanvas.Children.Add(shp.words);
                        }
                        break;

                    case 2:
                        if (shape.media != null)
                        {
                            DrawingCanvas.Children.Add(shape.media);
                        }
                        else
                        {
                            if (!DrawingCanvas.Children.Contains(shape.rectangle))
                            {
                                DrawingCanvas.Children.Add(shape.rectangle);
                            }
                            if (shape.image != null)
                            {
                                shape.rectangle.Fill = shape.image;
                            }
                            else
                            {
                                if (DarkMode == false)
                                {
                                    shp.rectangle.Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                                }
                                else
                                {
                                    shp.rectangle.Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255));
                                }
                            }
                        }
                        break;

                    default:
                        break;
                }
            }
        }

        private void NextPage(object sender, RoutedEventArgs e)
        {
            Pages list = Pages.PagesInstance;
            list.currentPage++;
            DrawingCanvas.Children.Clear();
            list.undoes.Clear();
            list.redoes.Clear();
            if (list.currentPage == list.pages.Count()) // Actual new page
            {
                List<PageShapes> empty = new List<PageShapes>();
                list.pages.Add(empty);
                Next.IsEnabled = false;
            }
            else // Otherwise it's just an old page
            {
                List<PageShapes> shapes = list.pages[list.currentPage];
                foreach (PageShapes shp in shapes)
                {
                    switch (shp.Box)
                    {
                        case 1:
                            if (!DrawingCanvas.Children.Contains(shp.words))
                            {
                                DrawingCanvas.Children.Add(shp.words);
                            }
                            break;

                        case 2:
                            if (shp.media != null)
                            {
                                DrawingCanvas.Children.Add(shp.media);
                            }
                            else
                            {
                                if (!DrawingCanvas.Children.Contains(shp.rectangle))
                                {
                                    DrawingCanvas.Children.Add(shp.rectangle);
                                }
                                if (shp.image != null)
                                {
                                    shp.rectangle.Fill = shp.image;
                                }
                                else
                                {
                                    if (DarkMode == false)
                                    {
                                        shp.rectangle.Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                                    }
                                    else
                                    {
                                        shp.rectangle.Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255));
                                    }
                                }
                            }
                            break;

                        default:
                            break;
                    }
                }
            }
            if (list.currentPage > 0)
            {
                Back.IsEnabled = true;
            }
            List<PageShapes> temp = new List<PageShapes>(list.pages[list.currentPage]);
            list.undoes = temp;
        }

        private void PrevPage(object sender, RoutedEventArgs e)
        {
            Pages list = Pages.PagesInstance;
            list.currentPage--;
            DrawingCanvas.Children.Clear();
            list.undoes.Clear();
            list.redoes.Clear();
            List<PageShapes> shapes = list.pages[list.currentPage];
            foreach (PageShapes shp in shapes)
            {
                switch (shp.Box)
                {
                    case 1:
                        if (!DrawingCanvas.Children.Contains(shp.words))
                        {
                            DrawingCanvas.Children.Add(shp.words);
                        }
                        break;

                    case 2:
                        if (shp.media != null)
                        {
                            DrawingCanvas.Children.Add(shp.media);
                        }
                        else
                        {
                            if (!DrawingCanvas.Children.Contains(shp.rectangle))
                            {
                                DrawingCanvas.Children.Add(shp.rectangle);
                            }
                            if (shp.image != null)
                            {
                                shp.rectangle.Fill = shp.image;
                            }
                            else
                            {
                                if (DarkMode == false)
                                {
                                    shp.rectangle.Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
                                }
                                else
                                {
                                    shp.rectangle.Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255));
                                }
                            }
                        }
                        break;

                    default:
                        break;
                }
            }
            if (list.currentPage == 0)
            {
               Back.IsEnabled = false;
            }
            Next.IsEnabled = true;
            List<PageShapes> temp = new List<PageShapes>(list.pages[list.currentPage]);
            list.undoes = temp;
        }

        private void SetBoxType(object sender, RoutedEventArgs e)
        {
            if (sender == Text)
            {
                BoxType = 1; // We'll use 1 for text boxes.
            }
            else if (sender == Media)
            {
                BoxType = 2; // We'll use 2 for media boxes.
            }
            else
            {
                BoxType = 0; // Using 0 for nothing.
            }
        }
        private void Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }

        private async void Instructions(object sender, RoutedEventArgs e)
        {
            ContentDialog help = new ContentDialog()
            {
                Title = "How To Use MM Pain(t)",
                Content = "- To place text on the page, simply click the Text button, then click and drag across the page to place a textbox. Now you can type in it!\n" +
                "- To add images or video to the page, click on the Media button, then click and drag across the page to place a Media Box. Click on the box you just created, " +
                "and select the image or video you'd like to place in that box.\n" +
                "- To undo recent changes, simply click the Undo button!\n" +
                "- If you undid something and want it back, be sure to click the Redo button!\n" +
                "- Once you've placed something on a fresh page, you may then navigate to a new page using the -> button, located in the top right of the screen.\n" +
                "- To go back to a previous page, click the <- button that's located in the top right of the screen.",
                CloseButtonText = "OK"
            };

            await help.ShowAsync();
        }

        private async void ButtonHelp(object sender, RoutedEventArgs e)
        {
            ContentDialog help = new ContentDialog()
            {
                Title = "Button Descriptions",
                Content = "Pointer: Sets the current action to Pointer, disables box creation when dragging.\n\n" +
                "Text: Sets the current action to Text, so when you click and drag on the page, it will create a textbox when you release the mouse button.\n\n" +
                "Media: Sets the current action to Media, so when you click and drag on the page, it will create a box for you to put media inside of (images, videos, or gifs).\n\n" +
                "Undo: Removes the most recent change from the page.\n\n" +
                "Redo: Undoes what Undo just did, placing the item back on the page.\n\n" +
                "Dark: Swaps the page style between Dark mode and Light mode.\n\n" +
                "->: Navigates to the next page.\n\n" +
                "<-: Navigates to the previous page.",
                CloseButtonText = "OK"
            };

            await help.ShowAsync();
        }

        private async void aboutClick(object sender, RoutedEventArgs e)
        {
            ContentDialog aboutDialogue = new ContentDialog()
            {
                Title = "About MM Pain(t)",
                Content = "Created by: Thomas, Jonathan, Miles, and Darcy in Spring 2024",
                CloseButtonText = "OK"
            };

            await aboutDialogue.ShowAsync();
        }
    }
}