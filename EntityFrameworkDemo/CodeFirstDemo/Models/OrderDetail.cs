using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeFirstDemo.Models
{
    public class OrderDetail
    {
        /// <summary>
        /// OrderDetailId
        /// </summary>
        [Key]
        public int OrderDetailId { get; set; }

        /// <summary>
        /// 订单明细单价
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 订单明细数量
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 外键，如果属性名和 Order 主键名称一样，默认会当成外键，可以不加 ForeignKey 特性，ForeignKey里面的值要和导航属性名称一致
        /// </summary>
        [ForeignKey("Order")]
        public int OrderId { get; set; }

        /// <summary>
        /// 导航属性
        /// </summary>
        public virtual Order Order { get; set; }
    }
}