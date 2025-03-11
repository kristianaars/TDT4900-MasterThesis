namespace TDT4900_MasterThesis.ViewModel.Component;

public class ComboBoxItemModel<T> : IEquatable<ComboBoxItemModel<T>>
{
    public T Value { get; set; }
    public string DisplayName { get; set; }

    public bool Equals(ComboBoxItemModel<T>? other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return EqualityComparer<T>.Default.Equals(Value, other.Value);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj.GetType() != GetType())
            return false;
        return Equals((ComboBoxItemModel<T>)obj);
    }

    public override int GetHashCode()
    {
        return EqualityComparer<T>.Default.GetHashCode(Value);
    }
}
