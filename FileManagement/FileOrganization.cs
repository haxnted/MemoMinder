using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;


namespace MemoMinder
{
    internal class FileOrganization
    {
        private string PathAppData { get;  set; }
        private List<string> MemoFiles { get; set; }
        static public bool IsCreatedNewWindow { get; set; }
        static public bool IsSaveName { get; set; }

        private readonly string NameFile = "\\LastOpenedNote.json";
        public FileOrganization() 
        {
            PathAppData = GetPath();
            CheckExistingPaths(PathAppData);
            MemoFiles = GetFilesInPath();
        }
    
        public List<string>? GetFilesInPath()
        {
            List<string> files = new List<string>();

            string[] tempfiles = Directory.GetFiles(PathAppData + "\\Notes");
            
            int tempFilesLength = tempfiles.Length;
            
            if (tempFilesLength == 0 )
            {
                CreateDefaultNote();
            }


            if (PathAppData != null)
            {
                var dir = new DirectoryInfo(PathAppData +"\\Notes");
                var fileList = dir.GetFiles();

                foreach (var file in fileList)
                {
                    files.Add(System.IO.Path.GetFileNameWithoutExtension(file.FullName));
                }
            }

            return files;
        }

        public void SetLastOpenedNote(string nameLastOpenedFile)
        {
            DataWindow datawindow = new DataWindow(); 
            datawindow.LastOpenedFile = nameLastOpenedFile;

            string jsonString = JsonSerializer.Serialize(datawindow);
            File.WriteAllText(PathAppData + NameFile, jsonString);
            MessageBox.Show($"SET last note: {PathAppData + NameFile}");
        }

        public string GetLastOpenedNote()
        {
            string jsonString = File.ReadAllText(PathAppData + NameFile);

            DataWindow dataWindow = JsonSerializer.Deserialize<DataWindow>(jsonString);
            
            return dataWindow.LastOpenedFile;

        }

        private void CheckExistingPaths(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path); 
            }

            if (!File.Exists(path + "\\LastOpenedNote.json"))
            {
                File.Create(path + "\\LastOpenedNote.json");

                SetLastOpenedNote("LastOpenedNote.json");
            }
            MessageBox.Show("Путь файла с дефолт загрузками: " + path + "\\DefaultThemeNote.json");
            if (!File.Exists(path + "\\DefaultThemeNote.json"))
            {
                using (var fileStream = File.Create(path + "\\DefaultThemeNote.json"))
                {
                    fileStream.Close();
                }
                DataMemo memo = new DataMemo
                {
                    BackgroundWindow = Brushes.White,
                    BackgroundTextBox = Brushes.LightGray,
                    TextBoxForeground = Brushes.Black,
                    CaptionForeground = Brushes.Black,
                    TextBoxfontFamily = new FontFamily("Arial"),
                    CaptionFontFamily = new FontFamily("Arial"),
                    BackgroundWindowColorPath = "",
                    BackgroundTextBoxPath = "",
                    CaptionText = "Caption",
                    MemoText = "Text",
                    TextBoxMargin = 10.0,
                    TextBoxFontSize = 16.0,
                    VerticalScrollBarVisibility = true,
                    IsToggleWindow = false,
                    IsCaptionActive = true,
                    CaptionFontSize = 18.0,
                    IsUnderlineCaption = true,
                    HeightWindow = 400.0,
                    WidthWindow = 600.0
                };
                IsCreatedNewWindow = false;
                IsSaveName = false;
                //написать отдельную сериализацию!!!
                string filename = PathAppData + $"\\DefaultThemeNote" + ".json";



                SaveDataToFile(memo, filename);
                //SerializateSettings(memo, "DefaultThemeNote");
            }
            if (!Directory.Exists(path + "\\Backgrounds"))
            {
                Directory.CreateDirectory(path + "\\Backgrounds");
            }
        
            if (!Directory.Exists(path + "\\Notes"))
            {
                Directory.CreateDirectory(path + "\\Notes");
                
                CreateDefaultNote();
            }
        }
        public void CreateDefaultNote()
        {
            string fileName = PathAppData + $"\\DefaultThemeNote" + ".json";

            MessageBox.Show($"Deserialize CreateDefaultNote: {fileName}");

            string jsonString = File.ReadAllText(fileName);

            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Converters.Add(new BrushConverter());
            options.Converters.Add(new FontFamilyConverter());

            DataMemo memo = JsonSerializer.Deserialize<DataMemo>(jsonString, options);


            IsCreatedNewWindow = true;
            IsSaveName = true;
            SerializateSettings(memo, "Note");

            MainWindow mainWindow = new MainWindow(memo);
            mainWindow.Show();
           
        }
        private string GetPath()
        {
            string NameFolder = "MemoMinder"; //NameFolder is main folder project
            string resPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), NameFolder);

            return resPath;

        }
        
        private void SaveDataToFile(DataMemo data, string path)
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true,
                ReferenceHandler = ReferenceHandler.Preserve
            };
            //Brush and FontFamily cannot serialize, so i create converters to keep save data in file.
            options.Converters.Add(new BrushConverter());
            options.Converters.Add(new FontFamilyConverter());

            string jsonString = JsonSerializer.Serialize(data, options);
            File.WriteAllText(path, jsonString);
        }
        public void SerializateSettings(DataMemo data, string nameFile)
        {
            string filename;
            
            if (IsCreatedNewWindow)
            {
                int index = 0;
                filename = System.IO.Path.Combine(PathAppData, $"Notes\\");

                while (File.Exists(filename + nameFile + Convert.ToString(index) + ".json"))
                {
                    index++;
                }
                filename += nameFile + Convert.ToString(index) + ".json";
                
                if (IsSaveName)
                {
                    SetLastOpenedNote(nameFile + Convert.ToString(index));
                }
            }
            else
            {
                filename = PathAppData + $"\\Notes\\{nameFile}" + ".json";
                SetLastOpenedNote(nameFile);
            }

            SaveDataToFile(data, filename);
 

        }
        public DataMemo DeserializeSettings(string file)
        {
            string fileName = PathAppData + $"\\Notes\\{file}" + ".json";
            
            MessageBox.Show($"Deserialize: {fileName}");

            try
            {
                string jsonString = File.ReadAllText(fileName);
                
                JsonSerializerOptions options = new JsonSerializerOptions();
                options.Converters.Add(new BrushConverter());
                options.Converters.Add(new FontFamilyConverter());

                DataMemo data = JsonSerializer.Deserialize<DataMemo>(jsonString, options);
                
                return data;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to deserialize {file}: {ex.Message}");
                return null;
            }
        }
    }
}
