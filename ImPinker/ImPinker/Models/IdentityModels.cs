using Microsoft.AspNet.Identity.EntityFramework;

namespace ImPinker.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("ImpinkerUserSystem")
        {
            //用户登录验证，用ImpinkerUserSystem  数据库，以后别的系统也可以用同一套账号登录
        }
    }
}