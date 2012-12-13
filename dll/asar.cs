using System;
using System.Runtime.InteropServices;

namespace AsarCLR
{
    /// <summary>
    /// Contains various functions to apply patches.
    /// </summary>
    public static unsafe class Asar
    {
        const int expectedapiversion=200;

        [DllImport("asar", EntryPoint = "asar_init", CharSet = CharSet.Ansi)]
        private static extern bool asar_init();

        [DllImport("asar", EntryPoint = "asar_close", CharSet = CharSet.Ansi)]
        private static extern bool asar_close();

        [DllImport("asar", EntryPoint = "asar_version", CharSet = CharSet.Ansi)]
        private static extern int asar_version();

        [DllImport("asar", EntryPoint = "asar_apiversion", CharSet = CharSet.Ansi)]
        private static extern int asar_apiversion();

        [DllImport("asar", EntryPoint = "asar_reset", CharSet = CharSet.Ansi)]
        private static extern bool asar_reset();

        [DllImport("asar", EntryPoint = "asar_patch", CharSet = CharSet.Ansi)]
        private static extern bool asar_patch(string patchLocation, byte* romData, int bufLen, int* romLength);

        [DllImport("asar", EntryPoint = "asar_maxromsize", CharSet = CharSet.Ansi)]
        private static extern int asar_maxromsize();

        [DllImport("asar", EntryPoint = "asar_geterrors", CharSet = CharSet.Ansi)]
        private static extern Rawasarerror* asar_geterrors(out int length);

        [DllImport("asar", EntryPoint = "asar_getwarnings", CharSet = CharSet.Ansi)]
        private static extern Rawasarerror* asar_getwarnings(out int length);

        [DllImport("asar", EntryPoint = "asar_getprints", CharSet = CharSet.Ansi)]
        private static extern void** asar_getprints(out int length);

        [DllImport("asar", EntryPoint = "asar_getalllabels", CharSet = CharSet.Ansi)]
        private static extern Rawasarlabel* asar_getalllabels(out int length);

        [DllImport("asar", EntryPoint = "asar_getlabelval", CharSet = CharSet.Ansi)]
        private static extern int asar_getlabelval(string labelName);

        [DllImport("asar", EntryPoint = "asar_getdefine", CharSet = CharSet.Ansi)]
        private static extern IntPtr asar_getdefine(string defineName);

        [DllImport("asar", EntryPoint = "asar_getalldefines", CharSet = CharSet.Ansi)]
        private static extern Rawasardefine* asar_getalldefines(out int length);

        [DllImport("asar", EntryPoint = "asar_resolvedefines", CharSet = CharSet.Ansi)]
        private static extern IntPtr asar_resolvedefines(string data, bool learnNew);

        [DllImport("asar", EntryPoint = "asar_math", CharSet = CharSet.Ansi)]
        private static extern double asar_math(string math, out IntPtr error);

        /// <summary>
        /// Loads and initializes the DLL. You must call this before using any other Asar function.
        /// </summary>
        /// <returns>True if success</returns>
        public static bool init()
        {
            try {
                if (apiversion()<expectedapiversion || (apiversion()/100)>(expectedapiversion/100)) return false;
                if (!asar_init()) return false;
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// Returns the version, in the format major*10000+minor*100+bugfix*1.
        /// This means that 1.2.34 would be returned as 10234.
        /// </summary>
        /// <returns>Asar version</returns>
        public static int version()
        {
            return asar_version();
        }

        /// <summary>
        /// Returns the API version, format major*100+minor. Minor is incremented on backwards compatible
        ///  changes; major is incremented on incompatible changes. Does not have any correlation with the
        ///  Asar version.
        /// It's not very useful directly, since Asar.init() verifies this automatically.
        /// </summary>
        /// <returns>Asar API version</returns>
        public static int apiversion()
        {
            return asar_apiversion();
        }

        /// <summary>
        /// Clears out all errors, warnings and printed statements, and clears the file cache. Not useful for much, since patch() already does this.
        /// </summary>
        /// <returns>True if success</returns>
        public static bool reset()
        {
            return asar_reset();
        }

        /// <summary>
        /// Applies a patch.
        /// </summary>
        /// <param name="patchLocation">The patch location.</param>
        /// <param name="romData">The rom data. It must not be headered.</param>
        /// <returns>True if no errors.</returns>
        public static bool patch(string patchLocation, ref byte[] romData)
        {
            int newsize=maxromsize();
            int length = romData.Length;
            if (length<newsize) Array.Resize(ref romData, newsize);
            bool success;
            fixed (byte* ptr = romData)
            {
                success=asar_patch(patchLocation, ptr, newsize, &length);
            }
            if (length<newsize) Array.Resize(ref romData, length);
            return success;
        }

        /// <summary>
        /// Returns the maximum possible size of the output ROM from asar_patch(). Giving this size to buflen
        /// guarantees you will not get any buffer too small errors; however, it is safe to give smaller
        /// buffers if you don't expect any ROMs larger than 4MB or something.
        /// It's not very useful directly, since Asar.patch() uses this automatically.
        /// </summary>
        /// <returns>Maximum output size of the ROM.</returns>
        public static int maxromsize()
        {
            return asar_maxromsize();
        }
        
        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct Rawasarerror {
            public IntPtr fullerrdata;
            public IntPtr rawerrdata;
            public IntPtr block;
            public IntPtr filename;
            public int line;
            public IntPtr callerfilename;
            public int callerline;
        };
        
        private static Asarerror[] cleanerrors(Rawasarerror* ptr, int length)
        {
            Asarerror[] output = new Asarerror[length];

            // Better create a new array
            // to avoid pointer erros, corruption and may other problems.
            for (int i = 0; i < length; i++)
            {
                output[i].Fullerrdata = Marshal.PtrToStringAnsi(ptr[i].fullerrdata);
                output[i].Rawerrdata = Marshal.PtrToStringAnsi(ptr[i].rawerrdata);
                output[i].Block = Marshal.PtrToStringAnsi(ptr[i].block);
                output[i].Filename = Marshal.PtrToStringAnsi(ptr[i].filename);
                output[i].Line = ptr[i].line;
                output[i].Callerfilename = Marshal.PtrToStringAnsi(ptr[i].callerfilename);
                output[i].Callerline = ptr[i].callerline;
            }

            return output;
        }

        /// <summary>
        /// Gets all Asar current errors. They're safe to keep for as long as you want.
        /// </summary>
        /// <returns>All Asar's errors.</returns>
        public static Asarerror[] geterrors()
        {
            int length = 0;
            Rawasarerror* ptr = asar_geterrors(out length);
            return cleanerrors(ptr, length);
        }

        /// <summary>
        /// Gets all Asar current warning. They're safe to keep for as long as you want.
        /// </summary>
        /// <returns>All Asar's warnings.</returns>
        public static Asarerror[] getwarnings()
        {
            int length = 0;
            Rawasarerror* ptr = asar_getwarnings(out length);
            return cleanerrors(ptr, length);
        }

        /// <summary>
        /// Gets all prints generated by the patch
        /// (Note: to see warnings/errors, check getwarnings() and geterrors()
        /// </summary>
        /// <returns>All prints</returns>
        public static string[] getprints()
        {
            int length = 0;
            void** ptr = asar_getprints(out length);
            string[] output = new string[length];

            // Too annoying!
            for (int i = 0; i < length; i++)
            {
                output[i] = Marshal.PtrToStringAnsi((IntPtr)ptr[i]);
            }

            return output;
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct Rawasarlabel
        {
            public IntPtr name;
            public int location;
        }

        /// <summary>
        /// Gets all Asar current labels. They're safe to keep for as long as you want.
        /// </summary>
        /// <returns>All Asar's labels.</returns>
        public static Asarlabel[] getlabels()
        {
            int length = 0;
            Rawasarlabel* ptr = asar_getalllabels(out length);
            Asarlabel[] output = new Asarlabel[length];

            // Better create a new array
            // to avoid pointer erros, corruption and may other problems.
            for (int i = 0; i < length; i++)
            {
                output[i].Name = Marshal.PtrToStringAnsi(ptr[i].name);
                output[i].Location = ptr[i].location;
            }

            return output;
        }

        /// <summary>
        /// Gets a value of a specific label. Returns "-1" if label has not found.
        /// </summary>
        /// <param name="labelName">The label name.</param>
        /// <returns>The value of label. If not found, it returns -1 here.</returns>
        public static int getlabelval(string labelName)
        {
            return asar_getlabelval(labelName);
        }

        /// <summary>
        /// Gets contents of a define. If define doesn't exists, a null string will be generated.
        /// </summary>
        /// <param name="defineName">The define name.</param>
        /// <returns>The define content. If define has not found, this will be null.</returns>
        public static string getdefine(string defineName)
        {
            return Marshal.PtrToStringAnsi(asar_getdefine(defineName));
        }
        
        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct Rawasardefine
        {
            public IntPtr name;
            public IntPtr contents;
        }
        /// <summary>
        /// Gets all Asar current defines. They're safe to keep for as long as you want.
        /// </summary>
        /// <returns>All Asar's defines.</returns>
        public static Asardefine[] getalldefines()
        {
            int length = 0;
            Rawasardefine* ptr = asar_getalldefines(out length);
            Asardefine[] output = new Asardefine[length];

            // Better create a new array
            // to avoid pointer erros, corruption and may other problems.
            for (int i = 0; i < length; i++)
            {
                output[i].Name = Marshal.PtrToStringAnsi(ptr[i].name);
                output[i].Contents = Marshal.PtrToStringAnsi(ptr[i].contents);
            }

            return output;
        }

        /// <summary>
        /// </summary>
        /// <param name="data"></param>
        /// <param name="learnNew"></param>
        /// <returns></returns>
        public static string resolvedefines(string data, bool learnNew)
        {
            return Marshal.PtrToStringAnsi(asar_resolvedefines(data, learnNew));
        }

        /// <summary>
        /// Parse a string of math.
        /// </summary>
        /// <param name="math">The math string, i.e "1+1"</param>
        /// <param name="error">If occurs any error, it will showed here.</param>
        /// <returns>Product.</returns>
        public static double math(string math, out string error)
        {
            IntPtr err = IntPtr.Zero;
            double value = asar_math(math, out err);

            error = Marshal.PtrToStringAnsi(err);
            return value;
        }

    }

    /// <summary>
    /// Contains full information of a Asar error or warning.
    /// </summary>
    public struct Asarerror
    {
        public String Fullerrdata;
        public String Rawerrdata;
        public String Block;
        public String Filename;
        public int Line;
        public String Callerfilename;
        public int Callerline;
    }

    /// <summary>
    /// Contains a label from Asar.
    /// </summary>
    public struct Asarlabel
    {
        public String Name;
        public int Location;
    }

    /// <summary>
    /// Contains a Asar define.
    /// </summary>
    public struct Asardefine
    {
        public String Name;
        public String Contents;
    }
}
