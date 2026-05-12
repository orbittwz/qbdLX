using QobuzApiSharp.Models.Content;
using System.Collections.Generic;

namespace QobuzDownloaderX.Shared.Tools
{
    public static class QualityStringMappings
    {
        private static readonly Dictionary<string, (string, string)> QualityMappings = new Dictionary<string, (string, string)>
        {
            {"5", ("MP3 320kbps CBR", "MP3")},
            {"6", ("FLAC (16bit/44.1kHz)", "FLAC (16bit-44.1kHz)")},
            {"7", ("FLAC (24bit/96kHz)", "FLAC (24bit-96kHz)")},
            {"27", ("FLAC (24bit/192kHz)", "FLAC (24bit-192kHz)")}
        };
        private static readonly Dictionary<string, (double, double)> MaximumBitDepthAndSampleRateMappings = new Dictionary<string, (double, double)>
        {
            {"5", (0, 0)}, // N/A, using 0 for easy calculation check.
            {"6", (16, 44.1)},
            {"7", (24, 96)},
            {"27", (24, 192)}
        };

        private static double GetMaxBitDepth(string formatIdString)
        {
            if (MaximumBitDepthAndSampleRateMappings.TryGetValue(formatIdString, out var value))
                return value.Item1;
            else
                throw new KeyNotFoundException($"QualityFormatID '{formatIdString}' not found.");
        }

        private static double GetMaxSampleRate(string formatIdString)
        {
            if (MaximumBitDepthAndSampleRateMappings.TryGetValue(formatIdString, out var value))
                return value.Item2;
            else
                throw new KeyNotFoundException($"QualityFormatID '{formatIdString}' not found.");
        }

        private static (string displayQuality, string pathSafeQuality) GetQualityStrings(string formatIdString)
        {
            if (QualityMappings.TryGetValue(formatIdString, out var value))
                return value;
            else
                throw new KeyNotFoundException($"QualityFormatID '{formatIdString}' not found.");
        }

        public static (string displayQuality, string pathSafeQuality) GetQualityStrings(string formatIdString, Album qobuzAlbum)
        {
            // Get Max bitDepth & sampleRate from API result.
            double bitDepth = qobuzAlbum.MaximumBitDepth.GetValueOrDefault();
            double sampleRate = qobuzAlbum.MaximumSamplingRate.GetValueOrDefault();
            double maxSelectedQuality = GetMaxBitDepth(formatIdString) * GetMaxSampleRate(formatIdString);
            double maxItemQuality = bitDepth * sampleRate;
            // Limit to selected quality if album quality is higher.
            if (maxSelectedQuality <= maxItemQuality)
                return GetQualityStrings(formatIdString);
            var displayQuality = "FLAC (" + bitDepth + "bit/" + sampleRate + "kHz)";
            var pathSafeQuality = displayQuality.Replace(@"\", "-").Replace("/", "-");
            return (displayQuality, pathSafeQuality);
        }
    }
}