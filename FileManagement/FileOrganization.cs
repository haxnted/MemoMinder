﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Media;


namespace MemoMinder
{
    internal class FileOrganization
    {
        private string PathAppData { get;  set; }
        private List<string> MemoFiles { get; set; 
        }
        private readonly string NameFile = "\\LastOpenedNote.json";
        public FileOrganization() 
        {
            PathAppData = GetPath();
            MemoFiles = GetFilesInPath();
            CheckExistingPaths(PathAppData);
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
            MessageBox.Show(PathAppData + NameFile);
        }

        public string GetLastOpenedNote()
        {
            string jsonString = File.ReadAllText(PathAppData + NameFile);

            DataWindow dataWindow = JsonSerializer.Deserialize<DataWindow>(jsonString);
            
            return dataWindow.LastOpenedFile;

        }
        private void CreateDefaultNote()
        {
            DataMemo memo = new DataMemo
            {
                BackgroundWindow = Brushes.White,
                BackgroundTextBox = Brushes.LightGray,
                TextBoxForeground = Brushes.Black,
                CaptionForeground = Brushes.Black,
                TextBoxfontFamily = new FontFamily("Arial"),
                CaptionFontFamily = new FontFamily("Arial"),
                BackgroundWindowColorPath = "White",
                BackgroundTextBoxPath = "LightGray",
                CaptionText = "FirstMemo",
                MemoText = "This is my memo text.",
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
            SerializateSettings(memo, "Note", true);
        }
        private void CheckExistingPaths(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path); 
            }
            if (!File.Exists(path + "LastOpenedNote.json"))
            {
                File.Create(path + "LastOpenedNote.json");

                SetLastOpenedNote("LastOpenedNote.json");
            }

            if(Directory.Exists(path + "\\Backgrounds"))
            {
                Directory.CreateDirectory(path + "\\Backgrounds");
            }
        
            if (!Directory.Exists(path + "\\Notes"))
            {
                Directory.CreateDirectory(path + "\\Notes");

                CreateDefaultNote();
            }
        }

        private string GetPath()
        {
            string NameFolder = "MemoMinder"; //NameFolder is main folder project
            string resPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), NameFolder);

            return resPath;

        }
        
        public void SerializateSettings(DataMemo data)
        {
            string fileName = System.IO.Path.Combine(PathAppData, $"Notes\\{data.CaptionText}.json"); //CaptionText is name file.

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true,
                ReferenceHandler = ReferenceHandler.Preserve
            };
            //Brush and FontFamily cannot serialize, so i create converters to keep save data in file.
            options.Converters.Add(new BrushConverter()); 
            options.Converters.Add(new FontFamilyConverter());

            string jsonString = JsonSerializer.Serialize(data, options);

            File.WriteAllText(fileName, jsonString);
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
        public void SerializateSettings(DataMemo data, string nameFile, bool newFile = false)
        {
            string filename;
            
            if (newFile)
            {
                int index = 0;
                filename = System.IO.Path.Combine(PathAppData, $"Notes\\");

                while (File.Exists(filename + nameFile + Convert.ToString(index) + ".json"))
                {
                    index++;
                }
                filename += nameFile + Convert.ToString(index) + ".json";
                SetLastOpenedNote(nameFile + Convert.ToString(index));
            }
            else
            {
                filename = PathAppData + $"Notes\\{nameFile}" + "json";
                SetLastOpenedNote(nameFile);
            }

            SaveDataToFile(data, filename);
 

        }
        public DataMemo DeserializeSettings(string file)
        {
            string fileName = PathAppData + $"\\Notes\\{file}" + ".json";
            
            MessageBox.Show(fileName);

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

        public string GetLastMemo()
        {
            try
            {
                DirectoryInfo directory = new DirectoryInfo(PathAppData + "\\Notes");
                FileInfo[] files = directory.GetFiles();
                DateTime earliestDate = DateTime.Now;
                FileInfo earliestFile = null;

                foreach (FileInfo file in files)
                {
                    if (file.LastAccessTime < earliestDate)
                    {
                        earliestDate = file.LastAccessTime;
                        earliestFile = file;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(earliestFile.FullName).ToString();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error get last file: {ex}");
            }
            
        }
    }
}