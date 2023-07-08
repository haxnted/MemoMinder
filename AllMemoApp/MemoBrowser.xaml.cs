using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace MemoMinder.AllMemoApp
{
    /// <summary>
    /// Логика взаимодействия для MemoBrowser.xaml
    /// </summary>
    public partial class MemoBrowser : Window
    {
        FileOrganization fileorg = new FileOrganization();
        public List<string> MemoFiles { get; private set; }
        private int TotalColumns { get; set; }
        public MemoBrowser()
        {
            InitializeComponent();
            MemoFiles = fileorg.GetFilesInPath();
            CreateTable();
        }

        private void CreateTable()
        {
            int length = MemoFiles.Count;
            int maxColumnsInRow = 4;

            int rows = (int)Math.Ceiling((double)length / maxColumnsInRow);
            int columns = Math.Min(maxColumnsInRow, length);

            TotalColumns = columns;
            CreateGrid(columns, rows);
        }
        private void CreateGrid(int columnDefinitions, int rowDefinitions)
        {
            Grid gridAllMemo = GridAllMemo;
            const int SIZENOTE = 200;
            int heightWindow = rowDefinitions * SIZENOTE;
            int widthWindow = columnDefinitions * SIZENOTE;

            Height = heightWindow + 38;
            Width = Math.Min(widthWindow, SIZENOTE * 4) + 15;

            int k = 0;
            for (int i = 0; i < rowDefinitions; i++)
            {
                gridAllMemo.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(SIZENOTE) });
            }

            for (int j = 0; j < columnDefinitions; j++)
            {
                gridAllMemo.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(SIZENOTE) });
            }

            for (int i = 0; i < rowDefinitions; i++)
            {
                for (int j = 0; j < columnDefinitions; j++)
                {
                    DataMemo memo = new DataMemo();

                    memo = fileorg.DeserializeSettings(MemoFiles[k]);
                    Border border = new Border()           
                    {
                        
                        MinHeight = 140,
                        MinWidth = 100,
                        Background = new SolidColorBrush(Colors.Black),
                    };
                    Button button = new Button()
                    {
                        Background = new SolidColorBrush(Colors.Transparent),
                        BorderThickness = new Thickness(0)
                    };

                    button.Click += OpenMemo;
                    
                    Grid.SetRow(border, i);
                    Grid.SetColumn(border, j);

                    Grid.SetRow(button, i);
                    Grid.SetColumn(button, j);

                    if (!string.IsNullOrEmpty(memo.BackgroundWindowColorPath))
                    {
                        ImageBrush imageBrush = new ImageBrush();
                        imageBrush.ImageSource = new BitmapImage(new Uri(memo.BackgroundWindowColorPath));
                        border.Background = imageBrush;
                    }
                    else
                    {
                        border.Background = memo.BackgroundWindow;
                    }

                    Grid grid = new Grid {
                        Margin = new Thickness(10,20,10,10)
                    };

                    
                    grid.RowDefinitions.Add(new RowDefinition() { Height = memo.IsCaptionActive == true ? new GridLength(25 ) : new GridLength(0) });
                    grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

                    Label captionLabel = new Label()
                    {
                        FontWeight = FontWeights.Bold,
                        Content = memo.CaptionText,
                        FontSize = 13.0,
                        BorderThickness = new Thickness(0),
                        FontFamily = memo.CaptionFontFamily,
                        VerticalAlignment = VerticalAlignment.Top,
                        HorizontalAlignment = HorizontalAlignment.Center,
                    };
                    grid.Children.Add(captionLabel);
                    Grid.SetRow(captionLabel, 0);

                    TextBox contentLabel = new TextBox()
                    {
                        Margin = new Thickness(5),
                        Text = memo.MemoText,
                        BorderThickness = new Thickness(0),
                        TextWrapping = TextWrapping.Wrap,
                        FontSize = 10,
                        FontFamily = memo.TextBoxfontFamily,
                        
                    };
                    grid.Children.Add(contentLabel);
                    Grid.SetRow(contentLabel, 1);

                    if (!string.IsNullOrEmpty(memo.BackgroundTextBoxPath))
                    {
                        ImageBrush imageBrush = new ImageBrush();
                        imageBrush.ImageSource = new BitmapImage(new Uri(memo.BackgroundTextBoxPath));
                        contentLabel.Background = imageBrush;
                    }
                    else
                    {
                        contentLabel.Background = memo.BackgroundTextBox;
                    }

                    border.Child = grid;
                    gridAllMemo.Children.Add(border);
                    gridAllMemo.Children.Add(button);
                    Panel.SetZIndex(button, 1);

                    k++;

                    if (k == MemoFiles.Count)
                    {
                        return;
                    }
                }
                if (k == MemoFiles.Count)
                {
                    return;
                }
            }
        }

        private void OpenMemo(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            int row = Grid.GetRow(button);
            int column = Grid.GetColumn(button);

            int number = row * TotalColumns + column;

            fileorg.SetLastOpenedNote(MemoFiles[number]);

            DataMemo dataMemo = new DataMemo();
            dataMemo = fileorg.DeserializeSettings(MemoFiles[number]);
            MainWindow.dataMemo = dataMemo;
            this.Close();

        }
    }
}
