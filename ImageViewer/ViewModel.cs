using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace ImageViewer
{
    class ViewModel : INotifyPropertyChanged
    {
        private ImageSource _image;
        public ImageSource Image
        {
            get { return _image; }
            set
            {
                _image = value;
                OnPropertyChanged("Image");
            }
        }
        OpenFileDialog dialog = new OpenFileDialog();
        public Command Open { get; set; } = new Command();
        public ViewModel()
        {
            Open.Com = OpenImg;
            dialog.Filter = "Image Files (*.BMP; *.JPG; *.GIF)| *.bmp; *.jpg; *.gif| All files(*.*) | *.*";
        }
        public void OpenImg(object ob)
        {
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    BitmapImage img = new BitmapImage();
                    img.BeginInit();
                    img.CacheOption = BitmapCacheOption.OnLoad;
                    img.UriSource = new Uri(dialog.FileName, UriKind.Absolute);
                    img.EndInit();
                    Image = img;
                    WriteSQL(dialog.SafeFileName);
                }
                catch (Exception e)
                {
                    MessageBox.Show($"Ошибка: {e}");
                    WriteSQL(e.ToString());
                }
            }
            else MessageBox.Show("Файл не открыт");
        }
        public void WriteSQL(string file)
        {
            using (SqlConnection db = new SqlConnection(Properties.Settings.Default.connect))
            {
                SqlCommand newCommand = new SqlCommand($"insert into History values(SYSDATETIME(), '{file}')", db);
                try
                {
                    db.Open();
                    newCommand.ExecuteNonQuery();
                    db.Close();
                }
                catch
                {
                    MessageBox.Show("База данных недоступна");
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string prop)
        {
            if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }

    public class Command : ICommand
    {
        public event EventHandler CanExecuteChanged;
        public Action<object> Com { get; set; }
        public bool CanExecute(object parameter) { return true; }
        public void Execute(object parameter)
        {
            Com(parameter);
        }
    }

}
