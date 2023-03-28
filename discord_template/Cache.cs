public class Cache<T>
{
    private T? _Value;
    private readonly Func<T> _Getter;

    // Valueを取得しようとしたときに初めての取得であれば_Getterの保持する処理を実行する
    public T Value
    {
        get
        {
            if (_Value == null)
            {
                _Value = _Getter.Invoke();
            }
            return _Value;
        }
    }

    public Cache(Func<T> getter)
    {
        _Getter = getter;
    }
}