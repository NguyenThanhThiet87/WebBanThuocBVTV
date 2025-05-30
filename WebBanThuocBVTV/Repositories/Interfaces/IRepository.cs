using WebBanThuocBVTV.Helper;

namespace WebBanThuocBVTV.Repositories.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<string> CreateId();
        Task<AlertMessage> Add(T entity);
        Task<bool> Update(T entity);
        Task<bool> Delete(string id);
    }
}
