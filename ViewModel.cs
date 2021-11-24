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

        public ReactivePropertySlim<string> Content
        {
            get; set;
        }
        public ReactiveProperty<string?> Path
        {
            get; set;
        }
        public ReactiveProperty<string> Title
        { get; set; }
        public ReactiveProperty<bool> Dirty
        { get; set; }

        private string _oldContent = "";


        public ViewModel()
        {
            Content = new ReactivePropertySlim<string>("");
            Path = new ReactiveProperty<string?>();
            Dirty = new ReactiveProperty<bool>(false);

            Title = Path.CombineLatest(Dirty, (i, d) =>
            {
                var star = d ? " *" : "";
                var filename = i != null ? System.IO.Path.GetFileName(i) : "New File";
                return $"hashtext - {filename}{star}";
            }).ToReactiveProperty<string>();

            Content.Subscribe((_) =>
            {
                if (!Dirty.Value && _oldContent != Content.Value)
                {
                    Dirty.Value = true;
                }
            });

            SaveFileCommand.Subscribe(_ =>
            {
                if (Path.Value == null)
                {
                    SaveNewFile();
                } else {
                    File.WriteAllText(Path.Value, Content.Value);
                    ClearCache();
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
                ClearCache();
            });

        }

        private void ClearCache()
        {
            Dirty.Value = false;
            _oldContent = Content.Value;
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
            ClearCache();
            Path.Value = dialog.FileName;
        }

        public ReactiveCommand SaveFileAsCommand { get; } = new ReactiveCommand();
        public ReactiveCommand SaveFileCommand { get; } = new ReactiveCommand();
        public ReactiveCommand OpenFileCommand { get; } = new ReactiveCommand();
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
