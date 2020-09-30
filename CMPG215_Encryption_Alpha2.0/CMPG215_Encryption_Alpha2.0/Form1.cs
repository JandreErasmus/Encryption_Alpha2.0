using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;

namespace CMPG215_Encryption_Alpha2._0
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public string key, sourceFile;
        public bool flag = false;
        public bool flagEnc = false;
        string sty = "";

        public MemoryStream memStream;
        public CryptoStream cryptStream;
        public FileStream fileS;        

        private void Form1_Load(object sender, EventArgs e)
        {
            radj.Checked = true;
            txtOther.Enabled = false;
        }

        private void EncryptFile()
        {
            try 
            { 
                if (flag == true)
                {
                    DESCryptoServiceProvider DESCrypt = new DESCryptoServiceProvider();
                    fileS = File.OpenRead(sourceFile);

                    byte[] bytes = new byte[fileS.Length];
                    fileS.Read(bytes, 0, (int)fileS.Length);
                    fileS.Close();

                    byte[] byteKey = Encoding.Default.GetBytes(key);
                    SHA1 hash = new SHA1Managed();
                    byte[] byteHash = hash.ComputeHash(byteKey);

                    byte[] Keyb = new byte[8];
                    byte[] IV = new byte[8];

                    for (int i = 0; i < 8; i++)
                    {
                        Keyb[i] = byteHash[i];
                    }

                    for (int i = 8; i < 16; i++)
                    {
                        IV[i - 8] = byteHash[i];
                    }

                    DESCrypt.Key = Keyb;
                    DESCrypt.IV = IV;

                    memStream = new MemoryStream();
                    cryptStream = new CryptoStream(memStream, DESCrypt.CreateEncryptor(), CryptoStreamMode.Write);

                    cryptStream.Write(bytes, 0, bytes.Length);
                    cryptStream.FlushFinalBlock();

                    txtStatus.Text += "File encrypted succesfully\n";

                    flagEnc = true;

                    fileS.Close();
                }
            }   
            catch(Exception ex)
            {
                MessageBox.Show("ERROR ENCRYPTING FILE: " + ex);
            }
        }

        public void EncryptText()
        {
            int len = txtKey.TextLength;
            int mesLen = txtMessage.TextLength;
            byte[] asc = Encoding.ASCII.GetBytes(txtMessage.Text);


            txtMessage.Clear();
            for(int i = 0; i<mesLen; i++)
            {
                txtMessage.Text += (char)(asc[i] + len);
            }
                
            

        }

        private void txtKey_TextChanged(object sender, EventArgs e)
        {
            key = txtKey.Text;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            flag = true;
            txtMessage.Text = "";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                sourceFile = openFileDialog1.FileName;                
            }
        }

        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            if (flag == true)
                EncryptFile();
            else
                EncryptText();
        }

        public void writeFile(MemoryStream ms, string type, string path)
        {
            if (!path.Contains("."))
            {
                if (radj.Checked)
                    sty = ".jpg";
                else if (radp.Checked)
                    sty = ".png";
                else if (radz.Checked)
                    sty = ".zip";
                else if (radt.Checked)
                    sty = ".txt";
                else if (radOther.Checked)
                    txtOther.Enabled = true;

                path += sty;
            }

            fileS = File.OpenWrite(path);

            foreach (byte b in ms.ToArray())
                fileS.WriteByte(b);

            txtStatus.Text += type + " file succesfully written to " + path + "\n";

            fileS.Close();
                
        }

        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            if (flag == true)
                DecryptFile();
            else
                DecryptText();

        }

        public void DecryptText()
        {
           int len = txtKey.TextLength;
            int mesLen = txtMessage.TextLength;
            byte[] asc = Encoding.ASCII.GetBytes(txtMessage.Text);

            txtMessage.Clear();
            for (int i = 0; i < mesLen; i++)
            {
                txtMessage.Text += (char)(asc[i] - len);
            }

        }

        public void DecryptFile()
        {
            try
            {
                DESCryptoServiceProvider DESCrypt = new DESCryptoServiceProvider();

                fileS = File.OpenRead(sourceFile);
                byte[] bytes = new byte[fileS.Length];
                fileS.Read(bytes, 0, (int)fileS.Length);
                fileS.Close();

                byte[] byteKey = Encoding.Default.GetBytes(key);
                SHA1 hash = new SHA1Managed();
                byte[] hashByte = hash.ComputeHash(byteKey);

                byte[] Key = new byte[8];
                byte[] IV = new byte[8];

                for (int i = 0; i < 8; i++)
                {
                    Key[i] = hashByte[i];
                }

                for (int i = 8; i < 16; i++)
                {
                    IV[i - 8] = hashByte[i];
                }

                DESCrypt.Key = Key;
                DESCrypt.IV = IV;

                memStream = new MemoryStream();
                cryptStream = new CryptoStream(memStream, DESCrypt.CreateDecryptor(), CryptoStreamMode.Write);

                cryptStream.Write(bytes, 0, bytes.Length);
                cryptStream.FlushFinalBlock();

                txtStatus.Text += "File decrypted succesfully\n";
                flagEnc = false;

                fileS.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR DECRYPTING FILE" + ex);
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void txtMessage_TextChanged(object sender, EventArgs e)
        {
            flag = false;            
        }

        private void txtOther_TextChanged(object sender, EventArgs e)
        {
            sty = txtOther.Text;
        }

        private void radOther_CheckedChanged(object sender, EventArgs e)
        {
            if (radOther.Checked)
                txtOther.Enabled = true;
            else
                txtOther.Enabled = false;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.Delete(openFileDialog1.FileName);
                   txtStatus.Text += openFileDialog1.FileName + " deleted successfully\n";
                }
                catch(IOException ex)
                {
                    MessageBox.Show("ERROR deleting file: " + ex);
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        { 
            string type = "";

            if (flagEnc == true)
            {
                type = "Encrypted";
                saveFileDialog1.Title = "Save ENCRYPTED file";
            }
                
            else
            {
                type = "Decrypted";
                saveFileDialog1.Title = "Save DECRYPTED file";
            }
                

            if(saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (flag == true)
                    writeFile(memStream, type, saveFileDialog1.FileName);
                else
                    writeText(saveFileDialog1.FileName);
            }
        }

        public void writeText(string path)
        {
            StreamWriter sWrite = new StreamWriter(path);

            sWrite.WriteLine(txtMessage.Text);

            txtStatus.Text +=  " Message succesfully written to " + path + "\n";

            sWrite.Close();
        }
    }
}
