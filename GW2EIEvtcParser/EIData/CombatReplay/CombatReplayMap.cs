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
    //private (int topX, int topY, int bottomX, int bottomY) _fullRect;
    //private (int bottomX, int bottomY, int topX, int topY) _worldRect;

    /// <summary>
    ///
    /// </summary>
    /// <param name="link">Url to the image</param>
    /// <param name="urlPixelSize">Width and Height of the image in pixel</param>
    /// <param name="rectInMap">The map rectangle region corresponding to the image in map coordinates</param>
    internal CombatReplayMap((int width, int height) urlPixelSize, (double topX, double topY, double bottomX, double bottomY) rectInMap)
    {
        _pixelSize = urlPixelSize;
        _rectInMap = rectInMap;
    }

    /*/internal CombatReplayMap(string link, (int width, int height) size, (int topX, int topY, int bottomX, int bottomY) rect, (int topX, int topY, int bottomX, int bottomY) fullRect, (int bottomX, int bottomY, int topX, int topY) worldRect)
    {
        _maps.Add(new MapItem()
        {
            Link = link,
            Start = -1,
            End = -1
        });
        _size = size;
        _rect = rect;
        _fullRect = fullRect;
        _worldRect = worldRect;
    }*/

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
}
