using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
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

        private readonly string NameFile = "\\LastOpenedNote.json";
        public FileOrganization() 
        {
            MemoFiles = new List<string>();

            PathAppData = GetPath();
            CheckExistingPaths(PathAppData);
            MemoFiles = GetFilesInPath();
        }
    
        public List<string>? GetFilesInPath()
        {
            string folderNotes = Path.Combine(PathAppData + "\\Notes");

            List<string> files = new List<string>();

            string[] tempfiles = Directory.GetFiles(folderNotes);
            
            int tempFilesLength = tempfiles.Length;
            
            if (tempFilesLength == 0 )
            {
                CreateDefaultNote();
            }

            if (PathAppData != null)
            {
                var dir = new DirectoryInfo(folderNotes);
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

            string textFromFile = JsonSerializer.Serialize(datawindow);

            using (StreamWriter streamwriter = new StreamWriter(PathAppData + NameFile))
            {
                streamwriter.Write(textFromFile);
                streamwriter.Close();
            }
        }

        public string GetLastOpenedNote()
        {
            string textFromFile = "";

            using (StreamReader streamreader = new StreamReader(PathAppData + NameFile))
            {
                textFromFile = streamreader.ReadToEnd();
            }

            DataWindow? dataWindow = JsonSerializer.Deserialize<DataWindow>(textFromFile);
            
            return dataWindow.LastOpenedFile;

        }

        private void CheckExistingPaths(string path)
        {
            string folderBackground = Path.Combine(path, "Backgrounds");
            string folderNotes = Path.Combine(path, "Notes");
            string pathLastOpenedFile = Path.Combine(path, "LastOpenedNote.json");
            string pathDefaultNote = Path.Combine(path, "DefaultThemeNote.json");
            
            if (!Directory.Exists(path)) Directory.CreateDirectory(path); 
            
            if (!Directory.Exists(folderBackground)) Directory.CreateDirectory(folderBackground);
            
            if (!File.Exists(pathLastOpenedFile))
            {
                using (var fsream = File.Create(pathLastOpenedFile))
                {
                    fsream.Close();
                }

                SetLastOpenedNote("");
           
            }

           
            if (!File.Exists(pathDefaultNote))
            {
                using (FileStream fileStream = File.Create(pathDefaultNote))
                {
                    fileStream.Close();
                }

                DataMemo dataMemo = new DataMemo
                {
                    BackgroundWindow = Brushes.White,
                    BackgroundTextBox = Brushes.Gray,
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
                    HeightWindow = 200.0,
                    WidthWindow = 200.0
                };

                SaveDataToFile(dataMemo, pathDefaultNote);

            }
            if (!Directory.Exists(folderNotes))
            {
                Directory.CreateDirectory(folderNotes);

                string jsonString = "";

                using (StreamReader reader = new StreamReader(pathDefaultNote))
                {
                    jsonString = reader.ReadToEnd();
                    reader.Close();
                }

                JsonSerializerOptions options = new JsonSerializerOptions();
                
                options.Converters.Add(new BrushConverter());
                options.Converters.Add(new FontFamilyConverter());

                DataMemo memo = JsonSerializer.Deserialize<DataMemo>(jsonString, options);

                SerializateSettings(memo, "Note", true, true);

            }
        }
        public void CreateDefaultNote()
        {
            string fileName = Path.Combine(PathAppData, "DefaultThemeNote.json");

            string fileFromText = "";
            using (StreamReader fileRead = new StreamReader(fileName))
            {
                fileFromText = fileRead.ReadToEnd();
                fileRead.Close();
            }

            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Converters.Add(new BrushConverter());
            options.Converters.Add(new FontFamilyConverter());

            DataMemo memo = JsonSerializer.Deserialize<DataMemo>(fileFromText, options);

            SerializateSettings(memo, "Note", true, true);

            MainWindow mainWindow = new MainWindow(memo);
            mainWindow.Show();
           
        }
        private string GetPath()
        {
            string NameFolder = "MemoMinder"; 
            string resPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), NameFolder);

            return resPath;
        }
        public void DeleteFile(string fileName)
        {
            if (MemoFiles.Count == 1)
            {
                MessageBox.Show("Удалить один существующий файл нельзя");
                return;
            }
            else
            {
                File.Delete(PathAppData + $"\\Notes\\{fileName}" + ".json");

                Random random = new Random();
                int position =  MemoFiles.IndexOf(fileName);
                MemoFiles.RemoveAt(position);
                int randomIndex = random.Next(0, MemoFiles.Count);

                DataMemo dataMemo = new DataMemo();
                dataMemo = DeserializeSettings(MemoFiles[randomIndex]);
                MainWindow.dataMemo = dataMemo;
                string nameFile = MemoFiles[randomIndex]; //Grab random file
                SetLastOpenedNote(nameFile);


            }
        }
        private void SaveDataToFile(DataMemo data, string path)
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true,
                ReferenceHandler = ReferenceHandler.Preserve
            };      

            options.Converters.Add(new BrushConverter());
            options.Converters.Add(new FontFamilyConverter());

            string textToFile = JsonSerializer.Serialize(data, options);

            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.Write(textToFile);
                writer.Close();
            }
        }
        public void SerializateSettings(DataMemo dataMemo, string nameFile, bool IsCreatedNewWindow, bool IsSaveName)
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

                MemoFiles.Add(nameFile + Convert.ToString(index));
                
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

            SaveDataToFile(dataMemo, filename);
 

        }
        public DataMemo DeserializeSettings(string file)
        {
            string fileName = PathAppData + $"\\Notes\\{file}" + ".json";

            try
            {
                string jsonString = "";
                using (StreamReader filestream = new StreamReader(fileName))
                {
                    jsonString = filestream.ReadToEnd();
                    filestream.Close();
                }

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
