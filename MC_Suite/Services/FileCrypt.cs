using System;
using System.IO;
using System.Security.Cryptography;


namespace MC_Suite.Services
{
    class FileCrypt
    {
        private static FileCrypt _instance;
        public static FileCrypt Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new FileCrypt();
                return _instance;
            }
        }

        #region Dichiarazioni

        public Storage FileManager
        {
            get
            {
                return Storage.Instance;
            }
        }

        // Declare CspParmeters and RsaCryptoServiceProvider
        // objects with global scope of your Form class.
        CspParameters cspp = new CspParameters();
        RSACryptoServiceProvider rsa;

        public bool CryptEnabled = false;

        // Path variables for source, encryption, and
        // decryption folders. Must end with a backslash.
        const string EncrFolder = @"c:\Encrypt\";
        const string DecrFolder = @"c:\Decrypt\";
        const string SrcFolder = @"c:\docs\";

        // Public key file
        const string PubKeyFileName = "VerificatorKey.key";

        // Key container name for
        // private/public key value pair.
        const string keyName = "Key01";

        #endregion

        public bool InitCryptography()
        {
            string PubKeyFile = FileManager.CurrentFolder.Path + "\\" + PubKeyFileName;

            if ( File.Exists(PubKeyFile) == false )
            { 
                InitCryptoKey();
                ExportPublicKey();                
            }
            return ImportPublicKey();
        }


        public void InitCryptoKey()
        {
            // Stores a key pair in the key container.
            cspp.KeyContainerName = keyName;
            rsa = new RSACryptoServiceProvider(cspp);
            rsa.PersistKeyInCsp = true;
        }

        public void ExportPublicKey()
        {
            string PubKeyFile = FileManager.CurrentFolder.Path + "\\" + PubKeyFileName;

            // Save the public key created by the RSA
            // to a file. Caution, persisting the
            // key to a file is a security risk.
           // Directory.CreateDirectory(EncrFolder);
            StreamWriter sw = new StreamWriter(PubKeyFile, false);
            byte[] blob = rsa.ExportCspBlob(true);
            char[] blobstr = new char[blob.Length];
            for (int i = 0; i < blob.Length; i++)
                blobstr[i] = Convert.ToChar(blob[i]);
            sw.Write(blobstr, 0, blobstr.Length);
            sw.Close();
        }

        public bool ImportPublicKey()
        {
            string PubKeyFile = FileManager.CurrentFolder.Path + "\\" + PubKeyFileName;

            if( File.Exists(PubKeyFile) )
            {
                StreamReader sr = new StreamReader(PubKeyFile);
                cspp.KeyContainerName = keyName;
                rsa = new RSACryptoServiceProvider(cspp);
                string keytxt = sr.ReadToEnd();

                char[] blobstr = keytxt.ToCharArray();
                byte[] blob = new byte[blobstr.Length];
                for (int i = 0; i < blob.Length; i++)
                    blob[i] = Convert.ToByte(blobstr[i]);

                rsa.ImportCspBlob(blob);
                rsa.PersistKeyInCsp = true;
                sr.Close();

                CryptEnabled = true;
            }

            return CryptEnabled;
        }

        public string EncryptFile(string inFile, string outFileExt)
        {
            int startFileName = inFile.LastIndexOf("\\") + 1;
            // Change the file's extension to ".enc"
            string outFile = FileManager.CurrentFolder.Path + "\\" + inFile.Substring(startFileName, inFile.LastIndexOf(".") - startFileName) + outFileExt;

            //Se crittografia attivata critto il file
            if (CryptEnabled)
            { 
                // Create instance of Rijndael for
                // symetric encryption of the data.
                RijndaelManaged rjndl = new RijndaelManaged();
                rjndl.KeySize = 128;
                rjndl.BlockSize = 128;
                rjndl.Mode = CipherMode.CBC;
                ICryptoTransform transform = rjndl.CreateEncryptor();

                // Use RSACryptoServiceProvider to
                // enrypt the Rijndael key.
                // rsa is previously instantiated:
                //    rsa = new RSACryptoServiceProvider(cspp);
                byte[] keyEncrypted = rsa.Encrypt(rjndl.Key, false);

                // Create byte arrays to contain
                // the length values of the key and IV.
                byte[] LenK = new byte[4];
                byte[] LenIV = new byte[4];

                int lKey = keyEncrypted.Length;
                LenK = BitConverter.GetBytes(lKey);
                int lIV = rjndl.IV.Length;
                LenIV = BitConverter.GetBytes(lIV);

                // Write the following to the FileStream
                // for the encrypted file (outFs):
                // - length of the key
                // - length of the IV
                // - ecrypted key
                // - the IV
                // - the encrypted cipher content

                string fileToCrypt = FileManager.CurrentFolder.Path + "\\" + inFile;

                using (FileStream outFs = new FileStream(outFile, FileMode.Create))
                {
                    outFs.Write(LenK, 0, 4);
                    outFs.Write(LenIV, 0, 4);
                    outFs.Write(keyEncrypted, 0, lKey);
                    outFs.Write(rjndl.IV, 0, lIV);

                    // Now write the cipher text using
                    // a CryptoStream for encrypting.
                    using (CryptoStream outStreamEncrypted = new CryptoStream(outFs, transform, CryptoStreamMode.Write))
                    {

                        // By encrypting a chunk at
                        // a time, you can save memory
                        // and accommodate large files.
                        int count = 0;
                        int offset = 0;

                        // blockSizeBytes can be any arbitrary size.
                        int blockSizeBytes = rjndl.BlockSize / 8;
                        byte[] data = new byte[blockSizeBytes];
                        int bytesRead = 0;

                        using (FileStream inFs = new FileStream(fileToCrypt, FileMode.Open))
                        {
                            do
                            {
                                count = inFs.Read(data, 0, blockSizeBytes);
                                offset += count;
                                outStreamEncrypted.Write(data, 0, count);
                                bytesRead += blockSizeBytes;
                            }
                            while (count > 0);
                            inFs.Close();
                        }
                        outStreamEncrypted.FlushFinalBlock();
                        outStreamEncrypted.Close();
                    }
                    outFs.Close();
                }
            }
            else
            {
                //Crittografia non attiva, creo copia in chiaro del file originale
                File.Copy(FileManager.CurrentFolder.Path + "\\" + inFile, outFile);
            }
            return outFile;
        }

        public void DecryptFile(string inFile)
        {

            // Create instance of Rijndael for
            // symetric decryption of the data.
            RijndaelManaged rjndl = new RijndaelManaged();
            rjndl.KeySize = 128;
            rjndl.BlockSize = 128;
            rjndl.Mode = CipherMode.CBC;

            // Create byte arrays to get the length of
            // the encrypted key and IV.
            // These values were stored as 4 bytes each
            // at the beginning of the encrypted package.
            byte[] LenK = new byte[4];
            byte[] LenIV = new byte[4];

            // Consruct the file name for the decrypted file.
            string outFile = DecrFolder + inFile.Substring(0, inFile.LastIndexOf(".")) + ".txt";

            // Use FileStream objects to read the encrypted
            // file (inFs) and save the decrypted file (outFs).
            using (FileStream inFs = new FileStream(EncrFolder + inFile, FileMode.Open))
            {

                inFs.Seek(0, SeekOrigin.Begin);
                inFs.Seek(0, SeekOrigin.Begin);
                inFs.Read(LenK, 0, 3);
                inFs.Seek(4, SeekOrigin.Begin);
                inFs.Read(LenIV, 0, 3);

                // Convert the lengths to integer values.
                int lenK = BitConverter.ToInt32(LenK, 0);
                int lenIV = BitConverter.ToInt32(LenIV, 0);

                // Determine the start postition of
                // the ciphter text (startC)
                // and its length(lenC).
                int startC = lenK + lenIV + 8;
                int lenC = (int)inFs.Length - startC;

                // Create the byte arrays for
                // the encrypted Rijndael key,
                // the IV, and the cipher text.
                byte[] KeyEncrypted = new byte[lenK];
                byte[] IV = new byte[lenIV];

                // Extract the key and IV
                // starting from index 8
                // after the length values.
                inFs.Seek(8, SeekOrigin.Begin);
                inFs.Read(KeyEncrypted, 0, lenK);
                inFs.Seek(8 + lenK, SeekOrigin.Begin);
                inFs.Read(IV, 0, lenIV);
                Directory.CreateDirectory(DecrFolder);
                // Use RSACryptoServiceProvider
                // to decrypt the Rijndael key.
                byte[] KeyDecrypted = rsa.Decrypt(KeyEncrypted, false);

                // Decrypt the key.
                ICryptoTransform transform = rjndl.CreateDecryptor(KeyDecrypted, IV);

                // Decrypt the cipher text from
                // from the FileSteam of the encrypted
                // file (inFs) into the FileStream
                // for the decrypted file (outFs).
                using (FileStream outFs = new FileStream(outFile, FileMode.Create))
                {

                    int count = 0;
                    int offset = 0;

                    // blockSizeBytes can be any arbitrary size.
                    int blockSizeBytes = rjndl.BlockSize / 8;
                    byte[] data = new byte[blockSizeBytes];

                    // By decrypting a chunk a time,
                    // you can save memory and
                    // accommodate large files.

                    // Start at the beginning
                    // of the cipher text.
                    inFs.Seek(startC, SeekOrigin.Begin);
                    using (CryptoStream outStreamDecrypted = new CryptoStream(outFs, transform, CryptoStreamMode.Write))
                    {
                        do
                        {
                            count = inFs.Read(data, 0, blockSizeBytes);
                            offset += count;
                            outStreamDecrypted.Write(data, 0, count);
                        }
                        while (count > 0);

                        outStreamDecrypted.FlushFinalBlock(); 
                        outStreamDecrypted.Close();
                    }
                    outFs.Close();
                }
                inFs.Close();
            }
        }
    }
}
