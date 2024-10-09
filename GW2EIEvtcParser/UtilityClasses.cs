using System.Collections.Generic;

namespace GW2EIEvtcParser;

/// HashSet that only exposes Contains, saves a copy of the whole set in the one case where its used.
class ReadonlyHashSet<T> {
    HashSet<T> _source;

    public ReadonlyHashSet(HashSet<T> source) => _source = source;

    public bool Contains(in T needle) => _source.Contains(needle);
}
