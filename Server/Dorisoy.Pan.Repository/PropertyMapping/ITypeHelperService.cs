namespace Dorisoy.Pan.Repository
{
    public interface ITypeHelperService
    {
        bool TypeHasProperties<T>(string fields);
    }
}
