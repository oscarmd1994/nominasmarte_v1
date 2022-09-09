using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Payroll.Models.Utilerias
{
    public class Encrypt
    {

        static readonly string password = "qHz3f8#@#?89p9o9DBxtSefr#";
        static readonly string route    = "h5fR9IOIstTViHMgAaM0UfphEbcEpWR4xx9C5V+rWlvcGxh9291A6jMuBy5HZI3M+FUdXkKXJPSRUN/k7+TxTz/jQqoQvBo6/9epdldHefs=";

        public static string EncryptD(string plainText)
        {
            if (plainText == null) {
                return null;
            }
            var bytesToBeEncrypted = Encoding.UTF8.GetBytes(plainText);
            var passwordBytes  = Encoding.UTF8.GetBytes(password);
            passwordBytes      = SHA512.Create().ComputeHash(passwordBytes);
            var bytesEncrypted = EncryptD(bytesToBeEncrypted, passwordBytes);
            return Convert.ToBase64String(bytesEncrypted);
        }

        public static string DecryptD(string encryptedText)
        {
            if (encryptedText == null) {
                return null;
            }
            var bytesToBeDecrypted = Convert.FromBase64String(encryptedText);
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            passwordBytes = SHA512.Create().ComputeHash(passwordBytes);
            var bytesDecrypted = DecryptD(bytesToBeDecrypted, passwordBytes);
            return Encoding.UTF8.GetString(bytesDecrypted);
        }

        private static byte[] EncryptD(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;
            var saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream()) {
                using (RijndaelManaged AES = new RijndaelManaged()) {
                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.KeySize = 256;
                    AES.BlockSize = 128;
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);
                    AES.Mode = CipherMode.CBC;
                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write)) {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }
            return encryptedBytes;
        }

        private static byte[] DecryptD(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;
            var saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream()) {
                using (RijndaelManaged AES = new RijndaelManaged()) {
                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.KeySize = 256;
                    AES.BlockSize = 128;
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);
                    AES.Mode = CipherMode.CBC;
                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write)) {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    } 
                    decryptedBytes = ms.ToArray();
                }
            }
            return decryptedBytes;
        }

        public static string[] readParams()
        {
            int quantity   = 0;
            string[] param = new string[7];
            try {
                string routeDecrypt = DecryptD(route);
                if (routeDecrypt is null) {
                    return param;
                }
                string fileToRead = routeDecrypt;
                using (StreamReader reader = new StreamReader(fileToRead)) {
                    string line  = "";
                    while ((line = reader.ReadLine()) != null) {
                        if (line.ToString() != "") {
                            string[] result = line.ToString().Split('-');
                            param[quantity] = result[1];
                        }
                        quantity += 1;
                    }
                }
            } catch (FileNotFoundException flExc) {
                param[6] = "Archivo no encontrado " + flExc.Message.ToString();
            } catch (IOException ioExc) {
                param[6] = "Problema al abrir el archivo " + ioExc.Message.ToString();
            } catch (Exception exc) {
                param[6] = "ERROR " + exc.Message.ToString();
            }
            return param;
        }

    }
}