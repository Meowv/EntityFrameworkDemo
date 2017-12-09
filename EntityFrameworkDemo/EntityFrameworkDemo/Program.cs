using EntityFrameworkDemo.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;

namespace EntityFrameworkDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            //Add();
            //QueryDelay1();
            //QueryDelay2();
            //var list = GetListBy(c => c.CompanyName != "", c => c.CustomerID);
            //var list = GetPageList(1, 10, (c) => c.CompanyName != "", (c) => c.CustomerID);
            //Edit1();
            //Edit2();
            //Delete();
            //BatcheAdd();
        }

        #region 新增
        static int Add()
        {
            using (NorthwindDbContext db = new NorthwindDbContext())
            {
                Customers customers = new Customers()
                {
                    CustomerID = "MEOWV",
                    Address = "ShangHaiJiaDing",
                    City = "ShangHai",
                    Phone = "13477996338",
                    CompanyName = "Gasgoo",
                    ContactName = "qix"
                };

                //方法一
                //db.Customers.Add(customers);

                //方法二
                DbEntityEntry<Customers> entity = db.Entry<Customers>(customers);
                entity.State = EntityState.Added;

                return db.SaveChanges();
            }
        }
        #endregion

        #region 简单查询
        static void QueryDelay1()
        {
            using (NorthwindDbContext db = new NorthwindDbContext())
            {
                DbQuery<Customers> dbQuery = db.Customers.Where(u => u.ContactName == "qix").OrderBy(u => u.ContactName).Take(1) as DbQuery<Customers>;
                //获得延迟查询对象后，调用对象的获取方法，此时，就会根据之前的条件生成SQL语句，查询数据库。

                Customers customers = dbQuery.FirstOrDefault(); //或者 dbQuery.SingleOrDefault();

                Console.WriteLine(customers.ContactName);
            }
        }
        #endregion

        #region 延迟查询
        static void QueryDelay2()
        {
            using (NorthwindDbContext db = new NorthwindDbContext())
            {
                IQueryable<Orders> orders = db.Orders.Where(a => a.CustomerID == "VINET");
                //真实返回的DbQuery对象，以接口方式返回

                //此时只查询了订单表
                Orders order = orders.FirstOrDefault();

                //当访问订单对象的外键实体时，EF 会查询订单对应的用户表，查到之后，再将数据装入这个外键实体

                Console.WriteLine(order.Customers.ContactName);

                IQueryable<Orders> orderList = db.Orders;
                foreach (Orders o in orderList)
                {
                    Console.WriteLine(o.OrderID + ":ContactName=" + o.Customers.ContactName);
                }
            }
        }
        #endregion

        #region 根据条件排序和查询
        /// <summary>
        /// 根据条件排序和查询
        /// </summary>
        /// <typeparam name="TKey">排序字段类型</typeparam>
        /// <param name="whereLambda">查询条件 lambda 表达式</param>
        /// <param name="orderLambda">排序条件 lambda 表达式</param>
        /// <returns></returns>
        static List<Customers> GetListBy<T>(Expression<Func<Customers, bool>> whereLambda, Expression<Func<Customers, T>> orderLambda)
        {
            using (NorthwindDbContext db = new NorthwindDbContext())
            {
                return db.Customers.Where(whereLambda).OrderBy(orderLambda).ToList();
            }
        }
        #endregion

        #region 分页查询
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="whereLambda">条件 lambda 表达式</param>
        /// <param name="orderBy">排序 lambda 表达式</param>
        /// <returns></returns>
        static List<Customers> GetPageList<T>(int pageIndex, int pageSize, Expression<Func<Customers, bool>> whereLambda, Expression<Func<Customers, T>> orderBy)
        {
            using (NorthwindDbContext db = new NorthwindDbContext())
            {
                // 分页要注意，Skip之前一定要 OrderBy
                return db.Customers.Where(whereLambda).OrderBy(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
        }
        #endregion

        #region 修改
        /// <summary>
        /// 官方推荐的修改方式 - 先查询，再修改
        /// </summary>
        static void Edit1()
        {
            using (NorthwindDbContext db = new NorthwindDbContext())
            {
                //1.先查询一个要修改的对象，此时返回的是一个Customers类的代理类对象
                Customers customers = db.Customers.Where(u => u.CustomerID == "MEOWV").FirstOrDefault();

                Console.WriteLine($"修改前：{customers.ContactName}");

                //2.修改内容，此时其实操作的是代理类对象的属性，这些属性会将值设置给内部的 Customers 对象对应的属性，同时标记此属性为已修改状态
                customers.ContactName = "阿星Plus";

                //3.重新保存到数据库，此时 EF 上下文会检查容器内部所有的对象，先找到标记为修改的对象，然后找到标记为修改的对象属性，生成对应的 update 语句并执行
                db.SaveChanges();

                Console.WriteLine($"修改后：{customers.ContactName}");
            }
        }

        /// <summary>
        /// 优化后的修改方式 - 创建对象，直接修改
        /// </summary>
        static void Edit2()
        {
            //1.查询出一个要修改的对象
            Customers customers = new Customers()
            {
                CustomerID = "MEOWV",
                Address = "ShangHaiJiaDing",
                City = "ShangHai",
                Phone = "13477996338",
                CompanyName = "Gasgoo",
                ContactName = "阿星Plus"
            };

            using (NorthwindDbContext db = new NorthwindDbContext())
            {
                //2.将对象加入EF容器，并获取当前实体对象的状态管理对象
                DbEntityEntry<Customers> entry = db.Entry<Customers>(customers);
                //3.设置该对象为被修改过
                entry.State = EntityState.Unchanged;
                //4.设置改对象的 ContactName 属性为修改状态，同时 entry.State 被修改为 Modified 状态
                entry.Property("ContactName").IsModified = true;

                var u = db.Customers.Attach(customers);
                u.ContactName = "qix";

                //5.重新保存到数据库，EF 上下文会根据实体对象的状态 entry.State = Modified 值生成对应的 update sql 语句
                db.SaveChanges();

                Console.WriteLine($"修改成功：{customers.ContactName}");
            }
        }
        #endregion

        #region 删除
        static void Delete()
        {
            using (NorthwindDbContext db = new NorthwindDbContext())
            {
                //1.创建要删除的对象
                Customers customer = new Customers() { CustomerID = "MEOWV" };

                //2.附加到 EF 中
                db.Customers.Attach(customer);

                //3.(方法一)标记为删除，此方法就是标记当前对象为删除状态
                db.Customers.Remove(customer);

                //3.(方法二)使用 Entry 来附加和删除
                //DbEntityEntry<Customers> entry = db.Entry<Customers>(customer);
                //entry.State = EntityState.Deleted;

                //4.执行删除SQL
                db.SaveChanges();

                Console.WriteLine("删除成功");
            }
        }
        #endregion

        #region 批处理
        /// <summary>
        /// 批处理，一次新增50条数据
        /// </summary>
        static void BatcheAdd()
        {
            using (NorthwindDbContext db = new NorthwindDbContext())
            {
                for (int i = 0; i < 50; i++)
                {
                    Customers customers = new Customers()
                    {
                        CustomerID = "MEOWV" + i,
                        Address = "ShangHaiJiaDing",
                        City = "ShangHai",
                        Phone = "13477996338",
                        CompanyName = "Gasgoo",
                        ContactName = "qi" + i,
                    };
                    db.Customers.Add(customers);
                }
                db.SaveChanges();
            }
        }
        #endregion
    }
}
