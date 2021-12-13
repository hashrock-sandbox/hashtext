using System.IO;
using System.Windows;

namespace hash_textarea
{


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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (vm.Dirty.Value && MessageBoxResult.Yes != MessageBox.Show("保存せずに終了しますか？", "確認", MessageBoxButton.YesNoCancel, MessageBoxImage.Information))
            {
                e.Cancel = true;
                return;
            }

        }
    }
}
