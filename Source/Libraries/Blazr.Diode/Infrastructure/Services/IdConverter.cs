/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Diode.Infrastructure;

public class IdConverter : IIdConverter
{
    public object Convert(object value)
    {
        if (this.TryConvert(value, out object? outValue))
            return outValue;

        return value;
    }

    public bool TryConvert(object inValue, [NotNullWhen(true)] out object? outValue)
    {
        //TODO -  use switch statement detect int long guid etc and return

        if (inValue is IRecordId id)
        {
            outValue = id.GetKeyObject();
            return true;
        }

        if (long.TryParse(inValue.ToString(), out long longValue))
        {
            outValue = longValue;
            return true;
        }

        if (Guid.TryParse(inValue.ToString(), out Guid guidValue))
        {
            outValue = guidValue;
            return true;
        }

        outValue = null;
        return false;
    }
}
