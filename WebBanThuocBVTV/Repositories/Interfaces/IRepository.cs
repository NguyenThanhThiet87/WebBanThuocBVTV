using WebBanThuocBVTV.Helper;

namespace WebBanThuocBVTV.Repositories.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<AlertMessage> Add(T entity);
        Task<AlertMessage> Update(T entity);
        Task<AlertMessage> Delete(string id);
    }
}
