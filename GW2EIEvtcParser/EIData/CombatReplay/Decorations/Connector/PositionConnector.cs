namespace GW2EIEvtcParser.EIData
{
    internal class PositionConnector : Connector
    {
        protected Point3D Position { get; set; }

        public PositionConnector()
        {

        }

        public PositionConnector(Point3D position)
        {
            Position = position;
        }

        public override object GetConnectedTo(CombatReplayMap map, ParsedEvtcLog log)
        {
            (double x, double y) = map.GetMapCoord(Position.X, Position.Y);
            return new double[2]
                       {
                        x,
                        y
                       };
        }
    }
}
