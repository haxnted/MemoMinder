using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MemoMinder.AllMemoApp;
using MemoMinder.SettingsApp;
namespace MemoMinder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public DataMemo dataMemo = new DataMemo();
        private FileOrganization fileOrg = new FileOrganization();
        private bool IsWindowToggled { get; set; } = true;
        private bool IsWindowPanelShow { get; set; } = false;
        private bool IsWindowMaximize { get; set; } = true;
        public string LastOpenedNameFile { get; set; }
        private double SavedLeft { get; set; }
        private double SavedTop { get; set; }
        private double SavedHeight { get; set; }
        private double SavedWidth { get; set; }
        
        public MainWindow() //constructor for the default window
        {
            InitializeComponent();
            dataMemo = fileOrg.DeserializeSettings(fileOrg.GetLastMemo());
            InitializeWindow(dataMemo);
        }
        public MainWindow(DataMemo memo) //constructor for open window in folder Memo
        {
            InitializeComponent();
            InitializeWindow(memo);
        }
        private void ToggleMemo(object sender, RoutedEventArgs e)
        {
            if (IsWindowToggled)
            {
                ResizeMode = ResizeMode.NoResize;
                Topmost = true;
                IsWindowToggled = false;
            }
            else
            {
                ResizeMode = ResizeMode.CanResize;
                Topmost = false;
                IsWindowToggled = true;
            }
        }
        private void ShowAllMemo(object sender, RoutedEventArgs e)
        {
            MemoBrowser memoBrowser = new MemoBrowser();
            memoBrowser.Show();
        }
        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void WindowKeyDown(object sender, KeyEventArgs e)
        {
            if (IsWindowPanelShow)
            {
                if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.S)
                {
                    GridwindowPanel.Visibility = Visibility.Visible;
                    windowPanelDefinition.Height = new GridLength(25);
                    IsWindowPanelShow = false;
                }
            }
            else if (!IsWindowPanelShow)
            {
                if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.S)
                {
                    GridwindowPanel.Visibility = Visibility.Hidden;
                    windowPanelDefinition.Height = new GridLength(0);
                    IsWindowPanelShow = true;
                }
            }
        }
        private void WindowMoveDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        private void ShowSettings(object sender, RoutedEventArgs e)
        {
            dataMemo = fileOrg.DeserializeSettings(dataMemo.CaptionText);
            SettingsWindow window = new SettingsWindow(this, dataMemo);
            fileOrg.SerializateSettings(dataMemo);
            window.Show();
        }
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
        private void HiddenWindow(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void CreateWindow(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }
        private void InitializeWindow(DataMemo data)
        {
            
            if (data.BackgroundWindowColorPath != null)
            {
                BitmapImage bitmap = new BitmapImage(new Uri(data.BackgroundWindowColorPath));
                ImageBrush imageBrush = new ImageBrush(bitmap);
                Background = imageBrush;
            }
            else
            {
                Background = data.BackgroundWindow;
            }

            if (data.BackgroundTextBoxPath != null)
            {
                BitmapImage bitmap = new BitmapImage(new Uri(data.BackgroundTextBoxPath));
                ImageBrush imageBrush = new ImageBrush(bitmap);
                textbox.Background = imageBrush;
            }
            else
            {
                textbox.Background = dataMemo.BackgroundTextBox;
            }

            textbox.HorizontalScrollBarVisibility = (ScrollBarVisibility)(data.VerticalScrollBarVisibility == true ? Visibility.Visible : Visibility.Hidden);
            textbox.Text = data.MemoText;           
            textbox.FontSize = data.TextBoxFontSize;
            textbox.Foreground = data.TextBoxForeground;
            textbox.Margin = new System.Windows.Thickness(data.TextBoxMargin);
            textbox.FontFamily = data.TextBoxfontFamily;
            captionMemo.FontFamily = data.CaptionFontFamily;
            captionMemo.FontSize = data.CaptionFontSize;
            captionMemo.Height = data.IsCaptionActive == true ? 25 : 0;
            captionMemo.Foreground = data.CaptionForeground;
            captionMemo.TextDecorations = data.IsUnderlineCaption == true ? TextDecorations.Underline : null;
            captionMemo.Text = data.CaptionText;
            Height = data.HeightWindow;
            Width = data.WidthWindow;
        }

        private void textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //tempNameFile = textbox.Text;
        }
    }
}
