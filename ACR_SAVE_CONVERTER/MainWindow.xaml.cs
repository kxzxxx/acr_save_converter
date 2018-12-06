using System;
using System.Collections.Generic;
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
using System.IO;

namespace ACR_SAVE_CONVERTER
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

            var openFileDialog = new Microsoft.Win32.OpenFileDialog()
            {
                Title = Properties.Resources.Choose_your_save,
                Filter = Properties.Resources.File_Filter,
                InitialDirectory = Directory.GetCurrentDirectory(),
                FilterIndex = 1,
                RestoreDirectory = true

            };

            if (openFileDialog.ShowDialog() == true)
            {
                from1.Text = openFileDialog.FileName;

            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog()
            {
                Title = Properties.Resources.Choose_your_save,
                Filter = Properties.Resources.File_Filter,
                InitialDirectory = Directory.GetCurrentDirectory(),
                FilterIndex = 1,
                RestoreDirectory = true

            };

            if (openFileDialog.ShowDialog() == true)
            {
                to1.Text = openFileDialog.FileName;

            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrEmpty(from1.Text)
                || string.IsNullOrEmpty(to1.Text)
                || string.IsNullOrEmpty(key1.Text)
                || string.IsNullOrEmpty(key2.Text)
                )
            {
                return;

            }

            byte[] keyToDecrypt = System.Text.Encoding.Default.GetBytes(key1.Text);
            byte[] keyToEncrypt = System.Text.Encoding.Default.GetBytes(key2.Text);

            var inputSave = new FileStream(from1.Text, FileMode.Open, FileAccess.Read);
            var tempSave = new MemoryStream();

            //Header
            switch (inputSave.ReadByte())
            {
                case 0x4:
                    inputSave.Seek(0x8, SeekOrigin.Begin);
                    break;
                case 0x24:
                    inputSave.Seek(0x28, SeekOrigin.Begin);
                    break;
            }


            tempSave.Seek(0x0, SeekOrigin.Begin);
            tempSave.Write(new byte[]  {
                0x4,0x2,0x0,0x0,0x0,0x0,0x0,0x0}, 0, 8
            );
            for (int i = 0x8; i <= 0x208; i++)
            {
                tempSave.WriteByte((byte)inputSave.ReadByte());
            }


            //Decrypt
            var j = (inputSave.Length - inputSave.Position) % keyToDecrypt.Length;


            do
            {
                byte a;
                int index = inputSave.ReadByte();

                if (index == -1)
                {
                    break;
                }
                else
                {
                    a = (byte)index;

                }
                tempSave.WriteByte((byte)(a ^ keyToDecrypt[j]));

                j = j - 1;

                if (j < 0)
                {
                    j = keyToDecrypt.Length - 1;
                }

            } while (true);
            inputSave.Close();

            //Encrypt
            var outputSave = new FileStream(to1.Text, FileMode.Create);
            outputSave.Seek(0x0, SeekOrigin.Begin);
            tempSave.Seek(0x0, SeekOrigin.Begin);

            for (int i = 0x0; i <= 0x208; i++)
            {
                outputSave.WriteByte((byte)tempSave.ReadByte());
            }

            j = (tempSave.Length - tempSave.Position) % keyToEncrypt.Length;

            do
            {
                byte a;
                int index = tempSave.ReadByte();

                if (index == -1)
                {
                    break;
                }
                else
                {
                    a = (byte)index;

                }
                outputSave.WriteByte((byte)(a ^ keyToEncrypt[j]));

                j = j - 1;

                if (j < 0)
                {
                    j = keyToDecrypt.Length - 1;
                }

            } while (true);

            tempSave.Close();
            outputSave.Close();


            MessageBox.Show(Properties.Resources.Done_, Properties.Resources.Result_);

        }

    }
}
