using System.Numerics;

namespace GW2EIEvtcParser.EIData;

public class CombatReplayMap
{
    private (int width, int height) _pixelSize;
    private (double topX, double topY, double bottomX, double bottomY) _rectInMap;
    public float Width => (float)(_rectInMap.bottomX - _rectInMap.topX);

    public float Height => (float)(_rectInMap.bottomY - _rectInMap.topY);
    public float TopX => (float)_rectInMap.topX;

    public float TopY => (float)_rectInMap.topY;
    public float BottomX => (float)_rectInMap.bottomX;

    public float BottomY => (float)_rectInMap.bottomY;

    public class CombatReplayMapViewpoint
    {
        public readonly double XTranslatePercent;
        public readonly double YTranslatePercent;
        public readonly double Scale;
        public readonly long EncounterID;

        internal CombatReplayMapViewpoint(double xTranslatePercent, double yTranslatePercent, double scale, long encounterID  )
        {
            XTranslatePercent = xTranslatePercent;
            YTranslatePercent = yTranslatePercent;
            Scale = scale;
            EncounterID = encounterID;
        }
    }

    private readonly List<CombatReplayMapViewpoint> _defaultViewpoints = [];
    public IReadOnlyList<CombatReplayMapViewpoint>? DefaultViewpoints => _defaultViewpoints.Count > 0 ? _defaultViewpoints : null;

    private (int topX, int topY, int bottomX, int bottomY) _mapRect;
    private (int topX, int topY, int bottomX, int bottomY) _continentRect;

    /// <summary>
    ///
    /// </summary>
    /// <param name="link">Url to the image</param>
    /// <param name="urlPixelSize">Width and Height of the image in pixel</param>
    /// <param name="rectInMap">The map rectangle region corresponding to the image in map coordinates</param>
    internal CombatReplayMap((int width, int height) urlPixelSize, (double topX, double topY, double bottomX, double bottomY) rectInMap)
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
    internal CombatReplayMap((int width, int height) urlPixelSize, (double topX, double topY, double bottomX, double bottomY) rectInMap, (int topX, int topY, int bottomX, int bottomY) mapRect, (int topX, int topY, int bottomX, int bottomY) continentRect) : this(urlPixelSize, rectInMap)
    {
        _mapRect = mapRect;
        _continentRect = continentRect;
    }

    private int _mapWidth => (_mapRect.topX - _mapRect.bottomX);
    private int _mapHeight => (_mapRect.topY - _mapRect.bottomY);
    private int _continentWidth => (_continentRect.topX - _continentRect.bottomX);
    private int _continentHeight => (_continentRect.topY - _continentRect.bottomY);

    internal Vector3 ContinentCoordToMapCoord(Vector3 iPos)
    {
        if (_mapWidth == 0 || _continentWidth == 0 || _continentHeight == 0 || _mapHeight == 0)
        {
            throw new InvalidOperationException("Missing continent data in CombatReplay map for conversion");
        }
        return new Vector3(
            (iPos.X - _continentRect.bottomX) / _continentWidth * _mapWidth + _mapRect.bottomX,
            (-iPos.Y + _continentRect.bottomY) / _continentHeight * _mapHeight - _mapRect.bottomY, 
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
        if (log.CanCombatReplay && _rectInMap.topX == _rectInMap.bottomX)
        {
            _rectInMap.topX = int.MaxValue;
            _rectInMap.topY = int.MaxValue;
            _rectInMap.bottomX = int.MinValue;
            _rectInMap.bottomY = int.MinValue;
            foreach (Player p in log.PlayerList)
            {
                if (!p.HasCombatReplayPositions(log))
                {
                    continue;
                }
                var pos = p.GetCombatReplayPolledPositions(log).Where(x => x.Time >= start && x.Time <= end);
                if (pos.Any())
                {
                    _rectInMap.topX = Math.Min(Math.Floor(pos.Min(x => x.XYZ.X)) - 250, _rectInMap.topX);
                    _rectInMap.topY = Math.Min(Math.Floor(pos.Min(x => x.XYZ.Y)) - 250, _rectInMap.topY);
                    _rectInMap.bottomX = Math.Max(Math.Floor(pos.Max(x => x.XYZ.X)) + 250, _rectInMap.bottomX);
                    _rectInMap.bottomY = Math.Max(Math.Floor(pos.Max(x => x.XYZ.Y)) + 250, _rectInMap.bottomY);
                }
            }
            FixAspectRatio();
        }
    }
#endif
    internal void ComputeBoundingBox(ParsedEvtcLog log)
    {
        if (log.CanCombatReplay && _rectInMap.topX == _rectInMap.bottomX)
        {
            _rectInMap.topX = int.MaxValue;
            _rectInMap.topY = int.MaxValue;
            _rectInMap.bottomX = int.MinValue;
            _rectInMap.bottomY = int.MinValue;
            foreach (Player p in log.PlayerList)
            {
                if (!p.HasCombatReplayPositions(log))
                {
                    continue;
                }
                var pos = p.GetCombatReplayPolledPositions(log);
                _rectInMap.topX = Math.Min(Math.Floor(pos.Min(x => x.XYZ.X)) - 250, _rectInMap.topX);
                _rectInMap.topY = Math.Min(Math.Floor(pos.Min(x => x.XYZ.Y)) - 250, _rectInMap.topY);
                _rectInMap.bottomX = Math.Max(Math.Floor(pos.Max(x => x.XYZ.X)) + 250, _rectInMap.bottomX);
                _rectInMap.bottomY = Math.Max(Math.Floor(pos.Max(x => x.XYZ.Y)) + 250, _rectInMap.bottomY);
            }
            FixAspectRatio();
        }
    }

    internal void FixAspectRatio()
    {
        var width = _rectInMap.bottomX - _rectInMap.topX;
        var height = _rectInMap.bottomY - _rectInMap.topY;
        var pad = Math.Abs(width - height) / 2.0;
        if (width > height)
        {
            _rectInMap.topY -= pad;
            _rectInMap.bottomY += pad;
        }
        else if (height > width)
        {
            _rectInMap.topX -= pad;
            _rectInMap.bottomX += pad;
        }
    }

    internal Vector2 GetMapCoordRounded(float realX, float realY)
    {
        var (width, height) = GetPixelMapSize();
        double scaleX = (double)width / _pixelSize.width;
        double scaleY = (double)height / _pixelSize.height;
        double x = (realX - _rectInMap.topX) / (_rectInMap.bottomX - _rectInMap.topX);
        double y = (realY - _rectInMap.topY) / (_rectInMap.bottomY - _rectInMap.topY);
        if (Double.IsNaN(x) || Double.IsNaN(y) || Double.IsInfinity(x) || Double.IsInfinity(y))
        {
            throw new InvalidOperationException("Positions are NaN");
        }
        return new(
            (float)Math.Round(scaleX * _pixelSize.width * x, ParserHelper.CombatReplayDataDigit), 
            (float)Math.Round(scaleY * (_pixelSize.height - _pixelSize.height * y), ParserHelper.CombatReplayDataDigit)
        );
    }
    internal Vector2 GetMapCoordRounded(in Vector2 realPos) => GetMapCoordRounded(realPos.X, realPos.Y);

    public float GetInchToPixel()
    {
        float ratio = (float)(_rectInMap.bottomX - _rectInMap.topX) / GetPixelMapSize().width;
        return (float)Math.Round(1.0f / ratio, 3);
    }

    internal CombatReplayMap Translate(double x, double y)
    {
        _rectInMap.bottomX -= x;
        _rectInMap.topX -= x;
        _rectInMap.bottomY -= y;
        _rectInMap.topY -= y;
        return this;
    }

    internal CombatReplayMap Scale(double scale)
    {
        double centerX = (_rectInMap.bottomX + _rectInMap.topX) / 2.0;
        double halfWidth = scale * (_rectInMap.bottomX - centerX);
        double centerY = (_rectInMap.bottomY + _rectInMap.topY) / 2.0;
        double halfHeigth = scale * (_rectInMap.bottomY - centerY);
        _rectInMap.bottomX = centerX + halfWidth;
        _rectInMap.bottomY = centerY + halfHeigth;
        _rectInMap.topX = centerX - halfWidth;
        _rectInMap.topY = centerY - halfHeigth;
        return this;
    }

    internal CombatReplayMap AdjustForAspectRatio()
    {
        double ratio = (double)_pixelSize.width / _pixelSize.height;
        double centerY = (_rectInMap.bottomY + _rectInMap.topY) / 2.0;
        double halfHeigth = (_rectInMap.bottomY - centerY);
        double centerX = (_rectInMap.bottomX + _rectInMap.topX) / 2.0;
        double halfWidth = ratio * halfHeigth;
        _rectInMap.bottomX = centerX + halfWidth;
        _rectInMap.bottomY = centerY + halfHeigth;
        _rectInMap.topX = centerX - halfWidth;
        _rectInMap.topY = centerY - halfHeigth;
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
            var centerY = (other._rectInMap.topY + other._rectInMap.bottomY) / 2.0;
            var topY = (other._rectInMap.topY - centerY) * ratio + centerY;
            var bottomY = (other._rectInMap.bottomY - centerY) * ratio + centerY;
            return new CombatReplayMap((other._pixelSize.width, height), (other._rectInMap.topX, topY, other._rectInMap.bottomX, bottomY));
        }
        else
        {
            var width = other._pixelSize.height;
            var ratio = (double)other._pixelSize.height / other._pixelSize.width;
            var centerX = (other._rectInMap.topX + other._rectInMap.bottomX) / 2.0;
            var topX = (other._rectInMap.topX - centerX) * ratio + centerX;
            var bottomX = (other._rectInMap.bottomX - centerX) * ratio + centerX;
            return new CombatReplayMap((width, other._pixelSize.height), (topX, other._rectInMap.topY, bottomX, other._rectInMap.bottomY));
        }
    }
}
