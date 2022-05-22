namespace myonAPI.Services;

internal class UniqueNumberService
{
    private int _n;

    private readonly Dictionary<object, int> _uniqueNumbers = new();

    public bool TryGetNumber(object key, out int value)
    {
        if (!_uniqueNumbers.TryGetValue(key, out value))
        {
            value = _n;
            return TryAddNumber(key);
        }
        return true;
    }

    public bool TryAddNumber(object key) => _uniqueNumbers.TryAdd(key, _n++);
}