using System;
using System.Linq;
using Newtonsoft.Json;
using static GW2EIEvtcParser.EIData.MovingPlatformDecoration;

namespace GW2EIEvtcParser.EIData
{
    internal class MovingPlatformDecorationMetadataDescription : BackgroundDecorationMetadataDescription
    {


        public string Image { get; }
        public int Height { get; }
        public int Width { get; }



        internal MovingPlatformDecorationMetadataDescription(MovingPlatformDecorationMetadata decoration) : base(decoration)
        {
            Type = "MovingPlatform";
            Image = decoration.Image;
            Width = decoration.Width;
            Height = decoration.Height;
        }

    }

}
