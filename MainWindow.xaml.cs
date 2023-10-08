using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MemoMinder.AllMemoApp;
using System.Windows.Shell;
using MemoMinder.SettingsApp;
using System.Text.RegularExpressions;
using System.Linq;

namespace MemoMinder
{
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
        private bool IsDragging { get; set; }
        private Point DragOffset { get; set; }
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
                IsDragging = true;
                DragOffset = e.GetPosition(this);
                CaptureMouse();
            }
        }
        private void MainWindow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IsDragging)
            {
                IsDragging = false;
                ReleaseMouseCapture();
            }
        }
        private void MainWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsDragging)
            {
                Point currentPosition = e.GetPosition(this);
                double offsetX = currentPosition.X - DragOffset.X;
                double offsetY = currentPosition.Y - DragOffset.Y;

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
            fileOrg.SerializateSettings(dataMemo, LastOpenedName, false, false);
            if (MemoBrowserManager.Instance.CanOpenAllMemoWindow())
            {
                MemoBrowser memoBrowser = new MemoBrowser();
                memoBrowser.Closed += (s, args) => MemoBrowserManager.Instance.DecrementWindowCount();
                MemoBrowserManager.Instance.IncrementWindowCount();
                memoBrowser.ShowDialog();
                LastOpenedName = fileOrg.GetLastOpenedNote();
                InitializeWindow(dataMemo);
            }
            else
                return;

        }
        private void CreateWindow(object sender, RoutedEventArgs e)
        {
            fileOrg.SerializateSettings(dataMemo, LastOpenedName, false, false);
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

                dataMemo.MemoText = textbox.Text;
                dataMemo.CaptionText = captionMemo.Text;
                SettingsWindow window = new SettingsWindow(this, dataMemo, LastOpenedName);
                window.Closed += (s, args) => SettingsWindowManager.Instance.DecrementWindowCount();
                SettingsWindowManager.Instance.IncrementWindowCount();

                window.Show();
            }
            else
                return;

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
            byte minvalue = 5;
            byte maxvalue = 20;
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.LeftShift){
                dataMemo.MemoText = textbox.Text;
                dataMemo.CaptionText = captionMemo.Text;
                fileOrg.SerializateSettings(dataMemo, LastOpenedName, false, true);
                return;
            }
            else if (IsWindowPanelShow)
            {
                if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.H)
                {
                    GridwindowPanel.Visibility = Visibility.Visible;
                    windowPanelDefinition.Height = new GridLength(maxvalue);
                    IsWindowPanelShow = false;
                }
                return;
            }
            else if (!IsWindowPanelShow)
            {
                if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.H)
                {
                    GridwindowPanel.Visibility = Visibility.Hidden;
                    windowPanelDefinition.Height = new GridLength(minvalue);
                    IsWindowPanelShow = true;
                }
                return;
            }

        }
        public void InitializeWindow(DataMemo dataMemo)
        {

            if (!string.IsNullOrEmpty(dataMemo.BackgroundWindowColorPath))
            {
                BitmapImage bitmap = new BitmapImage(new Uri(dataMemo.BackgroundWindowColorPath));
                ImageBrush imageBrush = new ImageBrush(bitmap);
                Background = imageBrush;
            }
            else 
                Background = dataMemo.BackgroundWindow;


            if (!string.IsNullOrEmpty(dataMemo.BackgroundTextBoxPath))
            {
                BitmapImage bitmap = new BitmapImage(new Uri(dataMemo.BackgroundTextBoxPath));
                ImageBrush imageBrush = new ImageBrush(bitmap);
                textbox.Background = imageBrush;
            }
            else 
                textbox.Background = MainWindow.dataMemo.BackgroundTextBox;

            if (dataMemo.IsCaptionActive) 
                caption.Height = new GridLength(17);
            else 
                caption.Height = new GridLength(0);

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
            if (dataMemo.VerticalScrollBarVisibility == true) 
                textbox.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            else 
                textbox.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;

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
            int tempCursor = textbox.SelectionStart;
            MatchCollection matches = Regex.Matches(textbox.Text, @"\.-(\d+) ");
            if (matches.Count == 0)
            {
                return;
            }
            int CountCircles = Convert.ToInt32(matches[0].Groups[1].Value);

            string temptext = Regex.Replace(textbox.Text, @"\.-(\d+) ", "");
            textbox.Text = Regex.Replace(textbox.Text, @"\.-(\d+) ", string.Concat(Enumerable.Repeat("● \n", CountCircles)));

            if (tempCursor <= textbox.Text.Length) textbox.SelectionStart = temptext.Length + 2;
            else textbox.SelectionStart = textbox.Text.Length;
            dataMemo.MemoText = textbox.Text;
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            dataMemo.WidthWindow = e.NewSize.Width;
            dataMemo.HeightWindow = e.NewSize.Height;
        }

        private void textbox_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            TextBox temp = (TextBox)sender;
            float scrollvalue = 1.0F; 
            if (e.Delta > 0)
                temp.FontSize += scrollvalue;
            else
            {
                if (temp.FontSize - scrollvalue <= 1.0) //1.0 text size threshold, cuz if e.Delta < 0.4 - program crashes 
                    return;
                temp.FontSize -= scrollvalue;
            }
            dataMemo.TextBoxFontSize = temp.FontSize;
        }
    }
    
} 

