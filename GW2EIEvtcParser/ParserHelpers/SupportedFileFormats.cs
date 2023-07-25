using System;
using System.Collections.Generic;

namespace GW2EIEvtcParser.ParserHelpers
{
    /// <summary>
    /// Class containing the supported file formats by Elite Insights.
    /// </summary>
    public static class SupportedFileFormats
    {
        /// <summary>
        /// Supported compressed file formats.
        /// <list type="bullet">
        /// <item>.zevtc</item>
        /// <item>.evtc.zip</item>
        /// </list>
        /// </summary>
        private static readonly HashSet<string> _compressedFiles = new HashSet<string>()
        {
            ".zevtc",
            ".evtc.zip",
        };

        /// <summary>
        /// Temporary compressed file formats.
        /// <list type="bullet">
        /// <item>.tmp.zip</item>
        /// </list>
        /// </summary>
        private static readonly HashSet<string> _tmpCompressedFiles = new HashSet<string>()
        {
            ".tmp.zip"
        };

        /// <summary>
        /// Temporary file formats.
        /// </summary>
        private static readonly HashSet<string> _tmpFiles = new HashSet<string>()
        {
            ""
        };

        /// <summary>
        /// Supported file formats.
        /// <list type="bullet">
        /// <item>.evtc</item>
        /// </list>
        /// </summary>
        private static readonly HashSet<string> _supportedFiles = new HashSet<string>(_compressedFiles)
        {
            ".evtc"
        };

        /// <summary>
        /// Supported file formats.
        /// </summary>
        public static IReadOnlyList<string> SupportedFormats => new List<string>(_supportedFiles);

        /// <summary>
        /// Checks if the file is compressed supported format in <see cref="_compressedFiles"/>.
        /// </summary>
        /// <param name="fileName">The entire file name.</param>
        /// <returns><see langword="true"/> if valid, otherwise <see langword="false"/>.</returns>
        public static bool IsCompressedFormat(string fileName)
        {
            foreach (string format in _compressedFiles)
            {
                if (fileName.EndsWith(format, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if the file is a supported format in <see cref="_supportedFiles"/>.
        /// </summary>
        /// <param name="fileName">The entire file name.</param>
        /// <returns><see langword="true"/> if valid, otherwise <see langword="false"/>.</returns>
        public static bool IsSupportedFormat(string fileName)
        {
            foreach (string format in _supportedFiles)
            {
                if (fileName.EndsWith(format, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if the file is a supported temporary compressed format in <see cref="_tmpCompressedFiles"/>.
        /// </summary>
        /// <param name="fileName">The entire file name.</param>
        /// <returns><see langword="true"/> if valid, otherwise <see langword="false"/>.</returns>
        public static bool IsTemporaryCompressedFormat(string fileName)
        {
            foreach (string format in _tmpCompressedFiles)
            {
                if (fileName.EndsWith(format, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if the file is a supported temporary format in <see cref="_tmpFiles"/>.
        /// </summary>
        /// <param name="fileName">The entire file name.</param>
        /// <returns><see langword="true"/> if valid, otherwise <see langword="false"/>.</returns>
        public static bool IsTemporaryFormat(string fileName)
        {
            foreach (string format in _tmpFiles)
            {
                if (fileName.EndsWith(format, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
