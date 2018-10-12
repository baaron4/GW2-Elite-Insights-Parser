using Newtonsoft.Json;
using System;

namespace LuckParser.Models.ParseModels
{
    public class LineActor : Actor
    {
        public Object Target { get; }
        public int Width { get; }

        public LineActor(int growing, int width, Object targetAgent, Tuple<int, int> lifespan, string color) : base(false, growing, lifespan, color)
        {
            Target = targetAgent;
            Width = width;
        }

        public LineActor(int growing, int width, Object targetAgent, Tuple<int, int> lifespan, string color, Point3D position) : base(false, growing, lifespan, color, position)
        {
            Target = targetAgent;
            Width = width;
        }

        public LineActor(int growing, int width, Object targetAgent, Tuple<int, int> lifespan, string color, Point3D prev, Point3D next, int time) : base(false, growing, lifespan, color, prev, next, time)
        {
            Target = targetAgent;
            Width = width;
        }

        private class LineSerializable : Serializable
        {
            public Object Target { get; set; }
            public int Width { get; set; }
        }

        public override string GetCombatReplayJSON(CombatReplayMap map, AbstractMasterPlayer master)
        {
            LineSerializable aux = new LineSerializable
            {
                Type = "Line",
                Width = Width,
                Fill = Filled,
                Color = Color,
                Growing = Growing,
                Start = Lifespan.Item1,
                End = Lifespan.Item2
            };
            if (Target is Point3D)
            {
                Point3D TargetPoint = (Point3D)Target;
                Tuple<int, int> mapPos = map.GetMapCoord(TargetPoint.X, TargetPoint.Y);
                aux.Target = new int[2]
                {
                        mapPos.Item1,
                        mapPos.Item2
                };
            }
            else if (Target is int)
            {
                aux.Target = (int)Target;
            }
            else if (Target is AbstractMasterPlayer)
            {
                if (((AbstractMasterPlayer)Target).CombatReplay != null)
                {
                    aux.Target = Target.GetType().GetMethod("GetCombatReplayID").Invoke(Target, null);
                }
                else
                {
                    aux.Target = master.GetCombatReplayID(); // Line Actor with zero length: same origin and destination
                }
            }
            else
            {
                throw new InvalidOperationException("Target object is neihter a 3DPoint, nor AbstractMasterPlayer (or one of its children) nor an agent ID");
            }
            if (Position != null)
            {
                Tuple<int, int> mapPos = map.GetMapCoord(Position.X, Position.Y);
                aux.ConnectedTo = new int[2]
                {
                        mapPos.Item1,
                        mapPos.Item2
                };
                return JsonConvert.SerializeObject(aux);
            }
            else
            {
                aux.ConnectedTo = master.GetCombatReplayID();
                return JsonConvert.SerializeObject(aux);
            }
        }
    }
}
