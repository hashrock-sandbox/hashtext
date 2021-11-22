using Microsoft.Win32;
using Reactive.Bindings;
using System;
using System.ComponentModel;
using System.IO;
using System.Reactive.Linq;

namespace hash_textarea
{
    class ViewModel : INotifyPropertyChanged
    {
        private const string _pattern = "テキストファイル(*.txt)|*.txt|Markdownファイル(*.md)|*.md|全てのファイル(*.*)|*.*";

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
                var filename = i != null ? System.IO.Path.GetFileName(i) : "New File";
                return $"hashtext - {filename}";
            }).ToReactiveProperty<string>();

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
}
