using CodeFirstDemo.Models;
using System.Data.Entity;

namespace CodeFirstDemo.EntityFramework
{
    public class CodeFirstContext : DbContext
    {
        /// <summary>
        /// name的值要和配置文件里面配置的上下文连接字符串名称一致
        /// </summary>
        public CodeFirstContext() : base("name=CodeFirstContext") { }

        public DbSet<Order> Order { get; set; }
        public DbSet<OrderDetail> OrderDetail { get; set; }
    }
}