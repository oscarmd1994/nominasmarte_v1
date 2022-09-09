using System;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Payroll.Models.Utilerias
{
    public class Encriptamiento
    {

        public static string KEY = "12345678901234567890123456789012";//
        public static string IV = "8U350sAToRTu8XQC"; //

        public static string SHA512(string str)
        {
            SHA512 sha512 = SHA512Managed.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = sha512.ComputeHash(encoding.GetBytes(str));
            for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            return sb.ToString();
        }
        public string SHA512Decrypt(string str)
        {
            SHA512 sha512 = SHA512Managed.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            //sha512.Hash(encoding.GetBytes(str));
            //byte[] stream = null;
            //StringBuilder sb = new StringBuilder();
            //stream = sha512.ComputeHash(encoding.GetBytes(str));
            //for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            return "";
        }
        public string AES256Encrypt( string plainText)
        {

            //byte[] iv = new byte[16];
            byte[] iv = Encoding.UTF8.GetBytes(IV);
            byte[] array;
             
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(KEY);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);

        }

        public string AES256Decrypt( string cipherText)
        {
            //byte[] iv = new byte[16];
            byte[] iv = Encoding.UTF8.GetBytes(IV);
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(KEY);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }

        }

        public static string Encrypt(string InputString, string SecretKey, CipherMode CyphMode) {
            try {
                TripleDESCryptoServiceProvider Des = new TripleDESCryptoServiceProvider();
                //Put the string into a byte array
                byte[] InputbyteArray = Encoding.UTF8.GetBytes(InputString);
                //Create the crypto objects, with the key, as passed in
                MD5CryptoServiceProvider hashMD5 = new MD5CryptoServiceProvider();
                Des.Key = hashMD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(SecretKey));
                Des.Mode = CyphMode;
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, Des.CreateEncryptor(), CryptoStreamMode.Write);
                //Write the byte array into the crypto stream
                //(It will end up in the memory stream)
                cs.Write(InputbyteArray, 0, InputbyteArray.Length);
                cs.FlushFinalBlock();
                //Get the data back from the memory stream, and into a string
                StringBuilder ret = new StringBuilder();
                byte[] b = ms.ToArray();
                ms.Close();
                int I = 0;
                for (I = 0; I <= b.GetUpperBound(0); I++) {
                    //Format as hex
                    ret.AppendFormat("{0:X2}", b[I]);
                }
                return ret.ToString();
            } catch (System.Security.Cryptography.CryptographicException generatedExceptionName) {
                return "";
            }
        }

        public static string Decrypt(string p_InputString, string p_SecretKey, CipherMode p_CyphMode) {
            if (String.IsNullOrEmpty(p_InputString)) {
                return String.Empty;
            } else {
                StringBuilder ret = new StringBuilder();
                byte[] InputbyteArray = new byte[Convert.ToInt32(p_InputString.Length) / 2];
                TripleDESCryptoServiceProvider Des = new TripleDESCryptoServiceProvider();
                MD5CryptoServiceProvider hashMD5 = new MD5CryptoServiceProvider();
                try {
                    Des.Key = hashMD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(p_SecretKey));
                    Des.Mode = p_CyphMode;
                    for (int X = 0; X <= InputbyteArray.Length - 1; X++) {
                        Int32 IJ = (Convert.ToInt32(p_InputString.Substring(X * 2, 2), 16));
                        ByteConverter BT = new ByteConverter();
                        InputbyteArray[X] = new byte();
                        InputbyteArray[X] = Convert.ToByte(BT.ConvertTo(IJ, typeof(byte)));
                    }
                    MemoryStream ms = new MemoryStream();
                    CryptoStream cs = new CryptoStream(ms, Des.CreateDecryptor(), CryptoStreamMode.Write);
                    //Flush the data through the crypto stream into the memory stream
                    cs.Write(InputbyteArray, 0, InputbyteArray.Length);
                    cs.FlushFinalBlock();
                    //Get the decrypted data back from the memory stream
                    byte[] B = ms.ToArray();
                    ms.Close();
                    for (int I = 0; I <= B.GetUpperBound(0); I++) {
                        ret.Append(Convert.ToChar(B[I]));
                    }
                } catch (Exception ex) {
                    //   ME.Publish("SF.Utils.Utils", "DecryptString", ex, ManageException_Enumerators.ErrorLevel.FatalError);
                    return String.Empty;
                }
                return ret.ToString();

            }

        }

        private byte[] Encrypt(byte[] clearData, byte[] Key, byte[] IV)
        {
            MemoryStream ms = new MemoryStream();
            Rijndael alg = Rijndael.Create();
            alg.Key = Key;
            alg.IV = IV;
            CryptoStream cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(clearData, 0, clearData.Length);
            cs.Close();
            byte[] encryptedData = ms.ToArray();
            return encryptedData;
        }

        public string Encrypt2(string Data, string Password, int Bits)
        {
            byte[] clearBytes = System.Text.Encoding.Unicode.GetBytes(Data);
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password, new byte[] { 0x0, 0x1, 0x2, 0x1C, 0x1D, 0x1E, 0x3, 0x4, 0x5, 0xF, 0x20, 0x21, 0xAD, 0xAF, 0xA4 });
            if (Bits == 128)
            {
                byte[] encryptedData = Encrypt(clearBytes, pdb.GetBytes(16), pdb.GetBytes(16));
                return Convert.ToBase64String(encryptedData);
            }
            else if (Bits == 192)
            {
                byte[] encryptedData = Encrypt(clearBytes, pdb.GetBytes(24), pdb.GetBytes(16));
                return Convert.ToBase64String(encryptedData);
            }
            else if (Bits == 256)
            {
                byte[] encryptedData = Encrypt(clearBytes, pdb.GetBytes(32), pdb.GetBytes(16));
                return Convert.ToBase64String(encryptedData);
            }
            else
            {
                return String.Concat(Bits);
            }

        }

        private byte[] Decrypt(byte[] cipherData, byte[] Key, byte[] IV)
        {
            MemoryStream ms = new MemoryStream();
            Rijndael alg = Rijndael.Create();
            alg.Key = Key;
            alg.IV = IV;
            CryptoStream cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(cipherData, 0, cipherData.Length);
            cs.Close();
            byte[] decryptedData = ms.ToArray();
            return decryptedData;
        }
        public string Decrypt2(string Data, string Password, int Bits)
        {
            try
            {
                byte[] cipherBytes = Convert.FromBase64String(Data);
                PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password, new byte[] { 0x0, 0x1, 0x2, 0x1C, 0x1D, 0x1E, 0x3, 0x4, 0x5, 0xF, 0x20, 0x21, 0xAD, 0xAF, 0xA4 });
                if (Bits == 128)
                {
                    byte[] decryptedData = Decrypt(cipherBytes, pdb.GetBytes(16), pdb.GetBytes(16));
                    return System.Text.Encoding.Unicode.GetString(decryptedData);
                }
                else if (Bits == 192)
                {
                    byte[] decryptedData = Decrypt(cipherBytes, pdb.GetBytes(24), pdb.GetBytes(16));
                    return System.Text.Encoding.Unicode.GetString(decryptedData);
                }
                else if (Bits == 256)
                {
                    byte[] decryptedData = Decrypt(cipherBytes, pdb.GetBytes(32), pdb.GetBytes(16));
                    return System.Text.Encoding.Unicode.GetString(decryptedData);
                }
                else
                {
                    return String.Concat(Bits);
                }
            }
            catch (Exception ex)
            {
                return String.Concat(Bits);
            }
        }


    }

}