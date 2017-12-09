using CodeFirstDemo.EntityFramework;
using System.Runtime.Remoting.Messaging;
using System.Web.Mvc;

namespace CodeFirstDemo.Controllers
{
    public class BaseController : Controller
    {
        // GET: Base
        public CodeFirstContext db
        {
            get
            {
                //从当前线程获取 CodeFirstContext 对象
                CodeFirstContext db = CallContext.GetData("DB") as CodeFirstContext;
                if (db==null)
                {
                    db = new CodeFirstContext();
                    CallContext.SetData("DB", db);
                }
                return db;
            }
        }
    }
}