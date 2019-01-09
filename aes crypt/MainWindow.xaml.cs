using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace aes_crypt
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        byte[] bytes;
        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                OpenFileDialog fd = new OpenFileDialog();

                fd.ShowDialog();

                using (FileStream fsSource = new FileStream(fd.FileName,
           FileMode.Open, FileAccess.Read))
                {

                    bytes = new byte[fsSource.Length];
                    int numBytesToRead = (int)fsSource.Length;
                    int numBytesRead = 0;
                    while (numBytesToRead > 0)
                    {

                        int n = fsSource.Read(bytes, numBytesRead, numBytesToRead);


                        if (n == 0)
                            break;

                        numBytesRead += n;
                        numBytesToRead -= n;
                    }
                    numBytesToRead = bytes.Length;



                }
                SaveFileDialog sd = new SaveFileDialog();
                sd.Filter = "Abs (*.abs)|*.abs";
                sd.ShowDialog();
                byte[] passwordBytes = Encoding.UTF8.GetBytes(pass.Text);

                passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

                byte[] bytesEncrypted = AES_Encrypt(bytes, passwordBytes);

                string fileEncrypted = sd.FileName;

                File.WriteAllBytes(fileEncrypted, bytesEncrypted);
                MessageBox.Show("Ok!");
            }
            catch
            {

            }
        }
        public byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;


            byte[] saltBytes = new byte[] { 1, 2, 3, 5, 7, 11, 13, 17, 19, 23 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }
        public byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;

            byte[] saltBytes = new byte[] { 1, 2, 3, 5, 7, 11, 13, 17, 19, 23 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }

        private void button_Copy1_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog sd2 = new OpenFileDialog();
            sd2.Filter = "Abs (*.abs)|*.abs";
            sd2.ShowDialog();
            byte[] bytesToBeDecrypted = File.ReadAllBytes(sd2.FileName);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(pass.Text);
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesDecrypted = AES_Decrypt(bytesToBeDecrypted, passwordBytes);
            SaveFileDialog sd = new SaveFileDialog();
            // sd.Filter = "File (*." + decrypt(sbytes[0]) + ")|*." + decrypt(sbytes[0]);
            sd.ShowDialog();
            string file = sd.FileName;
            File.WriteAllBytes(file, bytesDecrypted);
            MessageBox.Show("Ok!");
        }


    }
}

