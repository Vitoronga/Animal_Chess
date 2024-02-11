namespace WpfCMExperiments.Network
{
    public static class ByteHelper
    {
        public static byte[] DoOROperationOnByteArrays(byte[] array1, byte[] array2)
        {
            int length = array1.Length < array2.Length ? array1.Length : array2.Length;
            byte[] newArray = new byte[length];

            for (int i = 0; i < length; i++)
            {
                newArray[i] = (byte)(array1[i] | array2[i]);
            }

            return newArray;
        }

        public static byte[] DoANDOperationOnByteArrays(byte[] array1, byte[] array2)
        {
            int length = array1.Length < array2.Length ? array1.Length : array2.Length;
            byte[] newArray = new byte[length];

            for (int i = 0; i < length; i++)
            {
                newArray[i] = (byte)(array1[i] & array2[i]);
            }

            return newArray;
        }

        public static bool IncrementByteArray(ref byte[] array, int limitTopIndex = -1)
        {
            //for (int i = array.Length - 1; i >= 0; i--)
            //{
            //    if (array[i] == 255)
            //    {
            //        if (i == 0) return false;

            //        array[i] = 0;
            //        array[i - 1]++;
            //    }

            //    array[i]++;
            //}

            int currentIndex = array.Length - 1;

            while (IncrementByte(ref array[currentIndex], true)) // Will keep being true while the byte keeps overflowing
            {
                // Check if it couldn't increment even at first index
                if (currentIndex-- == 0 || currentIndex == limitTopIndex) return false; // It will end with currentIndex at -1
            }

            return true;
        }

        public static bool IncrementByte(ref byte b, bool allowAndReturnIfOverflowed = true)
        {
            if (b == 255)
            {
                if (!allowAndReturnIfOverflowed) return false; // no success

                b = 0;
                return true; // overflowed
            }

            b++;
            return !allowAndReturnIfOverflowed; // Return false for no overflow, return true for success
        }

        public static bool IsByteArrayBitsFullySet(byte[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] != 255) return false;
            }

            return true;
        }

        public static byte GetCountOfSequenceBitsOn(byte[] array)
        {
            byte bits = 0;

            // Check up to which bit is set in the mask value
            foreach (byte b in array)
            {
                byte i = 128;

                while (i != 0 && (b & i) != 0) // while i isn't 0 and certain bit is on
                {
                    i >>= 1;
                    bits++;
                }

                // Check for when the network mask bits stops in the middle of a byte
                //if (i != 0) 
            }

            return bits;
        }
    }
}
