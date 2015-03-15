using System;
using System.Security.Cryptography;
using System.Text;


namespace Crypto3
{
    class Program
    {
        public static readonly int BLOCK_SIZE = 1024;
        public static readonly int HASH_SIZE = 32;

        public static string Filename = "Target.mp4";

        private static SHA256 shaM;
   

        public static byte[] SubArray (byte[] bytes, int start, int end)
        {
            byte[] result = new byte[end - start];
            int contResult = 0;

            for (int cont = start; cont < end; cont++)
            {
                result[contResult++] = bytes[cont];
            }
            return result;
        }
        public static byte[] ConcatArrays(byte[] array1, byte[] array2)
        {
            byte[] result = new byte[array1.Length + array2.Length];
            int contResult = 0;

            foreach (byte b in array1)
            {
                result[contResult++] = b;
            }
            foreach (byte b in array2)
            {
                result[contResult++] = b;
            }
            return result;
        }

        public static byte[] HexStringToByteArray(string hexstring)
        {
            byte[] buff = new byte[hexstring.Length / 2];
            for (int i = 0; i < hexstring.Length; i += 2)
            {
                buff[i / 2] = byte.Parse(hexstring.Substring(i, 2), System.Globalization.NumberStyles.HexNumber);
            }
            return buff;
        }
        public static string ByteArrayToHexString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        public static byte[] CalculeHash(byte[] bytes)
        {
            return shaM.ComputeHash(bytes);
        }

        static byte[] CalculeHashRecursive(byte[] bytes)
        {
            if (bytes.Length <= BLOCK_SIZE)
            {
                return CalculeHash(bytes);
            }
          
            byte[] block = SubArray(bytes, 0, BLOCK_SIZE);
            byte[] rest = SubArray(bytes, BLOCK_SIZE, bytes.Length);

            byte[] result = CalculeHash(ConcatArrays(block, CalculeHashRecursive(rest)));
            return result;
        }


        static void Main(string[] args)
        {
            var bytes = System.IO.File.ReadAllBytes(Filename);

            Console.WriteLine("File: " + Filename);
            Console.WriteLine("Size: " + bytes.Length + " Bytes");
            
             shaM = new SHA256Managed();

             // Recursive way
             byte[] result = CalculeHashRecursive(bytes);
             Console.WriteLine("Last Hash: " + ByteArrayToHexString(result));

            //Secuential way
            int numFullBlocks = bytes.Length/BLOCK_SIZE;
            Console.WriteLine("Num Blocks: " + numFullBlocks + " Full Blocks of " + BLOCK_SIZE + " bytes");
             
            byte[] previousHash = {};
            
            if (bytes.Length%BLOCK_SIZE != 0)
            {
                byte[] lastBlock = SubArray(bytes, numFullBlocks * BLOCK_SIZE, bytes.Length);
                Console.WriteLine(" + 1 block of: " + lastBlock.Length + " Bytes");
                
                previousHash = CalculeHash(lastBlock);
            }


            for (var i = numFullBlocks; i > 0; i--)
            {
                var currentBlock = SubArray(bytes, (i-1)*BLOCK_SIZE, i*BLOCK_SIZE);
                previousHash = CalculeHash(ConcatArrays(currentBlock, previousHash));
            }

            Console.WriteLine("Last Hash: " + ByteArrayToHexString(previousHash));          
        }
    }
}
