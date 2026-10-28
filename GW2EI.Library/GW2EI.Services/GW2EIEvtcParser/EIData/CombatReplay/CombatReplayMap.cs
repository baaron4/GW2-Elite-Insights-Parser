using System.Numerics;

namespace GW2EIEvtcParser.EIData;

public class CombatReplayMap
{
    private (int width, int height) _pixelSize;
    private (double minX, double minY, double maxX, double maxY) _rectInMap;
    public float Width => (float)(_rectInMap.maxX - _rectInMap.minX);

    public float Height => (float)(_rectInMap.maxY - _rectInMap.minY);
    public float MinX => (float)_rectInMap.minX;

    public float MinY => (float)_rectInMap.minY;
    public float MaxX => (float)_rectInMap.maxX;

    public float MaxY => (float)_rectInMap.maxY;

    public class CombatReplayMapViewpoint
    {
        public readonly double XTranslatePercent;
        public readonly double YTranslatePercent;
        public readonly double Scale;
        public readonly long EncounterID;

        internal CombatReplayMapViewpoint(double xTranslatePercent, double yTranslatePercent, double scale, long encounterID)
        {
            XTranslatePercent = xTranslatePercent;
            YTranslatePercent = yTranslatePercent;
            Scale = scale;
            EncounterID = encounterID;
        }
    }

    private readonly List<CombatReplayMapViewpoint> _defaultViewpoints = [];
    public IReadOnlyList<CombatReplayMapViewpoint>? DefaultViewpoints => _defaultViewpoints.Count > 0 ? _defaultViewpoints : null;

    private (int minX, int minY, int maxX, int maxY) _mapRect;
    private (int minX, int minY, int maxX, int maxY) _continentRect;

    /// <summary>
    ///
    /// </summary>
    /// <param name="link">Url to the image</param>
    /// <param name="urlPixelSize">Width and Height of the image in pixel</param>
    /// <param name="rectInMap">The map rectangle region corresponding to the image in map coordinates</param>
    internal CombatReplayMap((int width, int height) urlPixelSize, (double minX, double minY, double maxX, double maxY) rectInMap)
    {
        if (urlPixelSize.width <= 0 || urlPixelSize.height <= 0)
        {
            throw new InvalidDataException("Width and height must be strictly positive in CombatReplay map");
        }
        _pixelSize = urlPixelSize;
        _rectInMap = rectInMap;
    }

    /// <summary>
    /// Constructor with continentRect and mapRect. Use this contructor if you need to convert continent coordinates to map coordinates
    /// </summary>
    /// <param name="link">Url to the image</param>
    /// <param name="urlPixelSize">Width and Height of the image in pixel</param>
    /// <param name="rectInMap">The map rectangle region corresponding to the image in map coordinates</param>
    /// <param name="mapRect">The full map rectangle region</param>
    /// <param name="continentRect">The continent rectangle region corresponding to the image in map coordinates</param>
    internal CombatReplayMap((int width, int height) urlPixelSize, (double minX, double minY, double maxX, double maxY) rectInMap, (int minX, int minY, int maxX, int maxY) mapRect, (int minX, int minY, int maxX, int maxY) continentRect) : this(urlPixelSize, rectInMap)
    {
        _mapRect = mapRect;
        _continentRect = continentRect;
    }

    private int _mapWidth => (_mapRect.maxX - _mapRect.minX);
    private int _mapHeight => (_mapRect.maxY - _mapRect.minY);
    private int _continentWidth => (_continentRect.maxX - _continentRect.minX);
    private int _continentHeight => (_continentRect.maxY - _continentRect.minY);

    internal Vector3 ContinentCoordToMapCoord(Vector3 iPos)
    {
        if (_mapWidth == 0 || _continentWidth == 0 || _continentHeight == 0 || _mapHeight == 0)
        {
            throw new InvalidOperationException("Missing continent data in CombatReplay map for conversion");
        }
        // Continent rect is Y down, map rect is Y up
        return new Vector3(
            (iPos.X - _continentRect.minX) / _continentWidth * _mapWidth + _mapRect.minX,
            (-iPos.Y + _continentRect.maxY) / _continentHeight * _mapHeight - _mapRect.maxY,
            iPos.Z);
    }

    public (int width, int height) GetPixelMapSize()
    {
        double ratio = (double)_pixelSize.width / _pixelSize.height;
        const int pixelSize = 750;
        if (ratio > 1.0)
        {
            return (pixelSize, (int)Math.Round(pixelSize / ratio));
        }
        else if (ratio < 1.0)
        {
            return ((int)Math.Round(ratio * pixelSize), pixelSize);
        }
        else
        {
            return (pixelSize, pixelSize);
        }
    }
    internal void AddDefaultViewpoint(double xTranslatePercent, double yTranslatePercent, double scale, long encounterID)
    {
        _defaultViewpoints.Add(new CombatReplayMapViewpoint(xTranslatePercent, yTranslatePercent, scale, encounterID));
    }
#if DEBUG
    internal void ComputeBoundingBox(ParsedEvtcLog log, long start, long end)
    {
        if (log.CanCombatReplay && _rectInMap.minX == _rectInMap.maxX)
        {
            _rectInMap.minX = int.MaxValue;
            _rectInMap.minY = int.MaxValue;
            _rectInMap.maxX = int.MinValue;
            _rectInMap.maxY = int.MinValue;
            foreach (Player p in log.PlayerList)
            {
                if (!p.HasCombatReplayPositions(log))
                {
                    continue;
                }
                var pos = p.GetCombatReplayPolledPositions(log).Where(x => x.Time >= start && x.Time <= end).ToList();
                if (pos.Count != 0)
                {
                    _rectInMap.minX = Math.Min(Math.Floor(pos.Min(x => x.XYZ.X)) - 250, _rectInMap.minX);
                    _rectInMap.minY = Math.Min(Math.Floor(pos.Min(x => x.XYZ.Y)) - 250, _rectInMap.minY);
                    _rectInMap.maxX = Math.Max(Math.Floor(pos.Max(x => x.XYZ.X)) + 250, _rectInMap.maxX);
                    _rectInMap.maxY = Math.Max(Math.Floor(pos.Max(x => x.XYZ.Y)) + 250, _rectInMap.maxY);
                }
            }
            FixAspectRatio();
        }
    }
#endif
    internal void ComputeBoundingBox(ParsedEvtcLog log)
    {
        if (log.CanCombatReplay && _rectInMap.minX == _rectInMap.maxX)
        {
            _rectInMap.minX = int.MaxValue;
            _rectInMap.minY = int.MaxValue;
            _rectInMap.maxX = int.MinValue;
            _rectInMap.maxY = int.MinValue;
            foreach (Player p in log.PlayerList)
            {
                if (!p.HasCombatReplayPositions(log))
                {
                    continue;
                }
                var pos = p.GetCombatReplayPolledPositions(log);
                _rectInMap.minX = Math.Min(Math.Floor(pos.Min(x => x.XYZ.X)) - 250, _rectInMap.minX);
                _rectInMap.minY = Math.Min(Math.Floor(pos.Min(x => x.XYZ.Y)) - 250, _rectInMap.minY);
                _rectInMap.maxX = Math.Max(Math.Floor(pos.Max(x => x.XYZ.X)) + 250, _rectInMap.maxX);
                _rectInMap.maxY = Math.Max(Math.Floor(pos.Max(x => x.XYZ.Y)) + 250, _rectInMap.maxY);
            }
            FixAspectRatio();
        }
    }

    internal void FixAspectRatio()
    {
        var width = _rectInMap.maxX - _rectInMap.minX;
        var height = _rectInMap.maxY - _rectInMap.minY;
        var pad = Math.Abs(width - height) / 2.0;
        if (width > height)
        {
            _rectInMap.minY -= pad;
            _rectInMap.maxY += pad;
        }
        else if (height > width)
        {
            _rectInMap.minX -= pad;
            _rectInMap.maxX += pad;
        }
    }

    internal Vector2 GetMapCoordRounded(float realX, float realY)
    {
        var (width, height) = GetPixelMapSize();
        double scaleX = (double)width / _pixelSize.width;
        double scaleY = (double)height / _pixelSize.height;
        double x = (realX - _rectInMap.minX) / (_rectInMap.maxX - _rectInMap.minX);
        double y = (realY - _rectInMap.minY) / (_rectInMap.maxY - _rectInMap.minY);
        if (Double.IsNaN(x) || Double.IsNaN(y) || Double.IsInfinity(x) || Double.IsInfinity(y))
        {
            throw new InvalidOperationException("Positions are NaN");
        }
        return new(
            (float)Math.Round(scaleX * _pixelSize.width * x, ParserHelper.CombatReplayDataDigit),
            // Canvas is Y down
            (float)Math.Round(scaleY * (_pixelSize.height - _pixelSize.height * y), ParserHelper.CombatReplayDataDigit)
        );
    }
    internal Vector2 GetMapCoordRounded(in Vector2 realPos) => GetMapCoordRounded(realPos.X, realPos.Y);

    public float GetInchToPixel()
    {
        float ratio = (float)(_rectInMap.maxX - _rectInMap.minX) / GetPixelMapSize().width;
        return (float)Math.Round(1.0f / ratio, 3);
    }

    internal CombatReplayMap Translate(double x, double y)
    {
        _rectInMap.maxX -= x;
        _rectInMap.minX -= x;
        _rectInMap.maxY -= y;
        _rectInMap.minY -= y;
        return this;
    }

    internal CombatReplayMap Scale(double scale)
    {
        double centerX = (_rectInMap.maxX + _rectInMap.minX) / 2.0;
        double halfWidth = scale * (_rectInMap.maxX - centerX);
        double centerY = (_rectInMap.maxY + _rectInMap.minY) / 2.0;
        double halfHeigth = scale * (_rectInMap.maxY - centerY);
        _rectInMap.maxX = centerX + halfWidth;
        _rectInMap.maxY = centerY + halfHeigth;
        _rectInMap.minX = centerX - halfWidth;
        _rectInMap.minY = centerY - halfHeigth;
        return this;
    }

    internal CombatReplayMap AdjustForAspectRatio()
    {
        double ratio = (double)_pixelSize.width / _pixelSize.height;
        double centerY = (_rectInMap.maxY + _rectInMap.minY) / 2.0;
        double halfHeigth = (_rectInMap.maxY - centerY);
        double centerX = (_rectInMap.maxX + _rectInMap.minX) / 2.0;
        double halfWidth = ratio * halfHeigth;
        _rectInMap.maxX = centerX + halfWidth;
        _rectInMap.maxY = centerY + halfHeigth;
        _rectInMap.minX = centerX - halfWidth;
        _rectInMap.minY = centerY - halfHeigth;
        return this;
    }

    internal static CombatReplayMap CreateSquareMapFrom(CombatReplayMap other)
    {
        if (other._pixelSize.width == other._pixelSize.height)
        {
            return other;
        }
        if (other._pixelSize.width > other._pixelSize.height)
        {
            var height = other._pixelSize.width;
            var ratio = (double)other._pixelSize.width / other._pixelSize.height;
            var centerY = (other._rectInMap.minY + other._rectInMap.maxY) / 2.0;
            var minY = (other._rectInMap.minY - centerY) * ratio + centerY;
            var maxY = (other._rectInMap.maxY - centerY) * ratio + centerY;
            return new CombatReplayMap((other._pixelSize.width, height), (other._rectInMap.minX, minY, other._rectInMap.maxX, maxY));
        }
        else
        {
            var width = other._pixelSize.height;
            var ratio = (double)other._pixelSize.height / other._pixelSize.width;
            var centerX = (other._rectInMap.minX + other._rectInMap.maxX) / 2.0;
            var minX = (other._rectInMap.minX - centerX) * ratio + centerX;
            var maxX = (other._rectInMap.maxX - centerX) * ratio + centerX;
            return new CombatReplayMap((width, other._pixelSize.height), (minX, other._rectInMap.minY, maxX, other._rectInMap.maxY));
        }
    }
}
