using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MemoMinder.AllMemoApp;
using System.Windows.Shell;
using MemoMinder.SettingsApp;

namespace MemoMinder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static DataMemo dataMemo = new DataMemo();
        private FileOrganization fileOrg = new FileOrganization();
        private bool IsWindowPanelShow { get; set; } = false;
        private bool IsWindowMaximize { get; set; } = true;
        private double SavedLeft { get; set; }
        private double SavedTop { get; set; }
        private double SavedHeight { get; set; }
        private double SavedWidth { get; set; }
        private string LastOpenedName { get; set; }
        private bool isDragging { get; set; }
        private Point dragOffset { get; set; }
        public MainWindow() 
        {
            InitializeComponent();
            var windowChrome = CreateWindowChrome();

            WindowChrome.SetWindowChrome(this, windowChrome);

            LastOpenedName = fileOrg.GetLastOpenedNote();
            dataMemo = fileOrg.DeserializeSettings(LastOpenedName);
            InitializeWindow(dataMemo);

            MouseLeftButtonDown += MainWindow_MouseLeftButtonDown;
            MouseLeftButtonUp += MainWindow_MouseLeftButtonUp;
            MouseMove += MainWindow_MouseMove;
        }
        private void MainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                isDragging = true;
                dragOffset = e.GetPosition(this);
                CaptureMouse();
            }
        }
        private void MainWindow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isDragging)
            {
                isDragging = false;
                ReleaseMouseCapture();
            }
        }
        private void MainWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point currentPosition = e.GetPosition(this);
                double offsetX = currentPosition.X - dragOffset.X;
                double offsetY = currentPosition.Y - dragOffset.Y;

                Left += offsetX;
                Top += offsetY;
            }
        }
        private WindowChrome CreateWindowChrome()
        {
            var windowChrome = new WindowChrome
            {
                ResizeBorderThickness = new Thickness(8),
                CaptionHeight = 0,
                CornerRadius = new CornerRadius(0),
            };
            return windowChrome;
        }
        private void ShowAllMemo(object sender, RoutedEventArgs e)
        {
            if (MemoBrowserManager.Instance.CanOpenAllMemoWindow())
            {
                MemoBrowser memoBrowser = new MemoBrowser();
                memoBrowser.Closed += (s,args) => MemoBrowserManager.Instance.DecrementWindowCount();
                MemoBrowserManager.Instance.IncrementWindowCount();
                memoBrowser.ShowDialog();
                LastOpenedName = fileOrg.GetLastOpenedNote();
                InitializeWindow(dataMemo);
            }
            else
            {
                return;
            }
        }
        private void CreateWindow(object sender, RoutedEventArgs e)
        {
            fileOrg.CreateDefaultNote();
            LastOpenedName = fileOrg.GetLastOpenedNote();
            InitializeWindow(dataMemo);
        }
        private void ToggleMemo(object sender, RoutedEventArgs e)
        {
            if (Topmost)
            {
                ResizeMode = ResizeMode.CanResize;
                Topmost = false;
            }
            else
            {
                ResizeMode = ResizeMode.NoResize;
                Topmost = true;
            }
        }
        private void ShowSettings(object sender, RoutedEventArgs e)
        {
            if (SettingsWindowManager.Instance.CanOpenSettingsWindow())
            {
                
                //dataMemo = fileOrg.DeserializeSettings(LastOpenedName);
                SettingsWindow window = new SettingsWindow(this, dataMemo, LastOpenedName);
                window.Closed += (s, args) => SettingsWindowManager.Instance.DecrementWindowCount();
                SettingsWindowManager.Instance.IncrementWindowCount();

                window.Show();
            }
            else
            {
                return;
            }
        }
        private void DeleteFile(object sender, RoutedEventArgs e)
        {
            fileOrg.DeleteFile(LastOpenedName);
            InitializeWindow(dataMemo);
            LastOpenedName = fileOrg.GetLastOpenedNote();
        }
        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            dataMemo.MemoText = textbox.Text;
            dataMemo.CaptionText = captionMemo.Text;

            fileOrg.SerializateSettings(dataMemo, LastOpenedName, false, true);
            this.Close();
        }
        private void HiddenWindow(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
        private void MaximizeWindow(object sender, RoutedEventArgs e)
        {
            if (IsWindowMaximize)
            {
                SavedLeft = Left;
                SavedTop = Top;
                SavedHeight = Height;
                SavedWidth = Width;

                Left = SystemParameters.WorkArea.Left;
                Top = SystemParameters.WorkArea.Top;
                Height = SystemParameters.WorkArea.Height - (SystemParameters.PrimaryScreenHeight - SystemParameters.WorkArea.Height);
                Width = SystemParameters.WorkArea.Width;

                IsWindowMaximize = false;
            }
            else
            {     
                Left = SavedLeft;
                Top = SavedTop;
                Height = SavedHeight;
                Width = SavedWidth;

                IsWindowMaximize = true;
            }
        }
        private void WindowKeyDown(object sender, KeyEventArgs e)
        {
            if (IsWindowPanelShow)
            {
                if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.H)
                {
                    GridwindowPanel.Visibility = Visibility.Visible;
                    windowPanelDefinition.Height = new GridLength(17);
                    IsWindowPanelShow = false;
                }
            }
            else if (!IsWindowPanelShow)
            {
                if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.H)
                {
                    GridwindowPanel.Visibility = Visibility.Hidden;
                    windowPanelDefinition.Height = new GridLength(0);
                    IsWindowPanelShow = true;
                }
            }

        }
        public  void InitializeWindow(DataMemo dataMemo)
        {

            if (!string.IsNullOrEmpty(dataMemo.BackgroundWindowColorPath))
            {
                BitmapImage bitmap = new BitmapImage(new Uri(dataMemo.BackgroundWindowColorPath));
                ImageBrush imageBrush = new ImageBrush(bitmap);
                Background = imageBrush;
            }
            else Background = dataMemo.BackgroundWindow;
            

            if (!string.IsNullOrEmpty(dataMemo.BackgroundTextBoxPath))
            {
                BitmapImage bitmap = new BitmapImage(new Uri(dataMemo.BackgroundTextBoxPath));
                ImageBrush imageBrush = new ImageBrush(bitmap);
                textbox.Background = imageBrush;
            }
            else textbox.Background = MainWindow.dataMemo.BackgroundTextBox;
            
            if (dataMemo.IsCaptionActive) caption.Height = new GridLength(17);
            else  caption.Height = new GridLength(0);

            if (dataMemo.IsToggleWindow)
            {
                ResizeMode = ResizeMode.NoResize;
                Topmost = true;
            }
            else
            {
                ResizeMode = ResizeMode.CanResize;
                Topmost = false;
            }
            if (dataMemo.VerticalScrollBarVisibility == true) textbox.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;

            else textbox.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;

            textbox.Text = dataMemo.MemoText;           
            textbox.FontSize = dataMemo.TextBoxFontSize;
            textbox.Foreground = dataMemo.TextBoxForeground;
            textbox.Margin = new Thickness(dataMemo.TextBoxMargin);
            textbox.FontFamily = dataMemo.TextBoxfontFamily;
            captionMemo.FontFamily = dataMemo.CaptionFontFamily;
            captionMemo.FontSize = dataMemo.CaptionFontSize;
            captionMemo.Height = dataMemo.IsCaptionActive == true ? 25 : 0;
            captionMemo.Foreground = dataMemo.CaptionForeground;
            captionMemo.TextDecorations = dataMemo.IsUnderlineCaption == true ? TextDecorations.Underline : null;
            captionMemo.Text = dataMemo.CaptionText;
            Height = dataMemo.HeightWindow;
            Width = dataMemo.WidthWindow;
        }
        private void textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string Text = textbox.Text;
            int CountCircles;
            int previousCursorPosition = textbox.SelectionStart;

            for (int i = 0; i < Text.Length - 1; i++)
            {
                if (i + 2 <= Text.Length - 1)
                {
                    string tempText = "";
                    if (Text[i] == '.' && Text[i + 1] == '-' && int.TryParse(Convert.ToString(Text[i + 2]), out CountCircles))
                    {
                        if (CountCircles <= 0) { return; }
                        else
                        {
                            for (int j = 0; j < CountCircles; j++)
                            {
                                tempText = tempText + "● " + '\n';
                            }
                            Text = Text.Substring(0, i) + tempText + Text.Substring(i + 3);
                        }
                    }
                }
            }
            textbox.Text = Text;
            dataMemo.MemoText = textbox.Text;
            if (previousCursorPosition <= Text.Length) textbox.SelectionStart = previousCursorPosition;
            else textbox.SelectionStart = Text.Length;
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //Height = e.NewSize.Width;
            //Width = e.NewSize.Width;
            dataMemo.WidthWindow = e.NewSize.Width;
            dataMemo.HeightWindow = e.NewSize.Height;
        }
    }
}
