using Microsoft.Win32;
using Reactive.Bindings;
using System;
using System.ComponentModel;
using System.IO;
using System.Reactive.Linq;
using System.Windows;

namespace hash_textarea
{
    class ViewModel : INotifyPropertyChanged
    {
        private const string _pattern = "テキストファイル(*.txt)|*.txt|CSVファイル(*.csv)|*.csv|全てのファイル(*.*)|*.*";

        public ReactiveProperty<string> Content
        {
            get; set;
        }
        public ReactiveProperty<string?> Path
        {
            get; set;
        }
        public ReactiveProperty<string> Title
        { get; set; }

        public ViewModel()
        {
            Content = new ReactiveProperty<string>("");
            Path = new ReactiveProperty<string?>();
            Title = Path.Select(i =>
            {
                return i != null ? "HashText - " + i : "HashText - New File";
            }).ToReactiveProperty<string>();

            //Title = Path.Value != null ? "HashText" + Path.Value : "HashText";

            SaveFileCommand.Subscribe(_ =>
            {
                if (Path.Value != null)
                {
                    File.WriteAllText(Path.Value, Content.Value);
                }
                else
                {
                    SaveNewFile();
                }
            });
            SaveFileAsCommand.Subscribe(_ =>
            {
                SaveNewFile();
            });

            OpenFileCommand.Subscribe(_ =>
            {
                var dialog = new OpenFileDialog();
                dialog.Filter = _pattern;

                var result = dialog.ShowDialog() ?? false;

                if (!result)
                {
                    return;
                }
                Content.Value = File.ReadAllText(dialog.FileName);
                Path.Value = dialog.FileName;
            });

        }

        private void SaveNewFile()
        {
            var dialog = new SaveFileDialog();
            dialog.Filter = _pattern;
            var result = dialog.ShowDialog() ?? false;

            if (!result)
            {
                return;
            }

            File.WriteAllText(dialog.FileName, Content.Value);
            Path.Value = dialog.FileName;
        }

        public ReactiveCommand SaveFileAsCommand { get; } = new ReactiveCommand();
        public ReactiveCommand SaveFileCommand { get; } = new ReactiveCommand();
        public ReactiveCommand OpenFileCommand { get; } = new ReactiveCommand();
        public event PropertyChangedEventHandler? PropertyChanged;
    }


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ViewModel vm;

        public MainWindow()
        {
            InitializeComponent();
            vm = new ViewModel();
            DataContext = vm;
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            string[]? files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (files == null)
            {
                return;
            }
            // 1つだけファイルを開く
            if (File.Exists(files[0]) == false)
            {
                return;
            }
            string filePath = files[0];
            string fullPath = Path.GetFileName(filePath);

            vm.Path.Value = fullPath;
            vm.Content.Value = File.ReadAllText(filePath);
        }

        private void Window_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }
    }
}
