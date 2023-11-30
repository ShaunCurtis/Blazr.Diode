namespace Blazr.Diode.Server.Web.Data
{
    public interface IMap<TRecord, TDbo>
    {
        public TDbo Map(TRecord record);
        public TRecord Map(TDbo record);
    }
}
