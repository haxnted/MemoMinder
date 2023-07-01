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
        private string LastOpenedName { get; set; }
        private bool isPointAdd { get; set; } = false;

        public MainWindow() //constructor for the default window
        {
            InitializeComponent();
            FileOrganization.IsCreatedNewWindow = false;

            LastOpenedName = fileOrg.GetLastOpenedNote();

            dataMemo = fileOrg.DeserializeSettings(LastOpenedName);

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
            //Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            if (MemoBrowserManager.Instance.CanOpenAllMemoWindow())
            {
                MemoBrowser memoBrowser = new MemoBrowser();
                memoBrowser.Closed += (s,args) => MemoBrowserManager.Instance.DecrementWindowCount();
                MemoBrowserManager.Instance.IncrementWindowCount();
                memoBrowser.Show();
                //this.Close();
            }
            else
            {
                return;
            }
        }
        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            dataMemo.MemoText = textbox.Text;
            dataMemo.CaptionText = captionMemo.Text;
            FileOrganization.IsSaveName = true;
            fileOrg.SerializateSettings(dataMemo, LastOpenedName);
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
            if (SettingsWindowManager.Instance.CanOpenSettingsWindow())
            {
                dataMemo = fileOrg.DeserializeSettings(LastOpenedName);
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
            
            fileOrg.CreateDefaultNote();

        }
        private void InitializeWindow(DataMemo data)
        {
            
            if (!string.IsNullOrEmpty(data.BackgroundWindowColorPath))
            {
                BitmapImage bitmap = new BitmapImage(new Uri(data.BackgroundWindowColorPath));
                ImageBrush imageBrush = new ImageBrush(bitmap);
                Background = imageBrush;
            }
            else
            {
                Background = data.BackgroundWindow;
            }

            if (!string.IsNullOrEmpty(data.BackgroundTextBoxPath))
            {
                BitmapImage bitmap = new BitmapImage(new Uri(data.BackgroundTextBoxPath));
                ImageBrush imageBrush = new ImageBrush(bitmap);
                textbox.Background = imageBrush;
            }
            else
            {
                textbox.Background = dataMemo.BackgroundTextBox;
            }
            if (data.IsCaptionActive)
            {
                caption.Height = new GridLength(25);
            }
            else
            {
                caption.Height = new GridLength(0);
            }
            textbox.HorizontalScrollBarVisibility = (ScrollBarVisibility)(data.VerticalScrollBarVisibility == true ? Visibility.Visible : Visibility.Hidden);
            textbox.Text = data.MemoText;           
            textbox.FontSize = data.TextBoxFontSize;
            textbox.Foreground = data.TextBoxForeground;
            textbox.Margin = new Thickness(data.TextBoxMargin);
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
            string Text = textbox.Text;
            int CountCircles = 0;
            bool isCheacked = Text.Contains(".-3");


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


            if (previousCursorPosition <= Text.Length)
            {
                textbox.SelectionStart = previousCursorPosition;
            }
            else
            {
                textbox.SelectionStart = Text.Length;
            }
        }

    }
}
