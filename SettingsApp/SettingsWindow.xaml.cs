using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MemoMinder.SettingsApp
{
    
    public partial class SettingsWindow : Window
    {
        private DataMemo dataMemo;
        private readonly MainWindow mainWindow;
        private Dictionary<int, FontFamily> fontDictionary = new Dictionary<int, FontFamily>() {
            {1, new FontFamily("Arial") },
            {2, new FontFamily("Times New Roman") },
            {3, new FontFamily("Verdana") },
            {4, new FontFamily("Calibri")},
            {5, new FontFamily("Cambria")},
            {6, new FontFamily("Georgia")},
            {7, new FontFamily("Helvetica")},
            {8, new FontFamily("Lucida Sans Unicode")},
            {9, new FontFamily("Palatino Linotype")},
            {10, new FontFamily("Segoe UI")},
            {11, new FontFamily("Tahoma")},
            {12, new FontFamily("Trebuchet MS")},
            {13, new FontFamily("Century Gothic")},
            {14, new FontFamily("Garamond")},
            {15, new FontFamily("Book Antiqua")},
            {16, new FontFamily("Franklin Gothic Medium")},
            {17, new FontFamily("Rockwell")},
            {18, new FontFamily("Baskerville Old Face")},
            {19, new FontFamily("Consolas")},
            {20, new FontFamily("Courier New")},
        };

        private string LastOpenedNote;
        public SettingsWindow(MainWindow mainWindow, DataMemo dataMemo, string LastOpenedNote)
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            this.mainWindow = mainWindow;
            this.dataMemo = dataMemo;
            this.LastOpenedNote = LastOpenedNote;
            InitializeComponent();
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
            if (isSaveTemplate.IsChecked == true)
                SaveDataToDefaultFile();
            
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
            fileOrg.SerializateSettings(dataMemo, LastOpenedNote, false, true);
            this.Close();
            mainWindow.Show();
            mainWindow.InitializeWindow(dataMemo);

        }
        private void SaveDataToDefaultFile()
        {
            string fileName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),"MemoMinder", "DefaultThemeNote.json");

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true,
                ReferenceHandler = ReferenceHandler.Preserve
            };

            options.Converters.Add(new BrushConverter());
            options.Converters.Add(new FontFamilyConverter());

            //fix prblem where user save default memo, and text with caption removed
            DataMemo savememo = (DataMemo)dataMemo.Clone();
            savememo.MemoText = "Text";
            savememo.CaptionText = "Caption";

            string textToFile = JsonSerializer.Serialize(savememo, options);

            using (StreamWriter writer = new StreamWriter(fileName))
            {
                writer.Write(textToFile);
                writer.Close();
            }
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

                        dataMemo.BackgroundWindowColorPath = backgroundWindowSettings.Text;
                        dataMemo.BackgroundWindow = null;
                    }
                    else
                    {
                        Color color = (Color)ColorConverter.ConvertFromString(backgroundWindowSettings.Text);
                        SolidColorBrush brush = new SolidColorBrush(color);

                        dataMemo.BackgroundWindowColorPath = null;
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
                MessageBox.Show($"Error save background Window.\n Exception:{ex}");
                return;
            }
        }
        private void ApplyBackgroundTextBox()
        {
            try
            {
                if (backgroundTextBoxSettings.Text.ToLower() == "transparent")
                {
                    dataMemo.BackgroundTextBoxPath = null;
                    dataMemo.BackgroundTextBox = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Transparent"));
                }
                else
                {

                    if (File.Exists(backgroundTextBoxSettings.Text))
                    {
                        BitmapImage bitmap = new BitmapImage(new Uri(backgroundTextBoxSettings.Text));
                        ImageBrush imageBrush = new ImageBrush(bitmap);
                        dataMemo.BackgroundTextBox = null;
                        dataMemo.BackgroundTextBoxPath = backgroundTextBoxSettings.Text;
                    }

                    else
                    {
                        dataMemo.BackgroundTextBoxPath = null;
                        Color color = (Color)ColorConverter.ConvertFromString(backgroundTextBoxSettings.Text);
                        dataMemo.BackgroundTextBox = new SolidColorBrush(color);
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
                dataMemo.TextBoxFontSize = Convert.ToDouble(fontSize.Text);
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
                dataMemo.CaptionFontSize = Convert.ToDouble(captionFontSizeSettings.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("FontsizeMemo" + ex);
            }
        }
        private void ApplyTextBoxMargin()
        {
            try
            {
                dataMemo.TextBoxMargin = Convert.ToDouble(textBoxMarginSettings.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Margin" + ex);
                return;
            }
        }
        private void ApplyUnderlineMemo() => dataMemo.IsUnderlineCaption = isUnderlineActiveSettings.IsChecked ?? false;
        private void ApplyVerticalScrollBarVisibility() => dataMemo.VerticalScrollBarVisibility = verticalScrollBarVisibilitySettings.IsChecked.Value;
        private void ApplyFontType(ComboBox comboBox) => dataMemo.TextBoxfontFamily = new FontFamily(comboBox.SelectedValue as string);
        private void ApplyCaptionFontType(ComboBox comboBox) => dataMemo.CaptionFontFamily = new FontFamily(comboBox.SelectedValue as string);
        private void ApplyIsToggleWindow() => dataMemo.IsToggleWindow = isVisibleAboveAppsSettings.IsChecked ?? false;
        private void ApplyIsCaptionActive() => dataMemo.IsCaptionActive = isCaptionActiveSettings.IsChecked ?? false;
        private void ApplySizeWindow()
        {
            dataMemo.HeightWindow = Convert.ToDouble(heightWindowSettings.Text);
            dataMemo.WidthWindow = Convert.ToDouble(widthWindowSettings.Text);
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
            catch (Exception)
            {
                return;
            }
        }
        private void backgroundWindowColorSettings_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (backgroundTextBoxSettings.Text.ToLower() == "transparent")
                {
                    MessageBox.Show($"{backgroundTextBoxSettings.Text} - do not apply");
                }
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
            catch (Exception)
            {
                return;
            }
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e) 
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed) this.DragMove();
        }
        private void Initialize()
        {
            LoadTextBox();
            LoadCheckBox();
            LoadComboBox();
        }
        private void LoadTextBox()
        {
            backgroundWindowSettings.Text = string.IsNullOrEmpty(dataMemo.BackgroundWindowColorPath) ? Convert.ToString(dataMemo.BackgroundWindow) : dataMemo.BackgroundWindowColorPath;
            backgroundTextBoxSettings.Text = string.IsNullOrEmpty(dataMemo.BackgroundTextBoxPath) ? Convert.ToString(dataMemo.BackgroundTextBox) : dataMemo.BackgroundTextBoxPath;
            textBoxForegroundSettings.Text = Convert.ToString(dataMemo.TextBoxForeground);
            captionForegroundSettings.Text = Convert.ToString(dataMemo.CaptionForeground);
            captionFontSizeSettings.Text = $"{Convert.ToString(dataMemo.CaptionFontSize)}:5F";
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
        private void backgroundWindowSettings_MouseDoubleClick(object sender, MouseButtonEventArgs e) => backgroundWindowSettings.Text = GetFile(backgroundWindowSettings);
        private void backgroundTextBoxSettings_MouseDoubleClick(object sender, MouseButtonEventArgs e) =>  backgroundTextBoxSettings.Text = GetFile(backgroundTextBoxSettings);
        private string GetFile(TextBox textBox)
        {
            string pathProject = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MemoMinder", "Backgrounds");
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Open image file.";
            openFileDialog.InitialDirectory = pathProject;
            openFileDialog.Filter = "Image Files (*.jpg; *.png; *.bmp)|*.jpg;*.png;*.bmp";

            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = System.IO.Path.GetFileName(openFileDialog.FileName);
                string targetPath = System.IO.Path.Combine(pathProject, fileName);

                if (!File.Exists(targetPath))
                {
                    File.Copy(openFileDialog.FileName, targetPath);
                }

                textBox.Text = targetPath;
            }

            return textBox.Text;
        }
    }
}

