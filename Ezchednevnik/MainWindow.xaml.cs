using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Newtonsoft.Json;

namespace DailyPlanner
{
    public class Note
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
    }

    public class JsonHelper
    {
        public static void Serialize<T>(List<T> data, string filePath)
        {
            string jsonData = JsonConvert.SerializeObject(data);
            File.WriteAllText(filePath, jsonData);
        }

        public static List<T> Deserialize<T>(string filePath)
        {
            if (File.Exists(filePath))
            {
                string jsonData = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<List<T>>(jsonData);
            }
            return new List<T>();
        }
    }

    public partial class MainWindow : Window
    {
        private const string DataFilePath = "notes.json";
        private List<Note> notes;
        private DateTime selectedDate;

        public MainWindow()
        {
            InitializeComponent();
            LoadData();
            datePicker.SelectedDate = DateTime.Today;
            selectedDate = DateTime.Today;
            ShowNotes();
        }

        private void LoadData()
        {
            notes = JsonHelper.Deserialize<Note>(DataFilePath);
        }

        private void SaveData()
        {
            JsonHelper.Serialize(notes, DataFilePath);
        }

        private void ShowNotes()
        {
            notesListBox.ItemsSource = notes.FindAll(note => note.Date.Date == selectedDate.Date);
        }

        private void datePicker_SelectedDateChanged(object sender, RoutedEventArgs e)
        {
            selectedDate = datePicker.SelectedDate ?? DateTime.Today;
            ShowNotes();
        }

        private void notesListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Note selectedNote = notesListBox.SelectedItem as Note;
            if (selectedNote != null)
            {
                titleTextBox.Text = selectedNote.Title;
                descriptionTextBox.Text = selectedNote.Description;
            }
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            Note newNote = new Note
            {
                Title = titleTextBox.Text,
                Description = descriptionTextBox.Text,
                Date = selectedDate
            };
            notes.Add(newNote);
            SaveData();
            ShowNotes();
            ClearTextBoxes();
        }

        private void updateButton_Click(object sender, RoutedEventArgs e)
        {
            Note selectedNote = notesListBox.SelectedItem as Note;
            if (selectedNote != null)
            {
                selectedNote.Title = titleTextBox.Text;
                selectedNote.Description = descriptionTextBox.Text;
                SaveData();
                ShowNotes();
                ClearTextBoxes();
            }
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            Note selectedNote = notesListBox.SelectedItem as Note;
            if (selectedNote != null)
            {
                notes.Remove(selectedNote);
                SaveData();
                ShowNotes();
                ClearTextBoxes();
            }
        }

        private void ClearTextBoxes()
        {
            titleTextBox.Text = string.Empty;
            descriptionTextBox.Text = string.Empty;
        }
    }
}
