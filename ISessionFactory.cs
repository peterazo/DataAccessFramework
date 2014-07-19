
namespace DAF
{
    public interface ISessionFactory
    {
        DataAccessConfiguration Configuration { get; }
        ICachePool CachePool { get; }
        ISession CreateSession();
        ISession CreateSession(ISecurityToken token);
    }
}
