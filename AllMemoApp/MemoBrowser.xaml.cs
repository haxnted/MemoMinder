using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace MemoMinder.AllMemoApp
{
    /// <summary>
    /// Логика взаимодействия для MemoBrowser.xaml
    /// </summary>
    public partial class MemoBrowser : Window
    {
        FileOrganization fileorg = new FileOrganization();
        public List<string> MemoFiles { get; private set; }
        public MemoBrowser()
        {
            InitializeComponent();
            MemoFiles = fileorg.GetFilesInPath();
            CreateTable();
        }

        private void CreateTable()
        {
         
            int length = MemoFiles.Count;
            if (length % 2 == 0)
            {
                int middle = length / 2;
                int height = middle;
                int width = middle;

                CreateGrid(width, height);
            }
            if(length == 1 || length == 2)
            {
                int height = 1;
                int width = 1;
                CreateGrid(width, height);
            }
            else
            {
                int middle = (length - 1) / 2;
                int height = middle;
                int width = middle + 1;

                CreateGrid(width, height);
            }

        }
        private void CreateGrid(object width, object height)
        {
            Grid gridAllMemo = GridAllMemo;

            int HeightWindow = (int)height * 200; //140 - MinHeight
            int WidthWindow = (int)width * 200; // 100 - MinWidth

            Height = HeightWindow;
            Width = WidthWindow;
            int k = 0;
            for (int i = 0; i < (int)height; i++)
            {
                gridAllMemo.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(HeightWindow / (int)height) });
            }

            for (int j = 0; j < (int)width; j++)
            {
                gridAllMemo.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(WidthWindow / (int)width) });
            }


            for (int i = 0; i < (int)height; i++)
            {
                for (int j = 0; j < (int)width; j++)
                {
                    DataMemo memo = new DataMemo();

                    memo = fileorg.DeserializeSettings(MemoFiles[k]);
                    Border border = new Border()           
                    {
                        
                        MinHeight = 140,
                        MinWidth = 100,
                        Background = new SolidColorBrush(Colors.Black),
                        CornerRadius = new CornerRadius(10),
                        Margin = new Thickness(0)
                    };
                    Button button = new Button()
                    {
                        Background = new SolidColorBrush(Colors.Transparent),
                        BorderThickness = new Thickness(0)
                    };

                    button.Click += OpenMemo;
                    
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

                    Grid.SetRow(border, i);
                    Grid.SetColumn(border, j);

                    Grid.SetRow(button, i);
                    Grid.SetColumn(button, j);


                    Grid grid = new Grid();
                    grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(25) });
                    grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

                    Label captionLabel = new Label()
                    {
                        FontWeight = FontWeights.Bold,
                        Content = memo.CaptionText,
                        FontSize = 13.0,
                        VerticalAlignment = VerticalAlignment.Top,
                        HorizontalAlignment = HorizontalAlignment.Center,
                    };
                    grid.Children.Add(captionLabel);
                    Grid.SetRow(captionLabel, 0);

                    Label contentLabel = new Label() { Content = memo.MemoText };
                    grid.Children.Add(contentLabel);
                    Grid.SetRow(contentLabel, 1);

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
            DataMemo memo = new DataMemo();
            Button button = (Button)sender;
            int row = Grid.GetRow(button);
            int column = Grid.GetColumn(button);

            Border border = null;

            foreach (var child in GridAllMemo.Children)
            {
                if (child is Border childBorder && Grid.GetRow(childBorder) == row && Grid.GetColumn(childBorder) == column)
                {
                    border = childBorder;
                    break;
                }
            }

            if (border != null)
            {
                Label label = FindLabelInBorder(border);
                if (label != null)
                {
                    string labelText = label.Content.ToString();
                    MessageBox.Show(labelText);
                    
                    memo = fileorg.DeserializeSettings(labelText);

                }
            }

            MainWindow main = new MainWindow(memo);
            main.Show();
        }

        private Label FindLabelInBorder(Border border)
        {
            Grid grid = border.Child as Grid;
            if (grid != null)
            {
                for (int i = 0; i < grid.Children.Count; i++)
                {
                    if (grid.Children[i] is Label label)
                    {
                        return label;
                    }
                }
            }

            return null;
        }


    }
}
