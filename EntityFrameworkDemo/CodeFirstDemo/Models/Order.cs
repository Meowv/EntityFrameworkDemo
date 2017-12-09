using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CodeFirstDemo.Models
{
    public class Order
    {
        /// <summary>
        /// 如果属性名后面包含Id,则默认会当作主键，可以不用添加 [Key] 属性
        /// </summary>
        [Key]
        public int OrderId { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        [StringLength(50)]
        public string OrderCode { get; set; }

        /// <summary>
        /// 订单金额
        /// </summary>
        public decimal OrderAmout { get; set; }

        /// <summary>
        /// 导航属性设置成 virtual ,可以实现延迟加载
        /// </summary>
        public virtual List<OrderDetail> OrderDetail { get; set; }
    }
}