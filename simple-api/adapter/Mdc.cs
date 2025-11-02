using System;
using System.Collections.Immutable;
using System.Threading;

namespace simple_api.adapter;

public static class Mdc
{
    private static readonly AsyncLocal<ImmutableDictionary<string, string>?> _state = new();

    public static void Put(string key, string value)
    {
        var prev = _state.Value ?? ImmutableDictionary<string, string>.Empty;
        _state.Value = prev.SetItem(key, value);
    }

    public static string? Get(string key) => TryGet(key, out var v) ? v : null;

    public static bool TryGet(string key, out string? value)
    {
        var map = _state.Value;
        if (map != null && map.TryGetValue(key, out var v))
        {
            value = v;
            return true;
        }
        value = null;
        return false;
    }

    public static void Remove(string key)
    {
        var prev = _state.Value ?? ImmutableDictionary<string, string>.Empty;
        _state.Value = prev.Remove(key);
    }

    public static void Clear() => _state.Value = ImmutableDictionary<string, string>.Empty;

    public static IDisposable BeginScope(string key, string value)
    {
        var prev = _state.Value ?? ImmutableDictionary<string, string>.Empty;
        _state.Value = prev.SetItem(key, value);
        return new Scope(prev);
    }

    private sealed class Scope : IDisposable
    {
        private readonly ImmutableDictionary<string, string> _prev;
        private bool _disposed;
        public Scope(ImmutableDictionary<string, string> prev) => _prev = prev;
        public void Dispose()
        {
            if (_disposed) return;
            _state.Value = _prev;
            _disposed = true;
        }
    }
}