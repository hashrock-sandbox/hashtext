using Microsoft.Win32;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace hash_textarea
{
    class ViewModel : INotifyPropertyChanged
    {
        public ReactiveProperty<string> Content
        {
            get; set;
        }

        public ViewModel()
        {
            Content = new ReactiveProperty<string>("");
            SaveFileCommand.Subscribe(_ =>
            {
                var dialog = new SaveFileDialog();
                dialog.Filter = "テキストファイル(*.txt)|*.txt|CSVファイル(*.csv)|*.csv|全てのファイル(*.*)|*.*";
                var result = dialog.ShowDialog() ?? false;

                if (!result)
                {
                    return;
                }

                File.WriteAllText(dialog.FileName, Content.Value);
            });

            OpenFileCommand.Subscribe(_ =>
            {
                var dialog = new OpenFileDialog();
                dialog.Filter = "テキストファイル(*.txt)|*.txt|CSVファイル(*.csv)|*.csv|全てのファイル(*.*)|*.*";

                var result = dialog.ShowDialog() ?? false;

                if (!result)
                {
                    return;
                }
                Content.Value = File.ReadAllText(dialog.FileName);
            });
        }

        public ReactiveCommand SaveFileCommand { get; } = new ReactiveCommand();
        public ReactiveCommand OpenFileCommand { get; } = new ReactiveCommand();
        public event PropertyChangedEventHandler PropertyChanged;
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
    }
}
