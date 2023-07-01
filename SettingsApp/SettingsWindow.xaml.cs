using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;

namespace MemoMinder.SettingsApp
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private DataMemo dataMemo;
        private readonly MainWindow mainWindow;
        private Dictionary<int, FontFamily> fontDictionary = new Dictionary<int, FontFamily>();
        private string LastOpenedNote;
        public SettingsWindow(MainWindow mainWindow, DataMemo dataMemo, string LastOpenedNote)
        {
            this.mainWindow = mainWindow;
            this.dataMemo = dataMemo;
            this.LastOpenedNote = LastOpenedNote;
            InitializeComponent();
            LoadDictionary(fontDictionary);
            Initialize();

        }
        private void Submit(object sender, RoutedEventArgs e)
        {
            mainWindow.Hide();

            string BackWindow = backgroundWindowSettings.Text;
            string BackTextBox = backgroundTextBoxSettings.Text; 
           
            if (BackWindow[0] == '#' && BackWindow.Length > 9 || 
                BackTextBox[0] == '#' && BackTextBox.Length > 9)
            {
                MessageBox.Show("Неверное поле.");
                return;
            }
            ApplySizeWindow();
            ApplyFontType(fontTypeComboBox);
            ApplyBackgroundWindow();
            ApplyBackgroundTextBox();
            ApplyTextBoxForeground();
            ApplyTextBoxMargin();
            ApplyVerticalScrollBarVisibility();
            ApplyIsToggleWindow();
            ApplyIsCaptionActive();
            ApplyFontsizeTextBox();
            ApplyFontsizeMemo();
            ApplyUnderlineMemo();
            ApplyCaptionForeground();
            ApplyCaptionFontType(captionFontFamilySettings);



            FileOrganization fileOrg = new FileOrganization();
            FileOrganization.IsSaveName = true;
            fileOrg.SerializateSettings(dataMemo, LastOpenedNote);
            mainWindow.dataMemo = dataMemo;

            this.Close();
            mainWindow.Show();
            
        }
        private void ApplyBackgroundWindow()
        {

            try
            {
                if (Convert.ToString(backgroundWindowSettings.Text) != Convert.ToString(dataMemo.BackgroundWindow) ||
                    Convert.ToString(backgroundWindowSettings.Text) != Convert.ToString(dataMemo.BackgroundWindowColorPath))
                {
                    if (File.Exists(backgroundWindowSettings.Text))
                    {
                        ImageBrush imageBrush = new ImageBrush();
                        imageBrush.ImageSource = new BitmapImage(new Uri(backgroundWindowSettings.Text));
                        //SetWindowBackground(imageBrush);
                        mainWindow.Background = imageBrush;
                        dataMemo.BackgroundWindowColorPath = backgroundWindowSettings.Text;


                    }
                    else
                    {
                        Color color = (Color)ColorConverter.ConvertFromString(backgroundWindowSettings.Text);
                        SolidColorBrush brush = new SolidColorBrush(color);
                        //SetWindowBackground(brush);
                        mainWindow.Background = brush;
                        dataMemo.BackgroundWindow = brush;
                    }
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error backgroundWindow: " + ex);
                return;
            }
        }
        private void ApplyBackgroundTextBox()
        {
            try
            {
                bool isChecked = isCaptionActiveSettings.IsChecked ?? false; //captionBackground

                if (backgroundTextBoxSettings.Text.ToLower() == "transparent")
                {
                    dataMemo.BackgroundTextBoxPath = null;
                    dataMemo.BackgroundTextBox = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Transparent"));
                    mainWindow.textbox.Background = dataMemo.BackgroundTextBox;
                }
                else
                {

                    if (File.Exists(backgroundTextBoxSettings.Text))
                    {
                        dataMemo.BackgroundWindow = null;
                        BitmapImage bitmap = new BitmapImage(new Uri(backgroundTextBoxSettings.Text));
                        ImageBrush imageBrush = new ImageBrush(bitmap);
                        mainWindow.textbox.Background = imageBrush;
                        dataMemo.BackgroundTextBoxPath = backgroundTextBoxSettings.Text;

                        if (isChecked)
                        {
                            mainWindow.GridwindowPanel.Background = imageBrush;
                        }
                    }

                    else
                    {
                        dataMemo.BackgroundTextBoxPath = null;
                        Color color = (Color)ColorConverter.ConvertFromString(backgroundTextBoxSettings.Text);
                        dataMemo.BackgroundTextBox = new SolidColorBrush(color);
                        mainWindow.textbox.Background = dataMemo.BackgroundTextBox;
                        if (isChecked)
                        {
                            mainWindow.GridwindowPanel.Background = dataMemo.BackgroundTextBox;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error backgroundTextBox: " + ex);
                return;
            }
        }
        private void ApplyTextBoxForeground()
        {
            try
            {
                if (new SolidColorBrush((Color)ColorConverter.ConvertFromString(textBoxForegroundSettings.Text)) != dataMemo.TextBoxForeground)
                {
                    dataMemo.TextBoxForeground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(textBoxForegroundSettings.Text));
                    mainWindow.textbox.Foreground = dataMemo.TextBoxForeground;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Foreground" + ex);
                return;
            }
        }
        private void ApplyCaptionForeground()
        {
            try
            {
                if (new SolidColorBrush((Color)ColorConverter.ConvertFromString(captionForegroundSettings.Text)) != dataMemo.CaptionForeground)
                {
                    dataMemo.CaptionForeground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(captionForegroundSettings.Text));
                    mainWindow.captionMemo.Foreground = dataMemo.CaptionForeground;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Caption Foreground" + ex);
                return;
            }
        }
        private void ApplyFontsizeTextBox()
        {
            try
            {
                if (Convert.ToDouble(fontSize.Text) != dataMemo.TextBoxFontSize)
                {
                    mainWindow.textbox.FontSize = Convert.ToDouble(fontSize.Text);
                    dataMemo.TextBoxFontSize = Convert.ToDouble(fontSize.Text);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("FontsizeTextBox" + ex);
            }
        }
        private void ApplyFontsizeMemo()
        {
            try
            {
                if (Convert.ToDouble(captionFontSizeSettings.Text) != dataMemo.CaptionFontSize)
                {
                    mainWindow.captionMemo.FontSize = Convert.ToDouble(captionFontSizeSettings.Text);
                    dataMemo.CaptionFontSize = Convert.ToDouble(captionFontSizeSettings.Text);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("FontsizeMemo" + ex);
            }
        }
        private void ApplyUnderlineMemo()
        {
            bool isChecked = isUnderlineActiveSettings.IsChecked ?? false;
            if (isChecked != dataMemo.IsUnderlineCaption)
            {
                if (isChecked)
                {
                    mainWindow.captionMemo.TextDecorations = TextDecorations.Underline;
                }
                else
                {
                    mainWindow.captionMemo.TextDecorations = null;
                }
                dataMemo.IsUnderlineCaption = isChecked;
            }
        }
        private void ApplyTextBoxMargin()
        {
            try
            {
                if (Convert.ToDouble(textBoxMarginSettings.Text) != dataMemo.TextBoxMargin)
                {
                    dataMemo.TextBoxMargin = Convert.ToDouble(textBoxMarginSettings.Text);
                    mainWindow.textbox.Margin = new System.Windows.Thickness(dataMemo.TextBoxMargin);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Margin" + ex);
                return;
            }
        }
        private void ApplyVerticalScrollBarVisibility()
        {
            try
            {
                if (verticalScrollBarVisibilitySettings.IsChecked.Value != dataMemo.VerticalScrollBarVisibility)
                {
                    dataMemo.VerticalScrollBarVisibility = verticalScrollBarVisibilitySettings.IsChecked.Value;
                    mainWindow.textbox.VerticalScrollBarVisibility = dataMemo.VerticalScrollBarVisibility ? ScrollBarVisibility.Visible : ScrollBarVisibility.Disabled;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error ScrollBar" + ex);
                return;
            }
        }
        private void ApplyFontType(ComboBox comboBox)
        {
            string? selectedValue = comboBox.SelectedValue as string;
            if (selectedValue != null)
            {
                dataMemo.TextBoxfontFamily = mainWindow.textbox.FontFamily;
                mainWindow.textbox.FontFamily = new FontFamily(selectedValue);
            }
            else
            {
                MessageBox.Show("FontType Error: " + selectedValue);
            }
        }
        private void ApplyCaptionFontType(ComboBox comboBox)
        {
            string? selectedValue = comboBox.SelectedValue as string;
            if (selectedValue != null)
            {
                dataMemo.CaptionFontFamily = mainWindow.captionMemo.FontFamily;
                mainWindow.captionMemo.FontFamily = new FontFamily(selectedValue);
            }
            else
            {
                MessageBox.Show("FontType Error: " + selectedValue);
            }
        }
        private void ApplyIsToggleWindow()
        {
            bool isChecked = isVisibleAboveAppsSettings.IsChecked ?? false;

            if (isChecked)
            {
                mainWindow.ResizeMode = ResizeMode.NoResize;
                mainWindow.Topmost = true;
            }
            else
            {
                mainWindow.ResizeMode = ResizeMode.CanResize;
                mainWindow.Topmost = false;
            }
            dataMemo.IsToggleWindow = isChecked;

            //try
        }
        private void ApplySizeWindow()
        {

            dataMemo.HeightWindow = Convert.ToDouble(heightWindowSettings.Text);
            dataMemo.WidthWindow = Convert.ToDouble(widthWindowSettings.Text);

            mainWindow.Height = dataMemo.HeightWindow;
            mainWindow.Width = dataMemo.WidthWindow;
        }
        private void ApplyIsCaptionActive()
        {
            bool isChecked = isCaptionActiveSettings.IsChecked ?? false;

            if (isChecked)
            {
                mainWindow.caption.Height = new GridLength(25);
            }
            else
            {
                mainWindow.caption.Height = new GridLength(0);
            }
            dataMemo.IsCaptionActive = isChecked;

        }
        private void backgroundTextBoxSettings_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (File.Exists(backgroundTextBoxSettings.Text))
                {
                    BitmapImage bitmap = new BitmapImage(new Uri(backgroundTextBoxSettings.Text));
                    ImageBrush imageBrush = new ImageBrush(bitmap);
                    ChangebackgroundTextBoxSettings.Background = imageBrush;
                }
                else
                {
                    ChangebackgroundTextBoxSettings.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(backgroundTextBoxSettings.Text));
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }
        private void backgroundWindowColorSettings_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (File.Exists(backgroundWindowSettings.Text))
                {
                    BitmapImage bitmap = new BitmapImage(new Uri(backgroundWindowSettings.Text));
                    ImageBrush imageBrush = new ImageBrush(bitmap);
                    ChangebackgroundWindowSettings.Background = imageBrush;
                }
                else
                {
                    ChangebackgroundWindowSettings.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(backgroundWindowSettings.Text));
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }

        }
        private void Initialize()
        {
            LoadTextBox();
            LoadCheckBox();
            LoadComboBox();
        }
        private void LoadTextBox()
        {
            //if (string.IsNullOrEmpty(dataMemo.BackgroundWindowColorPath))
            //{
            //    settingsWindow.backgroundWindowSettings.Text = Convert.ToString(dataMemo.BackgroundWindow);
            //}

            backgroundWindowSettings.Text =  string.IsNullOrEmpty(dataMemo.BackgroundWindowColorPath) ? Convert.ToString(dataMemo.BackgroundWindow) : Convert.ToString(dataMemo.BackgroundWindowColorPath);


            backgroundTextBoxSettings.Text =  dataMemo.BackgroundTextBoxPath ?? Convert.ToString(dataMemo.BackgroundTextBox);
            textBoxForegroundSettings.Text = Convert.ToString(dataMemo.TextBoxForeground);
            captionForegroundSettings.Text = Convert.ToString(dataMemo.CaptionForeground);
            captionFontSizeSettings.Text = Convert.ToString(dataMemo.CaptionFontSize);
            textBoxMarginSettings.Text = Convert.ToString(dataMemo.TextBoxMargin);

            heightWindowSettings.Text = Convert.ToString(dataMemo.HeightWindow);
            widthWindowSettings.Text = Convert.ToString(dataMemo.WidthWindow);

            fontSize.Text = Convert.ToString(dataMemo.TextBoxFontSize);
        }
        private void LoadCheckBox()
        {
            verticalScrollBarVisibilitySettings.IsChecked = dataMemo.VerticalScrollBarVisibility == true ? true : false;
            isCaptionActiveSettings.IsChecked = dataMemo.IsCaptionActive;
            isUnderlineActiveSettings.IsChecked = dataMemo.IsUnderlineCaption;
            isVisibleAboveAppsSettings.IsChecked = dataMemo.IsToggleWindow;
        }
        private void LoadComboBox()
        {
            InitFontTypeComboBox(fontTypeComboBox, dataMemo.TextBoxfontFamily);
            InitFontTypeComboBox(captionFontFamilySettings, dataMemo.CaptionFontFamily);
        }
        private void LoadDictionary(Dictionary<int, FontFamily> dictionary)
        {
            dictionary.Add(1, new FontFamily("Arial"));
            dictionary.Add(2, new FontFamily("Times New Roman"));
            dictionary.Add(3, new FontFamily("Verdana"));
            dictionary.Add(4, new FontFamily("Calibri"));
            dictionary.Add(5, new FontFamily("Cambria"));
            dictionary.Add(6, new FontFamily("Georgia"));
            dictionary.Add(7, new FontFamily("Helvetica"));
            dictionary.Add(8, new FontFamily("Lucida Sans Unicode"));
            dictionary.Add(9, new FontFamily("Palatino Linotype"));
            dictionary.Add(10, new FontFamily("Segoe UI"));
            dictionary.Add(11, new FontFamily("Tahoma"));
            dictionary.Add(12, new FontFamily("Trebuchet MS"));
            dictionary.Add(13, new FontFamily("Century Gothic"));
            dictionary.Add(14, new FontFamily("Garamond"));
            dictionary.Add(15, new FontFamily("Book Antiqua"));
            dictionary.Add(16, new FontFamily("Franklin Gothic Medium"));
            dictionary.Add(17, new FontFamily("Rockwell"));
            dictionary.Add(18, new FontFamily("Baskerville Old Face"));
            dictionary.Add(19, new FontFamily("Consolas"));
            dictionary.Add(20, new FontFamily("Courier New"));
        }
        private void InitFontTypeComboBox(ComboBox comboBox, FontFamily index)
        {
            string[] fontName = new string[fontDictionary.Count];
            int i = 0;

            foreach (var item in fontDictionary.Values)
            {
                fontName[i] = Convert.ToString(item);
                i++;
            }

            int position = -1;
            foreach (var kvp in fontDictionary)
            {
                if (kvp.Value.Equals(index))
                {
                    position = kvp.Key;
                    break;
                }
            }
            comboBox.ItemsSource = fontName;
            comboBox.SelectedIndex = position - 1;
        }

    }
}

