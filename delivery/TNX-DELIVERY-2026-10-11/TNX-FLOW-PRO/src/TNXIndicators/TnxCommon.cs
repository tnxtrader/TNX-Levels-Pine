// The three shared helper types of the TNX indicators (GdiCache, PictureArrays, TnxErrors), compiled into the indicator
// assembly. Shapes from the IL of the 2.2 build; semantics the indicator relies on: Path(site) returns the same reset path
// every picture; Get<T>(slot, n) returns an array of exactly n elements, reused between pictures; TnxErrors counts errors
// in memory (never written anywhere) and the card shows the count.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;

namespace TNXIndicators
{
    /// <summary>Reused GDI+ objects (paths, brushes, pens) so that a picture makes no new GDI+ object.</summary>
    internal sealed class GdiCache
    {
        private const int MaxEach = 4096;        // from the IL of the real class
        private const int GradientSlack = 512;   // from the IL of the real class (gradient helpers are not stubbed)

        private readonly Dictionary<int, GraphicsPath> _paths = new Dictionary<int, GraphicsPath>();
        private readonly Dictionary<int, SolidBrush> _brushes = new Dictionary<int, SolidBrush>();
        private readonly Dictionary<(int, float), Pen> _pens = new Dictionary<(int, float), Pen>();
        private int _picture;

        /// <summary>A new picture starts.</summary>
        public void BeginPaint()
        {
            _picture++;
        }

        /// <summary>The path for a drawing site, emptied (Reset) before it is returned; the same object every picture.</summary>
        public GraphicsPath Path(int site)
        {
            if (_paths.TryGetValue(site, out GraphicsPath path))
            {
                path.Reset();
                return path;
            }
            if (_paths.Count >= MaxEach)
            {
                Clear(_paths);
            }
            path = new GraphicsPath();
            _paths[site] = path;
            return path;
        }

        /// <summary>A solid brush of that colour, made once.</summary>
        public SolidBrush Brush(Color colour)
        {
            int key = colour.ToArgb();
            if (!_brushes.TryGetValue(key, out SolidBrush brush))
            {
                if (_brushes.Count >= MaxEach)
                {
                    Clear(_brushes);
                }
                brush = new SolidBrush(colour);
                _brushes[key] = brush;
            }
            return brush;
        }

        /// <summary>A pen of that colour and width, made once.</summary>
        public Pen Pen(Color colour, float width)
        {
            var key = (colour.ToArgb(), width);
            if (!_pens.TryGetValue(key, out Pen pen))
            {
                if (_pens.Count >= MaxEach)
                {
                    Clear(_pens);
                }
                pen = new Pen(colour, width);
                _pens[key] = pen;
            }
            return pen;
        }

        /// <summary>Disposes and forgets everything.</summary>
        public void Clear()
        {
            Clear(_paths);
            Clear(_brushes);
            Clear(_pens);
            _picture = 0;
        }

        private static void Clear<TKey, TValue>(Dictionary<TKey, TValue> cache) where TValue : IDisposable
        {
            foreach (TValue value in cache.Values)
            {
                try { value?.Dispose(); } catch { }
            }
            cache.Clear();
        }
    }

    /// <summary>Work arrays reused between pictures: Get&lt;T&gt;(slot, n) hands out the same array of exactly n elements for the same slot, n and T.</summary>
    internal sealed class PictureArrays
    {
        private const int Slack = 512;   // from the IL of the real class: pictures an unused array survives

        private sealed class Slot
        {
            public Array Array;
            public int Picture;
        }

        private readonly Dictionary<(int, int, Type), Slot> _slots = new Dictionary<(int, int, Type), Slot>();
        private readonly List<(int, int, Type)> _unused = new List<(int, int, Type)>();
        private int _picture;

        /// <summary>A new picture starts; arrays not asked for in the last Slack pictures are released.</summary>
        public void BeginPaint()
        {
            _picture++;
            if (_picture % Slack != 0)
            {
                return;
            }
            _unused.Clear();
            foreach (KeyValuePair<(int, int, Type), Slot> pair in _slots)
            {
                if (_picture - pair.Value.Picture > Slack)
                {
                    _unused.Add(pair.Key);
                }
            }
            foreach ((int, int, Type) key in _unused)
            {
                _slots.Remove(key);
            }
            _unused.Clear();
        }

        /// <summary>An array of exactly n elements of T for that slot (its contents are whatever the last picture left there).</summary>
        public T[] Get<T>(int slot, int n)
        {
            if (n < 0)
            {
                n = 0;
            }
            var key = (slot, n, typeof(T));
            if (_slots.TryGetValue(key, out Slot s) && s.Array is T[] existing && existing.Length == n)
            {
                s.Picture = _picture;
                return existing;
            }
            T[] array = new T[n];
            _slots[key] = new Slot { Array = array, Picture = _picture };
            return array;
        }

        /// <summary>Forgets every array.</summary>
        public void Clear()
        {
            _slots.Clear();
            _unused.Clear();
            _picture = 0;
        }
    }

    /// <summary>Errors caught inside the indicators: counted in memory (never written anywhere) and shown on the card.</summary>
    internal static class TnxErrors
    {
        private sealed class Place
        {
            public long Count;
            public Exception Last;
        }

        private static readonly ConcurrentDictionary<(string, string), Place> Places = new ConcurrentDictionary<(string, string), Place>();

        /// <summary>Every error reported so far (read with Interlocked.Read(ref TnxErrors.Total)).</summary>
        internal static long Total;

        /// <summary>Counts an error caught at a site ("indicator", "method#n"); cancellations are not errors.</summary>
        public static void Report(string indicator, string site, Exception error)
        {
            if (error == null || IsCancellation(error))
            {
                return;
            }
            Interlocked.Increment(ref Total);
            Place place = Places.GetOrAdd((indicator ?? "", site ?? ""), _ => new Place());
            Interlocked.Increment(ref place.Count);
            place.Last = error;
        }

        /// <summary>One line per site: "indicator site: count x LastExceptionType: message".</summary>
        internal static List<string> Summary()
        {
            var lines = new List<string>();
            foreach (KeyValuePair<(string, string), Place> pair in Places)
            {
                Exception last = pair.Value.Last;
                lines.Add(pair.Key.Item1 + " " + pair.Key.Item2 + ": " + Interlocked.Read(ref pair.Value.Count) + " x " + (last == null ? "" : last.GetType().Name + ": " + last.Message));
            }
            lines.Sort(StringComparer.Ordinal);
            return lines;
        }

        private static bool IsCancellation(Exception error)
        {
            return error is OperationCanceledException || error is ThreadInterruptedException;
        }
    }
}
