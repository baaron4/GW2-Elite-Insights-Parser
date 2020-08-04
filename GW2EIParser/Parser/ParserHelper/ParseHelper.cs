using System.IO;
using GW2EIParser.EIData;
using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.Parser
{
    internal static class ParseHelper
    {

        public static AgentItem UnknownAgent = new AgentItem();
        // use this for "null" in AbstractActor dictionaries
        public static NPC NullActor = new NPC(UnknownAgent);

        public static void SafeSkip(Stream stream, long bytesToSkip)
        {
            if (stream.CanSeek)
            {
                stream.Seek(bytesToSkip, SeekOrigin.Current);
            }
            else
            {
                while (bytesToSkip > 0)
                {
                    stream.ReadByte();
                    --bytesToSkip;
                }
            }
        }

        public static string GetString(Stream stream, int length, bool nullTerminated = true)
        {
            byte[] bytes = new byte[length];
            stream.Read(bytes, 0, length);
            if (nullTerminated)
            {
                for (int i = 0; i < length; ++i)
                {
                    if (bytes[i] == 0)
                    {
                        length = i;
                        break;
                    }
                }
            }
            return System.Text.Encoding.UTF8.GetString(bytes, 0, length);
        }
    }
}
